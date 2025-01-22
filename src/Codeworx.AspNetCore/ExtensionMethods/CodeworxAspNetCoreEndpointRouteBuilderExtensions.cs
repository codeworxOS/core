using System.Collections.Generic;
using Codeworx.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Builder
{
    public static class CodeworxAspNetCoreEndpointRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder MapRoutes(this IEndpointRouteBuilder builder, IEnumerable<IEndpointRouteInfo> routeInfos)
        {
            foreach (var routeInfo in routeInfos)
            {
                routeInfo.Map(builder);
            }

            return builder;
        }
    }
}