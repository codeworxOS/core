using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Codeworx.AspNetCore.Authentication.Introspection
{
    /// <summary>
    /// A <see cref="ResultContext{TOptions}"/> when access to a resource is forbidden.
    /// </summary>
    public class ForbiddenContext : ResultContext<IntrospectionOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenContext"/> class.
        /// </summary>
        /// <inheritdoc />
        public ForbiddenContext(
            HttpContext context,
            AuthenticationScheme scheme,
            IntrospectionOptions options)
            : base(context, scheme, options)
        {
        }
    }
}
