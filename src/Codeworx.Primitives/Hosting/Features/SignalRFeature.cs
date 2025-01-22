using System;

namespace Codeworx.Hosting.Features
{
    public class SignalRFeature : IHostingFeature
    {
        public Type[] Dependencies { get; } = new Type[] { typeof(WebHostingFeature) };

        public int SortOrder { get; } = 1;
    }
}