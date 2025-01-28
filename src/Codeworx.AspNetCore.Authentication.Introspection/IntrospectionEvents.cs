using System;
using System.Threading.Tasks;

namespace Codeworx.AspNetCore.Authentication.Introspection
{
    public class IntrospectionEvents
    {
        /// <summary>
        /// Gets or sets a function that is invoked if authentication fails during request processing. The exceptions will be re-thrown after this event unless suppressed.
        /// </summary>
        public Func<AuthenticationFailedContext, Task> OnAuthenticationFailed { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Gets or sets a function that is invoked if Authorization fails and results in a Forbidden response.
        /// </summary>
        public Func<ForbiddenContext, Task> OnForbidden { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Gets or sets a function that is invoked before a challenge is sent back to the caller.
        /// </summary>
        public Func<ChallengeContext, Task> OnChallenge { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Gets or sets a function that is invoked after a payload is received from the introspection endpoint.
        /// </summary>
        public Func<PayloadReceivedContext, Task> OnPayloadReceived { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Gets or sets a function that is invoked when a protocol message is first received.
        /// </summary>
        public Func<MessageReceivedContext, Task> OnMessageReceived { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Invoked if exceptions are thrown during request processing. The exceptions will be re-thrown after this event unless suppressed.
        /// </summary>
        public virtual Task AuthenticationFailed(AuthenticationFailedContext context) => OnAuthenticationFailed(context);

        /// <summary>
        /// Invoked when a protocol message is first received.
        /// </summary>
        public virtual Task MessageReceived(MessageReceivedContext context) => OnMessageReceived(context);

        /// <summary>
        /// Invoked when a introspection payload is first received.
        /// </summary>
        public virtual Task PayloadReceived(PayloadReceivedContext context) => OnPayloadReceived(context);

        /// <summary>
        /// Invoked if Authorization fails and results in a Forbidden response.
        /// </summary>
        public virtual Task Forbidden(ForbiddenContext context) => OnForbidden(context);

        /// <summary>
        /// Invoked before a challenge is sent back to the caller.
        /// </summary>
        public virtual Task Challenge(ChallengeContext context) => OnChallenge(context);
    }
}