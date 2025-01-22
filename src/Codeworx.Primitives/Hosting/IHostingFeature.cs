using System;

namespace Codeworx.Hosting
{
    public interface IHostingFeature
    {
        Type[] Dependencies { get; }

        int SortOrder { get; }
    }
}