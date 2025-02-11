using System.Collections.Generic;
using Codeworx.Demo.Swagger;
using Codeworx.Extensions.DependencyInjection;
using Codeworx.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;

[assembly: ServiceConfiguration(typeof(Startup))]
[assembly: HostingFeature(typeof(SwaggerFeature))]

namespace Codeworx.Demo.Swagger
{
    public class Startup : IServiceConfiguration
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void Configure(IServiceCollection services)
        {
            services.AddWebHostingFeature<SwaggerFeature>(
                p => p.Services.AddOpenApiDocument((options, sp) =>
                {
                    options.Title = sp.GetService<ISwaggerTitle>()?.Title ?? "Codeworx Demo Services";

                    var url = p.Configuration.GetValue<string>("Authentication:Schemes:Introspection:Authority");

                    options.AddSecurity("oauth2", new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            AuthorizationCode = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = $"{url}openid10",
                                TokenUrl = $"{url}openid10/token",
                                Scopes = new Dictionary<string, string> { { "openid", "Api Access" } },
                            },
                        },
                    });
                    options.OperationProcessors.Add(new OperationSecurityScopeProcessor("oauth2"));
                }),
                p =>
                {
                    List<string> scopes = ["openid"];

                    p.UseSwaggerUi(options =>
                    {
                        options.OAuth2Client = new OAuth2ClientSettings
                        {
                            ClientId = "ec364b3996db4d82b12f4f8808ddbf42",
                            AppName = "Codeworx Demo - Swagger",
                            UsePkceWithAuthorizationCodeGrant = true,
                        };
                        scopes.ForEach(options.OAuth2Client.Scopes.Add);
                    });
                    p.UseOpenApi();
                });
        }
    }
}