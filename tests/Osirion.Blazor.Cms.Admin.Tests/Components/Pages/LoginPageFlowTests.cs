using Bunit;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Components.Pages;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Components.Pages;

public class LoginPageFlowTests : LoginComponentTestContext
{
    public LoginPageFlowTests()
    {
        RenderInteractively();
    }

    [Fact]
    public void LoginPage_LoginWithGitHub_SendsTheBrowserToGitHub()
    {
        var cut = Render<LoginPage>();

        cut.Find("button.btn-dark").Click();

        Services.GetRequiredService<BunitNavigationManager>().Uri.ShouldStartWith("https://github.com/login/oauth/authorize");
    }

    [Fact]
    public void LoginPage_NoOAuthConfigured_HidesTheGitHubButton()
    {
        AdminOptions.Authentication.GitHubClientId = string.Empty;

        var cut = Render<LoginPage>();

        cut.FindAll("button.btn-dark").ShouldBeEmpty();
    }

    [Fact]
    public void LoginPage_ValidToken_SignsInAndOpensTheDashboard()
    {
        var cut = Render<LoginPage>();

        cut.Find("button.btn-outline-secondary").Click();
        cut.Find("#accessToken").Change(UserToken);
        cut.Find("button.btn-primary").Click();

        AuthService.Received(1).SetAccessTokenAsync(UserToken);
        Services.GetRequiredService<BunitNavigationManager>().Uri.ShouldBe("http://localhost/osirion");
    }

    [Fact]
    public void LoginPage_TokenInProgress_ShowsAuthenticatingAndCancelReturns()
    {
        var pending = new TaskCompletionSource<bool>();
        AuthService.SetAccessTokenAsync("slow-token").Returns(pending.Task);
        var cut = Render<LoginPage>();

        cut.Find("button.btn-outline-secondary").Click();
        cut.Find("#accessToken").Change("slow-token");
        cut.Find("button.btn-primary").Click();
        cut.Markup.ShouldContain("Authenticating...");
        pending.SetResult(false);
        cut.WaitForAssertion(() => cut.Markup.ShouldContain("Invalid access token"));

        cut.FindAll("button").Single(button => button.TextContent.Trim() == "Cancel").Click();

        cut.FindAll("#accessToken").ShouldBeEmpty();
    }
}
