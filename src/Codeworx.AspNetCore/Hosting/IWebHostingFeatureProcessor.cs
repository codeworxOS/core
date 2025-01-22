using Codeworx.Hosting;

namespace Codeworx.AspNetCore.Hosting
{
    public interface IWebHostingFeatureProcessor<out THostingFeature> : IHostingFeatureProcessor<THostingFeature>
        where THostingFeature : IHostingFeature
    {
        void BuildPipeline(IAppRegistrationBuilder app);
    }
}