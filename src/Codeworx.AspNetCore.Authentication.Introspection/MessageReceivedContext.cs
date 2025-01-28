using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Codeworx.AspNetCore.Authentication.Introspection
{
    public class MessageReceivedContext : ResultContext<IntrospectionOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivedContext"/> class.
        /// </summary>
        /// <inheritdoc />
        public MessageReceivedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            IntrospectionOptions options)
            : base(context, scheme, options)
        {
        }

        /// <summary>
        /// Gets or sets the Bearer Token. This will give the application an opportunity to retrieve a token from an alternative location.
        /// </summary>
        public string? Token { get; set; }
    }
}
