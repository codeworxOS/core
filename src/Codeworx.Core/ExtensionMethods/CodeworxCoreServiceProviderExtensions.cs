using System;
using Codeworx.Extension.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxCoreServiceProviderExtensions
    {
        public static bool IsRoot(this IServiceProvider serviceProvider)
        {
            var check = serviceProvider.GetRequiredService<IRootServiceProviderCheck>();

            return check.IsRoot(serviceProvider);
        }
    }
}