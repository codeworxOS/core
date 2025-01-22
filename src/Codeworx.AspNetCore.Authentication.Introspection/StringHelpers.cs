using System;

namespace Codeworx.AspNetCore.Authentication.Introspection
{
    internal static class StringHelpers
    {
        public static T ParseValueOrDefault<T>(string? stringValue, Func<string, T> parser, T defaultValue)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                return defaultValue;
            }

            return parser(stringValue);
        }
    }
}
