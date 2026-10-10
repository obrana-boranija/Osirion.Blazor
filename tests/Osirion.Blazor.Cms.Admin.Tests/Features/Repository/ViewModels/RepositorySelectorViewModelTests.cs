using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.Repository.Services;
using Osirion.Blazor.Cms.Admin.Features.Repository.ViewModels;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Repository.ViewModels;

public class RepositorySelectorViewModelTests
{
    private readonly IContentRepositoryAdapter _repositoryAdapter;
    private readonly CmsState _state;
    private readonly RepositorySelectorViewModel _viewModel;

    public RepositorySelectorViewModelTests()
    {
        _repositoryAdapter = Substitute.For<IContentRepositoryAdapter>();
        // RepositoryService has no virtual members, so the view model gets the real service over a substituted adapter.
        var repositoryService = new RepositoryService(_repositoryAdapter, NullLogger<RepositoryService>.Instance);
        _state = new CmsState();
        _viewModel = new RepositorySelectorViewModel(repositoryService, _state, NullLogger<RepositorySelectorViewModel>.Instance);
    }

    [Fact]
    public async Task LoadRepositoriesAsync_ShouldUpdateRepositoriesAndNotifyStateChanged()
    {
        // Arrange
        var repositories = new List<GitHubRepository>
        {
            new GitHubRepository { Name = "repo1", FullName = "owner/repo1", DefaultBranch = "main" },
            new GitHubRepository { Name = "repo2", FullName = "owner/repo2", DefaultBranch = "master" }
        };

        _repositoryAdapter.GetRepositoriesAsync().Returns(repositories);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        await _viewModel.LoadRepositoriesAsync();

        // Assert
        _viewModel.Repositories.ShouldBe(repositories);
        _viewModel.IsLoading.ShouldBeFalse();
        _viewModel.ErrorMessage.ShouldBeNull();
        stateChangedCalled.ShouldBeTrue();

        await _repositoryAdapter.Received(1).GetRepositoriesAsync();
    }

    [Fact]
    public async Task LoadRepositoriesAsync_WhenExceptionThrown_ShouldSetErrorAndUpdateState()
    {
        // Arrange
        var exception = new InvalidOperationException("Test error");
        _repositoryAdapter.GetRepositoriesAsync().Throws(exception);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        await _viewModel.LoadRepositoriesAsync();

        // Assert
        _viewModel.IsLoading.ShouldBeFalse();
        _viewModel.ErrorMessage.ShouldNotBeNullOrEmpty();
        _viewModel.ErrorMessage.ShouldContain("Failed to load repositories");
        _viewModel.ErrorMessage.ShouldContain(exception.Message);
        _state.ErrorMessage.ShouldBe(_viewModel.ErrorMessage);
        stateChangedCalled.ShouldBeTrue();

        await _repositoryAdapter.Received(1).GetRepositoriesAsync();
    }

    [Fact]
    public async Task SelectRepositoryAsync_ShouldUpdateStateAndLoadBranches()
    {
        // Arrange
        var repositories = new List<GitHubRepository>
        {
            new GitHubRepository { Name = "repo1", FullName = "owner/repo1", DefaultBranch = "main" },
            new GitHubRepository { Name = "repo2", FullName = "owner/repo2", DefaultBranch = "master" }
        };

        var branches = new List<GitHubBranch>
        {
            new GitHubBranch { Name = "main" },
            new GitHubBranch { Name = "develop" }
        };

        _viewModel.Repositories = repositories;
        _repositoryAdapter.GetBranchesAsync("repo1").Returns(branches);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        await _viewModel.SelectRepositoryAsync("repo1");

        // Assert
        _state.SelectedRepository.ShouldBe(repositories[0]);
        _viewModel.IsLoading.ShouldBeFalse();
        _viewModel.ErrorMessage.ShouldBeNull();
        stateChangedCalled.ShouldBeTrue();

        _repositoryAdapter.Received(1).SetRepository("repo1");
        await _repositoryAdapter.Received(1).GetBranchesAsync("repo1");
    }

    [Fact]
    public async Task SelectRepositoryAsync_WithEmptyName_ShouldClearSelection()
    {
        // Arrange
        var repository = new GitHubRepository { Name = "repo1", FullName = "owner/repo1", DefaultBranch = "main" };
        _state.SelectRepository(repository);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        await _viewModel.SelectRepositoryAsync("");

        // Assert
        _state.SelectedRepository.ShouldBeNull();
        stateChangedCalled.ShouldBeTrue();

        _repositoryAdapter.DidNotReceive().SetRepository(Arg.Any<string>());
        await _repositoryAdapter.DidNotReceive().GetBranchesAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task SelectRepositoryAsync_WhenRepositoryNotFound_ShouldDoNothing()
    {
        // Arrange
        _viewModel.Repositories = new List<GitHubRepository>
        {
            new GitHubRepository { Name = "repo1", FullName = "owner/repo1", DefaultBranch = "main" }
        };

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        await _viewModel.SelectRepositoryAsync("non-existent-repo");

        // Assert
        // State should not change because the repository was not found
        _state.SelectedRepository.ShouldBeNull();
        stateChangedCalled.ShouldBeFalse();

        _repositoryAdapter.DidNotReceive().SetRepository(Arg.Any<string>());
        await _repositoryAdapter.DidNotReceive().GetBranchesAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task SelectRepositoryAsync_WhenBranchLoadingFails_ShouldSetError()
    {
        // Arrange
        var repositories = new List<GitHubRepository>
        {
            new GitHubRepository { Name = "repo1", FullName = "owner/repo1", DefaultBranch = "main" }
        };

        var exception = new InvalidOperationException("Test error");

        _viewModel.Repositories = repositories;
        _repositoryAdapter.GetBranchesAsync("repo1").Throws(exception);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        await _viewModel.SelectRepositoryAsync("repo1");

        // Assert
        _state.SelectedRepository.ShouldBe(repositories[0]);
        _viewModel.IsLoading.ShouldBeFalse();
        _viewModel.ErrorMessage.ShouldNotBeNullOrEmpty();
        _viewModel.ErrorMessage.ShouldContain("Failed to select repository");
        _viewModel.ErrorMessage.ShouldContain(exception.Message);
        _state.ErrorMessage.ShouldBe(_viewModel.ErrorMessage);
        stateChangedCalled.ShouldBeTrue();

        _repositoryAdapter.Received(1).SetRepository("repo1");
        await _repositoryAdapter.Received(1).GetBranchesAsync("repo1");
    }

    [Fact]
    public async Task SelectRepositoryAsync_WhenDefaultBranchExists_ShouldSelectDefaultBranch()
    {
        // Arrange
        var repository = new GitHubRepository { Name = "repo1", FullName = "owner/repo1", DefaultBranch = "main" };
        var branches = new List<GitHubBranch>
        {
            new GitHubBranch { Name = "develop" },
            new GitHubBranch { Name = "main" }
        };

        _viewModel.Repositories = [repository];
        _repositoryAdapter.GetBranchesAsync("repo1").Returns(branches);

        // Act
        await _viewModel.SelectRepositoryAsync("repo1");

        // Assert
        _state.SelectedBranch.ShouldBe(branches[1]);
        _repositoryAdapter.Received(1).SetBranch("main");
    }
}

public class RepositorySelectorViewModelDisposalTests
{
    [Fact]
    public void Dispose_StopsReactingToStateChanges()
    {
        var state = new CmsState();
        var service = new RepositoryService(Substitute.For<IContentRepositoryAdapter>(), NullLogger<RepositoryService>.Instance);
        var viewModel = new RepositorySelectorViewModel(service, state, NullLogger<RepositorySelectorViewModel>.Instance);
        var notifications = 0;
        viewModel.StateChanged += () => notifications++;

        viewModel.Dispose();
        state.SelectRepository(new GitHubRepository { Name = "site" });

        notifications.ShouldBe(0);
    }
}
