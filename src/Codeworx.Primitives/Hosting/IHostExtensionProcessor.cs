using Microsoft.Extensions.Hosting;

namespace Codeworx.Hosting
{
    public interface IHostExtensionProcessor<out THostingFeature>
        where THostingFeature : IHostingFeature
    {
        void Extend(IHostBuilder hostBuilder);
    }
}