using Bunit;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Components.Pages;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Features.Authentication.ViewModels;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Components.Pages;

public class LoginPageTests : AdminComponentTestContext
{
    private const string ServerToken = "server-pat-value";
    private const string UserToken = "user-token";

    private readonly IAuthenticationService _authService = Substitute.For<IAuthenticationService>();
    private readonly IGitHubTokenProvider _tokenProvider = Substitute.For<IGitHubTokenProvider>();
    private readonly IGitHubTokenStore _tokenStore = Substitute.For<IGitHubTokenStore>();
    private readonly InMemoryGitHubOAuthStateStore _oauthStateStore = new();
    private readonly CmsAdminOptions _options = new()
    {
        Authentication = new AuthenticationOptions
        {
            GitHubClientId = "client-id",
            GitHubClientSecret = "client-secret"
        }
    };

    public LoginPageTests()
    {
        var stateStorage = Substitute.For<IStateStorageService>();
        stateStorage.IsInitialized.Returns(true);
        _authService.SetAccessTokenAsync(UserToken).Returns(true);
        _tokenStore.SaveTokenAsync(Arg.Any<string>()).Returns(true);

        Services.AddSingleton(_authService);
        Services.AddSingleton(_tokenProvider);
        Services.AddSingleton(_tokenStore);
        Services.AddSingleton<IGitHubOAuthStateStore>(_oauthStateStore);
        Services.AddSingleton(stateStorage);
        Services.AddSingleton(Substitute.For<IContentRepositoryAdapter>());
        Services.AddSingleton(new CmsEventMediator(NullLogger<CmsEventMediator>.Instance));
        Services.AddSingleton(Options.Create(_options));
        Services.AddSingleton<ILogger<LoginViewModel>>(NullLogger<LoginViewModel>.Instance);
        Services.AddScoped<LoginViewModel>();
        RenderInteractively();
    }

    [Fact]
    public void LoginPage_WithConfiguredServerToken_ShowsNoticeWithoutSigningIn()
    {
        // Proves the server token is no longer applied for whoever opens the page, nor copied into the form.
        _options.Authentication.PersonalAccessToken = ServerToken;

        var cut = RenderComponent<LoginPage>();
        cut.Find("button.btn-outline-secondary").Click();

        cut.Find(".server-credential-notice").ShouldNotBeNull();
        cut.Find("#accessToken").GetAttribute("value").ShouldBeNullOrEmpty();
        cut.Markup.ShouldNotContain(ServerToken);
        _authService.DidNotReceiveWithAnyArgs().SetAccessTokenAsync(default!);
    }

    [Fact]
    public void LoginPage_WithoutServerToken_ShowsNoNotice()
    {
        var cut = RenderComponent<LoginPage>();

        cut.FindAll(".server-credential-notice").ShouldBeEmpty();
    }

    [Fact]
    public void LoginPage_CallbackWithMatchingState_ExchangesCodeWithVerifierAndRedirectUri()
    {
        var pending = new GitHubOAuthRequest("expected-state", "code-verifier", "http://localhost/osirion/login", "/osirion/settings");
        _oauthStateStore.Pending = pending;
        _tokenProvider
            .ExchangeCodeForTokenAsync("abc", "client-id", "client-secret", pending.RedirectUri, pending.CodeVerifier, Arg.Any<CancellationToken>())
            .Returns(UserToken);
        var navigationManager = Services.GetRequiredService<FakeNavigationManager>();
        navigationManager.NavigateTo("osirion/login?code=abc&state=expected-state");

        RenderComponent<LoginPage>();

        _authService.Received(1).SetAccessTokenAsync(UserToken);
        navigationManager.Uri.ShouldBe("http://localhost/osirion/settings");
    }

    [Fact]
    public void LoginPage_CallbackWithoutPendingLogin_ShowsErrorWithoutExchangingCode()
    {
        // Proves a forged callback URL (login CSRF) is rejected before the code reaches GitHub.
        Services.GetRequiredService<FakeNavigationManager>().NavigateTo("osirion/login?code=attacker-code&state=attacker-state");

        var cut = RenderComponent<LoginPage>();

        _tokenProvider.DidNotReceiveWithAnyArgs()
            .ExchangeCodeForTokenAsync(default!, default!, default!, default, default, default);
        _tokenProvider.DidNotReceiveWithAnyArgs().ExchangeCodeForTokenAsync(default!, default!, default!);
        cut.Find(".alert-danger").TextContent.ShouldContain("could not be verified");
    }

    [Theory]
    [InlineData("//evil.example")]
    [InlineData("/\\evil.example")]
    [InlineData("https://evil.example")]
    public void LoginPage_WithOffSiteReturnUrl_StaysOnSiteAfterLogin(string returnUrl)
    {
        // Proves the returnUrl query value cannot send a signed-in user to another site.
        _tokenStore.GetTokenAsync().Returns(UserToken);
        var navigationManager = Services.GetRequiredService<FakeNavigationManager>();
        navigationManager.NavigateTo($"osirion/login?returnUrl={Uri.EscapeDataString(returnUrl)}");

        RenderComponent<LoginPage>();

        navigationManager.Uri.ShouldBe("http://localhost/osirion");
    }

    [Fact]
    public void LoginPage_WithConfiguredServerTokenAndOffSiteReturnUrl_NoticeLinksToDashboard()
    {
        // Proves the server-credential link, which no view-model navigation guards, cannot send a user off-site.
        _options.Authentication.PersonalAccessToken = ServerToken;
        Services.GetRequiredService<FakeNavigationManager>()
            .NavigateTo($"osirion/login?returnUrl={Uri.EscapeDataString("//evil.example")}");

        var cut = RenderComponent<LoginPage>();

        cut.Find(".server-credential-notice a").GetAttribute("href").ShouldBe("/osirion");
    }

    [Fact]
    public void LoginPage_ImplementsIDisposable_SoTheRendererReleasesViewModelSubscription()
    {
        typeof(IDisposable).IsAssignableFrom(typeof(LoginPage)).ShouldBeTrue();
    }
}
