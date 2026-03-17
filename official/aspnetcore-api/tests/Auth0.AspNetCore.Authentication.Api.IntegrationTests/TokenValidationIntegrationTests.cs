using System.Net;
using System.Net.Http.Headers;

namespace Auth0.AspNetCore.Authentication.Api.IntegrationTests;

/// <summary>
/// Integration tests for Auth0 JWT token validation middleware.
/// </summary>
public class TokenValidationIntegrationTests : IAsyncLifetime
{
    private TestWebApplicationFactory? _factory;
    private Auth0TokenHelper? _tokenHelper;
    private Auth0Scenario? _scenario;

    public async Task InitializeAsync()
    {
        _scenario = Auth0TestConfiguration.WithoutDPoP;
        _factory = new TestWebApplicationFactory(_scenario);
        _tokenHelper = new Auth0TokenHelper(_scenario.Domain, _scenario.ClientId, _scenario.ClientSecret,
            _scenario.Audience);

        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await (_factory?.DisposeAsync() ?? ValueTask.CompletedTask);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithValidToken_ReturnsOk()
    {
        // Arrange
        using HttpClient client = _factory!.CreateClient();
        var accessToken = await _tokenHelper!.GetAccessTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/protected");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("protected endpoint");
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        using HttpClient client = _factory!.CreateClient();
        // No token is set

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/protected");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        using HttpClient client = _factory!.CreateClient();
        var invalidToken = "invalid.token.here";
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", invalidToken);

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/protected");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PublicEndpoint_WithoutToken_ReturnsOk()
    {
        // Arrange
        using HttpClient client = _factory!.CreateClient();
        // No token is set

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/public");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("public endpoint");
    }
}
