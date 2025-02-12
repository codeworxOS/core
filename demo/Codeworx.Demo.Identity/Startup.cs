﻿using Codeworx.Demo.Swagger;
using Codeworx.Extensions.DependencyInjection;
using Codeworx.Hosting;
using Codeworx.Hosting.Features;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: ServiceConfiguration(typeof(Codeworx.Demo.Identity.Startup))]
[assembly: HostingFeature(typeof(IdentityFeature))]

namespace Codeworx.Demo.Identity
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
            services.AddWebHostingFeature<IdentityFeature>(
                p =>
                {
                    p.Services.AddTransient<IStartupEvent, MigrationIdentityStartupEvent>();
                    p.Services.AddCodeworxIdentity()
                        .UseDbContextSqlite("Data Source=code_demo.sqlite");
                },
                p => p.UseCodeworxIdentity());

            services.AddHostingFeature<SwaggerFeature>(
            p =>
            {
                p.Services.AddSingleton<ISwaggerTitle, OverrideSwaggerTitle>();
            });
        }

        private class OverrideSwaggerTitle : ISwaggerTitle
        {
            public string Title => "Override Title";
        }
    }
}