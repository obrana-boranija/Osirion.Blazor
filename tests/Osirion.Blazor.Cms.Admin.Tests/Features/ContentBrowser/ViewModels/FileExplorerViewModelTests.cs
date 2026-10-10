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

public class FileExplorerViewModelTests
{
    private readonly IContentRepositoryAdapter _repositoryAdapter;
    private readonly CmsState _state;
    private readonly TestNavigationManager _navigationManager;
    private readonly CmsEventMediator _eventMediator;
    private readonly List<ContentSelectedEvent> _selections;
    private readonly List<StatusNotificationEvent> _notifications;
    private readonly FileExplorerViewModel _viewModel;

    public FileExplorerViewModelTests()
    {
        _repositoryAdapter = Substitute.For<IContentRepositoryAdapter>();
        _eventMediator = new CmsEventMediator(NullLogger<CmsEventMediator>.Instance);
        // ContentBrowserService has no virtual members, so the view model gets the real service over a substituted adapter.
        var browserService = new ContentBrowserService(
            _repositoryAdapter,
            _eventMediator,
            NullLogger<ContentBrowserService>.Instance);
        _state = new CmsState();
        _navigationManager = new TestNavigationManager();
        _selections = EventRecorder.Record<ContentSelectedEvent>(_eventMediator);
        _notifications = EventRecorder.Record<StatusNotificationEvent>(_eventMediator);

        _viewModel = new FileExplorerViewModel(
            browserService,
            _state,
            _navigationManager,
            _eventMediator,
            NullLogger<FileExplorerViewModel>.Instance
        );
    }

    [Fact]
    public async Task LoadContentsAsync_ShouldUpdateStateWithContents()
    {
        // Arrange
        var repository = new GitHubRepository { Name = "test-repo" };
        var branch = new GitHubBranch { Name = "main" };
        _state.SelectRepository(repository);
        _state.SelectBranch(branch);

        var path = "content/blog";
        _state.SetCurrentPath(path, new List<GitHubItem>());

        var expectedContents = new List<GitHubItem>
        {
            new GitHubItem { Name = "post.md", Type = "file", Path = "content/blog/post.md" }
        };

        _repositoryAdapter.GetContentsAsync(path).Returns(expectedContents);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        await _viewModel.LoadContentsAsync();

        // Assert
        _state.CurrentItems.ShouldBe(expectedContents);
        _viewModel.IsLoading.ShouldBeFalse();
        _viewModel.ErrorMessage.ShouldBeNull();
        stateChangedCalled.ShouldBeTrue();

        await _repositoryAdapter.Received(1).GetContentsAsync(path);
    }

    [Fact]
    public async Task LoadContentsAsync_WhenRepositoryOrBranchNotSelected_ShouldDoNothing()
    {
        // Arrange - No repository and branch selected
        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        await _viewModel.LoadContentsAsync();

        // Assert
        await _repositoryAdapter.DidNotReceive().GetContentsAsync(Arg.Any<string>());
        stateChangedCalled.ShouldBeFalse();
    }

    [Fact]
    public async Task LoadContentsAsync_WhenExceptionThrown_ShouldSetErrorAndUpdateState()
    {
        // Arrange
        var repository = new GitHubRepository { Name = "test-repo" };
        var branch = new GitHubBranch { Name = "main" };
        _state.SelectRepository(repository);
        _state.SelectBranch(branch);

        var path = "content/blog";
        _state.SetCurrentPath(path, new List<GitHubItem>());

        var exception = new InvalidOperationException("Test error");
        _repositoryAdapter.GetContentsAsync(path).Throws(exception);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        await _viewModel.LoadContentsAsync();

        // Assert
        _viewModel.IsLoading.ShouldBeFalse();
        _viewModel.ErrorMessage.ShouldNotBeNullOrEmpty();
        _viewModel.ErrorMessage.ShouldContain("Failed to load contents");
        _viewModel.ErrorMessage.ShouldContain(exception.Message);
        _state.ErrorMessage.ShouldBe(_viewModel.ErrorMessage);
        stateChangedCalled.ShouldBeTrue();

        await _repositoryAdapter.Received(1).GetContentsAsync(path);
    }

    [Fact]
    public async Task NavigateToPathAsync_ShouldUpdatePathAndLoadContents()
    {
        // Arrange
        var repository = new GitHubRepository { Name = "test-repo" };
        var branch = new GitHubBranch { Name = "main" };
        _state.SelectRepository(repository);
        _state.SelectBranch(branch);

        var currentPath = "content";
        var newPath = "content/blog";
        _state.SetCurrentPath(currentPath, new List<GitHubItem>());

        var expectedContents = new List<GitHubItem>
        {
            new GitHubItem { Name = "post.md", Type = "file", Path = "content/blog/post.md" }
        };

        _repositoryAdapter.GetContentsAsync(newPath).Returns(expectedContents);

        // Act
        await _viewModel.NavigateToPathAsync(newPath);

        // Assert
        _state.CurrentPath.ShouldBe(newPath);
        await _repositoryAdapter.Received(1).GetContentsAsync(newPath);
    }

    [Fact]
    public async Task NavigateToRootAsync_ShouldNavigateToEmptyPath()
    {
        // Arrange
        var repository = new GitHubRepository { Name = "test-repo" };
        var branch = new GitHubBranch { Name = "main" };
        _state.SelectRepository(repository);
        _state.SelectBranch(branch);

        var currentPath = "content/blog";
        _state.SetCurrentPath(currentPath, new List<GitHubItem>());

        var rootContents = new List<GitHubItem>
        {
            new GitHubItem { Name = "content", Type = "dir", Path = "content" }
        };

        _repositoryAdapter.GetContentsAsync(string.Empty).Returns(rootContents);

        // Act
        await _viewModel.NavigateToRootAsync();

        // Assert
        _state.CurrentPath.ShouldBe(string.Empty);
        await _repositoryAdapter.Received(1).GetContentsAsync(string.Empty);
    }

    [Fact]
    public async Task NavigateToParentDirectoryAsync_ShouldNavigateToParentPath()
    {
        // Arrange
        var repository = new GitHubRepository { Name = "test-repo" };
        var branch = new GitHubBranch { Name = "main" };
        _state.SelectRepository(repository);
        _state.SelectBranch(branch);

        var currentPath = "content/blog";
        _state.SetCurrentPath(currentPath, new List<GitHubItem>());

        var parentContents = new List<GitHubItem>
        {
            new GitHubItem { Name = "blog", Type = "dir", Path = "content/blog" }
        };

        _repositoryAdapter.GetContentsAsync("content").Returns(parentContents);

        // Act
        await _viewModel.NavigateToParentDirectoryAsync();

        // Assert
        _state.CurrentPath.ShouldBe("content");
        await _repositoryAdapter.Received(1).GetContentsAsync("content");
    }

    [Fact]
    public async Task NavigateToParentDirectoryAsync_WhenAtRoot_ShouldDoNothing()
    {
        // Arrange
        var repository = new GitHubRepository { Name = "test-repo" };
        var branch = new GitHubBranch { Name = "main" };
        _state.SelectRepository(repository);
        _state.SelectBranch(branch);

        var currentPath = string.Empty;
        _state.SetCurrentPath(currentPath, new List<GitHubItem>());

        // Act
        await _viewModel.NavigateToParentDirectoryAsync();

        // Assert
        _state.CurrentPath.ShouldBe(string.Empty);
        await _repositoryAdapter.DidNotReceive().GetContentsAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task HandleItemClickAsync_WithDirectory_ShouldNavigateToDirectory()
    {
        // Arrange
        var repository = new GitHubRepository { Name = "test-repo" };
        var branch = new GitHubBranch { Name = "main" };
        _state.SelectRepository(repository);
        _state.SelectBranch(branch);

        var currentPath = "content";
        _state.SetCurrentPath(currentPath, new List<GitHubItem>());

        var directoryItem = new GitHubItem
        {
            Name = "blog",
            Type = "dir",
            Path = "content/blog"
        };

        var dirContents = new List<GitHubItem>
        {
            new GitHubItem { Name = "post.md", Type = "file", Path = "content/blog/post.md" }
        };

        _repositoryAdapter.GetContentsAsync("content/blog").Returns(dirContents);

        // Act
        await _viewModel.HandleItemClickAsync(directoryItem);

        // Assert
        _viewModel.SelectedItem.ShouldBe(directoryItem);
        _state.CurrentPath.ShouldBe("content/blog");
        await _repositoryAdapter.Received(1).GetContentsAsync("content/blog");
    }

    [Fact]
    public async Task HandleItemClickAsync_WithMarkdownFile_ShouldPublishContentSelectedEvent()
    {
        // Arrange
        var fileItem = new GitHubItem
        {
            Name = "post.md",
            Type = "file",
            Path = "content/blog/post.md"
        };

        // Act
        await _viewModel.HandleItemClickAsync(fileItem);

        // Assert
        _viewModel.SelectedItem.ShouldBe(fileItem);
        _selections.ShouldHaveSingleItem().Path.ShouldBe(fileItem.Path);
        _navigationManager.Navigations.ShouldHaveSingleItem().Uri.ShouldContain(fileItem.Path);
    }

    [Fact]
    public async Task HandleItemClickAsync_WithNonMarkdownFile_ShouldJustSelectItem()
    {
        // Arrange
        var fileItem = new GitHubItem
        {
            Name = "image.png",
            Type = "file",
            Path = "content/blog/image.png"
        };

        // Act
        await _viewModel.HandleItemClickAsync(fileItem);

        // Assert
        _viewModel.SelectedItem.ShouldBe(fileItem);
        var notification = _notifications.ShouldHaveSingleItem();
        notification.Message.ShouldContain("Selected file");
        notification.Message.ShouldContain(fileItem.Name);
        notification.Type.ShouldBe(StatusType.Info);
        _navigationManager.Navigations.ShouldBeEmpty();
    }

    [Fact]
    public async Task DeleteFileAsync_ShouldDeleteFileAndRefreshContents()
    {
        // Arrange
        var repository = new GitHubRepository { Name = "test-repo" };
        var branch = new GitHubBranch { Name = "main" };
        _state.SelectRepository(repository);
        _state.SelectBranch(branch);

        var path = "content/blog";
        _state.SetCurrentPath(path, new List<GitHubItem>());

        var fileItem = new GitHubItem
        {
            Name = "post.md",
            Type = "file",
            Path = "content/blog/post.md",
            Sha = "test-sha"
        };

        _viewModel.ShowDeleteConfirmation(fileItem);

        var updatedContents = new List<GitHubItem>
        {
            new GitHubItem { Name = "another-post.md", Type = "file", Path = "content/blog/another-post.md" }
        };

        _repositoryAdapter.GetContentsAsync(path).Returns(updatedContents);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        await _viewModel.DeleteFileAsync();

        // Assert
        _viewModel.IsShowingDeleteConfirmation.ShouldBeFalse();
        _viewModel.FileToDelete.ShouldBeNull();
        _viewModel.IsDeletingFile.ShouldBeFalse();
        stateChangedCalled.ShouldBeTrue();

        await _repositoryAdapter.Received(1).DeleteFileAsync(fileItem.Path, Arg.Any<string>(), fileItem.Sha!);
        await _repositoryAdapter.Received(1).GetContentsAsync(path);

        _state.StatusMessage!.ShouldContain("deleted successfully");
    }

    [Fact]
    public async Task DeleteFileAsync_WhenExceptionThrown_ShouldSetErrorAndUpdateState()
    {
        // Arrange
        var repository = new GitHubRepository { Name = "test-repo" };
        var branch = new GitHubBranch { Name = "main" };
        _state.SelectRepository(repository);
        _state.SelectBranch(branch);

        var fileItem = new GitHubItem
        {
            Name = "post.md",
            Type = "file",
            Path = "content/blog/post.md",
            Sha = "test-sha"
        };

        _viewModel.ShowDeleteConfirmation(fileItem);

        var exception = new InvalidOperationException("Test error");
        _repositoryAdapter.DeleteFileAsync(fileItem.Path, Arg.Any<string>(), fileItem.Sha!).Throws(exception);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        await _viewModel.DeleteFileAsync();

        // Assert
        _viewModel.IsShowingDeleteConfirmation.ShouldBeFalse();
        _viewModel.FileToDelete.ShouldBeNull();
        _viewModel.IsDeletingFile.ShouldBeFalse();
        _viewModel.ErrorMessage.ShouldNotBeNullOrEmpty();
        _viewModel.ErrorMessage.ShouldContain("Failed to delete file");
        _viewModel.ErrorMessage.ShouldContain(exception.Message);
        _state.ErrorMessage.ShouldBe(_viewModel.ErrorMessage);
        stateChangedCalled.ShouldBeTrue();

        await _repositoryAdapter.Received(1).DeleteFileAsync(fileItem.Path, Arg.Any<string>(), fileItem.Sha!);
        await _repositoryAdapter.DidNotReceive().GetContentsAsync(Arg.Any<string>());
    }

    [Fact]
    public void ShowDeleteConfirmation_ShouldShowConfirmationDialog()
    {
        // Arrange
        var fileItem = new GitHubItem
        {
            Name = "post.md",
            Type = "file",
            Path = "content/blog/post.md",
            Sha = "test-sha"
        };

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        _viewModel.ShowDeleteConfirmation(fileItem);

        // Assert
        _viewModel.IsShowingDeleteConfirmation.ShouldBeTrue();
        _viewModel.FileToDelete.ShouldBe(fileItem);
        stateChangedCalled.ShouldBeTrue();
    }

    [Fact]
    public void CancelDelete_ShouldHideConfirmationDialog()
    {
        // Arrange
        var fileItem = new GitHubItem
        {
            Name = "post.md",
            Type = "file",
            Path = "content/blog/post.md",
            Sha = "test-sha"
        };

        _viewModel.ShowDeleteConfirmation(fileItem);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        _viewModel.CancelDelete();

        // Assert
        _viewModel.IsShowingDeleteConfirmation.ShouldBeFalse();
        _viewModel.FileToDelete.ShouldBeNull();
        stateChangedCalled.ShouldBeTrue();
    }

    [Fact]
    public void IsItemSelected_ShouldCheckItemSelection()
    {
        // Arrange
        var item1 = new GitHubItem { Path = "path/to/item1" };
        var item2 = new GitHubItem { Path = "path/to/item2" };

        // Select item1
        _viewModel.SelectItem(item1);

        // Act & Assert
        _viewModel.IsItemSelected(item1).ShouldBeTrue();
        _viewModel.IsItemSelected(item2).ShouldBeFalse();
    }
}