using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.Providers;
using Shouldly;
using System.IO.Abstractions.TestingHelpers;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Providers;

/// <summary>
/// The provider owns two concrete repositories whose query methods are not virtual, so they cannot be substituted.
/// The tests therefore run the real repositories against an in-memory file system and a substituted directory cache,
/// and assert on what the provider returns.
/// </summary>
public sealed class FileSystemContentProviderTests : IDisposable
{
    private const string ProviderIdentity = "filesystem";

    private static readonly string BasePath = MockUnixSupport.Path(@"c:\test\content");

    private readonly MockFileSystem _fileSystem = new();
    private readonly Dictionary<string, DirectoryItem> _cachedDirectories = new();
    private readonly IDirectoryCacheManager _directoryCacheManager = Substitute.For<IDirectoryCacheManager>();
    private readonly IContentQueryFilter _queryFilter = Substitute.For<IContentQueryFilter>();
    private readonly IOptions<FileSystemOptions> _options;
    private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
    private readonly FileSystemContentRepository _contentRepository;
    private readonly FileSystemDirectoryRepository _directoryRepository;
    private readonly FileSystemContentProvider _provider;

    public FileSystemContentProviderTests()
    {
        _fileSystem.Directory.CreateDirectory(BasePath);

        _options = Options.Create(new FileSystemOptions
        {
            BasePath = BasePath,
            ContentRoot = "content",
            CreateDirectoriesIfNotExist = true,
            WatchForChanges = false,
            EnableLocalization = false,
            DefaultLocale = "en",
            SupportedExtensions = new List<string> { ".md", ".markdown" },
        });

        var markdownProcessor = Substitute.For<IMarkdownProcessor>();
        markdownProcessor.ExtractFrontMatterAndContent(Arg.Any<string>())
            .Returns(call => (ReadFrontMatter(call.Arg<string>()), call.Arg<string>()));
        markdownProcessor.RenderToHtmlAsync(Arg.Any<string>(), Arg.Any<bool>())
            .Returns(call => call.Arg<string>());

        _queryFilter.ApplyFilters(Arg.Any<IQueryable<ContentItem>>(), Arg.Any<ContentQuery>())
            .Returns(call => call.Arg<IQueryable<ContentItem>>());

        _directoryCacheManager.GetCachedDirectoriesAsync(
                Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<bool>())
            .Returns(_ => _cachedDirectories);

        var pathUtilities = Substitute.For<IPathUtilities>();
        pathUtilities.NormalizePath(Arg.Any<string>()).Returns(call => call.Arg<string>());

        _contentRepository = new FileSystemContentRepository(
            markdownProcessor,
            _queryFilter,
            _options,
            NullLogger<FileSystemContentRepository>.Instance,
            _fileSystem);

        _directoryRepository = new FileSystemDirectoryRepository(
            _options,
            _directoryCacheManager,
            Substitute.For<IDirectoryMetadataProcessor>(),
            pathUtilities,
            NullLogger<FileSystemDirectoryRepository>.Instance);

        _provider = new FileSystemContentProvider(
            _contentRepository,
            _directoryRepository,
            _options,
            _memoryCache,
            NullLogger<FileSystemContentProvider>.Instance);
    }

    public void Dispose()
    {
        _contentRepository.Dispose();
        _directoryRepository.Dispose();
        _memoryCache.Dispose();
    }

    [Fact]
    public void ProviderId_BasedOnOptionValues()
    {
        _provider.ProviderId.ShouldStartWith("filesystem-");
    }

    [Fact]
    public void ProviderId_WithConfiguredId_UsesConfiguredId()
    {
        _options.Value.ProviderId = "docs";

        _provider.ProviderId.ShouldBe("docs");
    }

    [Fact]
    public void DisplayName_IncludesBasePath()
    {
        _provider.DisplayName.ShouldContain(BasePath);
    }

    [Fact]
    public void IsReadOnly_IsFalse()
    {
        _provider.IsReadOnly.ShouldBeFalse();
    }

    [Fact]
    public async Task GetAllItemsAsync_CallsRepository()
    {
        WriteContent("path1.md", "Content 1");
        WriteContent("path2.md", "Content 2");

        var result = await _provider.GetAllItemsAsync();

        result.Select(item => item.Path).ShouldBe(["path1.md", "path2.md"], ignoreOrder: true);
    }

    [Fact]
    public async Task GetItemByIdAsync_CallsRepository()
    {
        WriteContent("path.md", "Content");
        var expected = (await _provider.GetAllItemsAsync()).Single();

        var result = await _provider.GetItemByIdAsync(expected.Id);

        result.ShouldBeSameAs(expected);
    }

    [Fact]
    public async Task GetItemByIdAsync_WithUnknownId_ReturnsNull()
    {
        WriteContent("path.md", "Content");

        var result = await _provider.GetItemByIdAsync("unknown-id");

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetItemByIdAsync_WithBlankId_ThrowsArgumentException()
    {
        await Should.ThrowAsync<ArgumentException>(() => _provider.GetItemByIdAsync(" "));
    }

    [Fact]
    public async Task GetItemByPathAsync_CallsRepository()
    {
        WriteContent("path.md", "Content");
        var expected = (await _provider.GetAllItemsAsync()).Single();

        var result = await _provider.GetItemByPathAsync("path.md");

        result.ShouldBeSameAs(expected);
    }

    [Fact]
    public async Task GetItemByUrlAsync_CallsRepository()
    {
        WriteContent("path.md", "Content");
        var expected = (await _provider.GetAllItemsAsync()).Single();

        var result = await _provider.GetItemByUrlAsync(expected.Url);

        result.ShouldBeSameAs(expected);
    }

    [Fact]
    public async Task GetItemsByQueryAsync_CallsRepository()
    {
        WriteContent("path1.md", "Content 1");
        WriteContent("path2.md", "Content 2");
        var query = new ContentQuery { Tag = "test-tag" };
        var matching = (await _provider.GetAllItemsAsync()).First();
        _queryFilter.ApplyFilters(Arg.Any<IQueryable<ContentItem>>(), query)
            .Returns(new[] { matching }.AsQueryable());

        var result = await _provider.GetItemsByQueryAsync(query);

        result.ShouldNotBeNull();
        result.ShouldBe([matching]);
    }

    [Fact]
    public async Task GetDirectoriesAsync_CallsRepository()
    {
        var parent = AddDirectory("dir1", "path1");
        parent.SetLocale("en");
        var child = AddDirectory("dir2", "path1/path2");
        child.SetLocale("en");
        parent.AddChild(child);

        var result = await _provider.GetDirectoriesAsync("en");

        result.ShouldBe([child]);
    }

    [Fact]
    public async Task GetDirectoryByPathAsync_CallsRepository()
    {
        var directory = AddDirectory("dir1", "path1");

        var result = await _provider.GetDirectoryByPathAsync("path1");

        result.ShouldBeSameAs(directory);
    }

    [Fact]
    public async Task GetDirectoryByIdAsync_CallsRepository()
    {
        var directory = AddDirectory("dir1", "path1");

        var result = await _provider.GetDirectoryByIdAsync("dir1");

        result.ShouldBeSameAs(directory);
    }

    [Fact]
    public async Task GetDirectoryByUrlAsync_CallsRepository()
    {
        var directory = AddDirectory("dir1", "path1");

        var result = await _provider.GetDirectoryByUrlAsync(directory.Url);

        result.ShouldBeSameAs(directory);
    }

    [Fact]
    public async Task GetCategoriesAsync_DerivedFromContentItems()
    {
        WriteContent("path1.md", "categories: Category A, Category B");
        WriteContent("path2.md", "categories: Category B, Category C");

        var result = await _provider.GetCategoriesAsync();

        result.Count.ShouldBe(3);
        result.ShouldContain(c => c.Name == "Category A" && c.Count == 1);
        result.ShouldContain(c => c.Name == "Category B" && c.Count == 2);
        result.ShouldContain(c => c.Name == "Category C" && c.Count == 1);
    }

    [Fact]
    public async Task GetTagsAsync_DerivedFromContentItems()
    {
        WriteContent("path1.md", "tags: tag1, tag2");
        WriteContent("path2.md", "tags: tag2, tag3");

        var result = await _provider.GetTagsAsync();

        result.Count.ShouldBe(3);
        result.ShouldContain(t => t.Name == "tag1" && t.Count == 1);
        result.ShouldContain(t => t.Name == "tag2" && t.Count == 2);
        result.ShouldContain(t => t.Name == "tag3" && t.Count == 1);
    }

    [Fact]
    public async Task RefreshCacheAsync_RefreshesBothRepositories()
    {
        WriteContent("path1.md", "Content 1");
        (await _provider.GetAllItemsAsync()).Count.ShouldBe(1);
        WriteContent("path2.md", "Content 2");

        await _provider.RefreshCacheAsync();

        (await _provider.GetAllItemsAsync()).Count.ShouldBe(2);
        await _directoryCacheManager.Received(1).InvalidateCacheAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public void Constructor_WithNullContentRepository_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => new FileSystemContentProvider(
            null!,
            _directoryRepository,
            _options,
            _memoryCache,
            NullLogger<FileSystemContentProvider>.Instance));
    }

    [Fact]
    public void Constructor_WithNullDirectoryRepository_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => new FileSystemContentProvider(
            _contentRepository,
            null!,
            _options,
            _memoryCache,
            NullLogger<FileSystemContentProvider>.Instance));
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => new FileSystemContentProvider(
            _contentRepository,
            _directoryRepository,
            null!,
            _memoryCache,
            NullLogger<FileSystemContentProvider>.Instance));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => new FileSystemContentProvider(
            _contentRepository,
            _directoryRepository,
            _options,
            _memoryCache,
            null!));
    }

    private void WriteContent(string relativePath, string body)
    {
        _fileSystem.File.WriteAllText(Path.Combine(BasePath, relativePath), body);
    }

    private DirectoryItem AddDirectory(string id, string path)
    {
        var directory = DirectoryItem.Create(id, path, id, ProviderIdentity);
        directory.SetUrl("/" + path);
        _cachedDirectories[id] = directory;
        return directory;
    }

    /// <summary>
    /// The substituted markdown processor reads <c>categories:</c> and <c>tags:</c> lines, comma separated, as front matter.
    /// </summary>
    private static FrontMatter ReadFrontMatter(string markdown)
    {
        var frontMatter = new FrontMatter { Title = "Title" };
        foreach (var line in markdown.Split('\n'))
        {
            var parts = line.Split(':', 2);
            if (parts.Length != 2)
            {
                continue;
            }

            var values = parts[1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
            switch (parts[0].Trim())
            {
                case "categories":
                    frontMatter.Categories = values;
                    break;
                case "tags":
                    frontMatter.Tags = values;
                    break;
            }
        }

        return frontMatter;
    }
}
