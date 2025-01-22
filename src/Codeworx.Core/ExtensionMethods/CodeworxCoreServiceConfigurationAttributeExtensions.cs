using System;
using Codeworx.Extensions.DependencyInjection;
using Codeworx.Hosting;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxCoreServiceConfigurationAttributeExtensions
    {
        public static IServiceConfiguration CreateInstance(this ServiceConfigurationAttribute configuration, IHostingContext ctx)
        {
            var constructor = configuration.ConfigurationType.GetConstructor(new[] { typeof(IConfiguration) });
            if (constructor != null)
            {
                return (IServiceConfiguration)Activator.CreateInstance(configuration.ConfigurationType, ctx.Configuration);
            }

            constructor = configuration.ConfigurationType.GetConstructor(new Type[] { });
            if (constructor != null)
            {
                return (IServiceConfiguration)Activator.CreateInstance(configuration.ConfigurationType);
            }

            throw new NotSupportedException($"No compatible consturctor was found for bootstrap class {configuration.ConfigurationType}.");
        }
    }
}