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
    private readonly IMarkdownProcessor _markdownProcessor = Substitute.For<IMarkdownProcessor>();
    private readonly FileSystemContentRepository _repository;

    public FileSystemContentRepositoryTests()
    {
        _fileSystem.Directory.CreateDirectory(ContentRoot);

        _markdownProcessor.ExtractFrontMatterAndContent(Arg.Any<string>())
            .Returns(call => ((FrontMatter?)null, call.Arg<string>()));
        _markdownProcessor.RenderToHtmlAsync(Arg.Any<string>(), Arg.Any<bool>())
            .Returns(call => call.Arg<string>());

        var options = Options.Create(new FileSystemOptions
        {
            BasePath = ContentRoot,
            WatchForChanges = false,
            CreateDirectoriesIfNotExist = true
        });

        _repository = new FileSystemContentRepository(
            _markdownProcessor,
            Substitute.For<IContentQueryFilter>(),
            options,
            NullLogger<FileSystemContentRepository>.Instance,
            _fileSystem);
    }

    [Fact]
    public async Task GetAllAsync_NoFrontMatterLastModified_UsesTheFileWriteTime()
    {
        var written = new DateTime(2026, 3, 4, 5, 6, 7, DateTimeKind.Utc);
        _fileSystem.AddFile(Path.Combine(ContentRoot, "post.md"), new MockFileData("Body") { LastWriteTime = written });

        var item = (await _repository.GetAllAsync()).ShouldHaveSingleItem();

        item.LastModified.ShouldBe(written);
    }

    [Fact]
    public async Task GetAllAsync_FrontMatterLastModified_WinsOverTheFileWriteTime()
    {
        _markdownProcessor.ExtractFrontMatterAndContent(Arg.Any<string>())
            .Returns(call => ((FrontMatter?)new FrontMatter { Title = "Post", LastModified = "2026-05-06" }, call.Arg<string>()));
        _fileSystem.AddFile(Path.Combine(ContentRoot, "post.md"), new MockFileData("Body") { LastWriteTime = new DateTime(2026, 3, 4, 0, 0, 0, DateTimeKind.Utc) });

        var item = (await _repository.GetAllAsync()).ShouldHaveSingleItem();

        item.LastModified.ShouldBe(new DateTime(2026, 5, 6, 0, 0, 0, DateTimeKind.Utc));
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

    private async Task<ContentItem> SeedAndLoadAsync(string relativePath)
    {
        _fileSystem.AddFile(Path.Combine(ContentRoot, relativePath), new MockFileData("# Post"));
        await _repository.RefreshCacheAsync();

        var items = await _repository.GetAllAsync();
        return items.ShouldHaveSingleItem();
    }
}
