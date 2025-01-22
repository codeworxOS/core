namespace Codeworx.Hosting
{
    public interface IHostingFeatureProcessor<out THostingFeature>
        where THostingFeature : IHostingFeature
    {
        void ApplyServices(IHostingContext context, IServiceBuilder builder);
    }
}