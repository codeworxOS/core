using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Codeworx.Hosting
{
    public class HostingOptions
    {
        public HostingOptions()
        {
            Plugins = new Dictionary<string, object>();

            FeatureAssemblies = new Dictionary<string, bool>();

            ThrowIncompatileServiceLifetimeException = true;
        }

        public Dictionary<string, bool> FeatureAssemblies { get; }

        public Dictionary<string, object> Plugins { get; }

        public bool ThrowIncompatileServiceLifetimeException { get; set; }

        public IEnumerable<Assembly> GetSortedPlugins()
        {
            var plugins = new Dictionary<int, HashSet<Assembly>>();

            foreach (var item in this.Plugins)
            {
                var sort = int.MaxValue;

                if (item.Value is bool boolValue)
                {
                    if (!boolValue)
                    {
                        continue;
                    }
                }
                else if (item.Value is int intValue)
                {
                    if (intValue <= 0)
                    {
                        continue;
                    }

                    sort = intValue;
                }
                else if (item.Value is string stringValue)
                {
                    if (int.TryParse(stringValue, out var intParsed))
                    {
                        if (intParsed <= 0)
                        {
                            continue;
                        }

                        sort = intParsed;
                    }
                    else if (bool.TryParse(stringValue, out var boolParsed))
                    {
                        if (!boolParsed)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                var assembly = Assembly.Load(new AssemblyName(item.Key));

                if (!plugins.ContainsKey(sort))
                {
                    plugins.Add(sort, new HashSet<Assembly>());
                }

                plugins[sort].Add(assembly);
            }

            var result = plugins.OrderBy(p => p.Key).SelectMany(p => p.Value).ToImmutableList();

            return result;
        }
    }
}