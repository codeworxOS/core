using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Codeworx.AspNetCore.Authentication.Introspection.Cache
{
    public class IntrospectionResponseCache : IIntrospectionResponseCache
    {
        private IDistributedCache _distributedCache;

        public IntrospectionResponseCache(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<JwtPayload> GetOrAddAsync(string token, Func<string, CancellationToken, Task<JwtPayload>> payloadFactory, CancellationToken cancellation)
        {
            var cacheResponse = await _distributedCache.GetStringAsync(token, cancellation);

            if (cacheResponse == null)
            {
                var payload = await payloadFactory(token, cancellation);

                var json = payload.SerializeToJson();

                var expiresIn = TimeSpan.FromMinutes(10);

                var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiresIn };

                await _distributedCache.SetStringAsync(token, json, options, cancellation);

                return payload;
            }
            else
            {
                return JwtPayload.Deserialize(cacheResponse);
            }
        }
    }
}
