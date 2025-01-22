using System;

namespace Codeworx.Hosting.Features
{
    public class WebHostingFeature : IHostingFeature
    {
        public Type[] Dependencies { get; } = new Type[] { };

        public int SortOrder { get; } = 1;
    }
}