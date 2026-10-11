using Bunit;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Admin.Common.Constants;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Components;
using Osirion.Blazor.Cms.Admin.Features.Dashboard.Components;
using Osirion.Blazor.Cms.Admin.Features.Repository.Components;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Shouldly;
using ContentEditorComponent = Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.ContentEditor;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Dashboard.Components;

public class CmsAdminDashboardBehaviorTests : AdminComponentTestContext
{
    private readonly IGitHubAdminService _gitHub = Substitute.For<IGitHubAdminService>();
    private readonly CmsState _state = new();

    public CmsAdminDashboardBehaviorTests()
    {
        Services.AddSingleton(_gitHub);
        Services.AddSingleton(_state);
        ComponentFactories.AddStub<RepositorySelector>();
        ComponentFactories.AddStub<BranchSelector>();
        ComponentFactories.AddStub<FileExplorer>();
        ComponentFactories.AddStub<ContentEditorComponent>();
        RenderInteractively();
    }

    private void SelectRepositoryAndBranch(string path = "")
    {
        _state.SelectRepository(new GitHubRepository { Name = "site" });
        _state.SelectBranch(new GitHubBranch { Name = "main" });
        if (path.Length > 0)
        {
            _state.SetCurrentPath(path, []);
        }
    }

    private static List<string> BreadcrumbItems(IRenderedComponent<CmsAdminDashboard> cut) =>
        cut.FindAll(".osirion-admin-breadcrumb-item").Select(item => item.TextContent).ToList();

    [Fact]
    public void CmsAdminDashboard_NothingSelected_ShowsOnlyTheDashboardCrumbAndNoNewFileButton()
    {
        var cut = Render<CmsAdminDashboard>();

        BreadcrumbItems(cut).ShouldBe(["Dashboard"]);
        cut.FindAll(".osirion-admin-button-primary").ShouldBeEmpty();
        cut.Find(".osirion-admin-dashboard").ClassList.ShouldContain("osirion-admin-theme-light");
    }

    [Fact]
    public void CmsAdminDashboard_RepositoryWithoutBranch_ShowsTheRepositoryCrumb()
    {
        _state.SelectRepository(new GitHubRepository { Name = "site" });

        var cut = Render<CmsAdminDashboard>();

        BreadcrumbItems(cut).ShouldBe(["Dashboard", "site"]);
    }

    [Fact]
    public void CmsAdminDashboard_RepositoryBranchAndPath_ShowsEveryCrumbAndTheNewFileButton()
    {
        SelectRepositoryAndBranch("blog");

        var cut = Render<CmsAdminDashboard>();

        BreadcrumbItems(cut).ShouldBe(["Dashboard", "site", "main", "blog"]);
        cut.Find(".osirion-admin-button-primary").TextContent.ShouldContain("New File");
    }

    [Fact]
    public void CmsAdminDashboard_CustomClass_IsAppendedToTheRootClass()
    {
        var cut = Render<CmsAdminDashboard>(parameters => parameters.Add(p => p.Class, "extra"));

        cut.Find(".osirion-admin-dashboard").ClassList.ShouldContain("extra");
    }

    [Fact]
    public void CmsAdminDashboard_NewFileInRoot_StartsEditingANewPostAtTheRoot()
    {
        SelectRepositoryAndBranch();
        var cut = Render<CmsAdminDashboard>();

        cut.Find(".osirion-admin-button-primary").Click();

        _state.IsEditing.ShouldBeTrue();
        _state.IsCreatingNewFile.ShouldBeTrue();
        _state.EditingPost!.Path.ShouldBe("new-post.md");
        cut.FindComponents<Stub<ContentEditorComponent>>().ShouldHaveSingleItem();
    }

    [Fact]
    public async Task CmsAdminDashboard_CreateFileFromExplorerInADirectory_PlacesThePostInThatDirectory()
    {
        SelectRepositoryAndBranch("blog");
        var cut = Render<CmsAdminDashboard>();
        var onCreate = cut.FindComponent<Stub<FileExplorer>>().Instance.Parameters.Get(p => p.OnCreateFile);

        await cut.InvokeAsync(() => onCreate.InvokeAsync());

        _state.EditingPost!.Path.ShouldBe("blog/new-post.md");
    }

    [Fact]
    public async Task CmsAdminDashboard_SelectorCallbacks_LeaveTheStateUnchanged()
    {
        var cut = Render<CmsAdminDashboard>();
        var onRepository = cut.FindComponent<Stub<RepositorySelector>>().Instance.Parameters.Get(p => p.OnRepositoryChange);
        var onBranch = cut.FindComponent<Stub<BranchSelector>>().Instance.Parameters.Get(p => p.OnBranchChange);

        await cut.InvokeAsync(() => onRepository.InvokeAsync(new GitHubRepository { Name = "other" }));
        await cut.InvokeAsync(() => onBranch.InvokeAsync(new GitHubBranch { Name = "dev" }));

        _state.SelectedRepository.ShouldBeNull();
        _state.SelectedBranch.ShouldBeNull();
    }

    [Fact]
    public async Task CmsAdminDashboard_SaveComplete_ReloadsTheDirectoryAndStopsEditing()
    {
        SelectRepositoryAndBranch("blog");
        var items = new List<GitHubItem> { new() { Name = "post.md", Type = "file", Path = "blog/post.md" } };
        _gitHub.GetRepositoryContentsAsync("blog").Returns(items);
        _state.SetEditingPost(new ContentItem());
        var cut = Render<CmsAdminDashboard>();
        var onSave = cut.FindComponent<Stub<ContentEditorComponent>>().Instance.Parameters.Get(p => p.OnSaveComplete);

        await cut.InvokeAsync(() => onSave.InvokeAsync(new ContentItem()));

        _state.CurrentItems.ShouldBe(items);
        _state.IsEditing.ShouldBeFalse();
        cut.FindComponents<Stub<ContentEditorComponent>>().ShouldBeEmpty();
    }

    [Fact]
    public async Task CmsAdminDashboard_SaveCompleteWithoutBranch_OnlyStopsEditing()
    {
        _state.SetEditingPost(new ContentItem());
        var cut = Render<CmsAdminDashboard>();
        var onSave = cut.FindComponent<Stub<ContentEditorComponent>>().Instance.Parameters.Get(p => p.OnSaveComplete);

        await cut.InvokeAsync(() => onSave.InvokeAsync(new ContentItem()));

        _state.IsEditing.ShouldBeFalse();
        await _gitHub.DidNotReceiveWithAnyArgs().GetRepositoryContentsAsync(default!);
    }

    [Fact]
    public async Task CmsAdminDashboard_SaveCompleteReloadFails_ShowsTheErrorAndStopsEditing()
    {
        SelectRepositoryAndBranch();
        _gitHub.GetRepositoryContentsAsync(string.Empty).ThrowsAsync(new InvalidOperationException("offline"));
        _state.SetEditingPost(new ContentItem());
        var cut = Render<CmsAdminDashboard>();
        var onSave = cut.FindComponent<Stub<ContentEditorComponent>>().Instance.Parameters.Get(p => p.OnSaveComplete);

        await cut.InvokeAsync(() => onSave.InvokeAsync(new ContentItem()));

        _state.IsEditing.ShouldBeFalse();
        cut.Find(".osirion-admin-alert-error").TextContent.ShouldContain(UserMessages.LoadFailed);
        cut.Find(".osirion-admin-alert-error").TextContent.ShouldNotContain("offline");
    }

    [Fact]
    public async Task CmsAdminDashboard_Discard_StopsEditing()
    {
        _state.SetEditingPost(new ContentItem());
        var cut = Render<CmsAdminDashboard>();
        var onDiscard = cut.FindComponent<Stub<ContentEditorComponent>>().Instance.Parameters.Get(p => p.OnDiscard);

        await cut.InvokeAsync(() => onDiscard.InvokeAsync());

        _state.IsEditing.ShouldBeFalse();
    }

    [Fact]
    public void CmsAdminDashboard_StatusMessage_IsShownAndCanBeClosed()
    {
        _state.SetStatusMessage("Saved");
        var cut = Render<CmsAdminDashboard>();
        cut.Find(".osirion-admin-alert-success").TextContent.ShouldContain("Saved");

        cut.Find(".osirion-admin-alert-success .osirion-admin-alert-close").Click();

        cut.FindAll(".osirion-admin-alert").ShouldBeEmpty();
        _state.StatusMessage.ShouldBeNull();
    }

    [Fact]
    public void CmsAdminDashboard_ErrorMessage_IsShownAndCanBeClosed()
    {
        _state.SetErrorMessage("Broken");
        var cut = Render<CmsAdminDashboard>();

        cut.Find(".osirion-admin-alert-error .osirion-admin-alert-close").Click();

        _state.ErrorMessage.ShouldBeNull();
        cut.FindAll(".osirion-admin-alert-error").ShouldBeEmpty();
    }

    [Fact]
    public async Task CmsAdminDashboard_Disposed_StopsRenderingOnStateChanges()
    {
        // The dashboard subscribes to the circuit-scoped CmsState; without IDisposable on the component the
        // renderer never called Dispose, so every disposed dashboard stayed subscribed.
        var cut = Render<CmsAdminDashboard>();
        var renders = cut.RenderCount;

        await DisposeComponentsAsync();
        _state.SetStatusMessage("changed");

        cut.RenderCount.ShouldBe(renders);
    }
}
