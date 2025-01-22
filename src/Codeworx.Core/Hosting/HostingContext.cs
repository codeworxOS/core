using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Codeworx.Hosting
{
    public class HostingContext : IHostingContext
    {
        private readonly bool _isDevelopment;

        public HostingContext(IEnumerable<Assembly> featureAssemblies, IEnumerable<Assembly> plugins, IConfiguration configuration, bool isDevelopment)
        {
            FeatureAssembles = featureAssemblies.ToImmutableList();
            Plugins = plugins.ToImmutableList();
            Configuration = configuration;
            _isDevelopment = isDevelopment;
        }

        public IConfiguration Configuration { get; }

        public IReadOnlyCollection<Assembly> FeatureAssembles { get; }

        public IReadOnlyCollection<Assembly> Plugins { get; }

        public bool IsDevelopment() => _isDevelopment;
    }
}