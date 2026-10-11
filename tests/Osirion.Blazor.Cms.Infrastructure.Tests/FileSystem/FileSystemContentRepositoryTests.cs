using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Shouldly;
using System.IO.Abstractions.TestingHelpers;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.FileSystem;

public class FileSystemContentRepositoryTests
{
    private static readonly string ContentRoot = MockUnixSupport.Path(@"c:\site\content");
    private static readonly string OutsideFile = MockUnixSupport.Path(@"c:\site\appsettings.json");

    private readonly MockFileSystem _fileSystem = new();
    private readonly FileSystemContentRepository _repository;

    public FileSystemContentRepositoryTests()
    {
        _fileSystem.Directory.CreateDirectory(ContentRoot);

        var markdownProcessor = Substitute.For<IMarkdownProcessor>();
        markdownProcessor.ExtractFrontMatterAndContent(Arg.Any<string>())
            .Returns(call => ((FrontMatter?)null, call.Arg<string>()));
        markdownProcessor.RenderToHtmlAsync(Arg.Any<string>(), Arg.Any<bool>())
            .Returns(call => call.Arg<string>());

        var options = Options.Create(new FileSystemOptions
        {
            BasePath = ContentRoot,
            WatchForChanges = false,
            CreateDirectoriesIfNotExist = true
        });

        _repository = new FileSystemContentRepository(
            markdownProcessor,
            Substitute.For<IContentQueryFilter>(),
            options,
            NullLogger<FileSystemContentRepository>.Instance,
            _fileSystem);
    }

    [Theory]
    [InlineData("../outside.md")]
    [InlineData("blog/../../outside.md")]
    [InlineData("..\\outside.md")]
    public async Task SaveWithCommitMessageAsync_PathTraversingOutOfTheRoot_ThrowsAndWritesNothing(string path)
    {
        var item = ContentItem.Create("item-1", "Post", "Body", path, "filesystem");

        var exception = await Should.ThrowAsync<ContentProviderException>(
            () => _repository.SaveWithCommitMessageAsync(item, "save"));

        exception.InnerException.ShouldBeOfType<ContentValidationException>();
        _fileSystem.AllFiles.ShouldBeEmpty();
    }

    [Fact]
    public async Task SaveWithCommitMessageAsync_RootedPath_ThrowsAndWritesNothing()
    {
        var item = ContentItem.Create("item-1", "Post", "Body", OutsideFile, "filesystem");

        var exception = await Should.ThrowAsync<ContentProviderException>(
            () => _repository.SaveWithCommitMessageAsync(item, "save"));

        exception.InnerException.ShouldBeOfType<ContentValidationException>();
        _fileSystem.AllFiles.ShouldBeEmpty();
    }

    [Fact]
    public async Task SaveWithCommitMessageAsync_RelativePath_WritesTheFileUnderTheRoot()
    {
        var item = ContentItem.Create("item-1", "Post", "Body", "blog/post.md", "filesystem");
        var expectedPath = Path.Combine(ContentRoot, "blog", "post.md");

        var saved = await _repository.SaveWithCommitMessageAsync(item, "save");

        _fileSystem.File.Exists(expectedPath).ShouldBeTrue();
        saved.ProviderSpecificId.ShouldBe(expectedPath);
    }

    [Fact]
    public async Task DeleteWithCommitMessageAsync_ProviderSpecificIdRedirectedOutsideTheRoot_ThrowsAndKeepsTheTargetFile()
    {
        var item = await SeedAndLoadAsync("post.md");
        _fileSystem.AddFile(OutsideFile, new MockFileData("{ \"secret\": true }"));
        item.SetProviderSpecificId(OutsideFile);

        var exception = await Should.ThrowAsync<ContentProviderException>(
            () => _repository.DeleteWithCommitMessageAsync(item.Id, "delete"));

        exception.InnerException.ShouldBeOfType<ContentValidationException>();
        _fileSystem.File.Exists(OutsideFile).ShouldBeTrue();
    }

    [Theory]
    [InlineData("../appsettings.json")]
    [InlineData("blog/../../appsettings.json")]
    public async Task DeleteWithCommitMessageAsync_EmptyProviderSpecificIdAndPathOutsideTheRoot_ThrowsAndKeepsTheTargetFile(string path)
    {
        var item = await SeedAndLoadAsync("post.md");
        _fileSystem.AddFile(OutsideFile, new MockFileData("{ \"secret\": true }"));
        item.SetProviderSpecificId(null);
        item.Path = path;

        var exception = await Should.ThrowAsync<ContentProviderException>(
            () => _repository.DeleteWithCommitMessageAsync(item.Id, "delete"));

        exception.InnerException.ShouldBeOfType<ContentValidationException>();
        _fileSystem.File.Exists(OutsideFile).ShouldBeTrue();
    }

    [Fact]
    public async Task DeleteWithCommitMessageAsync_EmptyProviderSpecificIdAndRootedPath_ThrowsAndKeepsTheTargetFile()
    {
        var item = await SeedAndLoadAsync("post.md");
        _fileSystem.AddFile(OutsideFile, new MockFileData("{ \"secret\": true }"));
        item.SetProviderSpecificId(null);
        item.Path = OutsideFile;

        var exception = await Should.ThrowAsync<ContentProviderException>(
            () => _repository.DeleteWithCommitMessageAsync(item.Id, "delete"));

        exception.InnerException.ShouldBeOfType<ContentValidationException>();
        _fileSystem.File.Exists(OutsideFile).ShouldBeTrue();
    }

    [Fact]
    public async Task DeleteWithCommitMessageAsync_EmptyProviderSpecificIdAndPathInsideTheRoot_DeletesTheFile()
    {
        var item = await SeedAndLoadAsync("post.md");
        item.SetProviderSpecificId(null);
        item.Path = "post.md";

        await _repository.DeleteWithCommitMessageAsync(item.Id, "delete");

        _fileSystem.File.Exists(Path.Combine(ContentRoot, "post.md")).ShouldBeFalse();
    }

    [Fact]
    public async Task DeleteWithCommitMessageAsync_ProviderSpecificIdInsideTheRoot_DeletesTheFile()
    {
        var item = await SeedAndLoadAsync("post.md");

        await _repository.DeleteWithCommitMessageAsync(item.Id, "delete");

        _fileSystem.File.Exists(Path.Combine(ContentRoot, "post.md")).ShouldBeFalse();
    }

    // Fails before the fix: the file system provider skipped _index.md, so a section's overview page answered 404
    // locally while the GitHub provider served it.
    [Fact]
    public async Task RefreshCacheAsync_SectionIndexFile_LoadsAsThePageAtTheSectionUrl()
    {
        _fileSystem.AddFile(Path.Combine(ContentRoot, "docs", "core", "_index.md"), new MockFileData("# Core components"));
        _fileSystem.AddFile(Path.Combine(ContentRoot, "docs", "core", "cards.md"), new MockFileData("# Cards"));

        await _repository.RefreshCacheAsync();
        var items = await _repository.GetAllAsync();

        items.Count.ShouldBe(2);
        var index = items.Where(item => item.Path.EndsWith("_index.md", StringComparison.OrdinalIgnoreCase)).ShouldHaveSingleItem();
        index.Url.ShouldBe("docs/core");
        items.ShouldContain(item => item.Path.EndsWith("cards.md", StringComparison.OrdinalIgnoreCase) && item.Url != "docs/core");
    }

    private async Task<ContentItem> SeedAndLoadAsync(string relativePath)
    {
        _fileSystem.AddFile(Path.Combine(ContentRoot, relativePath), new MockFileData("# Post"));
        await _repository.RefreshCacheAsync();

        var items = await _repository.GetAllAsync();
        return items.ShouldHaveSingleItem();
    }
}
