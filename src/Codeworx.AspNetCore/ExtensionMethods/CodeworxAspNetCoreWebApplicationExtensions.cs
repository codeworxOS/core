using System.Threading.Tasks;
using Codeworx.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore
{
    public static class CodeworxAspNetCoreWebApplicationExtensions
    {
        public static async Task InitializeAsync(this WebApplication app)
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