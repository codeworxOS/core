using System;
using Codeworx.Hosting;

namespace Codeworx.Demo.Swagger
{
    public class SwaggerFeature : IHostingFeature
    {
        public Type[] Dependencies => [];

        public int SortOrder => 20;
    }
}