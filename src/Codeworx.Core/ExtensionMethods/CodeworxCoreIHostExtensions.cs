using System.Threading.Tasks;
using Codeworx.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting
{
    public static class CodeworxCoreIHostExtensions
    {
        public static async Task InitializeAsync(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                foreach (var startup in scope.ServiceProvider.GetServices<IStartupEvent>())
                {
                    await startup.StartAsync();
                }
            }

            foreach (var item in host.Services.GetServices<IStartupInitializer>())
            {
                item.Initialize();
            }
        }
    }
}