using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Auth0.AspNetCore.Authentication.Api.IntegrationTests;

/// <summary>
/// Test server factory for integration tests using TestServer.
/// </summary>
public class TestWebApplicationFactory : IAsyncDisposable
{
    private readonly Auth0Scenario _scenario;
    private readonly IHost _host;

    public TestWebApplicationFactory(Auth0Scenario scenario)
    {
        _scenario = scenario;

        // Create and start the host once during construction
        _host = new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder.UseTestServer();

                webBuilder.ConfigureAppConfiguration(config =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Auth0:Domain"] = _scenario.Domain,
                        ["Auth0:Audience"] = _scenario.Audience
                    });
                });

                webBuilder.ConfigureServices((context, services) =>
                {
                    // Add Auth0 JWT validation
                    var authBuilder = services.AddAuth0ApiAuthentication(options =>
                    {
                        options.Domain = context.Configuration["Auth0:Domain"]
                                       ?? throw new InvalidOperationException("Auth0:Domain is required");
                        options.JwtBearerOptions = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions
                        {
                            Audience = context.Configuration["Auth0:Audience"]
                                     ?? throw new InvalidOperationException("Auth0:Audience is required")
                        };
                    });

                    // Configure DPoP based on scenario
                    if (_scenario.IsDPoPEnabled)
                    {
                        authBuilder.WithDPoP(dpopOptions =>
                        {
                            dpopOptions.Mode = _scenario.IsDPoPRequired 
                                ? DPoP.DPoPModes.Required 
                                : DPoP.DPoPModes.Allowed;
                        });
                    }

                    services.AddAuthorization();
                    services.AddRouting();
                });

                webBuilder.Configure(app =>
                {
                    app.UseRouting();
                    app.UseAuthentication();
                    app.UseAuthorization();

                    app.UseEndpoints(endpoints =>
                    {
                        // Open endpoint - no authentication required
                        endpoints.MapGet("/api/public", () => new { message = "This is a public endpoint" })
                           .WithName("PublicEndpoint");

                        // Protected endpoint - authentication required
                        endpoints.MapGet("/api/protected", () => new { message = "This is a protected endpoint" })
                           .WithName("ProtectedEndpoint")
                           .RequireAuthorization();
                    });
                });
            })
            .Build();

        _host.Start();
    }

    /// <summary>
    /// Creates a new HttpClient instance for a test.
    /// Each client is isolated with its own headers.
    /// </summary>
    public HttpClient CreateClient()
    {
        return _host.GetTestServer().CreateClient();
    }

    public async ValueTask DisposeAsync()
    {
        await _host.StopAsync();
        _host.Dispose();
    }
}
