using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Auth0.AspNetCore.Authentication.Api;

/// <summary>
///     Configuration options for Auth0 API authentication.
/// </summary>
public class Auth0ApiOptions
{
    /// <summary>
    ///     Auth0 domain name, e.g. tenant.auth0.com.
    /// </summary>
    public string? Domain { get; set; }

    /// <summary>
    ///     The configuration options for JWT Bearer authentication.
    /// </summary>
    public JwtBearerOptions? JwtBearerOptions { get; set; }
}
