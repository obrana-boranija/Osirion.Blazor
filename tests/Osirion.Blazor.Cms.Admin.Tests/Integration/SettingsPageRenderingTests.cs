using Microsoft.AspNetCore.TestHost;
using Shouldly;
using System.Net;

namespace Osirion.Blazor.Cms.Admin.Tests.Integration;

/// <summary>
/// Requests the settings page through the ASP.NET Core pipeline, as a host that makes only the admin pages
/// interactive serves it.
/// </summary>
public class SettingsPageRenderingTests
{
    [Fact]
    public async Task AuthenticatedRequest_ToSettings_RendersSignOutControl()
    {
        // Proves a signed-in host user reaches a sign-out control that removes the stored GitHub token: the settings
        // page used to fail on every render, and the static admin layout cannot show one.
        await using var app = await AdminTestHost.StartAsync();
        using var client = app.GetTestClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/osirion/settings");
        request.Headers.Add(AdminTestHost.UserHeader, "alice");

        using var response = await client.SendAsync(request);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).ShouldContain("Sign Out");
    }
}
