using System;

namespace Codeworx.Hosting.Features
{
    public class AuthorizationFeature : IHostingFeature
    {
        public Type[] Dependencies { get; } = new Type[] { typeof(WebHostingFeature) };

        public int SortOrder => 30;
    }
}