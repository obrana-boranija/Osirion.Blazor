using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Octokit;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentBrowser.Services;

public class ContentBrowserServiceTests
{
    private readonly IContentRepositoryAdapter _repositoryAdapter;
    private readonly CmsEventMediator _eventMediator;
    private readonly RecordingLogger<ContentBrowserService> _logger;
    private readonly List<ErrorOccurredEvent> _errors;
    private readonly List<ContentDeletedEvent> _deletions;
    private readonly List<StatusNotificationEvent> _notifications;
    private readonly ContentBrowserService _service;

    public ContentBrowserServiceTests()
    {
        _repositoryAdapter = Substitute.For<IContentRepositoryAdapter>();
        _eventMediator = new CmsEventMediator(NullLogger<CmsEventMediator>.Instance);
        _logger = new RecordingLogger<ContentBrowserService>();
        _errors = EventRecorder.Record<ErrorOccurredEvent>(_eventMediator);
        _deletions = EventRecorder.Record<ContentDeletedEvent>(_eventMediator);
        _notifications = EventRecorder.Record<StatusNotificationEvent>(_eventMediator);
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

        _logger.HasEntry(LogLevel.Information, "Fetching contents", path).ShouldBeTrue();

        _logger.HasEntry(LogLevel.Information, "Retrieved", path, expectedContents.Count.ToString()).ShouldBeTrue();
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

        _logger.HasEntry(LogLevel.Error, "Error fetching contents", path).ShouldBeTrue();
        _logger.Entries.ShouldContain(entry => entry.Level == LogLevel.Error && entry.Exception == exception);

        var published = _errors.ShouldHaveSingleItem();
        published.Message.ShouldContain("Failed to load contents");
        published.Message.ShouldContain(path);
        published.Exception.ShouldBe(exception);
    }

    [Fact]
    public async Task GetBlogPostAsync_ShouldReturnBlogPostFromAdapter()
    {
        // Arrange
        var path = "content/blog/post.md";
        var expectedPost = new ContentItem
        {
            Path = path,
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

        _logger.HasEntry(LogLevel.Information, "Fetching blog post", path).ShouldBeTrue();

        _logger.HasEntry(LogLevel.Information, "Blog post fetched successfully", path).ShouldBeTrue();
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

        _logger.HasEntry(LogLevel.Error, "Error fetching blog post", path).ShouldBeTrue();
        _logger.Entries.ShouldContain(entry => entry.Level == LogLevel.Error && entry.Exception == exception);

        var published = _errors.ShouldHaveSingleItem();
        published.Message.ShouldContain("Failed to load file");
        published.Message.ShouldContain(path);
        published.Exception.ShouldBe(exception);
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

        _logger.HasEntry(LogLevel.Information, "Deleting file", path).ShouldBeTrue();

        _logger.HasEntry(LogLevel.Information, "File deleted successfully", path).ShouldBeTrue();

        _deletions.ShouldHaveSingleItem().Path.ShouldBe(path);

        var notification = _notifications.ShouldHaveSingleItem();
        notification.Message.ShouldContain("File deleted successfully");
        notification.Message.ShouldContain(path);
        notification.Type.ShouldBe(StatusType.Success);
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

        _logger.HasEntry(LogLevel.Error, "Error deleting file", path).ShouldBeTrue();
        _logger.Entries.ShouldContain(entry => entry.Level == LogLevel.Error && entry.Exception == exception);

        var published = _errors.ShouldHaveSingleItem();
        published.Message.ShouldContain("Failed to delete file");
        published.Message.ShouldContain(path);
        published.Exception.ShouldBe(exception);
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

        _logger.HasEntry(LogLevel.Information, "Searching files with query", query).ShouldBeTrue();

        _logger.HasEntry(LogLevel.Information, "Found", query, expectedResults.Count.ToString()).ShouldBeTrue();
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

        _logger.HasEntry(LogLevel.Error, "Error searching files", query).ShouldBeTrue();
        _logger.Entries.ShouldContain(entry => entry.Level == LogLevel.Error && entry.Exception == exception);

        var published = _errors.ShouldHaveSingleItem();
        published.Message.ShouldContain("Failed to search files");
        published.Message.ShouldContain(query);
        published.Exception.ShouldBe(exception);
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

        _logger.HasEntry(LogLevel.Information, "Fetching directory tree", path).ShouldBeTrue();

        _logger.HasEntry(LogLevel.Information, "Retrieved", path, result.Count.ToString()).ShouldBeTrue();
    }
}