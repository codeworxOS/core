using System;

namespace Codeworx.Extension.DependencyInjection
{
    public interface IRootServiceProviderCheck
    {
        bool IsRoot(IServiceProvider provider);
    }
}