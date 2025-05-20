using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.ViewModels;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Shouldly;
using Microsoft.AspNetCore.Components;
using NSubstitute.ExceptionExtensions;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentBrowser.ViewModels;

public class FileExplorerViewModelTests
{
    private readonly ContentBrowserService _browserService;
    private readonly CmsState _state;
    private readonly NavigationManager _navigationManager;
    private readonly CmsEventMediator _eventMediator;
    private readonly ILogger<FileExplorerViewModel> _logger;
    private readonly FileExplorerViewModel _viewModel;

    public FileExplorerViewModelTests()
    {
        _browserService = Substitute.For<ContentBrowserService>(
            Substitute.For<Infrastructure.Adapters.IContentRepositoryAdapter>(),
            Substitute.For<CmsEventMediator>(Substitute.For<ILogger<CmsEventMediator>>()),
            Substitute.For<ILogger<ContentBrowserService>>()
        );
        _state = new CmsState();
        _navigationManager = Substitute.For<NavigationManager>();
        _eventMediator = Substitute.For<CmsEventMediator>(Substitute.For<ILogger<CmsEventMediator>>());
        _logger = Substitute.For<ILogger<FileExplorerViewModel>>();

        _viewModel = new FileExplorerViewModel(
            _browserService,
            _state,
            _navigationManager,
            _eventMediator,
            _logger
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

        _browserService.GetContentsAsync(path).Returns(expectedContents);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        await _viewModel.LoadContentsAsync();

        // Assert
        _state.CurrentItems.ShouldBe(expectedContents);
        _viewModel.IsLoading.ShouldBeFalse();
        _viewModel.ErrorMessage.ShouldBeNull();
        stateChangedCalled.ShouldBeTrue();

        await _browserService.Received(1).GetContentsAsync(path);
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
        await _browserService.DidNotReceive().GetContentsAsync(Arg.Any<string>());
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
        _browserService.GetContentsAsync(path).Throws(exception);

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

        await _browserService.Received(1).GetContentsAsync(path);
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

        _browserService.GetContentsAsync(newPath).Returns(expectedContents);

        // Act
        await _viewModel.NavigateToPathAsync(newPath);

        // Assert
        _state.CurrentPath.ShouldBe(newPath);
        await _browserService.Received(1).GetContentsAsync(newPath);
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

        _browserService.GetContentsAsync(string.Empty).Returns(rootContents);

        // Act
        await _viewModel.NavigateToRootAsync();

        // Assert
        _state.CurrentPath.ShouldBe(string.Empty);
        await _browserService.Received(1).GetContentsAsync(string.Empty);
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

        _browserService.GetContentsAsync("content").Returns(parentContents);

        // Act
        await _viewModel.NavigateToParentDirectoryAsync();

        // Assert
        _state.CurrentPath.ShouldBe("content");
        await _browserService.Received(1).GetContentsAsync("content");
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
        await _browserService.DidNotReceive().GetContentsAsync(Arg.Any<string>());
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

        _browserService.GetContentsAsync("content/blog").Returns(dirContents);

        // Act
        await _viewModel.HandleItemClickAsync(directoryItem);

        // Assert
        _viewModel.SelectedItem.ShouldBe(directoryItem);
        _state.CurrentPath.ShouldBe("content/blog");
        await _browserService.Received(1).GetContentsAsync("content/blog");
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
        _eventMediator.Received(1).Publish(
            Arg.Is<ContentSelectedEvent>(e => e.Path == fileItem.Path)
        );
        _navigationManager.Received(1).NavigateTo(
            Arg.Is<string>(s => s.Contains(fileItem.Path))
        );
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
        _eventMediator.Received(1).Publish(
            Arg.Is<StatusNotificationEvent>(e =>
                e.Message.Contains("Selected file") &&
                e.Message.Contains(fileItem.Name) &&
                e.Type == StatusType.Info)
        );
        _navigationManager.DidNotReceive().NavigateTo(Arg.Any<string>());
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

        _browserService.GetContentsAsync(path).Returns(updatedContents);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        await _viewModel.DeleteFileAsync();

        // Assert
        _viewModel.IsShowingDeleteConfirmation.ShouldBeFalse();
        _viewModel.FileToDelete.ShouldBeNull();
        _viewModel.IsDeletingFile.ShouldBeFalse();
        stateChangedCalled.ShouldBeTrue();

        await _browserService.Received(1).DeleteFileAsync(fileItem.Path, fileItem.Sha);
        await _browserService.Received(1).GetContentsAsync(path);

        _state.StatusMessage.ShouldContain("deleted successfully");
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
        _browserService.DeleteFileAsync(fileItem.Path, fileItem.Sha).Throws(exception);

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

        await _browserService.Received(1).DeleteFileAsync(fileItem.Path, fileItem.Sha);
        await _browserService.DidNotReceive().GetContentsAsync(Arg.Any<string>());
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