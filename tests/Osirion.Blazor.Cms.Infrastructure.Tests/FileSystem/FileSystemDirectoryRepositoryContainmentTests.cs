using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.FileSystem;

/// <summary>
/// The repository works on the real file system, so every test runs in its own temporary sandbox.
/// The content root and the "victim" directory both live inside the sandbox, so even a regression
/// that escapes the content root cannot touch anything outside it.
/// </summary>
public sealed class FileSystemDirectoryRepositoryContainmentTests : IDisposable
{
    private const string ProviderId = "filesystem-test";

    private readonly string _sandbox = Path.Combine(Path.GetTempPath(), "osirion-fs-" + Guid.NewGuid().ToString("N"));
    private readonly string _contentRoot;
    private readonly string _victimDirectory;
    private readonly Dictionary<string, DirectoryItem> _cachedDirectories = new();
    private readonly FileSystemDirectoryRepository _repository;

    public FileSystemDirectoryRepositoryContainmentTests()
    {
        _contentRoot = Path.Combine(_sandbox, "content");
        _victimDirectory = Path.Combine(_sandbox, "victim");
        System.IO.Directory.CreateDirectory(_contentRoot);
        System.IO.Directory.CreateDirectory(_victimDirectory);
        File.WriteAllText(Path.Combine(_victimDirectory, "keep.txt"), "keep");

        var cacheManager = Substitute.For<IDirectoryCacheManager>();
        cacheManager.GetCachedDirectoriesAsync(
                Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<bool>())
            .Returns(_ => _cachedDirectories);

        var metadataProcessor = Substitute.For<IDirectoryMetadataProcessor>();
        metadataProcessor.GenerateMetadataContent(Arg.Any<DirectoryItem>()).Returns("---\ntitle: Test\n---\n");

        var options = Options.Create(new FileSystemOptions
        {
            BasePath = _contentRoot,
            WatchForChanges = false,
            CreateDirectoriesIfNotExist = true
        });

        _repository = new FileSystemDirectoryRepository(
            options,
            cacheManager,
            metadataProcessor,
            Substitute.For<IPathUtilities>(),
            NullLogger<FileSystemDirectoryRepository>.Instance);
    }

    public void Dispose()
    {
        _repository.Dispose();
        if (System.IO.Directory.Exists(_sandbox))
        {
            System.IO.Directory.Delete(_sandbox, recursive: true);
        }
    }

    [Fact]
    public async Task SaveAsync_NewDirectoryTraversingOutOfTheRoot_ThrowsAndCreatesNothing()
    {
        var directory = DirectoryItem.Create("escaped", "../escaped", "escaped", ProviderId);

        var exception = await Should.ThrowAsync<ContentProviderException>(() => _repository.SaveAsync(directory));

        exception.InnerException.ShouldBeOfType<ContentValidationException>();
        System.IO.Directory.Exists(Path.Combine(_sandbox, "escaped")).ShouldBeFalse();
    }

    [Fact]
    public async Task SaveAsync_NewDirectoryWithRootedPath_ThrowsAndCreatesNothing()
    {
        var rootedPath = Path.Combine(_sandbox, "rooted");
        var directory = DirectoryItem.Create("rooted", rootedPath, "rooted", ProviderId);

        var exception = await Should.ThrowAsync<ContentProviderException>(() => _repository.SaveAsync(directory));

        exception.InnerException.ShouldBeOfType<ContentValidationException>();
        System.IO.Directory.Exists(rootedPath).ShouldBeFalse();
    }

    [Fact]
    public async Task SaveAsync_CachedDirectoryWithPathOutsideTheRoot_ThrowsAndWritesNoIndexFile()
    {
        var directory = AddCachedDirectory("victim", "../victim");

        var exception = await Should.ThrowAsync<ContentProviderException>(() => _repository.SaveAsync(directory));

        exception.InnerException.ShouldBeOfType<ContentValidationException>();
        File.Exists(Path.Combine(_victimDirectory, "_index.md")).ShouldBeFalse();
    }

    [Fact]
    public async Task SaveAsync_NewDirectoryInsideTheRoot_CreatesTheDirectoryAndIndexFile()
    {
        var directory = DirectoryItem.Create("guides", "guides", "Guides", ProviderId);

        await _repository.SaveAsync(directory);

        File.Exists(Path.Combine(_contentRoot, "guides", "_index.md")).ShouldBeTrue();
    }

    [Fact]
    public async Task DeleteRecursiveAsync_DirectoryPathOutsideTheRoot_ThrowsAndKeepsTheTarget()
    {
        AddCachedDirectory("victim", "../victim");

        var exception = await Should.ThrowAsync<ContentProviderException>(() => _repository.DeleteRecursiveAsync("victim"));

        exception.InnerException.ShouldBeOfType<ContentValidationException>();
        File.Exists(Path.Combine(_victimDirectory, "keep.txt")).ShouldBeTrue();
    }

    [Fact]
    public async Task MoveAsync_NewParentPathOutsideTheRoot_ThrowsAndLeavesTheSourceInPlace()
    {
        System.IO.Directory.CreateDirectory(Path.Combine(_contentRoot, "blog"));
        AddCachedDirectory("blog", "blog");
        AddCachedDirectory("victim", "../victim");

        var exception = await Should.ThrowAsync<ContentProviderException>(() => _repository.MoveAsync("blog", "victim"));

        exception.InnerException.ShouldBeOfType<ContentValidationException>();
        System.IO.Directory.Exists(Path.Combine(_contentRoot, "blog")).ShouldBeTrue();
        System.IO.Directory.Exists(Path.Combine(_victimDirectory, "blog")).ShouldBeFalse();
    }

    [Fact]
    public async Task MoveAsync_SourcePathOutsideTheRoot_ThrowsAndLeavesTheTargetInPlace()
    {
        AddCachedDirectory("victim", "../victim");

        var exception = await Should.ThrowAsync<ContentProviderException>(() => _repository.MoveAsync("victim", null));

        exception.InnerException.ShouldBeOfType<ContentValidationException>();
        File.Exists(Path.Combine(_victimDirectory, "keep.txt")).ShouldBeTrue();
    }

    private DirectoryItem AddCachedDirectory(string id, string path)
    {
        var directory = DirectoryItem.Create(id, path, id, ProviderId);
        _cachedDirectories[id] = directory;
        return directory;
    }
}
