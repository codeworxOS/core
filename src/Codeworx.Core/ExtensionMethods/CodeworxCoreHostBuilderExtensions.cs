using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Extensions.DependencyInjection;
using Codeworx.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting
{
    public static class CodeworxCoreHostBuilderExtensions
    {
        public static Task ApplyConfigurationAsync(this IHostBuilder builder, IConfiguration configuration, HostingOptions options)
        {
            options = options ?? throw new ArgumentNullException(nameof(options));

            return ApplyConfigurationAsync(builder, configuration, options, new Type[] { });
        }

        public static Task ApplyConfigurationAsync(this IHostBuilder builder, IConfiguration configuration, HostingOptions options, params Type[] excludeFeatures)
        {
            var featureHostingBuilder = new FeatureHostingBuilder(options, excludeFeatures);
            var featureHosting = featureHostingBuilder.Build(configuration);

            builder.ConfigureServices((ctx, services) =>
            {
                var hostingContext = new HostingContext(featureHosting.FeatureAssemblies, featureHosting.Plugins, ctx.Configuration, ctx.HostingEnvironment.IsDevelopment());

                // get all features used by the plugins
                foreach (var item in featureHosting.Processors)
                {
                    item.ApplyServices(hostingContext, new HostServiceBuilder(services, ctx));
                }

                services.AddFromAssemblies(hostingContext, hostingContext.Plugins);

                services.AddSingleton<IHostedService, AsyncShutdownHandler>();
                services.AddSingleton(featureHosting.Processors);
            });

            foreach (var item in featureHosting.ExtensionProcessors)
            {
                item.Extend(builder);
            }

            return Task.CompletedTask;
        }

        public static async Task<IHost> ConfigureAsync(this IHostBuilder builder, IConfiguration configuration, HostingOptions options)
        {
            options = options ?? throw new ArgumentNullException(nameof(options));

            await builder.ApplyConfigurationAsync(configuration, options);

            builder.ConfigureServices((ctx, services) =>
            {
                if (ctx.HostingEnvironment.IsDevelopment() && options.ThrowIncompatileServiceLifetimeException)
                {
                    var incompatible = services.Where(p => p.Lifetime == ServiceLifetime.Transient)
                            .Where(p => p.ImplementationType != null && p.ImplementationType.GetInterfaces().Contains(typeof(IDisposable)))
                            .Where(p => p.ImplementationType!
                                            .GetConstructors().All(x => !x.GetParameters().Any(y => y.ParameterType == typeof(IServiceLifecycle))))
                            .ToList();

                    if (incompatible.Any())
                    {
                        throw new IncompatibleServiceLifetimeException(incompatible.Select(p => p.ImplementationType!));
                    }
                }
            });

            var app = builder.Build();

            return app;
        }

        public static async Task HostAsync(this IHostBuilder builder, Func<IHostBuilder> hostBuilderFactory)
        {
            var configHost = hostBuilderFactory().ConfigureHostConfiguration(p => DefaultHostConfiguration(p, null)).Build();
            var config = configHost.Services.GetRequiredService<IConfiguration>();
            var options = new HostingOptions();
            config.GetSection("Hosting").Bind(options);

            var app = await builder.ConfigureAsync(config, options);
            await app.InitializeAsync();
            await app.RunAsync();
        }

        public static async Task HostAsync(this IHostBuilder builder, string[] args)
        {
            await builder.HostAsync(() => Host.CreateDefaultBuilder(args));
        }

        private static void DefaultHostConfiguration(IConfigurationBuilder builder, string? prefix)
        {
            builder
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("hostsettings.json", true)
                .AddEnvironmentVariables(prefix);
        }

        private class HostServiceBuilder : IServiceBuilder
        {
            private HostBuilderContext _ctx;

            public HostServiceBuilder(IServiceCollection services, HostBuilderContext ctx)
            {
                Services = services;
                _ctx = ctx;
            }

            public IConfiguration Configuration => _ctx.Configuration;

            public IServiceCollection Services { get; }
        }
    }
}