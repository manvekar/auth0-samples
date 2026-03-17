using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Auth0.AspNetCore.Authentication.Api;

/// <summary>
///     Provides a factory for creating configured JwtBearerEvents instances
/// </summary>
internal abstract class JwtBearerEventsFactory
{
    /// <summary>
    ///     Creates a new instance of <see cref="JwtBearerEvents" /> and assigns event handlers
    ///     based on the provided <paramref name="auth0Options" />
    /// </summary>
    /// <returns>A configured <see cref="JwtBearerEvents" /> instance.</returns>
    /// <param name="auth0Options">The Auth0 API options containing custom event handlers.</param>
    internal static JwtBearerEvents Create(Auth0ApiOptions? auth0Options)
    {
        ArgumentNullException.ThrowIfNull(auth0Options);

        return new JwtBearerEvents
        {
            OnTokenValidated = ProxyEvent(auth0Options.JwtBearerOptions?.Events?.OnTokenValidated),
            OnAuthenticationFailed = ProxyEvent(auth0Options.JwtBearerOptions?.Events?.OnAuthenticationFailed),
            OnMessageReceived = ProxyEvent(auth0Options.JwtBearerOptions?.Events?.OnMessageReceived),
            OnChallenge = ProxyEvent(auth0Options.JwtBearerOptions?.Events?.OnChallenge),
            OnForbidden = ProxyEvent(auth0Options.JwtBearerOptions?.Events?.OnForbidden)
        };
    }

    private static Func<T, Task> ProxyEvent<T>(Func<T, Task>? originalHandler, Func<T, Task>? additionalHandler = null)
    {
        return async context =>
        {
            if (additionalHandler != null)
            {
                await additionalHandler(context);
            }

            if (originalHandler != null)
            {
                await originalHandler(context);
            }
        };
    }
}
