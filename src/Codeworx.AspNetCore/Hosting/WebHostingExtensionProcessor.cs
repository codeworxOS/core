using System.Collections.Generic;
using System.Linq;
using Codeworx.Hosting;
using Codeworx.Hosting.Features;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Codeworx.AspNetCore.Hosting
{
    public class WebHostingExtensionProcessor : IHostExtensionProcessor<WebHostingFeature>
    {
        public void Extend(IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                 .Configure((ctx2, app) =>
                {
                    var processors = app.ApplicationServices.GetRequiredService<IReadOnlyList<IHostingFeatureProcessor<IHostingFeature>>>();

                    var wrapper = new WebHostAppRegistrationBuilder(app);

                    foreach (var item in processors.OfType<IWebHostingFeatureProcessor<IHostingFeature>>())
                    {
                        item.BuildPipeline(wrapper);
                    }

                    if (wrapper.DataSources.Any())
                    {
                        app.UseEndpoints(_ => { });
                    }
                });
            });
        }
    }
}