using System;

namespace Codeworx.Extensions.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class ServiceConfigurationAttribute : Attribute
    {
        public ServiceConfigurationAttribute(Type configurationType)
        {
            ConfigurationType = configurationType;
        }

        public Type ConfigurationType { get; }
    }
}
