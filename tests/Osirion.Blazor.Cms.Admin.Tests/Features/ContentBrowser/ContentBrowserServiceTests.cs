using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Octokit;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentBrowser.Services;

public class ContentBrowserServiceTests
{
    private readonly IContentRepositoryAdapter _repositoryAdapter;
    private readonly CmsEventMediator _eventMediator;
    private readonly ILogger<ContentBrowserService> _logger;
    private readonly ContentBrowserService _service;

    public ContentBrowserServiceTests()
    {
        _repositoryAdapter = Substitute.For<IContentRepositoryAdapter>();
        _eventMediator = Substitute.For<CmsEventMediator>(Substitute.For<ILogger<CmsEventMediator>>());
        _logger = Substitute.For<ILogger<ContentBrowserService>>();
        _service = new ContentBrowserService(_repositoryAdapter, _eventMediator, _logger);
    }

    [Fact]
    public async Task GetContentsAsync_ShouldReturnContentsFromAdapter()
    {
        // Arrange
        var path = "content/blog";
        var expectedContents = new List<GitHubItem>
        {
            new GitHubItem { Name = "post.md", Type = "file", Path = "content/blog/post.md" },
            new GitHubItem { Name = "images", Type = "dir", Path = "content/blog/images" }
        };

        _repositoryAdapter.GetContentsAsync(path).Returns(expectedContents);

        // Act
        var result = await _service.GetContentsAsync(path);

        // Assert
        result.ShouldBe(expectedContents);
        await _repositoryAdapter.Received(1).GetContentsAsync(path);

        _logger.Received(1).LogInformation(
            Arg.Is<string>(s => s.Contains("Fetching contents")),
            Arg.Is(path)
        );

        _logger.Received(1).LogInformation(
            Arg.Is<string>(s => s.Contains("Retrieved")),
            Arg.Is(expectedContents.Count),
            Arg.Is(path)
        );
    }

    [Fact]
    public async Task GetContentsAsync_WhenExceptionThrown_ShouldLogAndRethrow()
    {
        // Arrange
        var path = "content/blog";
        var exception = new InvalidOperationException("Test error");

        _repositoryAdapter.GetContentsAsync(path).Throws(exception);

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(async () =>
            await _service.GetContentsAsync(path));

        ex.ShouldBe(exception);

        _logger.Received(1).LogError(
            Arg.Is(exception),
            Arg.Is<string>(s => s.Contains("Error fetching contents")),
            Arg.Is(path)
        );

        _eventMediator.Received(1).Publish(
            Arg.Is<ErrorOccurredEvent>(e =>
                e.Message.Contains("Failed to load contents") &&
                e.Message.Contains(path) &&
                e.Exception == exception)
        );
    }

    [Fact]
    public async Task GetBlogPostAsync_ShouldReturnBlogPostFromAdapter()
    {
        // Arrange
        var path = "content/blog/post.md";
        var expectedPost = new BlogPost
        {
            FilePath = path,
            Content = "# Test Content",
            Metadata = FrontMatter.Create("Test Title", "Test Description", System.DateTime.Now),
            Sha = "test-sha"
        };

        _repositoryAdapter.GetBlogPostAsync(path).Returns(expectedPost);

        // Act
        var result = await _service.GetBlogPostAsync(path);

        // Assert
        result.ShouldBe(expectedPost);
        await _repositoryAdapter.Received(1).GetBlogPostAsync(path);

        _logger.Received(1).LogInformation(
            Arg.Is<string>(s => s.Contains("Fetching blog post")),
            Arg.Is(path)
        );

        _logger.Received(1).LogInformation(
            Arg.Is<string>(s => s.Contains("Blog post fetched successfully")),
            Arg.Is(path)
        );
    }

    [Fact]
    public async Task GetBlogPostAsync_WhenExceptionThrown_ShouldLogAndRethrow()
    {
        // Arrange
        var path = "content/blog/post.md";
        var exception = new InvalidOperationException("Test error");

        _repositoryAdapter.GetBlogPostAsync(path).Throws(exception);

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(async () =>
            await _service.GetBlogPostAsync(path));

        ex.ShouldBe(exception);

        _logger.Received(1).LogError(
            Arg.Is(exception),
            Arg.Is<string>(s => s.Contains("Error fetching blog post")),
            Arg.Is(path)
        );

        _eventMediator.Received(1).Publish(
            Arg.Is<ErrorOccurredEvent>(e =>
                e.Message.Contains("Failed to load file") &&
                e.Message.Contains(path) &&
                e.Exception == exception)
        );
    }

    [Fact]
    public async Task DeleteFileAsync_ShouldDeleteFileAndPublishEvents()
    {
        // Arrange
        var path = "content/blog/post.md";
        var sha = "test-sha";
        var fileName = "post.md";

        var expectedResponse = new GitHubFileCommitResponse
        {
            Commit = new GitHubCommitInfo { Sha = "commit-sha" }
        };

        _repositoryAdapter
            .DeleteFileAsync(path, Arg.Is<string>(s => s.Contains(fileName)), sha)
            .Returns(expectedResponse);

        // Act
        var result = await _service.DeleteFileAsync(path, sha);

        // Assert
        result.ShouldBe(expectedResponse);

        await _repositoryAdapter.Received(1).DeleteFileAsync(
            path,
            Arg.Is<string>(s => s.Contains(fileName)),
            sha
        );

        _logger.Received(1).LogInformation(
            Arg.Is<string>(s => s.Contains("Deleting file")),
            Arg.Is(path)
        );

        _logger.Received(1).LogInformation(
            Arg.Is<string>(s => s.Contains("File deleted successfully")),
            Arg.Is(path)
        );

        _eventMediator.Received(1).Publish(
            Arg.Is<ContentDeletedEvent>(e => e.Path == path)
        );

        _eventMediator.Received(1).Publish(
            Arg.Is<StatusNotificationEvent>(e =>
                e.Message.Contains("File deleted successfully") &&
                e.Message.Contains(path) &&
                e.Type == StatusType.Success)
        );
    }

    [Fact]
    public async Task DeleteFileAsync_WhenExceptionThrown_ShouldLogAndRethrow()
    {
        // Arrange
        var path = "content/blog/post.md";
        var sha = "test-sha";
        var exception = new InvalidOperationException("Test error");

        _repositoryAdapter
            .DeleteFileAsync(path, Arg.Any<string>(), sha)
            .Throws(exception);

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(async () =>
            await _service.DeleteFileAsync(path, sha));

        ex.ShouldBe(exception);

        _logger.Received(1).LogError(
            Arg.Is(exception),
            Arg.Is<string>(s => s.Contains("Error deleting file")),
            Arg.Is(path)
        );

        _eventMediator.Received(1).Publish(
            Arg.Is<ErrorOccurredEvent>(e =>
                e.Message.Contains("Failed to delete file") &&
                e.Message.Contains(path) &&
                e.Exception == exception)
        );
    }

    [Fact]
    public async Task SearchFilesAsync_ShouldReturnSearchResults()
    {
        // Arrange
        var query = "test";
        var expectedResults = new List<GitHubItem>
        {
            new GitHubItem { Name = "post1.md", Type = "file", Path = "content/blog/post1.md" },
            new GitHubItem { Name = "post2.md", Type = "file", Path = "content/blog/post2.md" }
        };

        _repositoryAdapter.SearchFilesAsync(query).Returns(expectedResults);

        // Act
        var result = await _service.SearchFilesAsync(query);

        // Assert
        result.ShouldBe(expectedResults);
        await _repositoryAdapter.Received(1).SearchFilesAsync(query);

        _logger.Received(1).LogInformation(
            Arg.Is<string>(s => s.Contains("Searching files with query")),
            Arg.Is(query)
        );

        _logger.Received(1).LogInformation(
            Arg.Is<string>(s => s.Contains("Found")),
            Arg.Is(expectedResults.Count),
            Arg.Is(query)
        );
    }

    [Fact]
    public async Task SearchFilesAsync_WhenExceptionThrown_ShouldLogAndRethrow()
    {
        // Arrange
        var query = "test";
        var exception = new InvalidOperationException("Test error");

        _repositoryAdapter.SearchFilesAsync(query).Throws(exception);

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(async () =>
            await _service.SearchFilesAsync(query));

        ex.ShouldBe(exception);

        _logger.Received(1).LogError(
            Arg.Is(exception),
            Arg.Is<string>(s => s.Contains("Error searching files")),
            Arg.Is(query)
        );

        _eventMediator.Received(1).Publish(
            Arg.Is<ErrorOccurredEvent>(e =>
                e.Message.Contains("Failed to search files") &&
                e.Message.Contains(query) &&
                e.Exception == exception)
        );
    }

    [Fact]
    public async Task GetDirectoryTreeAsync_ShouldFetchDirectoryItemsRecursively()
    {
        // Arrange
        var path = "content";
        var rootContents = new List<GitHubItem>
        {
            new GitHubItem { Name = "blog", Type = "dir", Path = "content/blog" },
            new GitHubItem { Name = "readme.md", Type = "file", Path = "content/readme.md" }
        };

        var blogContents = new List<GitHubItem>
        {
            new GitHubItem { Name = "post.md", Type = "file", Path = "content/blog/post.md" },
            new GitHubItem { Name = "images", Type = "dir", Path = "content/blog/images" }
        };

        var imagesContents = new List<GitHubItem>
        {
            new GitHubItem { Name = "image.png", Type = "file", Path = "content/blog/images/image.png" }
        };

        // Setup the recursive calls
        _repositoryAdapter.GetContentsAsync(path).Returns(rootContents);
        _repositoryAdapter.GetContentsAsync("content/blog").Returns(blogContents);
        _repositoryAdapter.GetContentsAsync("content/blog/images").Returns(imagesContents);

        // Act
        var result = await _service.GetDirectoryTreeAsync(path);

        // Assert
        result.Count.ShouldBe(rootContents.Count + blogContents.Count + imagesContents.Count);
        result.ShouldContain(rootContents[0]);
        result.ShouldContain(rootContents[1]);
        result.ShouldContain(blogContents[0]);
        result.ShouldContain(blogContents[1]);
        result.ShouldContain(imagesContents[0]);

        await _repositoryAdapter.Received(1).GetContentsAsync(path);
        await _repositoryAdapter.Received(1).GetContentsAsync("content/blog");
        await _repositoryAdapter.Received(1).GetContentsAsync("content/blog/images");

        _logger.Received(1).LogInformation(
            Arg.Is<string>(s => s.Contains("Fetching directory tree")),
            Arg.Is(path)
        );

        _logger.Received(1).LogInformation(
            Arg.Is<string>(s => s.Contains("Retrieved")),
            Arg.Is(result.Count),
            Arg.Is(path)
        );
    }
}