using Codeworx.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;

namespace Codeworx.AspNetCore.SignalR
{
    public class HubInfo<THub> : IEndpointRouteInfo
        where THub : Hub
    {
        private readonly string _pattern;

        public HubInfo(string pattern)
        {
            _pattern = pattern;
        }

        public void Map(IEndpointRouteBuilder endpoint)
        {
            endpoint.MapHub<THub>(_pattern);
        }
    }
}
