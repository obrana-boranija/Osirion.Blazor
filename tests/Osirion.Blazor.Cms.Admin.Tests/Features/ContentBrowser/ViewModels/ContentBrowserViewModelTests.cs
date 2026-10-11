using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.ViewModels;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentBrowser.ViewModels;

public class ContentBrowserViewModelTests
{
    private readonly IContentRepositoryAdapter _adapter = Substitute.For<IContentRepositoryAdapter>();
    private readonly CmsState _state = new();
    private readonly CmsEventMediator _mediator = new(NullLogger<CmsEventMediator>.Instance);
    private readonly ContentBrowserViewModel _viewModel;
    private int _notifications;

    public ContentBrowserViewModelTests()
    {
        var service = new ContentBrowserService(_adapter, _mediator, NullLogger<ContentBrowserService>.Instance);
        _viewModel = new ContentBrowserViewModel(service, _state, _mediator);
        _viewModel.StateChanged += () => _notifications++;
    }

    private void SelectRepositoryAndBranch()
    {
        _state.SelectRepository(new GitHubRepository { Name = "site" });
        _state.SelectBranch(new GitHubBranch { Name = "main" });
    }

    [Fact]
    public void IsValidState_NoRepository_ReturnsFalse()
    {
        _viewModel.IsValidState().ShouldBeFalse();
    }

    [Fact]
    public void IsValidState_RepositoryWithoutBranch_ReturnsFalse()
    {
        _state.SelectRepository(new GitHubRepository { Name = "site" });

        _viewModel.IsValidState().ShouldBeFalse();
    }

    [Fact]
    public void IsValidState_RepositoryAndBranch_ReturnsTrue()
    {
        SelectRepositoryAndBranch();

        _viewModel.IsValidState().ShouldBeTrue();
    }

    [Fact]
    public void AppStateChange_RaisesStateChanged()
    {
        _state.SetStatusMessage("hello");

        _notifications.ShouldBe(1);
    }

    [Fact]
    public async Task RefreshContentsAsync_InvalidState_DoesNotCallTheRepository()
    {
        await _viewModel.RefreshContentsAsync();

        await _adapter.DidNotReceiveWithAnyArgs().GetContentsAsync(default!);
        _viewModel.Contents.ShouldBeEmpty();
    }

    [Fact]
    public async Task RefreshContentsAsync_ValidState_LoadsContentsIntoViewModelAndState()
    {
        SelectRepositoryAndBranch();
        var items = new List<GitHubItem> { new() { Name = "post.md", Type = "file", Path = "post.md" } };
        _adapter.GetContentsAsync(string.Empty).Returns(items);

        await _viewModel.RefreshContentsAsync();

        _viewModel.Contents.ShouldBe(items);
        _state.CurrentItems.ShouldBe(items);
        _viewModel.IsLoading.ShouldBeFalse();
        _viewModel.ErrorMessage.ShouldBeNull();
    }

    [Fact]
    public async Task RefreshContentsAsync_RepositoryFails_SetsErrorMessageOnViewModelAndState()
    {
        SelectRepositoryAndBranch();
        _adapter.GetContentsAsync(string.Empty).ThrowsAsync(new InvalidOperationException("offline"));

        await _viewModel.RefreshContentsAsync();

        _viewModel.ErrorMessage.ShouldBe("Failed to load contents: offline");
        _state.ErrorMessage.ShouldBe("Failed to load contents: offline");
        _viewModel.IsLoading.ShouldBeFalse();
    }

    [Fact]
    public async Task NavigateToPathAsync_SetsThePathAndLoadsIt()
    {
        SelectRepositoryAndBranch();
        _adapter.GetContentsAsync("blog").Returns(new List<GitHubItem>());

        await _viewModel.NavigateToPathAsync("blog");

        _viewModel.CurrentPath.ShouldBe("blog");
        await _adapter.Received(1).GetContentsAsync("blog");
    }

    [Fact]
    public async Task SelectItemAsync_Directory_NavigatesIntoIt()
    {
        SelectRepositoryAndBranch();
        var directory = new GitHubItem { Name = "blog", Type = "dir", Path = "blog" };
        _adapter.GetContentsAsync("blog").Returns(new List<GitHubItem>());

        await _viewModel.SelectItemAsync(directory);

        _viewModel.SelectedItem.ShouldBe(directory);
        _viewModel.CurrentPath.ShouldBe("blog");
    }

    [Fact]
    public async Task SelectItemAsync_MarkdownFile_PublishesContentSelected()
    {
        var selections = EventRecorder.Record<ContentSelectedEvent>(_mediator);
        var file = new GitHubItem { Name = "post.md", Type = "file", Path = "blog/post.md" };

        await _viewModel.SelectItemAsync(file);

        selections.ShouldHaveSingleItem().Path.ShouldBe("blog/post.md");
        _viewModel.IsItemSelected(file).ShouldBeTrue();
    }

    [Fact]
    public async Task SelectItemAsync_OtherFile_OnlySelectsIt()
    {
        var selections = EventRecorder.Record<ContentSelectedEvent>(_mediator);
        var image = new GitHubItem { Name = "logo.png", Type = "file", Path = "logo.png" };

        await _viewModel.SelectItemAsync(image);

        selections.ShouldBeEmpty();
        _viewModel.SelectedItem.ShouldBe(image);
        _viewModel.IsItemSelected(new GitHubItem { Path = "other.png" }).ShouldBeFalse();
    }

    [Fact]
    public async Task DeleteFileAsync_Directory_DoesNothing()
    {
        await _viewModel.DeleteFileAsync(new GitHubItem { Name = "blog", Type = "dir", Path = "blog" });

        await _adapter.DidNotReceiveWithAnyArgs().DeleteFileAsync(default!, default!, default!);
        _notifications.ShouldBe(0);
    }

    [Fact]
    public async Task DeleteFileAsync_File_DeletesRefreshesAndReportsSuccess()
    {
        SelectRepositoryAndBranch();
        var deletions = EventRecorder.Record<ContentDeletedEvent>(_mediator);
        var file = new GitHubItem { Name = "post.md", Type = "file", Path = "post.md", Sha = "sha1" };
        _adapter.DeleteFileAsync("post.md", Arg.Any<string>(), "sha1").Returns(new GitHubFileCommitResponse());
        _adapter.GetContentsAsync(string.Empty).Returns(new List<GitHubItem>());

        await _viewModel.DeleteFileAsync(file);

        _state.StatusMessage.ShouldBe("File post.md deleted successfully");
        deletions.ShouldAllBe(deleted => deleted.Path == "post.md");
        deletions.Count.ShouldBe(2);
        await _adapter.Received(1).GetContentsAsync(string.Empty);
        _viewModel.IsLoading.ShouldBeFalse();
    }

    [Fact]
    public async Task DeleteFileAsync_DeleteFails_SetsErrorMessage()
    {
        var file = new GitHubItem { Name = "post.md", Type = "file", Path = "post.md", Sha = "sha1" };
        _adapter.DeleteFileAsync("post.md", Arg.Any<string>(), "sha1").ThrowsAsync(new InvalidOperationException("denied"));

        await _viewModel.DeleteFileAsync(file);

        _viewModel.ErrorMessage.ShouldBe("Failed to delete file: denied");
        _state.ErrorMessage.ShouldBe("Failed to delete file: denied");
        _viewModel.IsLoading.ShouldBeFalse();
    }
}
