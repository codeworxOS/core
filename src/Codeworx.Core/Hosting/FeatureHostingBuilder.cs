using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Hosting
{
    public class FeatureHostingBuilder
    {
        private readonly Type[] _excludeFeatures;
        private readonly HostingOptions _options;

        public FeatureHostingBuilder(HostingOptions options, Type[] excludeFeatures)
        {
            _options = options;
            _excludeFeatures = excludeFeatures;
        }

        public FeatureHosting Build(IConfiguration configuration)
        {
            var featureAssemblies = new HashSet<Assembly>();

            foreach (var item in _options.FeatureAssemblies)
            {
                if (!item.Value)
                {
                    continue;
                }

                var assembly = Assembly.Load(new AssemblyName(item.Key));

                featureAssemblies.Add(assembly);
            }

            var plugins = _options.GetSortedPlugins();

            Dictionary<Type, IHostingFeature> features = new Dictionary<Type, IHostingFeature>();

            foreach (var item in plugins)
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

            foreach (var item in _excludeFeatures)
            {
                features.Remove(item);
            }

            var hostingServices = new ServiceCollection();

            var hostingContext = new HostingContext(featureAssemblies, plugins, configuration, false);

            hostingServices.AddFromAssemblies(hostingContext, featureAssemblies);

            var processors = new List<IHostingFeatureProcessor<IHostingFeature>>();
            var extensionProcessors = new List<IHostExtensionProcessor<IHostingFeature>>();

            using (var provider = hostingServices.BuildServiceProvider(true))
            using (var scope = provider.CreateScope())
            {
                foreach (var item in features.OrderBy(p => p.Value.SortOrder))
                {
                    var extensionProcessor = typeof(IHostExtensionProcessor<>).MakeGenericType(item.Key);
                    var extensions = scope.ServiceProvider.GetServices(extensionProcessor).OfType<IHostExtensionProcessor<IHostingFeature>>().ToList();
                    extensionProcessors.AddRange(extensions);

                    var processorType = typeof(IHostingFeatureProcessor<>).MakeGenericType(item.Key);
                    var processor = (IHostingFeatureProcessor<IHostingFeature>)scope.ServiceProvider.GetRequiredService(processorType);
                    processors.Add(processor);
                }
            }

            return new FeatureHosting(featureAssemblies, plugins, processors, extensionProcessors);
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
    }
}