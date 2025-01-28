using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Codeworx.AspNetCore.Authentication.Introspection.Cache
{
    public class IntrospectionResponseCache : IIntrospectionResponseCache, IDisposable
    {
        private readonly IDisposable _subscription;
        private bool _disposedValue;
        private IDistributedCache _distributedCache;
        private IntrospectionOptions _options;

        public IntrospectionResponseCache(IDistributedCache distributedCache, IOptionsMonitor<IntrospectionOptions> monitor)
        {
            _distributedCache = distributedCache;
            _subscription = monitor.OnChange(p => _options = p)!;
            _options = monitor.CurrentValue;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task<JwtPayload> GetOrAddAsync(string token, Func<string, CancellationToken, Task<JwtPayload>> payloadFactory, CancellationToken cancellation)
        {
            var cacheResponse = await _distributedCache.GetStringAsync(token, cancellation);

            if (cacheResponse == null)
            {
                var payload = await payloadFactory(token, cancellation);

                var json = payload.SerializeToJson();
                var expiresIn = _options.CacheDuration;

                var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiresIn };

                await _distributedCache.SetStringAsync(token, json, options, cancellation);

                return payload;
            }
            else
            {
                return JwtPayload.Deserialize(cacheResponse);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _subscription.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}
