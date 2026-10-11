using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Shouldly;
using System.IO.Abstractions.TestingHelpers;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.FileSystem;

/// <summary>
/// Loading, linking and error paths of <see cref="FileSystemContentRepository"/> against an in-memory file system.
/// </summary>
public sealed class FileSystemContentRepositoryLoadingTests : IDisposable
{
    private static readonly string ContentRoot = MockUnixSupport.Path(@"c:\site\content");

    private readonly MockFileSystem _fileSystem = new();
    private readonly IMarkdownProcessor _markdownProcessor = Substitute.For<IMarkdownProcessor>();
    private readonly List<IDisposable> _disposables = new();

    public FileSystemContentRepositoryLoadingTests()
    {
        _markdownProcessor.ExtractFrontMatterAndContent(Arg.Any<string>())
            .Returns(call => ((FrontMatter?)null, call.Arg<string>()));
        _markdownProcessor.RenderToHtmlAsync(Arg.Any<string>(), Arg.Any<bool>())
            .Returns(call => call.Arg<string>());
    }

    public void Dispose()
    {
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }
    }

    private static FileSystemOptions CreateOptions(Action<FileSystemOptions>? configure = null)
    {
        var options = new FileSystemOptions
        {
            BasePath = ContentRoot,
            WatchForChanges = false,
            CreateDirectoriesIfNotExist = true,
            SupportedExtensions = new List<string> { ".md" },
        };
        configure?.Invoke(options);
        return options;
    }

    private FileSystemContentRepository Repository(FileSystemOptions options, FileSystemDirectoryRepository? directoryRepository = null)
    {
        var repository = directoryRepository is null
            ? new FileSystemContentRepository(
                _markdownProcessor,
                Substitute.For<IContentQueryFilter>(),
                Options.Create(options),
                NullLogger<FileSystemContentRepository>.Instance,
                _fileSystem)
            : new FileSystemContentRepository(
                _markdownProcessor,
                Substitute.For<IContentQueryFilter>(),
                Options.Create(options),
                NullLogger<FileSystemContentRepository>.Instance,
                _fileSystem,
                directoryRepository);
        _disposables.Add(repository);
        return repository;
    }

    private FileSystemDirectoryRepository DirectoryRepository(FileSystemOptions options, IDirectoryCacheManager cacheManager)
    {
        var pathUtilities = Substitute.For<IPathUtilities>();
        pathUtilities.NormalizePath(Arg.Any<string>()).Returns(call => call.Arg<string>());

        var repository = new FileSystemDirectoryRepository(
            Options.Create(options),
            cacheManager,
            Substitute.For<IDirectoryMetadataProcessor>(),
            pathUtilities,
            NullLogger<FileSystemDirectoryRepository>.Instance);
        _disposables.Add(repository);
        return repository;
    }

    private static IDirectoryCacheManager CacheReturning(params DirectoryItem[] directories)
    {
        var cacheManager = Substitute.For<IDirectoryCacheManager>();
        cacheManager.GetCachedDirectoriesAsync(
                Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<bool>())
            .Returns(_ => directories.ToDictionary(directory => directory.Id));
        return cacheManager;
    }

    private void Write(string relativePath, string content) =>
        _fileSystem.AddFile(_fileSystem.Path.Combine(ContentRoot, relativePath), new MockFileData(content));

    [Fact]
    public async Task GetAllAsync_BasePathMissingAndCreationAllowed_CreatesItAndLoadsNothing()
    {
        var repository = Repository(CreateOptions());

        var items = await repository.GetAllAsync();

        items.ShouldBeEmpty();
        _fileSystem.Directory.Exists(ContentRoot).ShouldBeTrue();
        repository.LastRefreshSucceeded.ShouldBeTrue();
    }

    [Fact]
    public async Task GetAllAsync_BasePathMissingAndCreationNotAllowed_FailsTheLoad()
    {
        var repository = Repository(CreateOptions(options => options.CreateDirectoriesIfNotExist = false));

        var items = await repository.GetAllAsync();

        items.ShouldBeEmpty();
        _fileSystem.Directory.Exists(ContentRoot).ShouldBeFalse();
        repository.LastRefreshSucceeded.ShouldBeFalse();
    }

    [Fact]
    public async Task GetAllAsync_IndexFilesAndEmptyFiles_AreNotLoaded()
    {
        Write("blog/_index.md", "# Blog");
        Write("blog/empty.md", "   ");
        Write("blog/post.md", "# Post");
        var repository = Repository(CreateOptions());

        var items = await repository.GetAllAsync();

        items.ShouldHaveSingleItem().Path.ShouldBe("blog/post.md");
    }

    [Fact]
    public async Task GetAllAsync_SubdirectoriesExcluded_LoadsOnlyTopLevelFiles()
    {
        Write("top.md", "# Top");
        Write("blog/post.md", "# Post");
        var repository = Repository(CreateOptions(options => options.IncludeSubdirectories = false));

        var items = await repository.GetAllAsync();

        items.ShouldHaveSingleItem().Path.ShouldBe("top.md");
    }

    [Fact]
    public async Task GetAllAsync_OneFileFailsToProcess_LoadsTheOthers()
    {
        Write("good.md", "good");
        Write("bad.md", "bad");
        _markdownProcessor.ExtractFrontMatterAndContent("bad").Throws(new FormatException("bad yaml"));
        var repository = Repository(CreateOptions());

        var items = await repository.GetAllAsync();

        items.ShouldHaveSingleItem().Path.ShouldBe("good.md");
    }

    [Fact]
    public async Task GetAllAsync_WithADirectoryRepository_LinksItemsToTheirDirectory()
    {
        var options = CreateOptions();
        var blog = DirectoryItem.Create("blog-id", "blog", "Blog", "filesystem");
        Write("blog/post.md", "# Post");
        Write("top.md", "# Top");
        Write("other/note.md", "# Note");
        var repository = Repository(options, DirectoryRepository(options, CacheReturning(blog)));

        var items = await repository.GetAllAsync();

        items.Single(item => item.Path == "blog/post.md").Directory.ShouldBeSameAs(blog);
        items.Single(item => item.Path == "top.md").Directory.ShouldBeNull();
        items.Single(item => item.Path == "other/note.md").Directory.ShouldBeNull();
    }

    [Fact]
    public async Task GetAllAsync_DirectoryLookupFails_StillLoadsTheItemWithoutADirectory()
    {
        var options = CreateOptions();
        var cacheManager = Substitute.For<IDirectoryCacheManager>();
        cacheManager.GetCachedDirectoriesAsync(
                Arg.Any<Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<bool>())
            .ThrowsAsync(new InvalidOperationException("directory scan failed"));
        Write("blog/post.md", "# Post");
        var repository = Repository(options, DirectoryRepository(options, cacheManager));

        var items = await repository.GetAllAsync();

        items.ShouldHaveSingleItem().Directory.ShouldBeNull();
    }

    [Fact]
    public void Constructor_NullDirectoryRepository_Throws()
    {
        Should.Throw<ArgumentNullException>(() => new FileSystemContentRepository(
            _markdownProcessor,
            Substitute.For<IContentQueryFilter>(),
            Options.Create(CreateOptions()),
            NullLogger<FileSystemContentRepository>.Instance,
            _fileSystem,
            null!));
    }

    [Fact]
    public async Task SaveWithCommitMessageAsync_NullEntity_Throws()
    {
        var repository = Repository(CreateOptions());

        await Should.ThrowAsync<ArgumentNullException>(() => repository.SaveWithCommitMessageAsync(null!, "msg"));
    }

    [Fact]
    public async Task SaveWithCommitMessageAsync_EmptyPath_Throws()
    {
        var repository = Repository(CreateOptions());
        var item = new ContentItem { Title = "Untitled" };

        await Should.ThrowAsync<ArgumentException>(() => repository.SaveWithCommitMessageAsync(item, "msg"));
    }

    [Fact]
    public async Task SaveWithCommitMessageAsync_FolderMissingAndCreationNotAllowed_ThrowsAndWritesNothing()
    {
        _fileSystem.Directory.CreateDirectory(ContentRoot);
        var repository = Repository(CreateOptions(options => options.CreateDirectoriesIfNotExist = false));
        var item = ContentItem.Create("1", "Post", "Body", "blog/post.md", "filesystem");

        var exception = await Should.ThrowAsync<ContentProviderException>(() => repository.SaveWithCommitMessageAsync(item, "msg"));

        exception.InnerException.ShouldBeOfType<Osirion.Blazor.Cms.Domain.Exceptions.DirectoryNotFoundException>();
        _fileSystem.Directory.Exists(_fileSystem.Path.Combine(ContentRoot, "blog")).ShouldBeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task DeleteWithCommitMessageAsync_BlankId_Throws(string id)
    {
        var repository = Repository(CreateOptions());

        await Should.ThrowAsync<ArgumentException>(() => repository.DeleteWithCommitMessageAsync(id, "msg"));
    }

    [Fact]
    public async Task DeleteWithCommitMessageAsync_UnknownId_ThrowsContentItemNotFound()
    {
        _fileSystem.Directory.CreateDirectory(ContentRoot);
        var repository = Repository(CreateOptions());

        await Should.ThrowAsync<ContentItemNotFoundException>(() => repository.DeleteWithCommitMessageAsync("missing", "msg"));
    }

    [Fact]
    public async Task DeleteWithCommitMessageAsync_FileRemovedSinceTheLoad_ThrowsContentProviderException()
    {
        Write("post.md", "# Post");
        var repository = Repository(CreateOptions());
        var item = (await repository.GetAllAsync()).Single();
        _fileSystem.File.Delete(_fileSystem.Path.Combine(ContentRoot, "post.md"));

        var exception = await Should.ThrowAsync<ContentProviderException>(() => repository.DeleteWithCommitMessageAsync(item.Id, "msg"));

        exception.InnerException.ShouldBeOfType<ContentProviderException>().Message.ShouldContain("Content file not found");
    }

    [Fact]
    public void Constructor_WatchEnabledAndBasePathMissingWithoutCreation_DoesNotCreateIt()
    {
        Repository(CreateOptions(options =>
        {
            options.WatchForChanges = true;
            options.CreateDirectoriesIfNotExist = false;
        }));

        _fileSystem.Directory.Exists(ContentRoot).ShouldBeFalse();
    }

    [Fact]
    public async Task Constructor_WatchEnabledAndBasePathMissingWithCreation_CreatesItAndStillLoads()
    {
        var repository = Repository(CreateOptions(options => options.WatchForChanges = true));

        _fileSystem.Directory.Exists(ContentRoot).ShouldBeTrue();
        (await repository.GetAllAsync()).ShouldBeEmpty();
        repository.LastRefreshSucceeded.ShouldBeTrue();
    }

    // The watcher itself needs a folder that exists on disk; the content is still read from the in-memory file system.
    [Fact]
    public async Task Constructor_WatchEnabledOnAnExistingFolder_SetsUpTheWatcherAndLoads()
    {
        var sandbox = System.IO.Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "osirion-fs-watch-" + Guid.NewGuid().ToString("N"))).FullName;
        try
        {
            _fileSystem.AddFile(Path.Combine(sandbox, "post.md"), new MockFileData("# Post"));
            var repository = Repository(CreateOptions(options =>
            {
                options.BasePath = sandbox;
                options.WatchForChanges = true;
            }));

            (await repository.GetAllAsync()).ShouldHaveSingleItem().Path.ShouldBe("post.md");
            repository.Dispose();
        }
        finally
        {
            System.IO.Directory.Delete(sandbox, recursive: true);
        }
    }
}
