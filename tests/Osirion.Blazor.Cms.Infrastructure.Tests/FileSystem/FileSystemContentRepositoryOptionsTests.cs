using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;
using Shouldly;
using System.IO.Abstractions.TestingHelpers;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.FileSystem;

/// <summary>
/// The options the content repository reads when it decides which files to load and how to address them.
/// </summary>
public sealed class FileSystemContentRepositoryOptionsTests : IDisposable
{
    private readonly MockFileSystem _fileSystem = new();
    private readonly IMarkdownProcessor _markdownProcessor = Substitute.For<IMarkdownProcessor>();
    private readonly List<IDisposable> _disposables = [];

    public FileSystemContentRepositoryOptionsTests()
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

    [Fact]
    public async Task GetAllAsync_FileMatchesAnExcludePattern_IsNotLoaded()
    {
        // Fails before the fix: ExcludePatterns was read by the directory repository only, so the content
        // repository loaded every file under BasePath.
        var basePath = MockUnixSupport.Path(@"c:\site\content");
        Write(basePath, "blog/post.md");
        Write(basePath, "drafts/idea.md");
        Write(basePath, "blog/wip.draft.md");
        var repository = Repository(basePath, options => options.ExcludePatterns = ["**/drafts/**", "**/*.draft.md"]);

        var items = await repository.GetAllAsync();

        items.Select(item => item.Path).ShouldBe(["blog/post.md"]);
    }

    [Fact]
    public async Task GetAllAsync_DefaultExcludePatterns_SkipTopLevelAndNestedNodeModules()
    {
        var basePath = MockUnixSupport.Path(@"c:\site\content");
        Write(basePath, "node_modules/pkg/readme.md");
        Write(basePath, "blog/node_modules/pkg/readme.md");
        Write(basePath, "blog/post.md");
        var repository = Repository(basePath);

        var items = await repository.GetAllAsync();

        items.Select(item => item.Path).ShouldBe(["blog/post.md"]);
    }

    [Fact]
    public async Task GetAllAsync_BasePathInsideAnExcludedFolderName_StillLoadsItsFiles()
    {
        // The patterns apply to the path under BasePath: content published into a bin folder must not vanish.
        var basePath = MockUnixSupport.Path(@"c:\site\bin\Release\content");
        Write(basePath, "blog/post.md");
        var repository = Repository(basePath);

        var items = await repository.GetAllAsync();

        items.Select(item => item.Path).ShouldBe(["blog/post.md"]);
    }

    [Fact]
    public async Task GetAllAsync_ContentPathWithoutContentRoot_StripsItFromTheUrlLikeContentRoot()
    {
        // Fails before the fix: FileSystemOptions.ContentPath was ignored, only ContentRoot took part in the URL.
        var basePath = MockUnixSupport.Path(@"c:\site");
        Write(basePath, "content/blog/post.md");

        var withoutRoot = await UrlOfAsync(basePath, _ => { });
        var withContentRoot = await UrlOfAsync(basePath, options => options.ContentRoot = "content");
        var withContentPath = await UrlOfAsync(basePath, options => options.ContentPath = "content");

        withContentRoot.ShouldNotBe(withoutRoot);
        withContentPath.ShouldBe(withContentRoot);
    }

    [Fact]
    public async Task GetAllAsync_ContentRootAndContentPathBothSet_ContentRootWins()
    {
        var basePath = MockUnixSupport.Path(@"c:\site");
        Write(basePath, "content/blog/post.md");

        var withContentRoot = await UrlOfAsync(basePath, options => options.ContentRoot = "content");
        var withBoth = await UrlOfAsync(basePath, options =>
        {
            options.ContentRoot = "content";
            options.ContentPath = "elsewhere";
        });

        withBoth.ShouldBe(withContentRoot);
    }

    private async Task<string?> UrlOfAsync(string basePath, Action<FileSystemOptions> configure)
    {
        var items = await Repository(basePath, configure).GetAllAsync();
        return items.ShouldHaveSingleItem().Url;
    }

    private FileSystemContentRepository Repository(string basePath, Action<FileSystemOptions>? configure = null)
    {
        var options = new FileSystemOptions
        {
            BasePath = basePath,
            WatchForChanges = false,
            SupportedExtensions = [".md"]
        };
        configure?.Invoke(options);

        var repository = new FileSystemContentRepository(
            _markdownProcessor,
            Substitute.For<IContentQueryFilter>(),
            Options.Create(options),
            NullLogger<FileSystemContentRepository>.Instance,
            _fileSystem);
        _disposables.Add(repository);
        return repository;
    }

    private void Write(string basePath, string relativePath) =>
        _fileSystem.AddFile(_fileSystem.Path.Combine(basePath, relativePath), new MockFileData("# Title"));
}
