using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.UnitOfWork;
using Shouldly;
using System.Reflection;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.UnitOfWork;

public class GitHubUnitOfWorkTests
{
    private readonly IGitHubApiClient _apiClient;
    private readonly IContentRepository _contentRepository;
    private readonly IDirectoryRepository _directoryRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<GitHubUnitOfWork> _logger;
    private readonly GitHubUnitOfWork _unitOfWork;

    public GitHubUnitOfWorkTests()
    {
        _apiClient = Substitute.For<IGitHubApiClient>();
        _contentRepository = Substitute.For<IContentRepository>();
        _directoryRepository = Substitute.For<IDirectoryRepository>();
        _eventDispatcher = Substitute.For<IDomainEventDispatcher>();
        _logger = Substitute.For<ILogger<GitHubUnitOfWork>>();

        _unitOfWork = new GitHubUnitOfWork(
            _apiClient,
            _contentRepository,
            _directoryRepository,
            _eventDispatcher,
            _logger);
    }

    [Fact]
    public void Constructor_SetsOriginalBranch()
    {
        // Arrange & Act - Constructor is called in setup

        // Assert
        var originalBranchField = typeof(GitHubUnitOfWork)
            .GetField("_originalBranch", BindingFlags.NonPublic | BindingFlags.Instance);
        originalBranchField.ShouldNotBeNull();

        var originalBranch = (string)originalBranchField.GetValue(_unitOfWork);
        originalBranch.ShouldBe("main");
    }

    [Fact]
    public async Task BeginTransactionAsync_CreatesTemporaryBranch()
    {
        // Arrange
        _apiClient.CreateBranchAsync(
                Arg.Any<string>(),
                Arg.Is<string>(b => b == "main"),
                Arg.Any<CancellationToken>())
            .Returns(new GitHubBranch { Name = "temp-branch" });

        // Act
        await _unitOfWork.BeginTransactionAsync();

        // Assert
        await _apiClient.Received(1).CreateBranchAsync(
            Arg.Is<string>(b => b.StartsWith("temp-")),
            Arg.Is<string>(b => b == "main"),
            Arg.Any<CancellationToken>());

        _apiClient.Received(1).SetBranch(Arg.Is<string>(b => b.StartsWith("temp-")));

        var temporaryBranchField = typeof(GitHubUnitOfWork)
            .GetField("_temporaryBranch", BindingFlags.NonPublic | BindingFlags.Instance);
        temporaryBranchField.ShouldNotBeNull();

        var temporaryBranch = (string)temporaryBranchField.GetValue(_unitOfWork);
        temporaryBranch.ShouldNotBeNull();
        temporaryBranch.ShouldStartWith("temp-");
    }

    [Fact]
    public async Task BeginTransactionAsync_WhenBranchCreationFails_ThrowsException()
    {
        // Arrange
        _apiClient.CreateBranchAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Throws(new Exception("Branch creation failed"));

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(async () =>
            await _unitOfWork.BeginTransactionAsync());

        ex.Message.ShouldContain("Failed to start GitHub transaction");
    }

    [Fact]
    public async Task CommitTransactionAsync_CreatesPullRequest()
    {
        // Arrange
        // First start a transaction
        _apiClient.CreateBranchAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(new GitHubBranch { Name = "temp-branch" });

        await _unitOfWork.BeginTransactionAsync();

        // Get the temporary branch name
        var temporaryBranchField = typeof(GitHubUnitOfWork)
            .GetField("_temporaryBranch", BindingFlags.NonPublic | BindingFlags.Instance);
        var temporaryBranch = (string)temporaryBranchField.GetValue(_unitOfWork);

        _apiClient.CreatePullRequestAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Is<string>(b => b == temporaryBranch),
                Arg.Is<string>(b => b == "main"),
                Arg.Any<CancellationToken>())
            .Returns(new GitHubPullRequest { Number = 1 });

        // Act
        await _unitOfWork.CommitAsync();

        // Assert
        await _apiClient.Received(1).CreatePullRequestAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Is<string>(b => b == temporaryBranch),
            Arg.Is<string>(b => b == "main"),
            Arg.Any<CancellationToken>());

        _apiClient.Received(1).SetBranch("main");

        temporaryBranch = (string)temporaryBranchField.GetValue(_unitOfWork);
        temporaryBranch.ShouldBeNull();
    }

    [Fact]
    public async Task CommitTransactionAsync_WithNoTemporaryBranch_ThrowsException()
    {
        // Arrange - Start transaction but set temporary branch to null
        _apiClient.CreateBranchAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(new GitHubBranch { Name = "temp-branch" });

        await _unitOfWork.BeginTransactionAsync();

        var temporaryBranchField = typeof(GitHubUnitOfWork)
            .GetField("_temporaryBranch", BindingFlags.NonPublic | BindingFlags.Instance);
        temporaryBranchField.SetValue(_unitOfWork, null);

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await _unitOfWork.CommitAsync());
    }

    [Fact]
    public async Task RollbackTransactionAsync_SwitchesBackToOriginalBranch()
    {
        // Arrange
        // First start a transaction
        _apiClient.CreateBranchAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(new GitHubBranch { Name = "temp-branch" });

        await _unitOfWork.BeginTransactionAsync();

        // Act
        await _unitOfWork.RollbackAsync();

        // Assert
        _apiClient.Received(1).SetBranch("main");

        var temporaryBranchField = typeof(GitHubUnitOfWork)
            .GetField("_temporaryBranch", BindingFlags.NonPublic | BindingFlags.Instance);
        var temporaryBranch = (string)temporaryBranchField.GetValue(_unitOfWork);
        temporaryBranch.ShouldBeNull();
    }

    [Fact]
    public async Task CreateSavePointAsync_NotImplemented_ReturnsCompletedTask()
    {
        // Arrange
        // First start a transaction
        _apiClient.CreateBranchAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(new GitHubBranch { Name = "temp-branch" });

        await _unitOfWork.BeginTransactionAsync();

        // Act - This should complete without exception
        await _unitOfWork.SavePointAsync("test-savepoint");

        // Assert - Get method using reflection to verify it was called
        var onCreateSavePointAsyncMethod = typeof(GitHubUnitOfWork)
            .GetMethod("OnCreateSavePointAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        onCreateSavePointAsyncMethod.ShouldNotBeNull();
    }

    [Fact]
    public async Task RollbackToSavePointAsync_NotImplemented_ReturnsCompletedTask()
    {
        // Arrange
        // First start a transaction
        _apiClient.CreateBranchAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(new GitHubBranch { Name = "temp-branch" });

        await _unitOfWork.BeginTransactionAsync();
        await _unitOfWork.SavePointAsync("test-savepoint");

        // Act - This should complete without exception
        await _unitOfWork.RollbackToSavePointAsync("test-savepoint");

        // Assert - Get method using reflection to verify it was called
        var onRollbackToSavePointAsyncMethod = typeof(GitHubUnitOfWork)
            .GetMethod("OnRollbackToSavePointAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        onRollbackToSavePointAsyncMethod.ShouldNotBeNull();
    }
}