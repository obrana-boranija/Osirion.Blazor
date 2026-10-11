using Osirion.Blazor.Cms.Infrastructure.Utilities;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Shouldly;
using DirectoryNotFoundException = Osirion.Blazor.Cms.Domain.Exceptions.DirectoryNotFoundException;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.FileSystem;

/// <summary>
/// The repository works on the real file system, so every test runs in its own temporary sandbox.
/// The cache manager substitute runs the loader it is given, so reads see what is on disk.
/// </summary>
public sealed class FileSystemDirectoryRepositoryTests : IDisposable
{
    private readonly string _sandbox = Path.Combine(Path.GetTempPath(), "osirion-fs-dir-" + Guid.NewGuid().ToString("N"));
    private readonly string _basePath;
    private readonly IDirectoryMetadataProcessor _metadataProcessor;
    private readonly IDirectoryCacheManager _cacheManager;
    private readonly FileSystemDirectoryRepository _repository;

    public FileSystemDirectoryRepositoryTests()
    {
        _basePath = Path.Combine(_sandbox, "content");
        System.IO.Directory.CreateDirectory(_basePath);

        _metadataProcessor = Substitute.For<IDirectoryMetadataProcessor>();
        _cacheManager = Substitute.For<IDirectoryCacheManager>();
        _cacheManager.GetCachedDirectoriesAsync(
                Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<bool>())
            .Returns(call => call.Arg<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>()(call.Arg<CancellationToken>()));

        var pathUtils = Substitute.For<IPathUtilities>();
        pathUtils.NormalizePath(Arg.Any<string>()).Returns(call => call.Arg<string>());
        pathUtils.ExtractLocaleFromPath(Arg.Any<string>()).Returns("en");

        var options = Options.Create(new FileSystemOptions
        {
            BasePath = _basePath,
            ContentRoot = "content",
            CreateDirectoriesIfNotExist = true,
            WatchForChanges = false,
            EnableLocalization = false,
            DefaultLocale = "en",
            SupportedExtensions = new List<string> { ".md", ".markdown" },
        });

        _repository = new FileSystemDirectoryRepository(
            options,
            _cacheManager,
            _metadataProcessor,
            pathUtils,
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
    public async Task GetAllAsync_UsesCache()
    {
        // Arrange
        var directories = new Dictionary<string, DirectoryItem>
        {
            { "dir1", DirectoryItem.Create("dir1", "path1", "Dir 1", "filesystem") },
            { "dir2", DirectoryItem.Create("dir2", "path2", "Dir 2", "filesystem") }
        };

        _cacheManager.GetCachedDirectoriesAsync(
                Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<bool>())
            .Returns(directories);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Count.ShouldBe(2);
        result.ShouldContain(d => d.Id == "dir1");
        result.ShouldContain(d => d.Id == "dir2");

        await _cacheManager.Received(1).GetCachedDirectoriesAsync(
            Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
            Arg.Any<CancellationToken>(),
            Arg.Any<bool>());
    }

    [Fact]
    public async Task RefreshCacheAsync_InvalidatesCacheManager()
    {
        // Act
        await _repository.RefreshCacheAsync();

        // Assert
        await _cacheManager.Received(1).InvalidateCacheAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveAsync_CreatesDirectoryAndMetadataFile()
    {
        // Arrange
        var directory = DirectoryItem.Create("test-dir", "test/path", "Test Directory", "filesystem");
        const string metadata = "---\ntitle: Test Directory\n---\n\nTest content";
        _metadataProcessor.GenerateMetadataContent(directory).Returns(metadata);

        // Act
        var result = await _repository.SaveAsync(directory);

        // Assert
        result.Id.ShouldBe("test-dir");
        var fullPath = Path.Combine(_basePath, "test", "path");
        System.IO.Directory.Exists(fullPath).ShouldBeTrue("Directory should be created");
        var metadataPath = Path.Combine(fullPath, "_index.md");
        File.ReadAllText(metadataPath).ShouldBe(metadata);
        result.ProviderSpecificId.ShouldBe(metadataPath);
        await _cacheManager.Received(1).InvalidateCacheAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_RemovesDirectoryAndFiles()
    {
        // Arrange
        var fullPath = CreateDirectoryOnDisk("test/path");
        File.WriteAllText(Path.Combine(fullPath, "_index.md"), "metadata");
        File.WriteAllText(Path.Combine(fullPath, "file.md"), "content");
        var directory = await GetByPathAsync("test/path");

        // Act
        await _repository.DeleteAsync(directory.Id);

        // Assert
        System.IO.Directory.Exists(fullPath).ShouldBeFalse("Directory should be removed");
        await _cacheManager.Received(1).InvalidateCacheAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteRecursiveAsync_RemovesDirectoryAndSubdirectories()
    {
        // Arrange
        var fullPath = CreateDirectoryOnDisk("parent/path");
        var childPath = CreateDirectoryOnDisk("parent/path/child");
        File.WriteAllText(Path.Combine(fullPath, "_index.md"), "parent metadata");
        File.WriteAllText(Path.Combine(childPath, "_index.md"), "child metadata");
        var parent = await GetByPathAsync("parent/path");

        // Act
        await _repository.DeleteRecursiveAsync(parent.Id);

        // Assert
        System.IO.Directory.Exists(fullPath).ShouldBeFalse("Parent directory should be removed");
        System.IO.Directory.Exists(childPath).ShouldBeFalse("Child directory should be removed");
        await _cacheManager.Received(1).InvalidateCacheAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WithUnknownId_ThrowsDirectoryNotFoundException()
    {
        // Act & Assert
        await Should.ThrowAsync<DirectoryNotFoundException>(() => _repository.DeleteAsync("unknown-id"));
    }

    [Fact]
    public async Task MoveAsync_MovesDirectoryToNewParent()
    {
        // Arrange
        var fullSourcePath = CreateDirectoryOnDisk("source/path");
        var fullTargetParentPath = CreateDirectoryOnDisk("target/parent");
        File.WriteAllText(Path.Combine(fullSourcePath, "_index.md"), "source metadata");
        File.WriteAllText(Path.Combine(fullSourcePath, "file.md"), "source content");
        File.WriteAllText(Path.Combine(fullTargetParentPath, "_index.md"), "target metadata");
        var source = await GetByPathAsync("source/path");
        var targetParent = await GetByPathAsync("target/parent");
        _metadataProcessor.GenerateMetadataContent(Arg.Any<DirectoryItem>())
            .Returns("---\ntitle: Source Directory\n---\n\nUpdated content");

        // Act
        var result = await _repository.MoveAsync(source.Id, targetParent.Id);

        // Assert
        result.Id.ShouldBe(source.Id);
        result.Parent.ShouldNotBeNull();
        result.Parent.Id.ShouldBe(targetParent.Id);
        System.IO.Directory.Exists(fullSourcePath).ShouldBeFalse("Source directory should be removed");
        var expectedNewPath = Path.Combine(fullTargetParentPath, "path");
        System.IO.Directory.Exists(expectedNewPath).ShouldBeTrue("New directory should be created");
        File.ReadAllText(Path.Combine(expectedNewPath, "file.md")).ShouldBe("source content");
        File.ReadAllText(Path.Combine(expectedNewPath, "_index.md")).ShouldContain("Updated content");
        await _cacheManager.Received(1).InvalidateCacheAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MoveAsync_WhenDestinationAlreadyExists_ThrowsAndKeepsBothDirectories()
    {
        // Arrange
        var fullSourcePath = CreateDirectoryOnDisk("source/path");
        var fullTargetParentPath = CreateDirectoryOnDisk("target/parent");
        var occupiedPath = CreateDirectoryOnDisk("target/parent/path");
        File.WriteAllText(Path.Combine(occupiedPath, "keep.md"), "keep");
        var source = await GetByPathAsync("source/path");
        var targetParent = await GetByPathAsync("target/parent");

        // Act
        var exception = await Should.ThrowAsync<ContentProviderException>(
            () => _repository.MoveAsync(source.Id, targetParent.Id));

        // Assert
        exception.InnerException.ShouldBeOfType<ContentValidationException>();
        System.IO.Directory.Exists(fullSourcePath).ShouldBeTrue();
        File.ReadAllText(Path.Combine(fullTargetParentPath, "path", "keep.md")).ShouldBe("keep");
    }

    [Fact]
    public async Task GetAllAsync_ScansFolderStructure()
    {
        // Arrange
        var contentFolder = CreateDirectoryOnDisk("content");
        var subFolder = CreateDirectoryOnDisk("content/subfolder");
        File.WriteAllText(Path.Combine(contentFolder, "_index.md"), "content folder metadata");
        File.WriteAllText(Path.Combine(subFolder, "_index.md"), "subfolder metadata");

        // Act
        var parent = await GetByPathAsync("content");
        var child = await GetByPathAsync("content/subfolder");

        // Assert
        child.Parent.ShouldBe(parent);
        parent.Children.ShouldContain(child);
        parent.Name.ShouldBe("content");
        child.Name.ShouldBe("subfolder");
    }

    [Fact]
    public async Task GetAllAsync_WithIndexFile_ReadsMetadataFile()
    {
        // Arrange
        var fullPath = CreateDirectoryOnDisk("test/directory");
        var metadataPath = Path.Combine(fullPath, "_index.md");
        const string metadata = "---\ntitle: Test Directory\n---\n\nTest content";
        File.WriteAllText(metadataPath, metadata);

        // Act
        var directory = await GetByPathAsync("test/directory");

        // Assert
        _metadataProcessor.Received(1).ProcessMetadata(
            Arg.Is<DirectoryItem>(d => d.Path == "test/directory"),
            metadata);
        directory.ProviderSpecificId.ShouldBe(metadataPath);
    }

    private string CreateDirectoryOnDisk(string relativePath)
    {
        var fullPath = Path.Combine(_basePath, relativePath.Replace('/', Path.DirectorySeparatorChar));
        System.IO.Directory.CreateDirectory(fullPath);
        return fullPath;
    }

    private async Task<DirectoryItem> GetByPathAsync(string path)
    {
        var directory = await _repository.GetByPathAsync(path);
        directory.ShouldNotBeNull();
        return directory;
    }

    [Fact]
    public async Task GetAllAsync_ScannedDirectory_GetsTheStableIdForItsPathNameAndProvider()
    {
        CreateDirectoryOnDisk("content/blog");

        var directory = await GetByPathAsync("content/blog");

        directory.Id.ShouldBe(IdGenerator.GenerateDirectoryId("content/blog", "blog", directory.ProviderId));
    }
}
