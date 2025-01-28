using System;

namespace Codeworx.AspNetCore.Authentication.Introspection
{
    public class IntrospectionEndpintMissingException : Exception
    {
        public IntrospectionEndpintMissingException()
            : this("The introspection endpoint is missing.")
        {
        }

        public IntrospectionEndpintMissingException(string? message)
            : base(message)
        {
        }

        public IntrospectionEndpintMissingException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public string? Authority { get; set; }
    }
}