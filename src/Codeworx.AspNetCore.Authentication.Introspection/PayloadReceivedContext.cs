using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Codeworx.AspNetCore.Authentication.Introspection
{
    public class PayloadReceivedContext : ResultContext<IntrospectionOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PayloadReceivedContext"/> class.
        /// </summary>
        public PayloadReceivedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            IntrospectionOptions options,
            JwtPayload payload)
            : base(context, scheme, options)
        {
            Payload = payload;
        }

        public JwtPayload Payload { get; set; }
    }
}