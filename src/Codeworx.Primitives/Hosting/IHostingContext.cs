using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Codeworx.Hosting
{
    public interface IHostingContext
    {
        IConfiguration Configuration { get; }

        IReadOnlyCollection<Assembly> FeatureAssembles { get; }

        IReadOnlyCollection<Assembly> Plugins { get; }

        bool IsDevelopment();
    }
}