using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Shouldly;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.FileSystem;

public class FileSystemDirectoryRepositoryTests
{
    private readonly MockFileSystem _mockFileSystem;
    private readonly IDirectoryMetadataProcessor _metadataProcessor;
    private readonly IDirectoryCacheManager _cacheManager;
    private readonly IPathUtilities _pathUtils;
    private readonly IOptions<FileSystemOptions> _options;
    private readonly ILogger<FileSystemDirectoryRepository> _logger;
    private readonly FileSystemDirectoryRepository _repository;
    private readonly string _basePath = "/test/content";

    public FileSystemDirectoryRepositoryTests()
    {
        // Setup mocks
        _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
        _mockFileSystem.Directory.CreateDirectory(_basePath);

        _metadataProcessor = Substitute.For<IDirectoryMetadataProcessor>();
        _cacheManager = Substitute.For<IDirectoryCacheManager>();
        _pathUtils = Substitute.For<IPathUtilities>();
        _logger = Substitute.For<ILogger<FileSystemDirectoryRepository>>();

        // Configure options
        var options = new FileSystemOptions
        {
            BasePath = _basePath,
            ContentRoot = "content",
            CreateDirectoriesIfNotExist = true,
            EnableLocalization = false,
            DefaultLocale = "en",
            SupportedExtensions = new List<string> { ".md", ".markdown" },
            CacheDurationMinutes = 30
        };
        _options = Options.Create(options);

        // Configure path utilities
        _pathUtils.NormalizePath(Arg.Any<string>()).Returns(x => x.Arg<string>());
        _pathUtils.ExtractLocaleFromPath(Arg.Any<string>()).Returns("en");
        _pathUtils.GenerateDirectoryUrl(Arg.Any<string>()).Returns(x => {
            var path = x.Arg<string>();
            return Path.GetFileName(path);
        });

        // Create repository
        _repository = new FileSystemDirectoryRepository(
            _options,
            _cacheManager,
            _metadataProcessor,
            _pathUtils,
            _logger);
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
        result.ShouldNotBeNull();
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
        var directory = DirectoryItem.Create(
            "test-dir",
            "test/path",
            "Test Directory",
            "filesystem");

        _metadataProcessor.GenerateMetadataContent(directory)
            .Returns("---\ntitle: Test Directory\n---\n\nTest content");

        // Act
        var result = await _repository.SaveAsync(directory);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe("test-dir");

        // Verify directory was created
        var fullPath = Path.Combine(_basePath, "test/path");
        _mockFileSystem.Directory.Exists(fullPath).ShouldBeTrue("Directory should be created");

        // Verify metadata file was created
        var metadataPath = Path.Combine(fullPath, "_index.md");
        _mockFileSystem.File.Exists(metadataPath).ShouldBeTrue("Metadata file should be created");

        // Verify placeholder file was created
        var placeholderPath = Path.Combine(fullPath, ".gitkeep");
        _mockFileSystem.File.Exists(placeholderPath).ShouldBeTrue("Placeholder file should be created");

        // Verify cache was refreshed
        await _cacheManager.Received(1).InvalidateCacheAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_RemovesDirectoryAndFiles()
    {
        // Arrange
        var directoryId = "test-dir";
        var directoryPath = "test/path";
        var fullPath = Path.Combine(_basePath, directoryPath);

        // Setup directory structure
        _mockFileSystem.Directory.CreateDirectory(fullPath);
        _mockFileSystem.File.WriteAllText(Path.Combine(fullPath, "_index.md"), "metadata");
        _mockFileSystem.File.WriteAllText(Path.Combine(fullPath, "file.md"), "content");

        var directory = DirectoryItem.Create(
            directoryId,
            directoryPath,
            "Test Directory",
            "filesystem");

        // Setup to return our directory when requested by ID
        var directories = new Dictionary<string, DirectoryItem>
        {
            { directoryId, directory }
        };

        _cacheManager.GetCachedDirectoriesAsync(
                Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<bool>())
            .Returns(directories);

        // Act
        await _repository.DeleteAsync(directoryId);

        // Assert
        _mockFileSystem.Directory.Exists(fullPath).ShouldBeFalse("Directory should be removed");

        // Verify cache was refreshed
        await _cacheManager.Received(1).InvalidateCacheAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteRecursiveAsync_RemovesDirectoryAndSubdirectories()
    {
        // Arrange
        var directoryId = "parent-dir";
        var directoryPath = "parent/path";
        var fullPath = Path.Combine(_basePath, directoryPath);
        var childPath = Path.Combine(fullPath, "child");

        // Setup directory structure
        _mockFileSystem.Directory.CreateDirectory(fullPath);
        _mockFileSystem.Directory.CreateDirectory(childPath);
        _mockFileSystem.File.WriteAllText(Path.Combine(fullPath, "_index.md"), "parent metadata");
        _mockFileSystem.File.WriteAllText(Path.Combine(childPath, "_index.md"), "child metadata");

        var parentDir = DirectoryItem.Create(
            directoryId,
            directoryPath,
            "Parent Directory",
            "filesystem");

        var childDir = DirectoryItem.Create(
            "child-dir",
            Path.Combine(directoryPath, "child").Replace('\\', '/'),
            "Child Directory",
            "filesystem");

        parentDir.AddChild(childDir);

        // Setup to return our directory when requested by ID
        var directories = new Dictionary<string, DirectoryItem>
        {
            { directoryId, parentDir },
            { "child-dir", childDir }
        };

        _cacheManager.GetCachedDirectoriesAsync(
                Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<bool>())
            .Returns(directories);

        // Act
        await _repository.DeleteRecursiveAsync(directoryId);

        // Assert
        _mockFileSystem.Directory.Exists(fullPath).ShouldBeFalse("Parent directory should be removed");
        _mockFileSystem.Directory.Exists(childPath).ShouldBeFalse("Child directory should be removed");

        // Verify cache was refreshed
        await _cacheManager.Received(1).InvalidateCacheAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MoveAsync_MovesDirectoryToNewParent()
    {
        // Arrange
        var directoryId = "dir-to-move";
        var sourcePath = "source/path";
        var fullSourcePath = Path.Combine(_basePath, sourcePath);

        var targetParentId = "target-parent";
        var targetParentPath = "target/parent";
        var fullTargetParentPath = Path.Combine(_basePath, targetParentPath);

        // Setup directory structure
        _mockFileSystem.Directory.CreateDirectory(fullSourcePath);
        _mockFileSystem.Directory.CreateDirectory(fullTargetParentPath);
        _mockFileSystem.File.WriteAllText(Path.Combine(fullSourcePath, "_index.md"), "source metadata");
        _mockFileSystem.File.WriteAllText(Path.Combine(fullTargetParentPath, "_index.md"), "target metadata");

        var sourceDir = DirectoryItem.Create(
            directoryId,
            sourcePath,
            "Source Directory",
            "filesystem");

        var targetParentDir = DirectoryItem.Create(
            targetParentId,
            targetParentPath,
            "Target Parent",
            "filesystem");

        // Setup to return our directories when requested by ID
        var directories = new Dictionary<string, DirectoryItem>
        {
            { directoryId, sourceDir },
            { targetParentId, targetParentDir }
        };

        _cacheManager.GetCachedDirectoriesAsync(
                Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<bool>())
            .Returns(directories);

        // Setup metadata processor
        _metadataProcessor.GenerateMetadataContent(Arg.Any<DirectoryItem>())
            .Returns("---\ntitle: Source Directory\n---\n\nUpdated content");

        // Act
        var result = await _repository.MoveAsync(directoryId, targetParentId);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(directoryId);
        result.Parent.ShouldNotBeNull();
        result.Parent.Id.ShouldBe(targetParentId);

        // Verify original directory was removed
        _mockFileSystem.Directory.Exists(fullSourcePath).ShouldBeFalse("Source directory should be removed");

        // Verify new directory was created
        var expectedNewPath = Path.Combine(fullTargetParentPath, Path.GetFileName(sourcePath));
        _mockFileSystem.Directory.Exists(expectedNewPath).ShouldBeTrue("New directory should be created");

        // Verify metadata file was created in new location
        var metadataPath = Path.Combine(expectedNewPath, "_index.md");
        _mockFileSystem.File.Exists(metadataPath).ShouldBeTrue("Metadata file should be created in new location");

        // Verify cache was refreshed
        await _cacheManager.Received(1).InvalidateCacheAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task FindDirectories_ScansFolderStructure()
    {
        // Arrange - Setup method via reflection since it's private
        var findDirectoriesMethod = typeof(FileSystemDirectoryRepository)
            .GetMethod("FindDirectoriesAsync",
                       BindingFlags.NonPublic | BindingFlags.Instance);

        // Setup directory structure
        var contentFolder = Path.Combine(_basePath, "content");
        var subFolder = Path.Combine(contentFolder, "subfolder");

        _mockFileSystem.Directory.CreateDirectory(contentFolder);
        _mockFileSystem.Directory.CreateDirectory(subFolder);
        _mockFileSystem.File.WriteAllText(Path.Combine(contentFolder, "_index.md"), "content folder metadata");
        _mockFileSystem.File.WriteAllText(Path.Combine(subFolder, "_index.md"), "subfolder metadata");

        // Setup metadata processor
        _metadataProcessor.ProcessMetadata(Arg.Any<DirectoryItem>(), Arg.Any<string>())
            .Returns(callInfo => callInfo.Arg<DirectoryItem>());

        // Act
        var result = await (Task<Dictionary<string, DirectoryItem>>)findDirectoriesMethod.Invoke(
            _repository,
            new object[] { "content", null, CancellationToken.None });

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2); // Content folder + subfolder

        result.Values.ShouldContain(d => d.Path == "content");
        result.Values.ShouldContain(d => d.Path == "content/subfolder");

        // Verify parent-child relationship
        var parentDir = result.Values.FirstOrDefault(d => d.Path == "content");
        var childDir = result.Values.FirstOrDefault(d => d.Path == "content/subfolder");

        parentDir.ShouldNotBeNull();
        childDir.ShouldNotBeNull();
        childDir.Parent.ShouldBe(parentDir);
        parentDir.Children.ShouldContain(childDir);
    }

    [Fact]
    public async Task ProcessDirectoryMetadata_ReadsMetadataFile()
    {
        // Arrange - Setup method via reflection since it's private
        var processMetadataMethod = typeof(FileSystemDirectoryRepository)
            .GetMethod("ProcessDirectoryMetadataAsync",
                       BindingFlags.NonPublic | BindingFlags.Instance);

        // Setup directory and metadata file
        var directoryPath = "test/directory";
        var fullPath = Path.Combine(_basePath, directoryPath);
        var metadataPath = Path.Combine(fullPath, "_index.md");

        _mockFileSystem.Directory.CreateDirectory(fullPath);
        _mockFileSystem.File.WriteAllText(metadataPath, "---\ntitle: Test Directory\n---\n\nTest content");

        var directory = DirectoryItem.Create(
            "test-dir",
            directoryPath,
            "Directory Name",
            "filesystem");

        // Setup metadata processor
        _metadataProcessor.ProcessMetadata(directory, "---\ntitle: Test Directory\n---\n\nTest content")
            .Returns(directory);

        // Act
        await (Task)processMetadataMethod.Invoke(
            _repository,
            new object[] { directory, CancellationToken.None });

        // Assert
        _metadataProcessor.Received(1).ProcessMetadata(
            directory,
            "---\ntitle: Test Directory\n---\n\nTest content");
    }
}