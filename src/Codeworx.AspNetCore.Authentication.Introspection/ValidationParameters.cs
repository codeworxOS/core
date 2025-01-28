using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Codeworx.AspNetCore.Authentication.Introspection
{
    public class ValidationParameters
    {
        public ValidationParameters(
            bool validateAudience,
            bool validateIssuer,
            bool validateLifetime,
            IEnumerable<string> issuers,
            IEnumerable<string> audiences,
            TimeSpan clockSkew)
        {
            ValidateAudience = validateAudience;
            ValidateIssuer = validateIssuer;
            ValidateLifetime = validateLifetime;
            ClockSkew = clockSkew;
            Issuers = issuers.ToImmutableList();
            Audiences = audiences.ToImmutableList();
        }

        public IReadOnlyList<string> Audiences { get; }

        public IReadOnlyList<string> Issuers { get; }

        public bool ValidateAudience { get; }

        public bool ValidateIssuer { get; }

        public bool ValidateLifetime { get; }

        public TimeSpan ClockSkew { get; }
    }
}
