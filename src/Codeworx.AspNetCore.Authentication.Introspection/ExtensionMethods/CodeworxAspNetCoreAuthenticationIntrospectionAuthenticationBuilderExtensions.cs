using System;
using Codeworx.AspNetCore.Authentication.Introspection.Cache;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Codeworx.AspNetCore.Authentication.Introspection.ExtensionMethods
{
    public static class CodeworxAspNetCoreAuthenticationIntrospectionAuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddIntrospection(this AuthenticationBuilder builder)
            => builder.AddIntrospection(IntrospectionDefaults.AuthenticationScheme);

        public static AuthenticationBuilder AddIntrospection(this AuthenticationBuilder builder, string authenticationScheme)
            => builder.AddIntrospection(authenticationScheme, _ => { });

        public static AuthenticationBuilder AddIntrospection(this AuthenticationBuilder builder, Action<IntrospectionOptions> configure)
            => builder.AddIntrospection(IntrospectionDefaults.AuthenticationScheme, configure);

        public static AuthenticationBuilder AddIntrospection(this AuthenticationBuilder builder, string authenticationScheme, Action<IntrospectionOptions> configure)
    => builder.AddIntrospection(authenticationScheme, null, configure);

        public static AuthenticationBuilder AddIntrospection(this AuthenticationBuilder builder, string authenticationScheme, string? displayName, Action<IntrospectionOptions> configure)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(authenticationScheme);
            ArgumentNullException.ThrowIfNull(configure);

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IIntrospectionResponseCache, IntrospectionResponseCache>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<IntrospectionOptions>, IntrospectionConfigureOptions>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<IntrospectionOptions>, IntrospectionPostConfigureOptions>());
            return builder.AddScheme<IntrospectionOptions, IntrospectionAuthenticationHandler>(authenticationScheme, displayName, configure);
        }
    }
}
