using System;
using Codeworx.Hosting;
using Codeworx.Hosting.Features;

namespace Codeworx.AspNetCore.Hosting.Features
{
    public class DelegateWebFeatureProcessor<TFeature> : DelegateFeatureProcessor<TFeature>, IWebHostingFeatureProcessor<TFeature>
        where TFeature : IHostingFeature, new()
    {
        private readonly Action<IAppRegistrationBuilder> _buildPipeline;

        public DelegateWebFeatureProcessor(Action<IHostingContext, IServiceBuilder> applyServices, Action<IAppRegistrationBuilder> buildPipeline)
            : base(applyServices)
        {
            _buildPipeline = buildPipeline;
        }

        public virtual void BuildPipeline(IAppRegistrationBuilder app)
        {
            _buildPipeline?.Invoke(app);
        }
    }
}