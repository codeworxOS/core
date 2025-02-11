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
    public static class CodeworxAspNetCoreWebHostBuilderExtensions
    {
        public static Task ApplyConfigurationAsync(this IWebHostBuilder builder, HostingOptions? options)
        {
            return ApplyConfigurationAsync(builder, options, new Type[] { });
        }

        public static Task ApplyConfigurationAsync(this IWebHostBuilder builder, HostingOptions? options, params Type[] excludeFeatures)
        {
            ////builder.UseDefaultServiceProvider(p => p.ValidateOnBuild = true);

            builder.ConfigureServices((ctx, services) =>
            {
                if (options == null)
                {
                    // Load HostingOptions from configuration if not provided as parameter.
                    var section = ctx.Configuration.GetSection("Hosting");
                    options = new HostingOptions();
                    section?.Bind(options);
                }

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

                var hostingContext = new HostingContext(featureAssemblies, plugins, ctx.Configuration, ctx.HostingEnvironment.IsDevelopment());

                // get all features used by the plugins
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

                var hostingServices = new ServiceCollection();
                AddAssemblies(hostingContext, hostingContext.FeatureAssembles, hostingServices);

                using (var provider = hostingServices.BuildServiceProvider(true))
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
                        item.ApplyServices(hostingContext, new WebHostServiceBuilder(services, ctx));
                    }

                    AddAssemblies(hostingContext, hostingContext.Plugins, services);

                    services.AddSingleton<IHostedService, AsyncShutdownHandler>();
                    services.AddSingleton(processors);
                }
            });

            builder.Configure((ctx2, app) =>
            {
                var processors = app.ApplicationServices.GetRequiredService<List<IHostingFeatureProcessor<IHostingFeature>>>()
                                        .OfType<IWebHostingFeatureProcessor<IHostingFeature>>();

                var wrapper = new WebHostAppRegistrationBuilder(app);

                foreach (var item in processors)
                {
                    item.BuildPipeline(wrapper);
                }

                if (wrapper.DataSources.Any())
                {
                    app.UseEndpoints(_ => { });
                }
            });

            return Task.CompletedTask;
        }

        public static async Task<IWebHost> ConfigureAsync(this IWebHostBuilder builder)
        {
            return await builder.ConfigureAsync(null);
        }

        public static async Task<IWebHost> ConfigureAsync(this IWebHostBuilder builder, HostingOptions? options)
        {
            await builder.ApplyConfigurationAsync(options);

            var app = builder.Build();

            return app;
        }

        public static async Task HostAsync(this IWebHostBuilder builder)
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
                // add feature to list and call the method recoursively for dependent features.
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

        private class WebHostServiceBuilder : IServiceBuilder
        {
            private WebHostBuilderContext _ctx;

            public WebHostServiceBuilder(IServiceCollection services, WebHostBuilderContext ctx)
            {
                Services = services;
                _ctx = ctx;
            }

            public IConfiguration Configuration => _ctx.Configuration;

            public IServiceCollection Services { get; }
        }
    }
}