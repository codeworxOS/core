using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Extensions.DependencyInjection
{
    public interface IServiceConfiguration
    {
        void Configure(IServiceCollection services);
    }
}
