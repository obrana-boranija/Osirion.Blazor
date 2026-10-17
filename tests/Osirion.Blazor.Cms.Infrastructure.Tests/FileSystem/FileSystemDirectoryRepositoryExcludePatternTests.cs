using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.Directory;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.FileSystem;

/// <summary>
/// <see cref="FileSystemOptions.ExcludePatterns"/> in <see cref="FileSystemDirectoryRepository"/>, with the real glob
/// matcher: the patterns match the path under <c>BasePath</c> with forward slashes, as the content repository does.
/// </summary>
public sealed class FileSystemDirectoryRepositoryExcludePatternTests : IDisposable
{
    private readonly string _sandbox = Path.Combine(Path.GetTempPath(), "osirion-fs-exclude-" + Guid.NewGuid().ToString("N"));
    private readonly IDirectoryCacheManager _cacheManager = Substitute.For<IDirectoryCacheManager>();
    private readonly IDirectoryMetadataProcessor _metadataProcessor = Substitute.For<IDirectoryMetadataProcessor>();
    private readonly List<FileSystemDirectoryRepository> _repositories = new();

    public FileSystemDirectoryRepositoryExcludePatternTests()
    {
        _cacheManager.GetCachedDirectoriesAsync(
                Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<bool>())
            .Returns(call => call.Arg<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>()(call.Arg<CancellationToken>()));
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

    private FileSystemDirectoryRepository Repository(string basePath, params string[] excludePatterns)
    {
        var options = Options.Create(new FileSystemOptions
        {
            BasePath = basePath,
            WatchForChanges = false,
            EnableLocalization = false,
            ExcludePatterns = excludePatterns.ToList(),
        });
        var repository = new FileSystemDirectoryRepository(
            options,
            _cacheManager,
            _metadataProcessor,
            new PathUtilities(options),
            NullLogger<FileSystemDirectoryRepository>.Instance);
        _repositories.Add(repository);
        return repository;
    }

    private static IEnumerable<string> Flatten(IEnumerable<DirectoryItem> directories) =>
        directories.SelectMany(directory => Flatten(directory.Children).Prepend(directory.Path));

    private static void CreateFolders(string basePath, params string[] relativePaths)
    {
        foreach (var relativePath in relativePaths)
        {
            System.IO.Directory.CreateDirectory(Path.Combine(basePath, relativePath.Replace('/', Path.DirectorySeparatorChar)));
        }
    }

    // Fails before the fix: the pattern was matched against the full path with backslashes on Windows, so
    // "**/drafts/**" never matched and the folder stayed in the tree.
    [Fact]
    public async Task GetAllAsync_PatternNamesAFolderAtAnyLevel_SkipsItOnEveryPlatform()
    {
        var basePath = Path.Combine(_sandbox, "content");
        CreateFolders(basePath, "docs/guide", "docs/drafts/wip", "drafts");
        var repository = Repository(basePath, "**/drafts/**");

        var paths = Flatten(await repository.GetAllAsync()).Order().ToList();

        paths.ShouldBe(new[] { "docs", "docs/guide" });
    }

    // Fails before the fix: the whole path was matched, so a content folder below a "bin" folder lost every directory.
    [Fact]
    public async Task GetAllAsync_BasePathInsideABinFolder_StillLoadsTheDirectories()
    {
        var basePath = Path.Combine(_sandbox, "bin", "Release", "content");
        CreateFolders(basePath, "docs/guide", "docs/bin");
        var repository = Repository(basePath, "**/bin/**", "**/obj/**");

        var paths = Flatten(await repository.GetAllAsync()).Order().ToList();

        paths.ShouldBe(new[] { "docs", "docs/guide" });
    }
}
