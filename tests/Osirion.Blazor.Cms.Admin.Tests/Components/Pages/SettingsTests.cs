using Bunit;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Components.Pages;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Components.Pages;

public class SettingsTests : AdminComponentTestContext
{
    private readonly IAuthenticationService _authService = Substitute.For<IAuthenticationService>();
    private readonly IGitHubTokenStore _tokenStore = Substitute.For<IGitHubTokenStore>();

    public SettingsTests()
    {
        Services.AddSingleton(_authService);
        Services.AddSingleton(_tokenStore);
        Services.AddSingleton<ILogger<Settings>>(NullLogger<Settings>.Instance);
        RenderInteractively();
    }

    [Fact]
    public void Settings_SignOut_RemovesStoredTokenAndOpensLoginPage()
    {
        // Proves the settings page renders and its sign-out removes the stored GitHub token, so the token can be
        // revoked from the admin even where the static admin layout cannot show a sign-out control.
        var cut = Render<Settings>();

        cut.Find("button.btn-outline-danger").Click();

        _authService.Received(1).SignOutAsync();
        _tokenStore.Received(1).RemoveTokenAsync();
        Services.GetRequiredService<BunitNavigationManager>().Uri.ShouldBe("http://localhost/osirion/login");
    }

    [Fact]
    public void Settings_SignOutFailure_ShowsMessageWithoutExceptionDetails()
    {
        // Proves browser or storage details carried by an exception are no longer shown in the admin alert.
        _tokenStore.RemoveTokenAsync().Returns(Task.FromException(new InvalidOperationException("browser storage detail")));
        var cut = Render<Settings>();

        cut.Find("button.btn-outline-danger").Click();

        var errorMessage = Services.GetRequiredService<CmsState>().ErrorMessage;
        errorMessage.ShouldNotBeNullOrWhiteSpace();
        errorMessage.ShouldNotContain("browser storage detail");
    }
}
