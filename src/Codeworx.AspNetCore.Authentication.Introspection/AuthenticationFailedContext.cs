using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Codeworx.AspNetCore.Authentication.Introspection
{
    public class AuthenticationFailedContext : ResultContext<IntrospectionOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationFailedContext"/> class.
        /// </summary>
        /// <inheritdoc />
        public AuthenticationFailedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            IntrospectionOptions options)
            : base(context, scheme, options)
        {
        }

        /// <summary>
        /// Gets or sets the exception associated with the authentication failure.
        /// </summary>
        public Exception Exception { get; set; } = default!;
    }
}
