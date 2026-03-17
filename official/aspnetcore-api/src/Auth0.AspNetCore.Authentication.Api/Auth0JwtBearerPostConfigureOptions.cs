using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Auth0.AspNetCore.Authentication.Api;

/// <summary>
///     Post-configures <see cref="JwtBearerOptions" />
/// </summary>
internal class Auth0JwtBearerPostConfigureOptions : IPostConfigureOptions<JwtBearerOptions>
{
    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        options.Backchannel?.DefaultRequestHeaders.Add("Auth0-Client", Utils.CreateAgentString());
    }
}
