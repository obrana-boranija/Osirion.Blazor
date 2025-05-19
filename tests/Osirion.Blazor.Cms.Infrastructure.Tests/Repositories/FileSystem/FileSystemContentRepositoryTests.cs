using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Shouldly;
using System.IO.Abstractions.TestingHelpers;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Repositories.FileSystem;

public class FileSystemContentRepositoryTests
{
    private readonly IMarkdownProcessor _markdownProcessor;
    private readonly IOptions<FileSystemOptions> _options;
    private readonly ILogger<FileSystemContentRepository> _logger;
    private readonly MockFileSystem _mockFileSystem;
    private readonly FileSystemContentRepository _repository;
    private readonly string _basePath = "/content";

    public FileSystemContentRepositoryTests()
    {
        // Setup NSubstitute mock for markdown processor
        _markdownProcessor = Substitute.For<IMarkdownProcessor>();
        ConfigureMarkdownProcessor();

        // Setup mock file system
        _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
        _mockFileSystem.Directory.CreateDirectory(_basePath);

        // Setup logging
        _logger = Substitute.For<ILogger<FileSystemContentRepository>>();

        // Setup options
        var options = new FileSystemOptions
        {
            BasePath = _basePath,
            ContentRoot = "content",
            SupportedExtensions = new() { ".md", ".markdown" },
            CreateDirectoriesIfNotExist = true,
            IncludeSubdirectories = true,
            CacheDurationMinutes = 5,
            EnableLocalization = false,
            DefaultLocale = "en",
            WatchForChanges = false // Disable file watching for tests
        };

        _options = Options.Create(options);

        // Create repository with mock file system
        _repository = new FileSystemContentRepository(
            _markdownProcessor,
            _options,
            _logger,
            _mockFileSystem);
    }

    private void ConfigureMarkdownProcessor()
    {
        // Setup front matter extraction
        _markdownProcessor.ExtractFrontMatterAsync(
            Arg.Any<string>(),
            Arg.Any<CancellationToken>()
        ).Returns(info => {
            var content = info.Arg<string>();
            var frontMatter = new Dictionary<string, string>();
            var contentPart = content;

            // Parse front matter for testing
            if (content.StartsWith("---"))
            {
                var endMarker = content.IndexOf("---", 3);
                if (endMarker > 0)
                {
                    var frontMatterText = content.Substring(3, endMarker - 3);
                    contentPart = content.Substring(endMarker + 3).Trim();

                    foreach (var line in frontMatterText.Split('\n'))
                    {
                        var parts = line.Split(':', 2);
                        if (parts.Length == 2)
                        {
                            frontMatter[parts[0].Trim()] = parts[1].Trim();
                        }
                    }
                }
            }

            return Task.FromResult((frontMatter, contentPart));
        });

        // Setup HTML rendering
        _markdownProcessor.RenderToHtmlAsync(
            Arg.Any<string>()
        ).Returns(info => {
            var content = info.Arg<string>();
            return Task.FromResult($"<p>{content}</p>");
        });

        // Set up other needed methods
        _markdownProcessor.RenderToHtml(Arg.Any<string>(), Arg.Any<bool>())
            .Returns(info => $"<p>{info.Arg<string>()}</p>");

        _markdownProcessor.ConvertHtmlToMarkdownAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(info => Task.FromResult(info.Arg<string>()));
    }

    [Fact]
    public async Task GetAllAsync_WithNoFiles_ReturnsEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public async Task GetAllAsync_WithFiles_ReturnsAllContentItems()
    {
        // Arrange
        var contentDir = System.IO.Path.Combine(_basePath, "content");
        _mockFileSystem.Directory.CreateDirectory(contentDir);

        // Create test files
        _mockFileSystem.File.WriteAllText(
            System.IO.Path.Combine(contentDir, "post1.md"),
            "---\ntitle: Post 1\n---\nContent 1");

        _mockFileSystem.File.WriteAllText(
            System.IO.Path.Combine(contentDir, "post2.md"),
            "---\ntitle: Post 2\n---\nContent 2");

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        var titles = result.Select(r => r.Title).ToList();
        titles.ShouldContain("Post 1");
        titles.ShouldContain("Post 2");
    }

    [Fact(Skip = "Temporarily disabled in CI until fixed")]
    public async Task GetByIdAsync_WithExistingId_ReturnsContentItem()
    {
        // Arrange
        var contentDir = System.IO.Path.Combine(_basePath, "content");
        _mockFileSystem.Directory.CreateDirectory(contentDir);

        var filePath = System.IO.Path.Combine(contentDir, "post1.md");
        _mockFileSystem.File.WriteAllText(
            filePath,
            "---\ntitle: Test Post\n---\nTest Content");

        // First load all items to populate cache
        await _repository.GetAllAsync();

        // Get the ID that would be generated for this file
        var id = filePath.GetHashCode().ToString("x");

        // Act
        var result = await _repository.GetByIdAsync(id);

        // Assert
        result.ShouldNotBeNull();
        result.Title.ShouldBe("Test Post");
    }

    [Fact]
    public async Task GetByPathAsync_WithExistingPath_ReturnsContentItem()
    {
        // Arrange
        var contentDir = System.IO.Path.Combine(_basePath, "content");
        _mockFileSystem.Directory.CreateDirectory(contentDir);

        var filePath = "content/post1.md";
        var fullPath = System.IO.Path.Combine(_basePath, filePath);
        _mockFileSystem.File.WriteAllText(
            fullPath,
            "---\ntitle: Test Post\n---\nTest Content");

        // Act
        var result = await _repository.GetByPathAsync(filePath);

        // Assert
        result.ShouldNotBeNull();
        result.Title.ShouldBe("Test Post");
        result.Path.ShouldBe(filePath);
    }

    [Fact(Skip = "Temporarily disabled in CI until fixed")]
    public async Task SaveAsync_WithNewContent_CreatesFile()
    {
        // Arrange
        var contentItem = ContentItem.Create(
            "test-id",
            "Test Title",
            "Test content",
            "test.md",
            "filesystem");

        contentItem.SetSlug("test-title");

        // Act
        var result = await _repository.SaveAsync(contentItem);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe("test-id");

        // Verify file was created in mock filesystem
        var filePath = System.IO.Path.Combine(_basePath, "test.md");
        _mockFileSystem.File.Exists(filePath).ShouldBeTrue();
        var fileContent = _mockFileSystem.File.ReadAllText(filePath);
        fileContent.ShouldContain("title: \"Test Title\"");
    }

    [Fact(Skip = "Temporarily disabled in CI until fixed")]
    public async Task SaveAsync_WithExistingContent_UpdatesFile()
    {
        // Arrange
        var filePath = System.IO.Path.Combine(_basePath, "existing.md");
        _mockFileSystem.File.WriteAllText(
            filePath,
            "---\ntitle: Original Title\n---\nOriginal Content");

        var contentItem = ContentItem.Create(
            "update-id",
            "Updated Title",
            "Updated content",
            "existing.md",
            "filesystem");

        contentItem.SetProviderSpecificId(filePath);
        contentItem.SetSlug("updated-title");

        // Act
        var result = await _repository.SaveAsync(contentItem);

        // Assert
        result.ShouldNotBeNull();

        // Verify file was updated
        _mockFileSystem.File.Exists(filePath).ShouldBeTrue();
        var fileContent = _mockFileSystem.File.ReadAllText(filePath);
        fileContent.ShouldContain("title: \"Updated Title\"");
        fileContent.ShouldContain("Updated content");
    }

    [Fact(Skip = "Temporarily disabled in CI until fixed")]
    public async Task DeleteAsync_WithExistingContent_RemovesFile()
    {
        // Arrange
        var contentId = "delete-id";
        var filePath = System.IO.Path.Combine(_basePath, "to-delete.md");
        _mockFileSystem.File.WriteAllText(
            filePath,
            "---\ntitle: Delete Me\n---\nContent to delete");

        var contentItem = ContentItem.Create(
            contentId,
            "Delete Me",
            "Content to delete",
            "to-delete.md",
            "filesystem");

        contentItem.SetProviderSpecificId(filePath);

        // Add the item to the repository
        await _repository.SaveAsync(contentItem);

        // Act
        await _repository.DeleteAsync(contentId);

        // Assert
        _mockFileSystem.File.Exists(filePath).ShouldBeFalse("File should be deleted");
    }

    [Fact]
    public async Task FindByQueryAsync_WithCategoryFilter_ReturnsMatchingItems()
    {
        // Arrange
        var contentDir = System.IO.Path.Combine(_basePath, "content");
        _mockFileSystem.Directory.CreateDirectory(contentDir);

        // Create test files with categories
        _mockFileSystem.File.WriteAllText(
            System.IO.Path.Combine(contentDir, "post1.md"),
            "---\ntitle: Post 1\ncategories: Category1, Category2\n---\nContent 1");

        _mockFileSystem.File.WriteAllText(
            System.IO.Path.Combine(contentDir, "post2.md"),
            "---\ntitle: Post 2\ncategories: Category1\n---\nContent 2");

        _mockFileSystem.File.WriteAllText(
            System.IO.Path.Combine(contentDir, "post3.md"),
            "---\ntitle: Post 3\ncategories: Category3\n---\nContent 3");

        var query = new ContentQuery { Category = "Category1" };

        // Act
        var result = await _repository.FindByQueryAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.ShouldAllBe(item =>
            item.Categories.Any(c => c.Equals("Category1", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact(Skip = "Temporarily disabled in CI until fixed")]
    public async Task RefreshCacheAsync_InvalidatesAndReloadsCache()
    {
        // Arrange
        var contentDir = System.IO.Path.Combine(_basePath, "content");
        _mockFileSystem.Directory.CreateDirectory(contentDir);

        // Create initial file
        _mockFileSystem.File.WriteAllText(
            System.IO.Path.Combine(contentDir, "post1.md"),
            "---\ntitle: Post 1\n---\nContent 1");

        // Load initial content
        var initialResult = await _repository.GetAllAsync();
        initialResult.Count.ShouldBe(1);

        // Add another file directly to file system
        _mockFileSystem.File.WriteAllText(
            System.IO.Path.Combine(contentDir, "post2.md"),
            "---\ntitle: Post 2\n---\nContent 2");

        // Act
        await _repository.RefreshCacheAsync();
        var result = await _repository.GetAllAsync();

        // Assert
        result.Count.ShouldBe(2);
        var titles = result.Select(item => item.Title).ToList();
        titles.ShouldContain("Post 1");
        titles.ShouldContain("Post 2");
    }
}