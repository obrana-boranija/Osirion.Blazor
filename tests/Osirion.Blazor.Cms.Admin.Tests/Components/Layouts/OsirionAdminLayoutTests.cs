using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Components.Layouts;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Features.Security;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Components.Layouts;

public class OsirionAdminLayoutTests : AdminComponentTestContext
{
    private readonly IAuthenticationService _authService = Substitute.For<IAuthenticationService>();
    private readonly IGitHubTokenStore _tokenStore = Substitute.For<IGitHubTokenStore>();
    private readonly BunitAuthorizationContext _authorization;

    public OsirionAdminLayoutTests()
    {
        Services.AddSingleton(_authService);
        Services.AddSingleton(_tokenStore);
        _authorization = this.AddAuthorization();
        RenderInteractively();
    }

    [Fact]
    public void OsirionAdminLayout_UserWithoutAdminPolicy_DoesNotRenderBody()
    {
        // Proves the layout backstop hides a page from a signed-in user the admin policy does not allow.
        _authorization.SetAuthorized("alice");

        var cut = RenderLayout();

        cut.FindAll("#admin-body").ShouldBeEmpty();
        cut.Find(".admin-not-authorized").ShouldNotBeNull();
    }

    [Fact]
    public void OsirionAdminLayout_AnonymousUser_DoesNotRenderBody()
    {
        _authorization.SetNotAuthorized();

        var cut = RenderLayout();

        cut.FindAll("#admin-body").ShouldBeEmpty();
        cut.Markup.ShouldNotContain("Admin content");
    }

    [Fact]
    public void OsirionAdminLayout_UserWithAdminPolicy_RendersBody()
    {
        _authorization.SetAuthorized("alice");
        _authorization.SetPolicies(CmsAdminAuthorization.PolicyName);

        var cut = RenderLayout();

        cut.Find("#admin-body").TextContent.ShouldBe("Admin content");
        cut.FindAll(".admin-not-authorized").ShouldBeEmpty();
    }

    [Fact]
    public async Task OsirionAdminLayout_SignOut_RemovesStoredToken()
    {
        // Proves signing out also removes the token kept for the next page load.
        _authorization.SetAuthorized("alice");
        _authorization.SetPolicies(CmsAdminAuthorization.PolicyName);
        _authService.IsAuthenticated.Returns(true);
        _authService.Username.Returns("octocat");
        var cut = RenderLayout();

        await cut.Find("button.dropdown-item").ClickAsync(new());

        await _authService.Received(1).SignOutAsync();
        await _tokenStore.Received(1).RemoveTokenAsync();
        Services.GetRequiredService<BunitNavigationManager>().Uri.ShouldEndWith("/osirion/login");
    }

    private IRenderedComponent<OsirionAdminLayout> RenderLayout()
    {
        RenderFragment body = builder => builder.AddMarkupContent(0, "<p id=\"admin-body\">Admin content</p>");

        return Render<OsirionAdminLayout>(parameters => parameters.Add(layout => layout.Body, body));
    }
}
