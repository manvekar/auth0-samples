using Microsoft.Extensions.DependencyInjection;

namespace Auth0.AspNetCore.Authentication.Api;

/// <summary>
///     Contains
///     <see href="https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection">IServiceCollection</see>
///     extension(s) for registering Auth0.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Auth0 API authentication to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The service collection to add authentication to.</param>
    /// <param name="configureOptions">An action to configure the <see cref="Auth0ApiOptions" />.</param>
    /// <returns>An <see cref="Auth0ApiAuthenticationBuilder" /> for further configuration.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="services" /> or
    ///     <paramref name="configureOptions" /> is null.
    /// </exception>
    public static Auth0ApiAuthenticationBuilder AddAuth0ApiAuthentication(
        this IServiceCollection services,
        Action<Auth0ApiOptions>? configureOptions)
    {
        ArgumentNullException.ThrowIfNull(configureOptions, nameof(configureOptions));

        return services.AddAuth0ApiAuthentication(Auth0Constants.AuthenticationScheme.Auth0, configureOptions);
    }

    /// <summary>
    ///     Adds Auth0 API authentication to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The service collection to add authentication to.</param>
    /// <param name="authenticationScheme">The authentication scheme to use.</param>
    /// <param name="configureOptions">An action to configure the <see cref="Auth0ApiOptions" />.</param>
    /// <returns>An <see cref="Auth0ApiAuthenticationBuilder" /> for further configuration.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="services" /> or
    ///     <paramref name="configureOptions" /> is null.
    /// </exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="authenticationScheme" /> is null or empty.</exception>
    public static Auth0ApiAuthenticationBuilder AddAuth0ApiAuthentication(this IServiceCollection services,
        string? authenticationScheme, Action<Auth0ApiOptions>? configureOptions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(authenticationScheme, nameof(authenticationScheme));
        ArgumentNullException.ThrowIfNull(configureOptions, nameof(configureOptions));

        return services
            .AddAuthentication(options => { options.DefaultScheme = authenticationScheme; })
            .AddAuth0ApiAuthentication(authenticationScheme, configureOptions);
    }
}
