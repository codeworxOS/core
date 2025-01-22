using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace Codeworx.Extensions.DependencyInjection
{
    public class IncompatibleServiceLifetimeException : Exception
    {
        public IncompatibleServiceLifetimeException(IEnumerable<Type> incompatibleServices)
            : this("Services with incompatible ServiceLifetime found!")
        {
            IncompatibleServices = incompatibleServices.ToImmutableList();
        }

        protected IncompatibleServiceLifetimeException(string message)
            : base(message)
        {
            IncompatibleServices = ImmutableArray<Type>.Empty;
        }

        protected IncompatibleServiceLifetimeException(string message, Exception innerException)
            : base(message, innerException)
        {
            IncompatibleServices = ImmutableArray<Type>.Empty;
        }

        protected IncompatibleServiceLifetimeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            IncompatibleServices = ImmutableArray<Type>.Empty;
        }

        public IReadOnlyList<Type> IncompatibleServices { get; }
    }
}