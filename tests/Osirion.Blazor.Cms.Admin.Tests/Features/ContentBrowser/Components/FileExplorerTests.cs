using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Admin.Common.Constants;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Components;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.ViewModels;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentBrowser.Components;

public class FileExplorerTests : AdminComponentTestContext
{
    private readonly IContentRepositoryAdapter _adapter = Substitute.For<IContentRepositoryAdapter>();
    private readonly CmsState _state = new();
    private FileExplorerViewModel? _viewModel;

    public FileExplorerTests()
    {
        var mediator = new CmsEventMediator(NullLogger<CmsEventMediator>.Instance);
        var service = new ContentBrowserService(_adapter, mediator, NullLogger<ContentBrowserService>.Instance);
        Services.AddSingleton(_state);
        Services.AddSingleton(provider => _viewModel = new FileExplorerViewModel(
            service,
            _state,
            provider.GetRequiredService<NavigationManager>(),
            mediator,
            NullLogger<FileExplorerViewModel>.Instance));
    }

    private void SelectRepositoryAndBranch()
    {
        _state.SelectRepository(new GitHubRepository { Name = "site" });
        _state.SelectBranch(new GitHubBranch { Name = "main" });
    }

    private void ReturnContents(string path, params GitHubItem[] items) =>
        _adapter.GetContentsAsync(path).Returns(_ => items.ToList());

    private static GitHubItem File(string name, string path) => new() { Name = name, Type = "file", Path = path, Sha = "sha-" + name };

    private static GitHubItem Directory(string name, string path) => new() { Name = name, Type = "dir", Path = path };

    [Fact]
    public void FileExplorer_NoRepositorySelected_AsksForARepositoryAndShowsRootPath()
    {
        var cut = Render<FileExplorer>();

        cut.Markup.ShouldContain("Please select a repository and branch.");
        cut.Find(".file-path").TextContent.Trim().ShouldBe("/");
        _adapter.DidNotReceiveWithAnyArgs().GetContentsAsync(default!);
    }

    [Fact]
    public void FileExplorer_EmptyDirectory_ShowsEmptyState()
    {
        SelectRepositoryAndBranch();
        ReturnContents(string.Empty);

        var cut = Render<FileExplorer>();

        cut.Markup.ShouldContain("No files found in this directory.");
    }

    [Fact]
    public void FileExplorer_Loading_ShowsSpinner()
    {
        SelectRepositoryAndBranch();
        var pending = new TaskCompletionSource<List<GitHubItem>>();
        _adapter.GetContentsAsync(string.Empty).Returns(pending.Task);

        var cut = Render<FileExplorer>();

        cut.Find(".spinner-border").ShouldNotBeNull();
        pending.SetResult([]);
    }

    [Fact]
    public void FileExplorer_LoadFails_ShowsErrorAndRetryReloads()
    {
        SelectRepositoryAndBranch();
        _adapter.GetContentsAsync(string.Empty).ThrowsAsync(new InvalidOperationException("offline"));
        RenderInteractively();

        var cut = Render<FileExplorer>();
        cut.Find(".alert-danger").TextContent.ShouldContain(UserMessages.LoadFailed);
        cut.Find(".alert-danger").TextContent.ShouldNotContain("offline");
        ReturnContents(string.Empty, File("post.md", "post.md"));
        cut.FindAll("button").Single(button => button.TextContent.Trim() == "Retry").Click();

        cut.FindAll(".alert-danger").ShouldBeEmpty();
        cut.Markup.ShouldContain("post.md");
    }

    [Fact]
    public void FileExplorer_RootContents_ListsDirectoriesThenFilesSortedWithMatchingIcons()
    {
        SelectRepositoryAndBranch();
        ReturnContents(string.Empty, File("b.txt", "b.txt"), Directory("zeta", "zeta"), File("a.md", "a.md"), Directory("alpha", "alpha"));

        var cut = Render<FileExplorer>();

        var names = cut.FindAll(".list-group-item span").Select(span => span.TextContent).ToList();
        names.ShouldBe(["alpha", "zeta", "a.md", "b.txt"]);
        cut.FindAll(".bi-folder-fill").Count.ShouldBe(2);
        cut.FindAll(".bi-markdown-fill").Count.ShouldBe(1);
        cut.FindAll(".bi-file-earmark").Count.ShouldBe(1);
        cut.Markup.ShouldNotContain("bi-reply-fill");
    }

    [Fact]
    public void FileExplorer_ClickDirectory_NavigatesIntoItAndShowsParentLink()
    {
        SelectRepositoryAndBranch();
        ReturnContents(string.Empty, Directory("blog", "blog"));
        ReturnContents("blog", File("post.md", "blog/post.md"));
        RenderInteractively();
        var cut = Render<FileExplorer>();

        cut.FindAll("button.list-group-item").Single(button => button.TextContent.Contains("blog")).Click();

        cut.Find(".file-path").TextContent.Trim().ShouldBe("blog");
        cut.Find(".bi-reply-fill").ShouldNotBeNull();
        cut.Markup.ShouldContain("post.md");
    }

    [Fact]
    public void FileExplorer_ClickParentLink_ReturnsToTheParentDirectory()
    {
        SelectRepositoryAndBranch();
        _state.SetCurrentPath("blog", []);
        ReturnContents("blog", File("post.md", "blog/post.md"));
        ReturnContents(string.Empty, Directory("blog", "blog"));
        RenderInteractively();
        var cut = Render<FileExplorer>();

        cut.Find(".bi-reply-fill").ParentElement!.ParentElement!.Click();

        cut.Find(".file-path").TextContent.Trim().ShouldBe("/");
    }

    [Fact]
    public void FileExplorer_ClickRootButton_LoadsTheRoot()
    {
        SelectRepositoryAndBranch();
        _state.SetCurrentPath("blog", []);
        ReturnContents("blog", File("post.md", "blog/post.md"));
        ReturnContents(string.Empty, Directory("blog", "blog"));
        RenderInteractively();
        var cut = Render<FileExplorer>();

        cut.Find("button[title='Go to root']").Click();

        cut.Find(".file-path").TextContent.Trim().ShouldBe("/");
        _adapter.Received(1).GetContentsAsync(string.Empty);
    }

    [Fact]
    public void FileExplorer_ClickRefresh_ReloadsTheCurrentPath()
    {
        SelectRepositoryAndBranch();
        ReturnContents(string.Empty, File("post.md", "post.md"));
        RenderInteractively();
        var cut = Render<FileExplorer>();

        cut.Find("button[title='Refresh']").Click();

        _adapter.Received(2).GetContentsAsync(string.Empty);
    }

    [Fact]
    public void FileExplorer_ClickMarkdownFile_MarksItActiveAndOpensTheEditor()
    {
        SelectRepositoryAndBranch();
        ReturnContents(string.Empty, File("post.md", "post.md"));
        RenderInteractively();
        var cut = Render<FileExplorer>();

        cut.Find("div.list-group-item").Click();

        cut.Find("div.list-group-item").ClassList.ShouldContain("active");
        cut.Find("button[title='Delete']").ClassList.ShouldContain("text-white");
        Services.GetRequiredService<NavigationManager>().Uri.ShouldEndWith("/osirion/content/edit?Path=post.md");
    }

    [Fact]
    public void FileExplorer_DeleteThenCancel_ClosesTheDialogWithoutDeleting()
    {
        SelectRepositoryAndBranch();
        ReturnContents(string.Empty, File("post.md", "post.md"));
        RenderInteractively();
        var cut = Render<FileExplorer>();

        cut.Find("button[title='Delete']").Click();
        cut.Find(".modal-body").TextContent.ShouldContain("\"post.md\"");
        cut.FindAll(".modal-footer button").Single(button => button.TextContent.Trim() == "Cancel").Click();

        cut.FindAll(".modal").ShouldBeEmpty();
        _adapter.DidNotReceiveWithAnyArgs().DeleteFileAsync(default!, default!, default!);
    }

    [Fact]
    public void FileExplorer_CloseButton_ClosesTheDialog()
    {
        SelectRepositoryAndBranch();
        ReturnContents(string.Empty, File("post.md", "post.md"));
        RenderInteractively();
        var cut = Render<FileExplorer>();

        cut.Find("button[title='Delete']").Click();
        cut.Find(".btn-close").Click();

        cut.FindAll(".modal").ShouldBeEmpty();
    }

    [Fact]
    public void FileExplorer_ConfirmDelete_ShowsProgressThenDeletesTheFile()
    {
        SelectRepositoryAndBranch();
        ReturnContents(string.Empty, File("post.md", "post.md"));
        var pendingDelete = new TaskCompletionSource<GitHubFileCommitResponse>();
        _adapter.DeleteFileAsync("post.md", Arg.Any<string>(), "sha-post.md").Returns(pendingDelete.Task);
        RenderInteractively();
        var cut = Render<FileExplorer>();

        cut.Find("button[title='Delete']").Click();
        cut.Find(".modal-footer .btn-danger").Click();
        cut.Find(".modal-footer .btn-danger").HasAttribute("disabled").ShouldBeTrue();
        cut.Find(".modal-footer").TextContent.ShouldContain("Deleting...");
        pendingDelete.SetResult(new GitHubFileCommitResponse());

        cut.WaitForAssertion(() => cut.FindAll(".modal").ShouldBeEmpty());
        _state.StatusMessage.ShouldBe("File post.md deleted successfully");
    }

    [Fact]
    public async Task FileExplorer_Dispose_StopsListeningToTheViewModel()
    {
        var cut = Render<FileExplorer>();
        var renders = cut.RenderCount;

        await DisposeComponentsAsync();
        _state.SetStatusMessage("changed");

        cut.RenderCount.ShouldBe(renders);
        _viewModel.ShouldNotBeNull();
    }
}
