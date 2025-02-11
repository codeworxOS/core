using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Codeworx.AspNetCore.Hosting;
using Codeworx.Extensions.DependencyInjection;
using Codeworx.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Builder
{
    public static class CodeworxAspNetCoreWebApplicationBuilderExtensions
    {
        public static async Task<WebApplication> ConfigureAsync(this WebApplicationBuilder builder)
        {
            var section = builder.Configuration.GetSection("Hosting");
            var options = new HostingOptions();
            section?.Bind(options);

            return await builder.ConfigureAsync(options);
        }

        public static Task<WebApplication> ConfigureAsync(this WebApplicationBuilder builder, HostingOptions options)
        {
            return ConfigureAsync(builder, options, new Type[] { });
        }

        public static Task<WebApplication> ConfigureAsync(this WebApplicationBuilder builder, HostingOptions options, params Type[] excludeFeatures)
        {
            using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Trace)
                .AddConsole());

            var logger = loggerFactory.CreateLogger<HostingContext>();

            var featureAssemblies = new HashSet<Assembly>();

            foreach (var item in options.FeatureAssemblies)
            {
                if (!item.Value)
                {
                    continue;
                }

                var assembly = Assembly.Load(new AssemblyName(item.Key));

                featureAssemblies.Add(assembly);
            }

            var plugins = options.GetSortedPlugins();

            Dictionary<Type, IHostingFeature> features = new Dictionary<Type, IHostingFeature>();

            var hostingContext = new HostingContext(featureAssemblies, plugins, builder.Configuration, builder.Environment.IsDevelopment());

            foreach (var item in hostingContext.Plugins)
            {
                var attribute = item.GetCustomAttribute<HostingFeatureAttribute>();
                if (attribute != null)
                {
                    foreach (var feature in attribute.Features)
                    {
                        AddFeatures(features, feature);
                    }
                }
            }

            foreach (var item in excludeFeatures)
            {
                features.Remove(item);
            }

            var services = new ServiceCollection();
            AddAssemblies(hostingContext, hostingContext.FeatureAssembles, services);

            using (var provider = services.BuildServiceProvider(true))
            using (var scope = provider.CreateScope())
            {
                var processors = new List<IHostingFeatureProcessor<IHostingFeature>>();

                foreach (var item in features.OrderBy(p => p.Value.SortOrder))
                {
                    var processorType = typeof(IHostingFeatureProcessor<>).MakeGenericType(item.Key);
                    var featureProcessors = (IEnumerable<IHostingFeatureProcessor<IHostingFeature>>)scope.ServiceProvider.GetServices(processorType);

                    if (!featureProcessors.Any())
                    {
                        throw new NotImplementedException($"No feature processor for feature {item.Key} found.");
                    }

                    foreach (var processor in featureProcessors)
                    {
                        processors.Add(processor);
                    }
                }

                foreach (var item in processors)
                {
                    item.ApplyServices(hostingContext, new WebApplicationBuilderServiceBuilder(builder));
                }

                AddAssemblies(hostingContext, hostingContext.Plugins, builder.Services);
                builder.Services.AddSingleton<IHostedService, AsyncShutdownHandler>();

                var app = builder.Build();

                foreach (var item in processors.OfType<IWebHostingFeatureProcessor<IHostingFeature>>())
                {
                    item.BuildPipeline(new WebApplicationAppRegistrationBuilder(app));
                }

                return Task.FromResult(app);
            }
        }

        public static async Task HostAsync(this WebApplicationBuilder builder)
        {
            var app = await builder.ConfigureAsync();
            await app.InitializeAsync();
            await app.RunAsync();
        }

        private static void AddAssemblies(IHostingContext ctx, IEnumerable<Assembly> assemblies, Microsoft.Extensions.DependencyInjection.IServiceCollection services)
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

        private static void AddFeatures(Dictionary<Type, IHostingFeature> features, Type feature)
        {
            if (!features.ContainsKey(feature))
            {
                if (Activator.CreateInstance(feature) is IHostingFeature hostingFeature)
                {
                    features.Add(feature, hostingFeature);

                    foreach (var dependency in hostingFeature.Dependencies)
                    {
                        AddFeatures(features, dependency);
                    }
                }
            }
        }

        private class WebApplicationBuilderServiceBuilder : IServiceBuilder
        {
            private WebApplicationBuilder _builder;

            public WebApplicationBuilderServiceBuilder(WebApplicationBuilder builder)
            {
                _builder = builder;
            }

            public IConfiguration Configuration => _builder.Configuration;

            public IServiceCollection Services => _builder.Services;
        }
    }
}