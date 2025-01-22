using Codeworx.AspNetCore;
using Codeworx.AspNetCore.Authentication.Introspection;
using Codeworx.AspNetCore.Authentication.Introspection.ExtensionMethods;
using Codeworx.AspNetCore.Configuration;
using Codeworx.AspNetCore.Hosting;
using Codeworx.AspNetCore.Mvc.Filters;
using Codeworx.AspNetCore.Routing;
using Codeworx.Extensions.DependencyInjection;
using Codeworx.Hosting.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: ServiceConfiguration(typeof(Startup))]

namespace Codeworx.AspNetCore
{
    public class Startup : IServiceConfiguration
    {
        public void Configure(IServiceCollection services)
        {
            services.AddHostExtension<WebHostingFeature, WebHostingExtensionProcessor>();
            services.AddHostingFeature<WebHostingFeature>(p => p.Services.AddHttpContextAccessor());
            services.AddWebHostingFeature<RoutingFeature>(p => p.Services.AddRouting(), p => p.UseRouting());
            services.AddWebHostingFeature<AuthenticationFeature>(
                p =>
                {
                    p.Services.AddAuthorization();
                },
                p =>
                {
                    p.UseAuthentication();
                });

            services.AddWebHostingFeature<SignalRFeature>(
                p =>
                {
                    p.Services.AddSignalR();
                },
                p =>
                {
                    p.MapRoutes(p.Services.GetServices<IEndpointRouteInfo>());
                });

            services.AddWebHostingFeature<AuthorizationFeature>(
                                    p => p.Services
                                        ////.AddSingleton<IAuthorizationHandler, SadeSessionInfoAuthorizationHandler>()
                                        ////.AddSingleton<IAuthorizationHandler, OperationPermissionAuthorizationHandler>()
                                        .AddAuthorization(options => options.FallbackPolicy = GetFallbackPolicy()),
                                    p => p.UseAuthorization());

            services.AddWebHostingFeature<ControllersFeature>(
                (ctx, p) =>
                {
                    var mvc = p.Services.AddControllers(options => options.Filters.Add(new UnhandledErrorFilter(ctx.IsDevelopment())))
                            .AddRestContract();

                    foreach (var item in ctx.Plugins)
                    {
                        mvc.AddApplicationPart(item);
                    }
                },
                p => p.MapControllers());

            services.AddWebHostingFeature<IntrospectionFeature>(
                (ctx, p) =>
                {
                    var auth = p.Services.AddAuthentication();
                    auth.AddIntrospection();

                    ////auth.AddOAuth2Introspection(Constants.SadeAuthenticationSchema, options =>
                    ////{
                    ////    var retriver = options.TokenRetriever;

                    ////    var authority = p.Configuration.GetValue<string>("Sade:Identity:Authority");
                    ////    options.Authority = authority;
                    ////    options.SaveToken = true;
                    ////    options.EnableCaching = true;
                    ////    options.ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader;
                    ////    options.ClientId = p.Configuration.GetValue<string>("Sade:Identity:ClientId");
                    ////    options.ClientSecret = p.Configuration.GetValue<string>("Sade:Identity:ClientSecret");
                    ////    options.TokenRetriever = ctx =>
                    ////    {
                    ////        if (ctx.Path.StartsWithSegments("/hub") && ctx.Query.TryGetValue(Constants.AccessToken, out var token))
                    ////        {
                    ////            return token;
                    ////        }

                    ////        return retriver(ctx);
                    ////    };
                    ////});
                },
                p => p.UseAuthentication());

            ////services.AddWebHostingFeature<IntrospectionFeature>(
            ////    (ctx, p) =>
            ////    {
            ////        var auth = p.Services.AddAuthentication();

            ////        auth.AddOAuth2Introspection(Constants.SadeAuthenticationSchema, options =>
            ////        {
            ////            var retriver = options.TokenRetriever;

            ////            var authority = p.Configuration.GetValue<string>("Sade:Identity:Authority");
            ////            options.Authority = authority;
            ////            options.SaveToken = true;
            ////            options.EnableCaching = true;
            ////            options.ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader;
            ////            options.ClientId = p.Configuration.GetValue<string>("Sade:Identity:ClientId");
            ////            options.ClientSecret = p.Configuration.GetValue<string>("Sade:Identity:ClientSecret");
            ////            options.TokenRetriever = ctx =>
            ////            {
            ////                if (ctx.Path.StartsWithSegments("/hub") && ctx.Query.TryGetValue(Constants.AccessToken, out var token))
            ////                {
            ////                    return token;
            ////                }

            ////                return retriver(ctx);
            ////            };
            ////        });
            ////    },
            ////    p => p.UseAuthentication());

            services.AddWebHostingFeature<CorsFeature>(
                    (ctx, p) =>
                    {
                        var cors = new CorsOptions();
                        var section = p.Configuration.GetSection("Cors");
                        section.Bind(cors);

                        p.Services.AddCors(x => x.AddDefaultPolicy(builder => builder
                                                            .WithOrigins(cors.Origins.ToArray())
                                                            .WithMethods(cors.Methods.ToArray())
                                                            .WithHeaders(cors.Headers.ToArray())));
                    },
                    p => p.UseCors());
        }

        private AuthorizationPolicy GetFallbackPolicy()
        {
            return new AuthorizationPolicyBuilder(IntrospectionDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
        }
    }
}
