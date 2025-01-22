using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeworx.AspNetCore.Authentication.Introspection
{
    public class IntrospectionAuthenticationHandler : AuthenticationHandler<IntrospectionOptions>
    {
        public IntrospectionAuthenticationHandler(
            IOptionsMonitor<IntrospectionOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(
                  options,
                  logger,
                  encoder)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authority = Options.Authority;

            string authHeader = Context.Request.Headers.Authorization.ToString();

            string token = string.Empty;

            if (!string.IsNullOrWhiteSpace(authHeader) &&
                AuthenticationHeaderValue.TryParse(authHeader, out var headerValue) &&
                headerValue.Scheme.Equals("bearer", System.StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(headerValue.Parameter))
            {
                token = headerValue.Parameter;
            }
            else
            {
                return AuthenticateResult.NoResult();
            }

            var config = await Options.ConfigurationManager!.GetConfigurationAsync(this.Context.RequestAborted);

            if (string.IsNullOrWhiteSpace(config.IntrospectionEndpoint))
            {
                return AuthenticateResult.Fail($"No introspection endpoint present in openid metadata for authority {Options.Authority}. Use Options.Configuration.IntrospectionEndpoint to override the meatadata.");
            }

            var values = new Dictionary<string, string>();
            values.Add("token", token);
            values.Add("token_type_hint", "access_token");

            var content = new FormUrlEncodedContent(values);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, config.IntrospectionEndpoint);
            requestMessage.Content = content;

            var clientAuthentication = $"{Options.ClientId}:{Options.ClientSecret}";

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes(clientAuthentication)));

            var introspectionResponse = await Options.Backchannel.SendAsync(requestMessage);

            if (introspectionResponse.IsSuccessStatusCode)
            {
                await using (var stream = await introspectionResponse.Content.ReadAsStreamAsync())
                {
                    var node = await JsonNode.ParseAsync(stream);
                    if (node == null || !(node is JsonObject))
                    {
                        return AuthenticateResult.Fail($"Invalid content on introspection response.");
                    }

                    if (((JsonObject)node).TryGetPropertyValue("active", out var active) && active!.GetValue<bool>())
                    {
                        var payload = JwtPayload.Deserialize(node.ToJsonString());
                        var principal = new ClaimsPrincipal(new ClaimsIdentity(payload.Claims, this.Scheme.Name));

                        return AuthenticateResult.Success(new AuthenticationTicket(principal, this.Scheme.Name));
                    }
                    else
                    {
                        return AuthenticateResult.Fail($"The provided token was invalid or has been disabled.");
                    }
                }
            }
            else
            {
                return AuthenticateResult.Fail($"The introspection was unsuccessful. Response status code: {introspectionResponse.StatusCode}");
            }
        }
    }
}
