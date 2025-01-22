using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Codeworx.AspNetCore.Hosting
{
    public sealed class WebHostAppRegistrationBuilder : IAppRegistrationBuilder, IApplicationBuilder, IEndpointRouteBuilder
    {
        internal const string GlobalEndpointRouteBuilderKey = "__GlobalEndpointRouteBuilder";

        private readonly List<EndpointDataSource> _dataSources = new List<EndpointDataSource>();
        private readonly IServiceProvider _service;

        internal WebHostAppRegistrationBuilder(IApplicationBuilder app)
        {
            _service = app.ApplicationServices;
            ApplicationBuilder = app;
            Logger = _service.GetRequiredService<ILoggerFactory>().CreateLogger(Environment.ApplicationName ?? nameof(WebHostAppRegistrationBuilder));

            Properties[GlobalEndpointRouteBuilderKey] = this;
        }

        IServiceProvider IApplicationBuilder.ApplicationServices
        {
            get => ApplicationBuilder.ApplicationServices;
            set => ApplicationBuilder.ApplicationServices = value;
        }

        public IConfiguration Configuration => _service.GetRequiredService<IConfiguration>();

        ICollection<EndpointDataSource> IEndpointRouteBuilder.DataSources => DataSources;

        public IWebHostEnvironment Environment => _service.GetRequiredService<IWebHostEnvironment>();

        public IHostApplicationLifetime Lifetime => _service.GetRequiredService<IHostApplicationLifetime>();

        public ILogger Logger { get; }

        IDictionary<string, object?> IApplicationBuilder.Properties => Properties;

        IFeatureCollection IApplicationBuilder.ServerFeatures => ServerFeatures;

        IServiceProvider IEndpointRouteBuilder.ServiceProvider => Services;

        public IServiceProvider Services => _service;

        internal IApplicationBuilder ApplicationBuilder { get; }

        internal ICollection<EndpointDataSource> DataSources => _dataSources;

        internal IDictionary<string, object?> Properties => ApplicationBuilder.Properties;

        internal IFeatureCollection ServerFeatures => _service.GetRequiredService<IServer>().Features;

        RequestDelegate IApplicationBuilder.Build() => ApplicationBuilder.Build();

        IApplicationBuilder IEndpointRouteBuilder.CreateApplicationBuilder() => ((IApplicationBuilder)this).New();

        IApplicationBuilder IApplicationBuilder.New()
        {
            var newBuilder = ApplicationBuilder.New();

            newBuilder.Properties.Remove(GlobalEndpointRouteBuilderKey);
            return newBuilder;
        }

        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            ApplicationBuilder.Use(middleware);
            return this;
        }
    }
}