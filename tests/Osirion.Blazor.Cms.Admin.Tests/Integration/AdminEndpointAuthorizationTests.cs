using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Features.Security;
using Shouldly;
using System.Net;

namespace Osirion.Blazor.Cms.Admin.Tests.Integration;

/// <summary>
/// Requests admin routes through the ASP.NET Core pipeline, so the page attributes are checked as endpoint
/// metadata by the authorization middleware, before any admin component renders.
/// </summary>
public class AdminEndpointAuthorizationTests
{
    // The login page renders with test doubles only; it also proves /osirion/login stays behind the admin policy
    private const string LoginRoute = "/osirion/login";
    private const string LoginPageText = "Sign in to manage your content";

    public static TheoryData<string> AdminRoutes => new()
    {
        "/osirion",
        "/osirion/dashboard",
        "/osirion/content",
        "/osirion/content/edit",
        "/osirion/settings",
        "/osirion/login"
    };

    [Theory]
    [MemberData(nameof(AdminRoutes))]
    public async Task AnonymousRequest_ToAdminRoute_IsChallenged(string route)
    {
        // Proves the admin pages are no longer anonymously reachable: the host's scheme challenges before rendering.
        await using var app = await AdminTestHost.StartAsync();
        using var client = app.GetTestClient();

        using var response = await client.GetAsync(route);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await response.Content.ReadAsStringAsync()).ShouldNotContain(LoginPageText);
    }

    [Fact]
    public async Task AuthenticatedRequest_ToAdminRoute_RendersPage()
    {
        await using var app = await AdminTestHost.StartAsync();
        using var client = app.GetTestClient();

        using var response = await client.SendAsync(CreateRequest(LoginRoute, user: "alice"));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).ShouldContain(LoginPageText);
    }

    [Fact]
    public async Task AuthenticatedRequest_WithoutRoleRequiredByHostPolicy_IsForbidden()
    {
        // Proves a host policy registered under the admin policy name replaces the default end to end.
        await using var app = await AdminTestHost.StartAsync(RequireEditorRole);
        using var client = app.GetTestClient();

        using var response = await client.SendAsync(CreateRequest(LoginRoute, user: "alice"));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AuthenticatedRequest_WithRoleRequiredByHostPolicy_RendersPage()
    {
        await using var app = await AdminTestHost.StartAsync(RequireEditorRole);
        using var client = app.GetTestClient();

        using var response = await client.SendAsync(CreateRequest(LoginRoute, user: "alice", role: "Editor"));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).ShouldContain(LoginPageText);
    }

    [Fact]
    public async Task PublicRequest_InHostWithoutAuthentication_IsServed()
    {
        // A public site that registers the admin but never configured authentication keeps serving its own pages.
        await using var app = await AdminTestHost.StartWithoutAuthenticationAsync();
        using var client = app.GetTestClient();

        using var response = await client.GetAsync(AdminTestHost.PublicRoute);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).ShouldBe(AdminTestHost.PublicResponse);
    }

    [Fact]
    public async Task PublicRazorPage_InHostWithoutAuthentication_IsServed()
    {
        // The cascading authentication state the admin registers must not break a public site's own pages.
        await using var app = await AdminTestHost.StartWithoutAuthenticationAsync();
        using var client = app.GetTestClient();

        using var response = await client.GetAsync(PublicTestPage.Route);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).ShouldContain($"{PublicTestPage.Text}, authenticated: False");
    }

    [Fact]
    public async Task AnonymousRequest_ToAdminRouteInHostWithoutAuthentication_FailsClosed()
    {
        // ASP.NET Core refuses to run an endpoint with authorization metadata when no authorization middleware ran
        // after routing, so the admin page is never rendered.
        await using var app = await AdminTestHost.StartWithoutAuthenticationAsync();
        using var client = app.GetTestClient();

        var exception = await Should.ThrowAsync<InvalidOperationException>(() => client.GetAsync(LoginRoute));

        exception.Message.ShouldContain("contains authorization metadata");
    }

    private static void RequireEditorRole(IServiceCollection services)
    {
        services.AddAuthorization(options =>
            options.AddPolicy(CmsAdminAuthorization.PolicyName, policy => policy.RequireRole("Editor")));
    }

    private static HttpRequestMessage CreateRequest(string route, string user, string? role = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, route);
        request.Headers.Add(AdminTestHost.UserHeader, user);

        if (role is not null)
        {
            request.Headers.Add(AdminTestHost.RoleHeader, role);
        }

        return request;
    }
}
