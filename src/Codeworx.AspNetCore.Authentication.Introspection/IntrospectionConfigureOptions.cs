using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Codeworx.AspNetCore.Authentication.Introspection;

internal sealed class IntrospectionConfigureOptions : IConfigureNamedOptions<IntrospectionOptions>
{
    private static readonly Func<string, TimeSpan> _invariantTimeSpanParse = (string timespanString) => TimeSpan.Parse(timespanString, CultureInfo.InvariantCulture);
    private readonly IAuthenticationConfigurationProvider _authenticationConfigurationProvider;

    public IntrospectionConfigureOptions(IAuthenticationConfigurationProvider configurationProvider)
    {
        _authenticationConfigurationProvider = configurationProvider;
    }

    public void Configure(string? name, IntrospectionOptions options)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        var configSection = _authenticationConfigurationProvider.GetSchemeConfiguration(name);

        if (configSection is null || !configSection.GetChildren().Any())
        {
            return;
        }

        options.ValidateIssuer = StringHelpers.ParseValueOrDefault(configSection[nameof(options.ValidateIssuer)], bool.Parse, options.ValidateIssuer);
        var issuers = configSection.GetSection(nameof(options.ValidIssuers)).GetChildren().Where(p => p.Value != null).Select(iss => iss.Value!).ToList();

        options.ValidateAudience = StringHelpers.ParseValueOrDefault(configSection[nameof(options.ValidateAudience)], bool.Parse, options.ValidateAudience);
        var audience = configSection[nameof(options.Audience)];
        var audiences = configSection.GetSection(nameof(options.ValidAudiences)).GetChildren().Where(p => p.Value != null).Select(aud => aud.Value!).ToList();
        if (audience is not null)
        {
            audiences.Add(audience);
        }

        options.ValidateLifetime = StringHelpers.ParseValueOrDefault(configSection[nameof(options.ValidateLifetime)], bool.Parse, options.ValidateLifetime);

        options.Authority = configSection[nameof(options.Authority)] ?? options.Authority;
        options.BackchannelTimeout = StringHelpers.ParseValueOrDefault(configSection[nameof(options.BackchannelTimeout)], _invariantTimeSpanParse, options.BackchannelTimeout);
        options.Challenge = configSection[nameof(options.Challenge)] ?? options.Challenge;
        options.ForwardAuthenticate = configSection[nameof(options.ForwardAuthenticate)] ?? options.ForwardAuthenticate;
        options.ForwardChallenge = configSection[nameof(options.ForwardChallenge)] ?? options.ForwardChallenge;
        options.ForwardDefault = configSection[nameof(options.ForwardDefault)] ?? options.ForwardDefault;
        options.ForwardForbid = configSection[nameof(options.ForwardForbid)] ?? options.ForwardForbid;
        options.ForwardSignIn = configSection[nameof(options.ForwardSignIn)] ?? options.ForwardSignIn;
        options.ForwardSignOut = configSection[nameof(options.ForwardSignOut)] ?? options.ForwardSignOut;
        options.IncludeErrorDetails = StringHelpers.ParseValueOrDefault(configSection[nameof(options.IncludeErrorDetails)], bool.Parse, options.IncludeErrorDetails);
        options.MetadataAddress = configSection[nameof(options.MetadataAddress)] ?? options.MetadataAddress;
        options.RefreshInterval = StringHelpers.ParseValueOrDefault(configSection[nameof(options.RefreshInterval)], _invariantTimeSpanParse, options.RefreshInterval);
        options.RequireHttpsMetadata = StringHelpers.ParseValueOrDefault(configSection[nameof(options.RequireHttpsMetadata)], bool.Parse, options.RequireHttpsMetadata);
        options.SaveToken = StringHelpers.ParseValueOrDefault(configSection[nameof(options.SaveToken)], bool.Parse, options.SaveToken);

        options.ClientId = configSection[nameof(options.ClientId)] ?? options.ClientId;
        options.ClientSecret = configSection[nameof(options.ClientSecret)] ?? options.ClientSecret;

        options.ValidateAudience = StringHelpers.ParseValueOrDefault(configSection[nameof(options.ValidateAudience)], bool.Parse, options.ValidateAudience);
        options.ValidateIssuer = StringHelpers.ParseValueOrDefault(configSection[nameof(options.ValidateIssuer)], bool.Parse, options.ValidateIssuer);
        options.ClockSkew = StringHelpers.ParseValueOrDefault(configSection[nameof(options.ClockSkew)], _invariantTimeSpanParse, options.ClockSkew);

        options.ValidationParameters = new ValidationParameters(options.ValidateAudience, options.ValidateIssuer, options.ValidateLifetime, issuers, audiences, options.ClockSkew);
    }

    public void Configure(IntrospectionOptions options)
    {
        Configure(Options.DefaultName, options);
    }
}