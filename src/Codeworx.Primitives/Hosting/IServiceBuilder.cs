using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Hosting
{
    public interface IServiceBuilder
    {
        IConfiguration Configuration { get; }

        IServiceCollection Services { get; }
    }
}