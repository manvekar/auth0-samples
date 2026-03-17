using Auth0.AspNetCore.Authentication.Api.DPoP;

using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Auth0.AspNetCore.Authentication.Api.UnitTests;

public class DPoPEventsFactoryTests
{
    [Fact]
    public void Create_WithNullAuth0Options_ThrowsArgumentNullException()
    {
        Func<JwtBearerEvents> act = () => DPoPEventsFactory.Create(null);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithNullAuth0OptionsEvents_ReturnsJwtBearerEventsWithNullHandlers()
    {
        var dPoPOptions = new DPoPOptions();
        var auth0Options = new Auth0ApiOptions { JwtBearerOptions = new JwtBearerOptions { Events = null } };

        JwtBearerEvents result = DPoPEventsFactory.Create(auth0Options);

        result.Should().NotBeNull();
        result.OnTokenValidated.Should().NotBeNull();
        result.OnAuthenticationFailed.Should().NotBeNull();
        result.OnMessageReceived.Should().NotBeNull();
        result.OnChallenge.Should().NotBeNull();
        result.OnForbidden.Should().NotBeNull();
    }
}
