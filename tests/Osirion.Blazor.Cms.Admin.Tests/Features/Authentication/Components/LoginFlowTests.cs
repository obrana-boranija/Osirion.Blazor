using Bunit;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Components;
using Osirion.Blazor.Cms.Admin.Features.Authentication.ViewModels;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Authentication.Components;

public class LoginFlowTests : LoginComponentTestContext
{
    public LoginFlowTests()
    {
        RenderInteractively();
    }

    [Fact]
    public void Login_Defaults_ShowTitleDescriptionAndGitHubButton()
    {
        var cut = Render<Login>(parameters => parameters.Add(p => p.Title, "Admin").Add(p => p.Description, "Sign in here"));

        cut.Markup.ShouldContain("Admin");
        cut.Markup.ShouldContain("Sign in here");
        cut.Markup.ShouldContain("Sign in with GitHub");
    }

    [Fact]
    public void Login_BlankDescriptionAndNoOAuth_HidesDescriptionAndGitHubButton()
    {
        AdminOptions.Authentication.GitHubClientSecret = string.Empty;

        var cut = Render<Login>(parameters => parameters.Add(p => p.Description, string.Empty));

        cut.Markup.ShouldNotContain("Sign in to manage your content.");
        cut.Markup.ShouldNotContain("Sign in with GitHub");
    }

    [Fact]
    public void Login_SignInWithGitHub_SendsTheBrowserToGitHub()
    {
        var cut = Render<Login>();

        cut.Find("button.btn-dark").Click();

        Services.GetRequiredService<BunitNavigationManager>().Uri.ShouldStartWith("https://github.com/login/oauth/authorize");
        OAuthStateStore.Pending.ShouldNotBeNull();
    }

    [Fact]
    public void Login_TokenFormCancel_ReturnsToTheButtons()
    {
        var cut = Render<Login>();

        cut.Find("button.btn-outline-secondary").Click();
        cut.Find("button.btn-primary").HasAttribute("disabled").ShouldBeTrue();
        cut.FindAll("button").Single(button => button.TextContent.Trim() == "Cancel").Click();

        cut.FindAll("#accessToken").ShouldBeEmpty();
    }

    [Fact]
    public void Login_ValidToken_SignsInAndReportsSuccess()
    {
        bool? result = null;
        var cut = Render<Login>(parameters => parameters.Add(p => p.OnLoginResult, (bool success) => result = success));

        cut.Find("button.btn-outline-secondary").Click();
        cut.Find("#accessToken").Change(UserToken);
        cut.Find("button.btn-primary").Click();

        result.ShouldBe(true);
        Services.GetRequiredService<BunitNavigationManager>().Uri.ShouldBe("http://localhost/admin");
    }

    [Fact]
    public void Login_RejectedToken_ReportsFailureAndTheErrorCanBeDismissed()
    {
        bool? result = null;
        var cut = Render<Login>(parameters => parameters.Add(p => p.OnLoginResult, (bool success) => result = success));

        cut.Find("button.btn-outline-secondary").Click();
        cut.Find("#accessToken").Change("bad-token");
        cut.Find("button.btn-primary").Click();
        result.ShouldBe(false);
        cut.Find(".alert-danger").TextContent.ShouldContain("Invalid access token");

        cut.Find(".alert-danger button").Click();

        cut.FindAll(".alert-danger").ShouldBeEmpty();
    }

    [Fact]
    public void Login_TokenWithoutListener_StillSignsIn()
    {
        var cut = Render<Login>();

        cut.Find("button.btn-outline-secondary").Click();
        cut.Find("#accessToken").Change(UserToken);
        cut.Find("button.btn-primary").Click();

        AuthService.Received(1).SetAccessTokenAsync(UserToken);
    }

    [Fact]
    public void Login_TokenInProgress_ShowsAuthenticating()
    {
        var pending = new TaskCompletionSource<bool>();
        AuthService.SetAccessTokenAsync("slow-token").Returns(pending.Task);
        var cut = Render<Login>();

        cut.Find("button.btn-outline-secondary").Click();
        cut.Find("#accessToken").Change("slow-token");
        cut.Find("button.btn-primary").Click();

        cut.Markup.ShouldContain("Authenticating...");
        pending.SetResult(false);
    }

    [Fact]
    public void Login_CallbackWithListener_ReportsTheResult()
    {
        bool? result = null;
        Services.GetRequiredService<BunitNavigationManager>().NavigateTo("osirion/login?code=abc&state=forged");

        Render<Login>(parameters => parameters.Add(p => p.OnLoginResult, (bool success) => result = success));

        result.ShouldBe(false);
    }

    [Fact]
    public async Task Login_Disposed_StopsListeningToTheViewModel()
    {
        var cut = Render<Login>();
        var renders = cut.RenderCount;

        await DisposeComponentsAsync();
        Services.GetRequiredService<LoginViewModel>().ToggleTokenInput();

        cut.RenderCount.ShouldBe(renders);
    }
}
