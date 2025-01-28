using System;

namespace Codeworx.AspNetCore.Authentication.Introspection
{
    public class IntrospectionInvalidResponseException : Exception
    {
        public IntrospectionInvalidResponseException()
            : this("The introspection call did not return a valid claims list.")
        {
        }

        public IntrospectionInvalidResponseException(string? message)
            : base(message)
        {
        }

        public IntrospectionInvalidResponseException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}