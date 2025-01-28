using System;
using System.Net;

namespace Codeworx.AspNetCore.Authentication.Introspection
{
    [Serializable]
    internal class IntrospectionResponseStausCodeException : Exception
    {
        public IntrospectionResponseStausCodeException()
            : this("The introspection call resulted in an unsuccessful response.")
        {
        }

        public IntrospectionResponseStausCodeException(string? message)
            : base(message)
        {
        }

        public IntrospectionResponseStausCodeException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public HttpStatusCode StatusCode { get; set; }
    }
}