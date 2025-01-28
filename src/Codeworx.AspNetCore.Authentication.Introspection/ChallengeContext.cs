using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Codeworx.AspNetCore.Authentication.Introspection
{
    /// <summary>
    /// A <see cref="PropertiesContext{TOptions}"/> when access to a resource authenticated using introspection is challenged.
    /// </summary>
    public class ChallengeContext : PropertiesContext<IntrospectionOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeContext" /> class.
        /// </summary>
        /// <inheritdoc />
        public ChallengeContext(
            HttpContext context,
            AuthenticationScheme scheme,
            IntrospectionOptions options,
            AuthenticationProperties properties)
            : base(context, scheme, options, properties)
        {
        }

        /// <summary>
        /// Gets or sets any failures encountered during the authentication process.
        /// </summary>
        public Exception? AuthenticateFailure { get; set; }

        /// <summary>
        /// Gets or sets the "error" value returned to the caller as part
        /// of the WWW-Authenticate header. This property may be null when
        /// <see cref="IntrospectionOptions.IncludeErrorDetails"/> is set to <c>false</c>.
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// Gets or sets the "error_description" value returned to the caller as part
        /// of the WWW-Authenticate header. This property may be null when
        /// <see cref="IntrospectionOptions.IncludeErrorDetails"/> is set to <c>false</c>.
        /// </summary>
        public string? ErrorDescription { get; set; }

        /// <summary>
        /// Gets or sets the "error_uri" value returned to the caller as part of the
        /// WWW-Authenticate header. This property is always null unless explicitly set.
        /// </summary>
        public string? ErrorUri { get; set; }

        /// <summary>
        /// Gets a value indicating whether any default logic for this challenge is skipped.
        /// </summary>
        public bool Handled { get; private set; }

        /// <summary>
        /// Skips any default logic for this challenge.
        /// </summary>
        public void HandleResponse() => Handled = true;
    }
}
