using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Codeworx.Hosting
{
    public class AsyncShutdownHandler : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public AsyncShutdownHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var events = _serviceProvider.GetServices<IShutdownEvent>();

            foreach (var evt in events)
            {
                await evt.ShutdownAsync();
            }
        }
    }
}