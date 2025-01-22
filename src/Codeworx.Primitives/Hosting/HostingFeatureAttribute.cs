using System;
using System.Linq;

namespace Codeworx.Hosting
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class HostingFeatureAttribute : Attribute
    {
        private readonly Type[] _features;

        public HostingFeatureAttribute(params Type[] features)
        {
            _features = features;

            if (_features.Any(p => !p.GetInterfaces().Contains(typeof(IHostingFeature))))
            {
                throw new NotSupportedException("Unsupported featrue type configured. A HostingFeature class must implement the IHostingFeature interface.");
            }
        }

        public Type[] Features => _features;
    }
}