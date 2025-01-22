using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace Codeworx.Hosting
{
    public class FeatureHosting
    {
        public FeatureHosting(
            IEnumerable<Assembly> featureAssemblies,
            IEnumerable<Assembly> plugins,
            IEnumerable<IHostingFeatureProcessor<IHostingFeature>> processors,
            List<IHostExtensionProcessor<IHostingFeature>> extensionProcessors)
        {
            FeatureAssemblies = featureAssemblies.ToImmutableList();
            Plugins = plugins.ToImmutableList();
            Processors = processors.ToImmutableList();
            ExtensionProcessors = extensionProcessors.ToImmutableList();
        }

        public IReadOnlyList<IHostExtensionProcessor<IHostingFeature>> ExtensionProcessors { get; }

        public IReadOnlyList<Assembly> FeatureAssemblies { get; }

        public IReadOnlyList<Assembly> Plugins { get; }

        public IReadOnlyList<IHostingFeatureProcessor<IHostingFeature>> Processors { get; }
    }
}