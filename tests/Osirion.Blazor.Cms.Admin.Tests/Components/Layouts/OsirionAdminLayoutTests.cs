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
    private readonly IGitHubSessionRestorer _sessionRestorer = Substitute.For<IGitHubSessionRestorer>();
    private readonly BunitAuthorizationContext _authorization;

    public OsirionAdminLayoutTests()
    {
        Services.AddSingleton(_authService);
        Services.AddSingleton(_tokenStore);
        Services.AddSingleton(_sessionRestorer);
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

    [Fact]
    public void OsirionAdminLayout_NotSignedInToGitHub_HoldsBackThePageUntilTheStoredSignInIsRestored()
    {
        // Pages call GitHub from their first lifecycle method, so they must not start before the token is restored.
        _authorization.SetAuthorized("alice");
        _authorization.SetPolicies(CmsAdminAuthorization.PolicyName);
        var restoration = new TaskCompletionSource<bool>();
        _sessionRestorer.RestoreAsync().Returns(restoration.Task);

        var cut = RenderLayout();

        cut.FindAll("#admin-body").ShouldBeEmpty();
        cut.Find(".admin-restoring-session").ShouldNotBeNull();

        restoration.SetResult(true);

        cut.WaitForAssertion(() => cut.Find("#admin-body").TextContent.ShouldBe("Admin content"));
        cut.FindAll(".admin-restoring-session").ShouldBeEmpty();
        _sessionRestorer.Received(1).RestoreAsync();
    }

    [Fact]
    public void OsirionAdminLayout_AlreadySignedInToGitHub_DoesNotRestoreAgain()
    {
        _authorization.SetAuthorized("alice");
        _authorization.SetPolicies(CmsAdminAuthorization.PolicyName);
        _authService.IsAuthenticated.Returns(true);
        _authService.Username.Returns("octocat");

        var cut = RenderLayout();

        cut.Find("#admin-body").ShouldNotBeNull();
        _sessionRestorer.DidNotReceive().RestoreAsync();
    }

    [Fact]
    public void OsirionAdminLayout_OnTheLoginPage_LeavesRestorationToThePage()
    {
        // The login page restores the sign-in itself, because it also redirects to the page the user came from.
        _authorization.SetAuthorized("alice");
        _authorization.SetPolicies(CmsAdminAuthorization.PolicyName);
        Services.GetRequiredService<BunitNavigationManager>().NavigateTo("/osirion/login");

        var cut = RenderLayout();

        cut.Find("#admin-body").ShouldNotBeNull();
        _sessionRestorer.DidNotReceive().RestoreAsync();
    }

    private IRenderedComponent<OsirionAdminLayout> RenderLayout()
    {
        RenderFragment body = builder => builder.AddMarkupContent(0, "<p id=\"admin-body\">Admin content</p>");

        return Render<OsirionAdminLayout>(parameters => parameters.Add(layout => layout.Body, body));
    }
}
