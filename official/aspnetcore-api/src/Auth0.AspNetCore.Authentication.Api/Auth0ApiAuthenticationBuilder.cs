using Microsoft.Extensions.DependencyInjection;

namespace Auth0.AspNetCore.Authentication.Api;

/// <summary>
///     Builder to add functionality on top of Auth0 API authentication.
/// </summary>
public class Auth0ApiAuthenticationBuilder
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Auth0ApiAuthenticationBuilder" /> class.
    /// </summary>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> instance used to register authentication services.
    /// </param>
    /// <param name="options">
    ///     The <see cref="Auth0ApiOptions" /> containing configuration options for Auth0 authentication.
    /// </param>
    public Auth0ApiAuthenticationBuilder(IServiceCollection services, Auth0ApiOptions options) : this(services,
        Auth0Constants.AuthenticationScheme.Auth0, options)
    {
    }

    /// <summary>
    ///     Constructs an instance of <see cref="Auth0ApiAuthenticationBuilder" />.
    /// </summary>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> instance used to register authentication services.
    /// </param>
    /// <param name="authenticationScheme">
    ///     The authentication scheme to use for the Auth0 authentication handler.
    /// </param>
    /// <param name="options">
    ///     The <see cref="Auth0ApiOptions" /> containing configuration options for Auth0 authentication.
    /// </param>
    public Auth0ApiAuthenticationBuilder(IServiceCollection services, string authenticationScheme,
        Auth0ApiOptions options)
    {
        Services = services;
        Options = options;
        AuthenticationScheme = authenticationScheme;
    }

    public string AuthenticationScheme { get; }
    public Auth0ApiOptions Options { get; }
    public IServiceCollection Services { get; }
}
