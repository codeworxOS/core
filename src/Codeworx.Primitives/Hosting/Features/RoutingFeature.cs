using System;

namespace Codeworx.Hosting.Features
{
    public class RoutingFeature : IHostingFeature
    {
        public Type[] Dependencies { get; } = new Type[] { typeof(WebHostingFeature) };

        public int SortOrder => 20;
    }
}