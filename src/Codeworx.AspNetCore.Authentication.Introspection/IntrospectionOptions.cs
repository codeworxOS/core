using System;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Codeworx.AspNetCore.Authentication.Introspection
{
    public class IntrospectionOptions : AuthenticationSchemeOptions
    {
        private ISecureDataFormat<AuthenticationTicket>? _accessTokenProtector;

        public IntrospectionOptions()
        {
            Events = new IntrospectionEvents { };
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenProtector
        {
            get => _accessTokenProtector ?? throw new InvalidOperationException($"{nameof(AccessTokenProtector)} was not set.");
            set => _accessTokenProtector = value;
        }

        /// <summary>
        /// Gets or sets a single valid audience value for any received OpenIdConnect token.
        /// This value is passed into TokenValidationParameters.ValidAudience if that property is empty.
        /// </summary>
        /// <value>
        /// The expected audience for any received OpenIdConnect token.
        /// </value>
        public string? Audience { get; set; }

        /// <summary>
        /// Gets or sets the Authority to use when making OpenIdConnect calls.
        /// </summary>
        public string? Authority { get; set; }

        /// <summary>
        /// Gets or sets how often an automatic metadata refresh should occur.
        /// </summary>
        /// <value>
        /// Defaults to <see cref="ConfigurationManager{OpenIdConnectConfiguration}.DefaultAutomaticRefreshInterval" />.
        /// </value>
        public TimeSpan AutomaticRefreshInterval { get; set; } = ConfigurationManager<OpenIdConnectConfiguration>.DefaultAutomaticRefreshInterval;

        /// <summary>
        /// Gets or sets the Backchannel used to retrieve metadata.
        /// </summary>
        public HttpClient Backchannel { get; set; } = default!;

        /// <summary>
        /// Gets or sets the HttpMessageHandler used to retrieve metadata.
        /// This cannot be set at the same time as BackchannelCertificateValidator unless the value
        /// is a WebRequestHandler.
        /// </summary>
        public HttpMessageHandler? BackchannelHttpHandler { get; set; }

        /// <summary>
        /// Gets or sets the timeout when using the backchannel to make an http call.
        /// </summary>
        public TimeSpan BackchannelTimeout { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Gets or sets the challenge to put in the "WWW-Authenticate" header.
        /// </summary>
        public string Challenge { get; set; } = IntrospectionDefaults.AuthenticationScheme;

        /// <summary>
        /// Gets or sets the configuration provided directly by the developer. If provided, then MetadataAddress and the Backchannel properties
        /// will not be used. This information should not be updated during request processing.
        /// </summary>
        public OpenIdConnectConfiguration? Configuration { get; set; }

        /// <summary>
        /// Gets or sets the configuration manager responsible for retrieving, caching, and refreshing the configuration from metadata.
        /// If not provided, then one will be created using the MetadataAddress and Backchannel properties.
        /// </summary>
        public IConfigurationManager<OpenIdConnectConfiguration>? ConfigurationManager { get; set; }

        /// <summary>
        /// gets or sets the object provided by the application to process events raised by the bearer authentication handler.
        /// The application may implement the interface fully, or it may create an instance of JwtBearerEvents
        /// and assign delegates only to the events it wants to process.
        /// </summary>
        public new IntrospectionEvents Events
        {
            get { return (IntrospectionEvents)base.Events!; }
            set { base.Events = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the token validation errors should be returned to the caller.
        /// Enabled by default, this option can be disabled to prevent the JWT handler
        /// from returning an error and an error_description in the WWW-Authenticate header.
        /// </summary>
        public bool IncludeErrorDetails { get; set; } = true;

        /// <summary>
        /// Gets or sets the discovery endpoint for obtaining metadata.
        /// </summary>
        public string MetadataAddress { get; set; } = default!;

        /// <summary>
        /// Gets or sets the minimum time between retrievals, in the event that a retrieval failed, or that a refresh was explicitly requested.
        /// </summary>
        /// <value>
        /// Defaults to <see cref="ConfigurationManager{OpenIdConnectConfiguration}.DefaultRefreshInterval" />.
        /// </value>
        public TimeSpan RefreshInterval { get; set; } = ConfigurationManager<OpenIdConnectConfiguration>.DefaultRefreshInterval;

        /// <summary>
        /// Gets or sets a value indicating whether HTTPS is required for the metadata address or authority.
        /// The default is true. This should be disabled only in development environments.
        /// </summary>
        public bool RequireHttpsMetadata { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the bearer token should be stored in the
        /// <see cref="AuthenticationProperties"/> after a successful authorization.
        /// </summary>
        public bool SaveToken { get; set; } = true;
    }
}