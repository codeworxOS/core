using Codeworx.Demo.PluginOne;
using Codeworx.Demo.PluginOne.Contract;
using Codeworx.Extensions.DependencyInjection;
using Codeworx.Hosting;
using Codeworx.Hosting.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: ServiceConfiguration(typeof(Startup))]
[assembly: HostingFeature(typeof(ControllersFeature), typeof(IntrospectionFeature))]

namespace Codeworx.Demo.PluginOne
{
    public class Startup : IServiceConfiguration
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void Configure(IServiceCollection services)
        {
            services.AddScoped<IPluginOneService, PluginOneService>();
        }
    }
}