using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.Repository.Services;
using Osirion.Blazor.Cms.Admin.Features.Repository.ViewModels;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Repository.ViewModels;

public class RepositorySelectorViewModelTests
{
    private readonly RepositoryService _repositoryService;
    private readonly CmsState _state;
    private readonly ILogger<RepositorySelectorViewModel> _logger;
    private readonly RepositorySelectorViewModel _viewModel;

    public RepositorySelectorViewModelTests()
    {
        _repositoryService = Substitute.For<RepositoryService>(
            Substitute.For<Infrastructure.Adapters.IContentRepositoryAdapter>(),
            Substitute.For<ILogger<RepositoryService>>()
        );
        _state = new CmsState();
        _logger = Substitute.For<ILogger<RepositorySelectorViewModel>>();
        _viewModel = new RepositorySelectorViewModel(_repositoryService, _state, _logger);
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

        _repositoryService.GetRepositoriesAsync().Returns(repositories);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        await _viewModel.LoadRepositoriesAsync();

        // Assert
        _viewModel.Repositories.ShouldBe(repositories);
        _viewModel.IsLoading.ShouldBeFalse();
        _viewModel.ErrorMessage.ShouldBeNull();
        stateChangedCalled.ShouldBeTrue();

        await _repositoryService.Received(1).GetRepositoriesAsync();
    }

    [Fact]
    public async Task LoadRepositoriesAsync_WhenExceptionThrown_ShouldSetErrorAndUpdateState()
    {
        // Arrange
        var exception = new InvalidOperationException("Test error");
        _repositoryService.GetRepositoriesAsync().Throws(exception);

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

        await _repositoryService.Received(1).GetRepositoriesAsync();
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
        _repositoryService.GetBranchesAsync("repo1").Returns(branches);

        var stateChangedCalled = false;
        _viewModel.StateChanged += () => stateChangedCalled = true;

        // Act
        await _viewModel.SelectRepositoryAsync("repo1");

        // Assert
        _state.SelectedRepository.ShouldBe(repositories[0]);
        _viewModel.IsLoading.ShouldBeFalse();
        _viewModel.ErrorMessage.ShouldBeNull();
        stateChangedCalled.ShouldBeTrue();

        _repositoryService.Received(1).SetRepository("repo1");
        await _repositoryService.Received(1).GetBranchesAsync("repo1");
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

        _repositoryService.DidNotReceive().SetRepository(Arg.Any<string>());
        await _repositoryService.DidNotReceive().GetBranchesAsync(Arg.Any<string>());
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

        _repositoryService.DidNotReceive().SetRepository(Arg.Any<string>());
        await _repositoryService.DidNotReceive().GetBranchesAsync(Arg.Any<string>());
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
        _repositoryService.GetBranchesAsync("repo1").Throws(exception);

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

        _repositoryService.Received(1).SetRepository("repo1");
        await _repositoryService.Received(1).GetBranchesAsync("repo1");
    }

    [Fact]
    public async Task LoadBranchesForRepositoryAsync_ShouldUpdateStateWithDefaultBranch()
    {
        // Arrange - Access the private method through reflection
        var method = typeof(RepositorySelectorViewModel).GetMethod(
            "LoadBranchesForRepositoryAsync",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var repository = new GitHubRepository { Name = "repo1", FullName = "owner/repo1", DefaultBranch = "main" };
        var branches = new List<GitHubBranch>
        {
            new GitHubBranch { Name = "main" },
            new GitHubBranch { Name = "develop" }
        };

        _state.SelectRepository(repository);
        _repositoryService.GetBranchesAsync("repo1").Returns(branches);

        // Act
        await (Task)method.Invoke(_viewModel, new object[] { "repo1" });

        // Assert
        _state.SelectedBranch.ShouldBe(branches[0]); // Should select "main" as it's the default branch
        _repositoryService.Received(1).SetBranch("main");
    }
}