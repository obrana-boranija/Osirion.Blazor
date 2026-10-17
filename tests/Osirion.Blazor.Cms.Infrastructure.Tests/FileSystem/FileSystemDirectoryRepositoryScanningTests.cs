using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;
using Shouldly;
using DirectoryNotFoundException = Osirion.Blazor.Cms.Domain.Exceptions.DirectoryNotFoundException;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.FileSystem;

/// <summary>
/// Scanning, saving, moving and watcher setup of <see cref="FileSystemDirectoryRepository"/>. The repository works on
/// the real file system, so every test runs in its own temporary sandbox; the cache manager substitute runs the loader.
/// </summary>
public sealed class FileSystemDirectoryRepositoryScanningTests : IDisposable
{
    private readonly string _sandbox = Path.Combine(Path.GetTempPath(), "osirion-fs-scan-" + Guid.NewGuid().ToString("N"));
    private readonly string _basePath;
    private readonly IDirectoryMetadataProcessor _metadataProcessor = Substitute.For<IDirectoryMetadataProcessor>();
    private readonly IDirectoryCacheManager _cacheManager = Substitute.For<IDirectoryCacheManager>();
    private readonly IPathUtilities _pathUtilities = Substitute.For<IPathUtilities>();
    private readonly List<FileSystemDirectoryRepository> _repositories = new();

    public FileSystemDirectoryRepositoryScanningTests()
    {
        _basePath = Path.Combine(_sandbox, "content");

        _cacheManager.GetCachedDirectoriesAsync(
                Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<bool>())
            .Returns(call => call.Arg<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>()(call.Arg<CancellationToken>()));

        _pathUtilities.NormalizePath(Arg.Any<string>()).Returns(call => call.Arg<string>().Replace('\\', '/').Trim('/'));
        _pathUtilities.MatchesGlobPattern(Arg.Any<string>(), Arg.Any<string>())
            .Returns(call => Path.GetFileName(call.ArgAt<string>(0).TrimEnd('/')).StartsWith("skip", StringComparison.Ordinal));
        _pathUtilities.ExtractLocaleFromPath(Arg.Any<string>())
            .Returns(call => call.Arg<string>().Split('/')[0] is "de" ? "de" : "en");
    }

    public void Dispose()
    {
        foreach (var repository in _repositories)
        {
            repository.Dispose();
        }

        if (System.IO.Directory.Exists(_sandbox))
        {
            System.IO.Directory.Delete(_sandbox, recursive: true);
        }
    }

    private FileSystemDirectoryRepository Repository(
        Action<FileSystemOptions>? configure = null,
        ILogger<FileSystemDirectoryRepository>? logger = null)
    {
        var options = new FileSystemOptions
        {
            BasePath = _basePath,
            CreateDirectoriesIfNotExist = true,
            WatchForChanges = false,
            EnableLocalization = false,
            DefaultLocale = "en",
            ExcludePatterns = new List<string> { "skip*" },
        };
        configure?.Invoke(options);

        var repository = new FileSystemDirectoryRepository(
            Options.Create(options),
            _cacheManager,
            _metadataProcessor,
            _pathUtilities,
            logger ?? NullLogger<FileSystemDirectoryRepository>.Instance);
        _repositories.Add(repository);
        return repository;
    }

    private string CreateOnDisk(string relativePath)
    {
        var fullPath = Path.Combine(_basePath, relativePath.Replace('/', Path.DirectorySeparatorChar));
        System.IO.Directory.CreateDirectory(fullPath);
        return fullPath;
    }

    private static async Task<DirectoryItem> Existing(FileSystemDirectoryRepository repository, string path) =>
        (await repository.GetByPathAsync(path)).ShouldNotBeNull();

    [Fact]
    public async Task GetAllAsync_BasePathMissingAndCreationAllowed_CreatesItAndReturnsNothing()
    {
        var repository = Repository();

        var directories = await repository.GetAllAsync();

        directories.ShouldBeEmpty();
        System.IO.Directory.Exists(_basePath).ShouldBeTrue();
    }

    [Fact]
    public async Task GetAllAsync_BasePathMissingAndCreationNotAllowed_Throws()
    {
        var repository = Repository(options => options.CreateDirectoriesIfNotExist = false);

        var exception = await Should.ThrowAsync<ContentProviderException>(() => repository.GetAllAsync());

        exception.InnerException.ShouldBeOfType<DirectoryNotFoundException>();
        System.IO.Directory.Exists(_basePath).ShouldBeFalse();
    }

    [Fact]
    public async Task GetAllAsync_ExcludedFolders_AreSkippedAtEveryLevel()
    {
        CreateOnDisk("docs/guide");
        CreateOnDisk("docs/skip-drafts");
        CreateOnDisk("skip-build");
        var repository = Repository();

        var docs = (await repository.GetAllAsync()).ShouldHaveSingleItem();

        docs.Path.ShouldBe("docs");
        docs.Children.ShouldHaveSingleItem().Path.ShouldBe("docs/guide");
    }

    [Fact]
    public async Task GetAllAsync_ScanningOneFolderFails_StillReturnsTheOthers()
    {
        CreateOnDisk("broken");
        CreateOnDisk("docs");
        _pathUtilities.MatchesGlobPattern(Arg.Is<string>(path => path.TrimEnd('/').EndsWith("broken", StringComparison.Ordinal)), Arg.Any<string>())
            .Returns(_ => throw new ArgumentException("bad pattern"));
        var repository = Repository();

        var directories = await repository.GetAllAsync();

        directories.ShouldHaveSingleItem().Path.ShouldBe("docs");
    }

    [Fact]
    public async Task GetAllAsync_SubdirectoriesExcluded_ScansOnlyTheTopLevel()
    {
        CreateOnDisk("docs/guide");
        var repository = Repository(options => options.IncludeSubdirectories = false);

        var docs = (await repository.GetAllAsync()).ShouldHaveSingleItem();

        docs.Children.ShouldBeEmpty();
        (await repository.GetByPathAsync("docs/guide")).ShouldBeNull();
    }

    [Fact]
    public async Task GetAllAsync_LocalizationEnabled_TakesTheLocaleFromThePath()
    {
        CreateOnDisk("de/blog");
        CreateOnDisk("en/blog");
        var repository = Repository(options => options.EnableLocalization = true);

        (await Existing(repository, "de/blog")).Locale.ShouldBe("de");
        (await Existing(repository, "en/blog")).Locale.ShouldBe("en");
    }

    [Fact]
    public async Task GetAllAsync_MetadataProcessingFails_KeepsTheDirectoryWithoutAProviderSpecificId()
    {
        var folder = CreateOnDisk("docs");
        await File.WriteAllTextAsync(Path.Combine(folder, "_index.md"), "---\ntitle: [\n---");
        _metadataProcessor.ProcessMetadata(Arg.Any<DirectoryItem>(), Arg.Any<string>()).Throws(new FormatException("bad yaml"));
        var repository = Repository();

        var docs = await Existing(repository, "docs");

        docs.ProviderSpecificId.ShouldBeNullOrEmpty();
    }

    [Fact]
    public async Task SaveAsync_Null_Throws()
    {
        var repository = Repository();

        await Should.ThrowAsync<ArgumentNullException>(() => repository.SaveAsync(null!));
    }

    [Fact]
    public async Task SaveAsync_ExistingDirectory_RewritesItsIndexFile()
    {
        var folder = CreateOnDisk("docs");
        _metadataProcessor.GenerateMetadataContent(Arg.Any<DirectoryItem>()).Returns("---\ntitle: Docs\n---\n");
        var repository = Repository();
        var docs = await Existing(repository, "docs");

        var saved = await repository.SaveAsync(docs);

        var indexPath = Path.Combine(folder, "_index.md");
        (await File.ReadAllTextAsync(indexPath)).ShouldBe("---\ntitle: Docs\n---\n");
        saved.ProviderSpecificId.ShouldBe(indexPath);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task MoveAsync_BlankId_Throws(string id)
    {
        var repository = Repository();

        await Should.ThrowAsync<ArgumentException>(() => repository.MoveAsync(id, null));
    }

    [Fact]
    public async Task MoveAsync_UnknownDirectory_ThrowsWrappingNotFound()
    {
        CreateOnDisk("docs");
        var repository = Repository();

        var exception = await Should.ThrowAsync<ContentProviderException>(() => repository.MoveAsync("missing", null));

        exception.InnerException.ShouldBeOfType<DirectoryNotFoundException>();
    }

    [Fact]
    public async Task MoveAsync_UnknownNewParent_ThrowsWrappingNotFoundAndLeavesTheFolder()
    {
        var folder = CreateOnDisk("docs");
        var repository = Repository();
        var docs = await Existing(repository, "docs");

        var exception = await Should.ThrowAsync<ContentProviderException>(() => repository.MoveAsync(docs.Id, "missing"));

        exception.InnerException.ShouldBeOfType<DirectoryNotFoundException>();
        System.IO.Directory.Exists(folder).ShouldBeTrue();
    }

    [Fact]
    public async Task MoveAsync_IntoItsOwnDescendant_IsRefused()
    {
        CreateOnDisk("docs/guide");
        var repository = Repository();
        var docs = await Existing(repository, "docs");
        var guide = await Existing(repository, "docs/guide");

        var exception = await Should.ThrowAsync<ContentProviderException>(() => repository.MoveAsync(docs.Id, guide.Id));

        exception.InnerException.ShouldBeOfType<ContentValidationException>();
        System.IO.Directory.Exists(Path.Combine(_basePath, "docs", "guide")).ShouldBeTrue();
    }

    [Fact]
    public async Task MoveAsync_SourceFolderRemovedSinceTheScan_ThrowsWrappingNotFound()
    {
        var folder = CreateOnDisk("docs");
        var repository = Repository();
        var docs = await Existing(repository, "docs");
        _cacheManager.GetCachedDirectoriesAsync(
                Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<bool>())
            .Returns(new Dictionary<string, DirectoryItem> { [docs.Id] = docs });
        System.IO.Directory.Delete(folder);

        var exception = await Should.ThrowAsync<ContentProviderException>(() => repository.MoveAsync(docs.Id, null));

        exception.InnerException.ShouldBeOfType<DirectoryNotFoundException>();
    }

    [Fact]
    public async Task MoveAsync_ToTheRoot_MovesTheFolderToTheTopLevel()
    {
        CreateOnDisk("docs/guide");
        _metadataProcessor.GenerateMetadataContent(Arg.Any<DirectoryItem>()).Returns("---\n---\n");
        var repository = Repository();
        var guide = await Existing(repository, "docs/guide");

        var moved = await repository.MoveAsync(guide.Id, null);

        moved.Path.ShouldBe("guide");
        moved.Parent.ShouldBeNull();
        System.IO.Directory.Exists(Path.Combine(_basePath, "guide")).ShouldBeTrue();
        System.IO.Directory.Exists(Path.Combine(_basePath, "docs", "guide")).ShouldBeFalse();
    }

    [Fact]
    public async Task DeleteRecursiveAsync_FolderRemovedSinceTheScan_ThrowsDirectoryNotFound()
    {
        var folder = CreateOnDisk("docs");
        var repository = Repository();
        var docs = await Existing(repository, "docs");
        _cacheManager.GetCachedDirectoriesAsync(
                Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<bool>())
            .Returns(new Dictionary<string, DirectoryItem> { [docs.Id] = docs });
        System.IO.Directory.Delete(folder);

        await Should.ThrowAsync<DirectoryNotFoundException>(() => repository.DeleteRecursiveAsync(docs.Id));
    }

    [Fact]
    public void Constructor_WatchEnabledAndBasePathMissingWithoutCreation_DoesNotCreateItAndSkipsTheWatcher()
    {
        var logger = new RecordingLogger<FileSystemDirectoryRepository>();

        Repository(
            options =>
            {
                options.WatchForChanges = true;
                options.CreateDirectoriesIfNotExist = false;
            },
            logger);

        System.IO.Directory.Exists(_basePath).ShouldBeFalse();
        logger.Contains(LogLevel.Warning, "Cannot set up file watcher").ShouldBeTrue();
        logger.Contains(LogLevel.Information, "Directory watcher set up").ShouldBeFalse();
    }

    [Fact]
    public void Constructor_WatchEnabledAndBasePathIsAFile_LogsTheWatcherFailureWithoutThrowing()
    {
        System.IO.Directory.CreateDirectory(_sandbox);
        File.WriteAllText(_basePath, "not a folder");
        var logger = new RecordingLogger<FileSystemDirectoryRepository>();

        Repository(options => options.WatchForChanges = true, logger);

        logger.Contains(LogLevel.Error, "Failed to set up directory watcher").ShouldBeTrue();
        logger.Contains(LogLevel.Information, "Directory watcher set up").ShouldBeFalse();
        File.Exists(_basePath).ShouldBeTrue();
    }

    // A refresh started by the watcher (forceRefresh) does not run the loader, so a change event raised while the
    // test creates folders cannot scan the sandbox while Dispose deletes it.
    [Fact]
    public async Task Constructor_WatchEnabledAndBasePathMissingWithCreation_CreatesItAndStillScans()
    {
        _cacheManager.GetCachedDirectoriesAsync(
                Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<bool>())
            .Returns(call => call.ArgAt<bool>(2)
                ? Task.FromResult(new Dictionary<string, DirectoryItem>())
                : call.Arg<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>()(call.Arg<CancellationToken>()));
        var logger = new RecordingLogger<FileSystemDirectoryRepository>();

        var repository = Repository(options => options.WatchForChanges = true, logger);
        CreateOnDisk("docs");

        System.IO.Directory.Exists(_basePath).ShouldBeTrue();
        logger.Contains(LogLevel.Information, "Directory watcher set up for path: " + _basePath).ShouldBeTrue();
        (await repository.GetAllAsync()).ShouldHaveSingleItem().Path.ShouldBe("docs");
    }
}
