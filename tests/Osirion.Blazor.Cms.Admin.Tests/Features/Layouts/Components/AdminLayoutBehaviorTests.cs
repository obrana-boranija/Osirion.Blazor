using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Features.Layouts.Components;
using Osirion.Blazor.Cms.Admin.Features.Layouts.Models;
using Osirion.Blazor.Cms.Admin.Features.Navigation.Components;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Layouts.Components;

public class AdminLayoutBehaviorTests : AdminComponentTestContext
{
    private readonly IAuthenticationService _authService = Substitute.For<IAuthenticationService>();
    private readonly CmsState _state = new();

    public AdminLayoutBehaviorTests()
    {
        Services.AddSingleton(_authService);
        Services.AddSingleton(Substitute.For<IGitHubTokenStore>());
        Services.AddSingleton(_state);
        ComponentFactories.AddStub<DefaultNavigation>("<nav id=\"default-navigation\"></nav>");
        RenderInteractively();
    }

    /// <summary>Exposes the inherited error message, which only a derived layout can set.</summary>
    public sealed class ErrorLayout : AdminLayout
    {
        public void ShowError(string message)
        {
            ErrorMessage = message;
        }
    }

    private static List<string> Crumbs<TLayout>(IRenderedComponent<TLayout> cut) where TLayout : AdminLayout =>
        cut.FindAll(".breadcrumb-item").Select(item => item.TextContent.Trim()).ToList();

    [Fact]
    public void AdminLayout_Defaults_ShowDefaultNavigationDashboardCrumbAndMoonIcon()
    {
        var cut = Render<AdminLayout>();

        cut.Find("#default-navigation").ShouldNotBeNull();
        Crumbs(cut).ShouldBe(["Dashboard"]);
        cut.Find(".theme-toggle .bi-moon").ShouldNotBeNull();
        cut.FindAll(".sidebar-subtitle").ShouldBeEmpty();
        cut.Find(".sidebar-footer").TextContent.Trim().ShouldBeEmpty();
        cut.FindAll(".alert").ShouldBeEmpty();
    }

    [Fact]
    public void AdminLayout_SubtitleAndCurrentPage_AreShown()
    {
        var cut = Render<AdminLayout>(parameters => parameters
            .Add(p => p.Subtitle, "Content")
            .Add(p => p.CurrentPage, "Settings"));

        cut.Find(".sidebar-subtitle").TextContent.ShouldBe("Content");
        Crumbs(cut).ShouldBe(["Dashboard", "Settings"]);
        cut.Find(".breadcrumb-item.active").TextContent.ShouldBe("Settings");
    }

    [Fact]
    public void AdminLayout_BreadcrumbItems_LinkEveryItemWithAUrlExceptTheLast()
    {
        var cut = Render<AdminLayout>(parameters => parameters
            .Add(p => p.CurrentPage, "Ignored")
            .Add(p => p.BreadcrumbItems, [new BreadcrumbItem("Content", "/admin/content"), new BreadcrumbItem("Drafts"), new BreadcrumbItem("Post", "/admin/post")]));

        Crumbs(cut).ShouldBe(["Dashboard", "Content", "Drafts", "Post"]);
        cut.FindAll(".breadcrumb-item a").Select(link => link.GetAttribute("href")).ShouldBe(["/admin", "/admin/content"]);
        cut.FindAll(".breadcrumb-item.active").Select(item => item.TextContent).ShouldBe(["Drafts", "Post"]);
    }

    [Fact]
    public void AdminLayout_EmptyBreadcrumbItems_FallBackToTheCurrentPage()
    {
        var cut = Render<AdminLayout>(parameters => parameters
            .Add(p => p.CurrentPage, "Media")
            .Add(p => p.BreadcrumbItems, new List<BreadcrumbItem>()));

        Crumbs(cut).ShouldBe(["Dashboard", "Media"]);
    }

    [Fact]
    public void AdminLayout_Templates_ReplaceTheDefaults()
    {
        var cut = Render<AdminLayout>(parameters => parameters
            .Add(p => p.NavigationTemplate, "<nav id=\"custom-navigation\"></nav>")
            .Add(p => p.HeaderTemplate, "<h1 id=\"custom-header\">Header</h1>")
            .Add(p => p.ActionsTemplate, "<button id=\"custom-action\">Act</button>")
            .Add(p => p.SidebarFooterTemplate, "<p id=\"custom-footer\">Footer</p>"));

        cut.Find("#custom-navigation").ShouldNotBeNull();
        cut.FindAll("#default-navigation").ShouldBeEmpty();
        cut.Find("#custom-header").ShouldNotBeNull();
        cut.FindAll(".breadcrumb").ShouldBeEmpty();
        cut.Find("#custom-action").ShouldNotBeNull();
        cut.Find("#custom-footer").ShouldNotBeNull();
    }

    [Fact]
    public void AdminLayout_Authenticated_ShowsTheUserInitialAndName()
    {
        _authService.IsAuthenticated.Returns(true);
        _authService.Username.Returns("alice");

        var cut = Render<AdminLayout>();

        cut.Find(".avatar-circle").TextContent.Trim().ShouldBe("A");
        cut.Find(".sidebar-footer .fw-medium").TextContent.ShouldBe("alice");
    }

    [Fact]
    public void AdminLayout_SignOutWithListener_InvokesIt()
    {
        _authService.IsAuthenticated.Returns(true);
        _authService.Username.Returns("alice");
        var signedOut = false;
        var cut = Render<AdminLayout>(parameters => parameters.Add(p => p.OnSignOut, () => signedOut = true));

        cut.Find(".sidebar-footer button").Click();

        signedOut.ShouldBeTrue();
    }

    [Fact]
    public void AdminLayout_ToggleTheme_SwitchesBetweenDarkAndLightAndNotifies()
    {
        var themes = new List<string>();
        var cut = Render<AdminLayout>(parameters => parameters.Add(p => p.ThemeChanged, (string theme) => themes.Add(theme)));

        cut.Find(".theme-toggle button").Click();
        cut.Find(".theme-toggle .bi-sun").ShouldNotBeNull();
        cut.Find(".theme-toggle button").Click();

        themes.ShouldBe(["dark", "light"]);
        cut.Find(".theme-toggle .bi-moon").ShouldNotBeNull();
    }

    [Fact]
    public void AdminLayout_ToggleThemeWithoutListener_StillSwitches()
    {
        var cut = Render<AdminLayout>();

        cut.Find(".theme-toggle button").Click();

        cut.Instance.Theme.ShouldBe("dark");
    }

    [Fact]
    public void AdminLayout_CloseStatusMessage_HidesItAndNotifies()
    {
        string? reported = "unchanged";
        var cut = Render<AdminLayout>(parameters => parameters
            .Add(p => p.StatusMessage, "Saved")
            .Add(p => p.StatusMessageChanged, (string? message) => reported = message));
        cut.Find(".alert-success").TextContent.ShouldContain("Saved");

        cut.Find(".alert-success .btn-close").Click();

        reported.ShouldBeNull();
        cut.FindAll(".alert-success").ShouldBeEmpty();
    }

    [Fact]
    public async Task AdminLayout_CloseErrorMessage_HidesItAndNotifies()
    {
        string? reported = "unchanged";
        var cut = Render<ErrorLayout>(parameters => parameters
            .Add(p => p.ErrorMessageChanged, (string? message) => reported = message));
        await cut.InvokeAsync(() => cut.Instance.ShowError("Broken"));
        cut.Render();
        cut.Find(".alert-danger").TextContent.ShouldContain("Broken");

        cut.Find(".alert-danger .btn-close").Click();

        reported.ShouldBeNull();
        cut.FindAll(".alert-danger").ShouldBeEmpty();
    }

    [Fact]
    public async Task AdminLayout_Disposed_StopsRenderingOnStateChanges()
    {
        var cut = Render<AdminLayout>();
        var renders = cut.RenderCount;

        await DisposeComponentsAsync();
        _state.SetStatusMessage("changed");

        cut.RenderCount.ShouldBe(renders);
    }
}
