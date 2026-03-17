using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Auth0.AspNetCore.Authentication.Api.DPoP;

/// <summary>
///     Provides a factory for creating configured JwtBearerEvents instances
/// </summary>
internal abstract class DPoPEventsFactory
{
    /// <summary>
    ///     Creates a new instance of <see cref="JwtBearerEvents" /> and assigns event handlers
    ///     based on the provided <paramref name="auth0Options" />.
    /// </summary>
    /// <param name="auth0Options">The Auth0 API options containing custom event handlers.</param>
    /// <returns>A configured <see cref="JwtBearerEvents" /> instance with integrated event handlers.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if either <paramref name="auth0Options" /> is null.
    /// </exception>
    internal static JwtBearerEvents Create(Auth0ApiOptions? auth0Options)
    {
        ArgumentNullException.ThrowIfNull(auth0Options);

        var dPoPEventHandlers = new DPoPEventHandlers();
        return new JwtBearerEvents
        {
            OnMessageReceived =
                ProxyEvent(auth0Options.JwtBearerOptions?.Events?.OnMessageReceived,
                    dPoPEventHandlers.HandleOnMessageReceived()),
            OnTokenValidated = ProxyEvent(auth0Options.JwtBearerOptions?.Events?.OnTokenValidated,
                dPoPEventHandlers.HandleOnTokenValidated()),
            OnAuthenticationFailed = ProxyEvent(auth0Options.JwtBearerOptions?.Events?.OnAuthenticationFailed),
            OnChallenge = ProxyEvent(auth0Options.JwtBearerOptions?.Events?.OnChallenge,
                dPoPEventHandlers.HandleOnChallenge()),
            OnForbidden = ProxyEvent(auth0Options.JwtBearerOptions?.Events?.OnForbidden)
        };
    }

    /// <summary>
    ///     Creates a composite event handler that executes an additional handler first,
    ///     followed by the original handler, if they are provided.
    /// </summary>
    /// <typeparam name="T">The type of the event context.</typeparam>
    /// <param name="originalHandler">The original event handler provided by the user.</param>
    /// <param name="additionalHandler">An additional event handler to execute before the original handler.</param>
    /// <returns>A composite event handler that executes both handlers in sequence.</returns>
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
