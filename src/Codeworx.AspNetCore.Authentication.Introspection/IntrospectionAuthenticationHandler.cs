using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeworx.AspNetCore.Authentication.Introspection
{
    public class IntrospectionAuthenticationHandler : AuthenticationHandler<IntrospectionOptions>
    {
        public IntrospectionAuthenticationHandler(
            IOptionsMonitor<IntrospectionOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(
                  options,
                  logger,
                  encoder)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authority = Options.Authority;

            var config = await Options.ConfigurationManager!.GetConfigurationAsync(default);

            return AuthenticateResult.Fail("whatever");
        }
    }
}
