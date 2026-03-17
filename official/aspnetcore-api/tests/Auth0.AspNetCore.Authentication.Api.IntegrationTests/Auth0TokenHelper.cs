using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;

namespace Auth0.AspNetCore.Authentication.Api.IntegrationTests;

/// <summary>
/// Helper class to obtain tokens from Auth0 for testing.
/// </summary>
public class Auth0TokenHelper
{
    private readonly string _domain;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _audience;

    public Auth0TokenHelper(string domain, string clientId, string clientSecret, string audience)
    {
        _domain = domain ?? throw new ArgumentNullException(nameof(domain));
        _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
        _clientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
        _audience = audience ?? throw new ArgumentNullException(nameof(audience));
    }

    /// <summary>
    /// Gets a valid access token from Auth0 using client credentials flow.
    /// </summary>
    /// <returns>A valid access token.</returns>
    public async Task<string> GetAccessTokenAsync()
    {
        var authenticationApiClient = new AuthenticationApiClient(_domain);

        var tokenRequest = new ClientCredentialsTokenRequest
        {
            ClientId = _clientId,
            ClientSecret = _clientSecret,
            Audience = _audience
        };

        var tokenResponse = await authenticationApiClient.GetTokenAsync(tokenRequest);

        if (string.IsNullOrEmpty(tokenResponse.AccessToken))
        {
            throw new InvalidOperationException("Failed to obtain access token from Auth0");
        }

        return tokenResponse.AccessToken;
    }
}
