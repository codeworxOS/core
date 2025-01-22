using System;
using Codeworx.Hosting;

namespace Codeworx.Hosting.Features
{
    public class DelegateFeatureProcessor<TFeature> : IHostingFeatureProcessor<TFeature>
        where TFeature : IHostingFeature, new()
    {
        private readonly Action<IHostingContext, IServiceBuilder> _applyServices;

        public DelegateFeatureProcessor(Action<IHostingContext, IServiceBuilder> applyServices)
        {
            _applyServices = applyServices;
        }

        public virtual void ApplyServices(IHostingContext context, IServiceBuilder builder)
        {
            _applyServices?.Invoke(context, builder);
        }
    }
}