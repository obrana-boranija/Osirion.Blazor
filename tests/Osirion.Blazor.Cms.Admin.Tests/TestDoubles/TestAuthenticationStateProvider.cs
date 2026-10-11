using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Osirion.Blazor.Cms.Admin.Tests.TestDoubles;

/// <summary>
/// Supplies a fixed host user, which a test can replace to simulate another user signing in.
/// </summary>
internal sealed class TestAuthenticationStateProvider : AuthenticationStateProvider
{
    public TestAuthenticationStateProvider(ClaimsPrincipal user)
    {
        User = user;
    }

    public ClaimsPrincipal User { get; set; }

    public static ClaimsPrincipal CreateUser(string? nameIdentifier, string? name = null)
    {
        var claims = new List<Claim>();
        if (nameIdentifier is not null)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, nameIdentifier));
        }

        if (name is not null)
        {
            claims.Add(new Claim(ClaimTypes.Name, name));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: "Test"));
    }

    public static ClaimsPrincipal CreateAnonymousUser()
    {
        return new ClaimsPrincipal(new ClaimsIdentity());
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(User));
    }
}
