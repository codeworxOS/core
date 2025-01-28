using System;

namespace Codeworx.AspNetCore.Authentication.Introspection
{
    [Serializable]
    internal class IntrospectionInactiveResponseException : Exception
    {
        public IntrospectionInactiveResponseException()
            : this("The introspection call resulted in a response that was marked as inactive.")
        {
        }

        public IntrospectionInactiveResponseException(string? message)
            : base(message)
        {
        }

        public IntrospectionInactiveResponseException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}