using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Codeworx.Extensions.DependencyInjection;
using Codeworx.Hosting;
using Codeworx.Hosting.Features;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxCoreServiceCollectionExtensions
    {
        public static void AddFromAssemblies(this Microsoft.Extensions.DependencyInjection.IServiceCollection services, IHostingContext ctx, IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var configAttribute = assembly.GetCustomAttribute<ServiceConfigurationAttribute>();
                if (configAttribute != null)
                {
                    var config = configAttribute.CreateInstance(ctx);

                    if (config != null)
                    {
                        config.Configure(services);
                    }
                }
            }
        }

        public static IServiceCollection AddHostExtension<TFeature, TImplementation>(this IServiceCollection services)
            where TFeature : class, IHostingFeature
            where TImplementation : class, IHostExtensionProcessor<TFeature>
        {
            return services.AddTransient<IHostExtensionProcessor<TFeature>, TImplementation>();
        }

        public static IServiceCollection AddHostingFeature<TFeature>(this IServiceCollection services, Action<IServiceBuilder> applyServices)
                                 where TFeature : IHostingFeature, new()
        {
            return services.AddOrReplace<IHostingFeatureProcessor<TFeature>, DelegateFeatureProcessor<TFeature>>(ServiceLifetime.Transient, sp => new DelegateFeatureProcessor<TFeature>((ctx, p) => applyServices(p)));
        }

        public static IServiceCollection AddHostingFeature<TFeature>(this IServiceCollection services, Action<IHostingContext, IServiceBuilder> applyServices)
            where TFeature : IHostingFeature, new()
        {
            return services.AddOrReplace<IHostingFeatureProcessor<TFeature>, DelegateFeatureProcessor<TFeature>>(ServiceLifetime.Transient, sp => new DelegateFeatureProcessor<TFeature>(applyServices));
        }

        public static IServiceCollection AddOrReplace<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime, Func<TImplementation> factory)
            where TService : class
            where TImplementation : class, TService
        {
            return services.AddOrReplace(ServiceDescriptor.Describe(typeof(TService), sp => factory(), lifetime));
        }

        public static IServiceCollection AddOrReplace<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime, Func<IServiceProvider, TImplementation> factory)
            where TService : class
            where TImplementation : class, TService
        {
            return services.AddOrReplace(ServiceDescriptor.Describe(typeof(TService), factory, lifetime));
        }

        public static IServiceCollection AddOrReplace<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime)
        {
            return services.AddOrReplace(ServiceDescriptor.Describe(typeof(TService), typeof(TImplementation), lifetime));
        }

        public static IServiceCollection AddOrReplace(this IServiceCollection services, ServiceDescriptor descriptor)
        {
            var found = services.Where(p => p.ServiceType == descriptor.ServiceType).FirstOrDefault();

            if (found != null)
            {
                services.Remove(found);
            }

            services.Add(descriptor);

            return services;
        }
    }
}