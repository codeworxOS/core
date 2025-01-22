using System;

namespace Codeworx.Hosting.Features
{
    public class IntrospectionFeature : IHostingFeature
    {
        public Type[] Dependencies { get; } = new Type[] { typeof(AuthenticationFeature), typeof(AuthorizationFeature) };

        public int SortOrder => 15;
    }
}