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

        ////var validateIssuer = StringHelpers.ParseValueOrDefault(configSection[nameof(TokenValidationParameters.ValidateIssuer)], bool.Parse, options.TokenValidationParameters.ValidateIssuer);
        ////var issuer = configSection[nameof(TokenValidationParameters.ValidIssuer)];
        ////var issuers = configSection.GetSection(nameof(TokenValidationParameters.ValidIssuers)).GetChildren().Select(iss => iss.Value).ToList();
        ////if (issuer is not null)
        ////{
        ////    issuers.Add(issuer);
        ////}

        ////var validateAudience = StringHelpers.ParseValueOrDefault(configSection[nameof(TokenValidationParameters.ValidateAudience)], bool.Parse, options.TokenValidationParameters.ValidateAudience);
        ////var audience = configSection[nameof(TokenValidationParameters.ValidAudience)];
        ////var audiences = configSection.GetSection(nameof(TokenValidationParameters.ValidAudiences)).GetChildren().Select(aud => aud.Value).ToList();
        ////if (audience is not null)
        ////{
        ////    audiences.Add(audience);
        ////}

        options.Authority = configSection[nameof(options.Authority)] ?? options.Authority;
        ////options.BackchannelTimeout = StringHelpers.ParseValueOrDefault(configSection[nameof(options.BackchannelTimeout)], _invariantTimeSpanParse, options.BackchannelTimeout);
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
    }

    public void Configure(IntrospectionOptions options)
    {
        Configure(Options.DefaultName, options);
    }
}


////using System;
////using Microsoft.AspNetCore.Authentication;
////using Microsoft.AspNetCore.DataProtection;
////using Microsoft.Extensions.Options;

////namespace Codeworx.AspNetCore.Authentication.Introspection
////{
////    internal sealed class IntrospectionConfigureOptions(IDataProtectionProvider dp) : IConfigureNamedOptions<IntrospectionOptions>
////    {
////        private const string _primaryPurpose = "Codeworx.AspNetCore.Authentication.Introspection";

////        public void Configure(string? schemeName, IntrospectionOptions options)
////        {
////            if (schemeName is null)
////            {
////                return;
////            }

////            options.AccessTokenProtector = new TicketDataFormat(dp.CreateProtector(_primaryPurpose, schemeName, "AccessToken"));
////        }

////        public void Configure(IntrospectionOptions options)
////        {
////            throw new NotImplementedException();
////        }
////    }
////}