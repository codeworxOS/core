using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;

namespace Codeworx.AspNetCore.Authentication.Introspection
{
    public interface IIntrospectionResponseCache
    {
        Task<JwtPayload> GetOrAddAsync(string token, Func<string, CancellationToken, Task<JwtPayload>> payloadFactory, CancellationToken cancellation);
    }
}
