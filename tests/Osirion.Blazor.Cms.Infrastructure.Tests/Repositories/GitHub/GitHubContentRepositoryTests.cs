using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Repositories.GitHub;

public class GitHubContentRepositoryTests
{
    private readonly IGitHubApiClient _apiClient;
    private readonly IMarkdownProcessor _markdownProcessor;
    private readonly IDirectoryRepository _directoryRepository;
    private readonly IOptions<GitHubOptions> _options;
    private readonly ILogger<GitHubContentRepository> _logger;
    private readonly GitHubContentRepository _repository;

    public GitHubContentRepositoryTests()
    {
        _apiClient = Substitute.For<IGitHubApiClient>();
        _markdownProcessor = Substitute.For<IMarkdownProcessor>();
        _directoryRepository = Substitute.For<IDirectoryRepository>();
        _logger = Substitute.For<ILogger<GitHubContentRepository>>();

        var options = new GitHubOptions
        {
            Owner = "testOwner",
            Repository = "testRepo",
            Branch = "main",
            ContentPath = "content",
            ApiToken = "test-token",
            CacheDurationMinutes = 5,
            EnableLocalization = false,
            DefaultLocale = "en"
        };

        _options = Options.Create(options);
        _repository = new GitHubContentRepository(
            _apiClient,
            _markdownProcessor,
            _options,
            _directoryRepository,
            _logger);

        SetupMarkdownProcessor();
    }

    private void SetupMarkdownProcessor()
    {
        _markdownProcessor.ExtractFrontMatterAsync(
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(info => {
                var content = info.Arg<string>();
                var frontMatter = new Dictionary<string, string>();
                var contentPart = content;

                // Simple front matter extraction for testing
                if (content.Contains("title:"))
                {
                    frontMatter["title"] = content.Contains("Post 1") ? "Post 1" : "Post 2";
                }

                return Task.FromResult((frontMatter, contentPart));
            });

        _markdownProcessor.RenderToHtmlAsync(Arg.Any<string>())
            .Returns(info => Task.FromResult($"<p>{info.Arg<string>()}</p>"));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsGitHubContents()
    {
        // Arrange
        var items = new List<GitHubItem>
        {
            new GitHubItem
            {
                Name = "post1.md",
                Path = "content/post1.md",
                Type = "file",
                Sha = "abc123"
            },
            new GitHubItem
            {
                Name = "post2.md",
                Path = "content/post2.md",
                Type = "file",
                Sha = "def456"
            }
        };

        _apiClient.GetRepositoryContentsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(items);

        _apiClient.GetFileContentAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var path = callInfo.Arg<string>();
                if (path == "content/post1.md")
                    return new GitHubFileContent
                    {
                        Name = "post1.md",
                        Path = "content/post1.md",
                        Sha = "abc123",
                        Content = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("---\ntitle: Post 1\n---\nContent 1")),
                        Encoding = "base64"
                    };
                else
                    return new GitHubFileContent
                    {
                        Name = "post2.md",
                        Path = "content/post2.md",
                        Sha = "def456",
                        Content = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("---\ntitle: Post 2\n---\nContent 2")),
                        Encoding = "base64"
                    };
            });

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.ShouldContain(item => item.Title == "Post 1");
        result.ShouldContain(item => item.Title == "Post 2");
    }

    [Fact(Skip = "Temporarily disabled in CI until fixed")]
    public async Task SaveWithCommitMessageAsync_CreatesNewFile()
    {
        // Arrange
        var contentItem = ContentItem.Create(
            "test-id",
            "Test Title",
            "Test content",
            "content/test.md",
            "github");

        contentItem.SetSlug("test-title");

        _apiClient.CreateOrUpdateFileAsync(
            Arg.Is<string>(s => s == "content/test.md"),
            Arg.Any<string>(),
            Arg.Is<string>(s => s == "Create test file"),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(new GitHubFileCommitResponse
            {
                Success = true,
                Content = new GitHubFileContent { Sha = "new-sha" }
            });

        // Act
        var result = await _repository.SaveWithCommitMessageAsync(contentItem, "Create test file");

        // Assert
        result.ShouldNotBeNull();
        result.ProviderSpecificId.ShouldBe("new-sha");

        await _apiClient.Received(1).CreateOrUpdateFileAsync(
            Arg.Is<string>(s => s == "content/test.md"),
            Arg.Any<string>(),
            Arg.Is<string>(s => s == "Create test file"),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(Skip = "Temporarily disabled in CI until fixed")]
    public async Task DeleteWithCommitMessageAsync_RemovesFile()
    {
        // Arrange
        var contentId = "delete-id";
        var contentItem = ContentItem.Create(
            contentId,
            "Delete Me",
            "Content to delete",
            "content/to-delete.md",
            "github");

        contentItem.SetProviderSpecificId("existing-sha");

        _repository.GetByIdAsync(contentId, Arg.Any<CancellationToken>())
            .Returns(contentItem);

        _apiClient.DeleteFileAsync(
            Arg.Is<string>(s => s == "content/to-delete.md"),
            Arg.Any<string>(),
            Arg.Is<string>(s => s == "existing-sha"),
            Arg.Any<CancellationToken>())
            .Returns(new GitHubFileCommitResponse { Success = true });

        // Act
        await _repository.DeleteWithCommitMessageAsync(contentId, "Delete test file");

        // Assert
        await _apiClient.Received(1).DeleteFileAsync(
            Arg.Is<string>(s => s == "content/to-delete.md"),
            Arg.Any<string>(),
            Arg.Is<string>(s => s == "existing-sha"),
            Arg.Any<CancellationToken>());
    }
}