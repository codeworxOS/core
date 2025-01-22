using Microsoft.AspNetCore.Routing;

namespace Codeworx.AspNetCore.Routing
{
    public interface IEndpointRouteInfo
    {
        void Map(IEndpointRouteBuilder endpoint);
    }
}
