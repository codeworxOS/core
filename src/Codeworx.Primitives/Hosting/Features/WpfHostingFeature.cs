using System;

namespace Codeworx.Hosting.Features
{
    public class WpfHostingFeature : IHostingFeature
    {
        public Type[] Dependencies { get; } = new Type[] { };

        public int SortOrder { get; } = 1;
    }
}