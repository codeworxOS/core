using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;

namespace Codeworx.AspNetCore.Hosting
{
    public class WebApplicationAppRegistrationBuilder : IAppRegistrationBuilder
    {
        private readonly IApplicationBuilder _app;
        private readonly IEndpointRouteBuilder _endpoint;

        public WebApplicationAppRegistrationBuilder(WebApplication app)
        {
            _app = app;
            _endpoint = app;
        }

        public IServiceProvider ApplicationServices { get => _app.ApplicationServices; set => _app.ApplicationServices = value; }

        public ICollection<EndpointDataSource> DataSources => _endpoint.DataSources;

        public IDictionary<string, object?> Properties => _app.Properties;

        public IFeatureCollection ServerFeatures => _app.ServerFeatures;

        public IServiceProvider ServiceProvider => _endpoint.ServiceProvider;

        public IServiceProvider Services => _app.ApplicationServices;

        public RequestDelegate Build()
        {
            return _app.Build();
        }

        public IApplicationBuilder CreateApplicationBuilder()
        {
            return _endpoint.CreateApplicationBuilder();
        }

        public IApplicationBuilder New()
        {
            return _app.New();
        }

        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            return _app.Use(middleware);
        }
    }
}