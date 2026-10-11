using AngleSharp.Dom;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Components.Pages;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Components.Pages;

public class SettingsDisplayTests : AdminComponentTestContext
{
    private readonly IAuthenticationService _authService = Substitute.For<IAuthenticationService>();
    private readonly CmsState _state = new();

    public SettingsDisplayTests()
    {
        Services.AddSingleton(_authService);
        Services.AddSingleton(Substitute.For<IGitHubTokenStore>());
        Services.AddSingleton<ILogger<Settings>>(NullLogger<Settings>.Instance);
        Services.AddSingleton(_state);
        RenderInteractively();
    }

    private IElement ThemeButton(IRenderedComponent<Settings> cut, string label) =>
        cut.FindAll("button").Single(button => button.TextContent.Trim() == label);

    [Fact]
    public void Settings_SignedOutWithNothingSelected_ShowsPlaceholders()
    {
        var cut = Render<Settings>();

        cut.Markup.ShouldContain("Not authenticated");
        cut.Markup.ShouldContain("No repository selected");
        cut.Markup.ShouldContain("No branch selected");
    }

    [Fact]
    public void Settings_SignedInWithSelections_ShowsUserRepositoryAndBranch()
    {
        _authService.IsAuthenticated.Returns(true);
        _authService.Username.Returns("octocat");
        _state.SelectRepository(new GitHubRepository { Name = "site" });
        _state.SelectBranch(new GitHubBranch { Name = "main" });

        var cut = Render<Settings>();

        cut.Markup.ShouldContain("Authenticated as octocat");
        cut.Find(".bi-github").ParentElement!.TextContent.ShouldContain("site");
        cut.Find(".bi-git").ParentElement!.TextContent.ShouldContain("main");
    }

    [Fact]
    public void Settings_ChooseDarkThenLight_UpdatesTheThemeAndNotifiesTheParent()
    {
        var themes = new List<string>();
        var cut = Render<Settings>(parameters => parameters.Add(p => p.ThemeChanged, (string theme) => themes.Add(theme)));

        ThemeButton(cut, "Dark").Click();
        ThemeButton(cut, "Dark").ClassList.ShouldContain("btn-primary");
        ThemeButton(cut, "Light").Click();

        cut.Instance.Theme.ShouldBe("light");
        ThemeButton(cut, "Dark").ClassList.ShouldContain("btn-outline-secondary");
        themes.ShouldContain("dark");
        themes.Last().ShouldBe("light");
    }

    [Fact]
    public void Settings_EmptyTheme_FallsBackToLightOnTheNextParameterSet()
    {
        var cut = Render<Settings>();
        cut.Instance.Theme = string.Empty;

        cut.Render();

        cut.Instance.Theme.ShouldBe("light");
    }
}
