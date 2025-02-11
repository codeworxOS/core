using System;
using Codeworx.AspNetCore.Hosting;
using Codeworx.AspNetCore.Hosting.Features;
using Codeworx.AspNetCore.Routing;
using Codeworx.AspNetCore.SignalR;
using Codeworx.Hosting;
using Microsoft.AspNetCore.SignalR;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxAspNetCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddHubInfo<THub>(this IServiceCollection services, string pattern)
                                            where THub : Hub
        {
            return services.AddSingleton<IEndpointRouteInfo>(new HubInfo<THub>(pattern));
        }

        public static IServiceCollection AddWebHostingFeature<TFeature>(this IServiceCollection services, Action<IHostingContext, IServiceBuilder> applyServices, Action<IAppRegistrationBuilder> buildPipeline)
                    where TFeature : IHostingFeature, new()
        {
            return services.AddTransient<IHostingFeatureProcessor<TFeature>, DelegateWebFeatureProcessor<TFeature>>(sp => new DelegateWebFeatureProcessor<TFeature>(applyServices, buildPipeline));
        }

        public static IServiceCollection AddWebHostingFeature<TFeature>(this IServiceCollection services, Action<IServiceBuilder> applyServices, Action<IAppRegistrationBuilder> buildPipeline)
                            where TFeature : IHostingFeature, new()
        {
            return services.AddTransient<IHostingFeatureProcessor<TFeature>, DelegateWebFeatureProcessor<TFeature>>(sp => new DelegateWebFeatureProcessor<TFeature>((ctx, p) => applyServices(p), buildPipeline));
        }
    }
}