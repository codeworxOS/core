using System;

namespace Codeworx.Hosting.Features
{
    public class ControllersFeature : IHostingFeature
    {
        public Type[] Dependencies { get; } = new Type[] { typeof(RoutingFeature) };

        public int SortOrder => 50;
    }
}