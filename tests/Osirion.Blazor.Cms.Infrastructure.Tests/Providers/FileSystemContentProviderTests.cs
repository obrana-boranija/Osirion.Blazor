using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.Providers;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Providers;

public class FileSystemContentProviderTests
{
    private readonly FileSystemContentRepository _contentRepository;
    private readonly FileSystemDirectoryRepository _directoryRepository;
    private readonly IOptions<FileSystemOptions> _options;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<FileSystemContentProvider> _logger;
    private readonly FileSystemContentProvider _provider;

    public FileSystemContentProviderTests()
    {
        // Setup mocks
        _contentRepository = Substitute.For<FileSystemContentRepository>();
        _directoryRepository = Substitute.For<FileSystemDirectoryRepository>();
        _memoryCache = Substitute.For<IMemoryCache>();
        _logger = Substitute.For<ILogger<FileSystemContentProvider>>();

        // Configure options
        var options = new FileSystemOptions
        {
            BasePath = "/test/content",
            ContentRoot = "content",
            CreateDirectoriesIfNotExist = true,
            EnableLocalization = false,
            DefaultLocale = "en",
            SupportedExtensions = new List<string> { ".md", ".markdown" },
        };
        _options = Options.Create(options);

        // Create provider
        _provider = new FileSystemContentProvider(
            _contentRepository,
            _directoryRepository,
            _options,
            _memoryCache,
            _logger);
    }

    [Fact]
    public void ProviderId_BasedOnOptionValues()
    {
        // Assert
        _provider.ProviderId.ShouldStartWith("filesystem-");
    }

    [Fact]
    public void DisplayName_IncludesBasePath()
    {
        // Assert
        _provider.DisplayName.ShouldContain("/test/content");
    }

    [Fact]
    public void IsReadOnly_IsFalse()
    {
        // Assert
        _provider.IsReadOnly.ShouldBeFalse();
    }

    [Fact]
    public async Task GetAllItemsAsync_CallsRepository()
    {
        // Arrange
        var contentItems = new List<ContentItem>
        {
            ContentItem.Create("id1", "Test 1", "Content 1", "path1.md", "filesystem"),
            ContentItem.Create("id2", "Test 2", "Content 2", "path2.md", "filesystem")
        };

        _contentRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(contentItems);

        // Act
        var result = await _provider.GetAllItemsAsync();

        // Assert
        result.ShouldBe(contentItems);
        await _contentRepository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetItemByIdAsync_CallsRepository()
    {
        // Arrange
        var contentItem = ContentItem.Create("test-id", "Test", "Content", "path.md", "filesystem");

        _contentRepository.GetByIdAsync("test-id", Arg.Any<CancellationToken>())
            .Returns(contentItem);

        // Act
        var result = await _provider.GetItemByIdAsync("test-id");

        // Assert
        result.ShouldBe(contentItem);
        await _contentRepository.Received(1).GetByIdAsync("test-id", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetItemByPathAsync_CallsRepository()
    {
        // Arrange
        var contentItem = ContentItem.Create("test-id", "Test", "Content", "path.md", "filesystem");

        _contentRepository.GetByPathAsync("path.md", Arg.Any<CancellationToken>())
            .Returns(contentItem);

        // Act
        var result = await _provider.GetItemByPathAsync("path.md");

        // Assert
        result.ShouldBe(contentItem);
        await _contentRepository.Received(1).GetByPathAsync("path.md", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetItemByUrlAsync_CallsRepository()
    {
        // Arrange
        var contentItem = ContentItem.Create("test-id", "Test", "Content", "path.md", "filesystem");

        _contentRepository.GetByUrlAsync("test-url", Arg.Any<CancellationToken>())
            .Returns(contentItem);

        // Act
        var result = await _provider.GetItemByUrlAsync("test-url");

        // Assert
        result.ShouldBe(contentItem);
        await _contentRepository.Received(1).GetByUrlAsync("test-url", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetItemsByQueryAsync_CallsRepository()
    {
        // Arrange
        var query = new ContentQuery { Tag = "test-tag" };
        var contentItems = new List<ContentItem>
        {
            ContentItem.Create("id1", "Test 1", "Content 1", "path1.md", "filesystem"),
            ContentItem.Create("id2", "Test 2", "Content 2", "path2.md", "filesystem")
        };

        _contentRepository.FindByQueryAsync(query, Arg.Any<CancellationToken>())
            .Returns(contentItems);

        // Act
        var result = await _provider.GetItemsByQueryAsync(query);

        // Assert
        result.ShouldBe(contentItems);
        await _contentRepository.Received(1).FindByQueryAsync(query, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetDirectoriesAsync_CallsRepository()
    {
        // Arrange
        var directories = new List<DirectoryItem>
        {
            DirectoryItem.Create("dir1", "path1", "Dir 1", "filesystem"),
            DirectoryItem.Create("dir2", "path2", "Dir 2", "filesystem")
        };

        _directoryRepository.GetByLocaleAsync("en", Arg.Any<CancellationToken>())
            .Returns(directories);

        // Act
        var result = await _provider.GetDirectoriesAsync("en");

        // Assert
        result.ShouldBe(directories);
        await _directoryRepository.Received(1).GetByLocaleAsync("en", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetDirectoryByPathAsync_CallsRepository()
    {
        // Arrange
        var directory = DirectoryItem.Create("dir1", "path1", "Dir 1", "filesystem");

        _directoryRepository.GetByPathAsync("path1", Arg.Any<CancellationToken>())
            .Returns(directory);

        // Act
        var result = await _provider.GetDirectoryByPathAsync("path1");

        // Assert
        result.ShouldBe(directory);
        await _directoryRepository.Received(1).GetByPathAsync("path1", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetDirectoryByIdAsync_CallsRepository()
    {
        // Arrange
        var directory = DirectoryItem.Create("dir1", "path1", "Dir 1", "filesystem");

        _directoryRepository.GetByIdAsync("dir1", Arg.Any<CancellationToken>())
            .Returns(directory);

        // Act
        var result = await _provider.GetDirectoryByIdAsync("dir1");

        // Assert
        result.ShouldBe(directory);
        await _directoryRepository.Received(1).GetByIdAsync("dir1", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetDirectoryByUrlAsync_CallsRepository()
    {
        // Arrange
        var directory = DirectoryItem.Create("dir1", "path1", "Dir 1", "filesystem");

        _directoryRepository.GetByUrlAsync("dir-url", Arg.Any<CancellationToken>())
            .Returns(directory);

        // Act
        var result = await _provider.GetDirectoryByUrlAsync("dir-url");

        // Assert
        result.ShouldBe(directory);
        await _directoryRepository.Received(1).GetByUrlAsync("dir-url", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCategoriesAsync_DerivedFromContentItems()
    {
        // Arrange
        var contentItems = new List<ContentItem>
        {
            ContentItem.Create("id1", "Test 1", "Content 1", "path1.md", "filesystem"),
            ContentItem.Create("id2", "Test 2", "Content 2", "path2.md", "filesystem")
        };

        contentItems[0].AddCategory("Category A");
        contentItems[0].AddCategory("Category B");
        contentItems[1].AddCategory("Category B");
        contentItems[1].AddCategory("Category C");

        _contentRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(contentItems);

        // Act
        var result = await _provider.GetCategoriesAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);
        result.ShouldContain(c => c.Name == "Category A" && c.Count == 1);
        result.ShouldContain(c => c.Name == "Category B" && c.Count == 2);
        result.ShouldContain(c => c.Name == "Category C" && c.Count == 1);
    }

    [Fact]
    public async Task GetTagsAsync_DerivedFromContentItems()
    {
        // Arrange
        var contentItems = new List<ContentItem>
        {
            ContentItem.Create("id1", "Test 1", "Content 1", "path1.md", "filesystem"),
            ContentItem.Create("id2", "Test 2", "Content 2", "path2.md", "filesystem")
        };

        contentItems[0].AddTag("tag1");
        contentItems[0].AddTag("tag2");
        contentItems[1].AddTag("tag2");
        contentItems[1].AddTag("tag3");

        _contentRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(contentItems);

        // Act
        var result = await _provider.GetTagsAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);
        result.ShouldContain(t => t.Name == "tag1" && t.Count == 1);
        result.ShouldContain(t => t.Name == "tag2" && t.Count == 2);
        result.ShouldContain(t => t.Name == "tag3" && t.Count == 1);
    }

    [Fact]
    public async Task RefreshCacheAsync_RefreshesBothRepositories()
    {
        // Act
        await _provider.RefreshCacheAsync();

        // Assert
        await _contentRepository.Received(1).RefreshCacheAsync(Arg.Any<CancellationToken>());
        await _directoryRepository.Received(1).RefreshCacheAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public void Constructor_WithNullContentRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new FileSystemContentProvider(
            null!,
            _directoryRepository,
            _options,
            _memoryCache,
            _logger));
    }

    [Fact]
    public void Constructor_WithNullDirectoryRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new FileSystemContentProvider(
            _contentRepository,
            null!,
            _options,
            _memoryCache,
            _logger));
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new FileSystemContentProvider(
            _contentRepository,
            _directoryRepository,
            null,
            _memoryCache,
            _logger));
    }

    [Fact]
    public void Constructor_WithNullMemoryCache_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new FileSystemContentProvider(
            _contentRepository,
            _directoryRepository,
            _options,
            null,
            _logger));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new FileSystemContentProvider(
            _contentRepository,
            _directoryRepository,
            _options,
            _memoryCache,
            null!));
    }
}