using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Codeworx.AspNetCore.Hosting
{
    public interface IAppRegistrationBuilder : IApplicationBuilder, IEndpointRouteBuilder
    {
        IServiceProvider Services { get; }
    }
}