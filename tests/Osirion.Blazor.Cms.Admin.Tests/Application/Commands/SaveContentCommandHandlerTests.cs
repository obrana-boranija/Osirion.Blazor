using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Admin.Application.Commands;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Application.Commands;

public class SaveContentCommandHandlerTests
{
    private readonly IContentRepositoryAdapter _repositoryAdapter;
    private readonly ILogger<SaveContentCommandHandler> _logger;
    private readonly SaveContentCommandHandler _handler;

    public SaveContentCommandHandlerTests()
    {
        _repositoryAdapter = Substitute.For<IContentRepositoryAdapter>();
        _logger = Substitute.For<ILogger<SaveContentCommandHandler>>();
        _handler = new SaveContentCommandHandler(_repositoryAdapter, _logger);
    }

    [Fact]
    public async Task HandleAsync_ShouldSaveContentAndReturnSuccessResult()
    {
        // Arrange
        var command = new SaveContentCommand
        {
            Path = "content/blog/post.md",
            Content = "# Test Content",
            CommitMessage = "Test commit message"
        };

        var commitResponse = new GitHubFileCommitResponse
        {
            Content = new GitHubFileContent
            {
                Path = command.Path,
                Sha = "test-sha-123"
            }
        };

        _repositoryAdapter
            .SaveContentAsync(command.Path, command.Content, command.CommitMessage, command.Sha)
            .Returns(commitResponse);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.ContentId.ShouldBe(command.Path);
        result.Sha.ShouldBe("test-sha-123");
        result.Message.ShouldContain("success");

        await _repositoryAdapter
            .Received(1)
            .SaveContentAsync(command.Path, command.Content, command.CommitMessage, command.Sha);

        _logger.Received(1).LogInformation(
            Arg.Is<string>(s => s.Contains("Saving content")),
            Arg.Is(command.Path)
        );

        _logger.Received(1).LogInformation(
            Arg.Is<string>(s => s.Contains("Content saved successfully")),
            Arg.Is(command.Path)
        );
    }

    [Fact]
    public async Task HandleAsync_WhenExceptionThrown_ShouldReturnFailureResult()
    {
        // Arrange
        var command = new SaveContentCommand
        {
            Path = "content/blog/post.md",
            Content = "# Test Content",
            CommitMessage = "Test commit message"
        };

        var exception = new InvalidOperationException("Test error");
        _repositoryAdapter
            .SaveContentAsync(command.Path, command.Content, command.CommitMessage, command.Sha)
            .Throws(exception);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.Message.ShouldContain("Failed to save content");
        result.Message.ShouldContain(exception.Message);

        await _repositoryAdapter
            .Received(1)
            .SaveContentAsync(command.Path, command.Content, command.CommitMessage, command.Sha);

        _logger.Received(1).LogError(
            Arg.Is(exception),
            Arg.Is<string>(s => s.Contains("Error saving content")),
            Arg.Is(command.Path)
        );
    }

    [Fact]
    public async Task HandleAsync_WithNullSha_ShouldPassNullToAdapter()
    {
        // Arrange
        var command = new SaveContentCommand
        {
            Path = "content/blog/post.md",
            Content = "# Test Content",
            CommitMessage = "Test commit message",
            Sha = null
        };

        var commitResponse = new GitHubFileCommitResponse
        {
            Content = new GitHubFileContent
            {
                Path = command.Path,
                Sha = "new-sha-123"
            }
        };

        _repositoryAdapter
            .SaveContentAsync(command.Path, command.Content, command.CommitMessage, null)
            .Returns(commitResponse);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Sha.ShouldBe("new-sha-123");

        await _repositoryAdapter
            .Received(1)
            .SaveContentAsync(command.Path, command.Content, command.CommitMessage, null);
    }
}