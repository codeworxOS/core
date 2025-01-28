using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace Codeworx.AspNetCore.Authentication.Introspection
{
    public class IntrospectionAuthenticationHandler : AuthenticationHandler<IntrospectionOptions>
    {
        private readonly IIntrospectionResponseCache _responseCache;

        public IntrospectionAuthenticationHandler(
            IOptionsMonitor<IntrospectionOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IIntrospectionResponseCache responseCache)
            : base(
                  options,
                  logger,
                  encoder)
        {
            _responseCache = responseCache;
        }

        protected new IntrospectionEvents Events
        {
            get => (IntrospectionEvents)base.Events!;
            set => base.Events = value;
        }

        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new IntrospectionEvents());

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? token = null;
            try
            {
                // Give application opportunity to find from a different location, adjust, or reject token
                var messageReceivedContext = new MessageReceivedContext(Context, Scheme, Options);

                // event can set the token
                await Events.MessageReceived(messageReceivedContext);
                if (messageReceivedContext.Result != null)
                {
                    return messageReceivedContext.Result;
                }

                // If application retrieved token from somewhere else, use that.
                token = messageReceivedContext.Token;

                if (string.IsNullOrEmpty(token))
                {
                    string authHeader = Context.Request.Headers.Authorization.ToString();

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
                }

                var authority = Options.Authority;

                var config = await Options.ConfigurationManager!.GetConfigurationAsync(this.Context.RequestAborted);

                if (string.IsNullOrWhiteSpace(config.IntrospectionEndpoint))
                {
                    return AuthenticateResult.Fail(new IntrospectionEndpintMissingException { Authority = Options.Authority });
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

                JwtPayload? payload = null;

                var introspectionResponse = await Options.Backchannel.SendAsync(requestMessage);

                if (introspectionResponse.IsSuccessStatusCode)
                {
                    await using (var stream = await introspectionResponse.Content.ReadAsStreamAsync())
                    {
                        var node = await JsonNode.ParseAsync(stream);
                        if (node == null || !(node is JsonObject))
                        {
                            return AuthenticateResult.Fail(new IntrospectionInvalidResponseException());
                        }

                        if (((JsonObject)node).TryGetPropertyValue("active", out var active) && active!.GetValue<bool>())
                        {
                            payload = JwtPayload.Deserialize(node.ToJsonString());
                            var payloadReceivedContext = new PayloadReceivedContext(Context, Scheme, Options, payload);

                            await Events.OnPayloadReceived(payloadReceivedContext);

                            if (payloadReceivedContext.Result != null)
                            {
                                return payloadReceivedContext.Result;
                            }

                            payload = payloadReceivedContext.Payload;
                        }
                        else
                        {
                            return AuthenticateResult.Fail(new IntrospectionInactiveResponseException());
                        }
                    }
                }
                else
                {
                    return AuthenticateResult.Fail(new IntrospectionResponseStausCodeException { StatusCode = introspectionResponse.StatusCode });
                }

                if (payload != null)
                {
                    if (Options.ValidationParameters != null)
                    {
                        ValidatePayload(payload, config, Options.ValidationParameters);
                    }

                    var principal = new ClaimsPrincipal(new ClaimsIdentity(payload.Claims, this.Scheme.Name));
                    return AuthenticateResult.Success(new AuthenticationTicket(principal, this.Scheme.Name));
                }

                return AuthenticateResult.Fail(new IntrospectionInvalidResponseException());
            }
            catch (Exception ex)
            {
                Logger.ErrorProcessingMessage(ex);

                var authenticationFailedContext = new AuthenticationFailedContext(Context, Scheme, Options)
                {
                    Exception = ex
                };

                await Events.AuthenticationFailed(authenticationFailedContext);
                if (authenticationFailedContext.Result != null)
                {
                    return authenticationFailedContext.Result;
                }

                throw;
            }
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var authResult = await HandleAuthenticateOnceSafeAsync();
            var eventContext = new ChallengeContext(Context, Scheme, Options, properties)
            {
                AuthenticateFailure = authResult?.Failure
            };

            // Avoid returning error=invalid_token if the error is not caused by an authentication failure (e.g missing token).
            if (Options.IncludeErrorDetails && eventContext.AuthenticateFailure != null)
            {
                eventContext.Error = "invalid_token";
                eventContext.ErrorDescription = CreateErrorDescription(eventContext.AuthenticateFailure);
            }

            await Events.Challenge(eventContext);
            if (eventContext.Handled)
            {
                return;
            }

            Response.StatusCode = 401;

            if (string.IsNullOrEmpty(eventContext.Error) &&
                string.IsNullOrEmpty(eventContext.ErrorDescription) &&
                string.IsNullOrEmpty(eventContext.ErrorUri))
            {
                Response.Headers.Append(HeaderNames.WWWAuthenticate, Options.Challenge);
            }
            else
            {
                // https://tools.ietf.org/html/rfc6750#section-3.1
                // WWW-Authenticate: Bearer realm="example", error="invalid_token", error_description="The access token expired"
                var builder = new StringBuilder(Options.Challenge);
                if (Options.Challenge.IndexOf(' ') > 0)
                {
                    // Only add a comma after the first param, if any
                    builder.Append(',');
                }

                if (!string.IsNullOrEmpty(eventContext.Error))
                {
                    builder.Append(" error=\"");
                    builder.Append(eventContext.Error);
                    builder.Append('\"');
                }

                if (!string.IsNullOrEmpty(eventContext.ErrorDescription))
                {
                    if (!string.IsNullOrEmpty(eventContext.Error))
                    {
                        builder.Append(',');
                    }

                    builder.Append(" error_description=\"");
                    builder.Append(eventContext.ErrorDescription);
                    builder.Append('\"');
                }

                if (!string.IsNullOrEmpty(eventContext.ErrorUri))
                {
                    if (!string.IsNullOrEmpty(eventContext.Error) ||
                        !string.IsNullOrEmpty(eventContext.ErrorDescription))
                    {
                        builder.Append(',');
                    }

                    builder.Append(" error_uri=\"");
                    builder.Append(eventContext.ErrorUri);
                    builder.Append('\"');
                }

                Response.Headers.Append(HeaderNames.WWWAuthenticate, builder.ToString());
            }
        }

        /// <inheritdoc />
        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            var forbiddenContext = new ForbiddenContext(Context, Scheme, Options);

            if (Response.StatusCode == 403)
            {
                // No-op
            }
            else if (Response.HasStarted)
            {
                Logger.ForbiddenResponseHasStarted();
            }
            else
            {
                Response.StatusCode = 403;
            }

            return Events.Forbidden(forbiddenContext);
        }

        private static string CreateErrorDescription(Exception authFailure)
        {
            IReadOnlyCollection<Exception> exceptions;
            if (authFailure is AggregateException aggregateException)
            {
                exceptions = aggregateException.InnerExceptions;
            }
            else
            {
                exceptions = new[] { authFailure };
            }

            var messages = new List<string>(exceptions.Count);

            foreach (var ex in exceptions)
            {
                // Order sensitive, some of these exceptions derive from others
                // and we want to display the most specific message possible.
                string? message = ex switch
                {
                    SecurityTokenInvalidAudienceException stia => $"The audience '{stia.InvalidAudience ?? "(null)"}' is invalid",
                    SecurityTokenInvalidIssuerException stii => $"The issuer '{stii.InvalidIssuer ?? "(null)"}' is invalid",
                    SecurityTokenNoExpirationException _ => "The token has no expiration",
                    SecurityTokenInvalidLifetimeException stil => "The token lifetime is invalid; NotBefore: "
                        + $"'{stil.NotBefore?.ToString(CultureInfo.InvariantCulture) ?? "(null)"}'"
                        + $", Expires: '{stil.Expires?.ToString(CultureInfo.InvariantCulture) ?? "(null)"}'",
                    SecurityTokenNotYetValidException stnyv => $"The token is not valid before '{stnyv.NotBefore.ToString(CultureInfo.InvariantCulture)}'",
                    SecurityTokenExpiredException ste => $"The token expired at '{ste.Expires.ToString(CultureInfo.InvariantCulture)}'",
                    IntrospectionInvalidResponseException iire => "Invalid content on introspection response.",
                    IntrospectionInactiveResponseException iire => "The provided token was invalid or has been disabled.",
                    IntrospectionEndpintMissingException ieme => $"No introspection endpoint present in openid metadata for authority {ieme.Authority}. Use Options.Configuration.IntrospectionEndpoint to override the meatadata.",
                    IntrospectionResponseStausCodeException irsce => $"The introspection was unsuccessful. Response status code: {irsce.StatusCode}",
                    _ => null,
                };

                if (message is not null)
                {
                    messages.Add(message);
                }
            }

            return string.Join("; ", messages);
        }

        private void ValidatePayload(JwtPayload payload, OpenIdConnectConfiguration config, ValidationParameters validationParameters)
        {
            if (validationParameters.ValidateIssuer)
            {
                if (payload.Iss == null)
                {
                    throw new SecurityTokenInvalidIssuerException();
                }
                else
                {
                    bool isValid = false;
                    foreach (var issuer in new[] { config.Issuer }.Concat(validationParameters.Issuers))
                    {
                        if (issuer.Equals(payload.Iss, StringComparison.OrdinalIgnoreCase))
                        {
                            isValid = true;
                            break;
                        }
                    }

                    if (!isValid)
                    {
                        throw new SecurityTokenInvalidIssuerException();
                    }
                }
            }

            if (validationParameters.ValidateAudience)
            {
                if (payload.Aud == null)
                {
                    throw new SecurityTokenInvalidIssuerException();
                }
                else
                {
                    bool isValid = false;
                    foreach (var audience in validationParameters.Audiences)
                    {
                        foreach (var tokenAudience in payload.Aud)
                        {
                            if (audience.Equals(tokenAudience, StringComparison.OrdinalIgnoreCase))
                            {
                                isValid = true;
                                break;
                            }
                        }

                        if (isValid)
                        {
                            break;
                        }
                    }

                    if (!isValid)
                    {
                        throw new SecurityTokenInvalidAudienceException();
                    }
                }
            }

            if (validationParameters.ValidateLifetime)
            {
                if (!payload.Expiration.HasValue)
                {
                    throw new SecurityTokenNoExpirationException();
                }

                var now = DateTime.UtcNow;

                if (payload.ValidFrom > now.Add(validationParameters.ClockSkew))
                {
                    throw new SecurityTokenNotYetValidException { NotBefore = payload.ValidFrom };
                }

                if (payload.ValidTo.Add(validationParameters.ClockSkew) < now)
                {
                    throw new SecurityTokenExpiredException { Expires = payload.ValidTo };
                }
            }
        }
    }
}
