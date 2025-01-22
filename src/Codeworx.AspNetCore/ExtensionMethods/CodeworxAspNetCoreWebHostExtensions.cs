using System.Threading.Tasks;
using Codeworx.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore
{
    public static class CodeworxAspNetCoreWebHostExtensions
    {
        public static async Task InitializeAsync(this IWebHost app)
        {
            var events = app.Services.GetServices<IStartupEvent>();

            foreach (var startupEvent in events)
            {
                await startupEvent.StartAsync();
            }

            foreach (var startup in app.Services.GetServices<IStartupInitializer>())
            {
                startup.Initialize();
            }
        }
    }
}