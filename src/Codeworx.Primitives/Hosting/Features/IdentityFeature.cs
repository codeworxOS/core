using System;

namespace Codeworx.Hosting.Features
{
    public class IdentityFeature : IHostingFeature
    {
        public Type[] Dependencies { get; } = new Type[] { typeof(CorsFeature) };

        public int SortOrder { get; } = 5;
    }
}