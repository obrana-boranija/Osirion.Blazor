using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.Utilities;
using Shouldly;
using System.IO.Abstractions.TestingHelpers;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Utilities;

/// <summary>
/// Content, directory and provider ids used to come from <see cref="string.GetHashCode()"/>, which .NET randomizes per
/// process, so they changed on every restart and differed between server instances. The expected values below are
/// fixed: SHA-256 of the UTF-8 input, first 8 bytes, lowercase hex. A per-process hash cannot produce them.
/// </summary>
public class StableIdTests
{
    private const string ProviderId = "filesystem-test";

    [Fact]
    public void GenerateStableId_KnownPath_ReturnsTheSameValueInEveryProcess()
    {
        IdGenerator.GenerateStableId("blog/post.md").ShouldBe("6ebcef4ada82f784");
    }

    [Theory]
    [InlineData("content")]
    [InlineData("content/")]
    [InlineData("content\\")]
    public void FileSystemProviderId_BasePathWithAnySeparator_ReturnsTheSameStableId(string basePath)
    {
        FileSystemProviderId.For(new FileSystemOptions { BasePath = basePath }).ShouldBe("filesystem-ed7002b439e9ac84");
    }

    [Fact]
    public void FileSystemProviderId_EmptyBasePath_ReturnsAStableId()
    {
        FileSystemProviderId.For(new FileSystemOptions()).ShouldBe("filesystem-cdb4ee2aea69cc6a");
    }

    [Fact]
    public void FileSystemProviderId_ConfiguredProviderId_IsUsedAsIs()
    {
        FileSystemProviderId.For(new FileSystemOptions { BasePath = "content", ProviderId = "docs" }).ShouldBe("docs");
    }

    [Fact]
    public async Task FileSystemContentRepository_LoadedItems_GetStableIdsAndTranslationsShareTheirLocalizationId()
    {
        var root = MockUnixSupport.Path(@"c:\site\content");
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [Path.Combine(root, "en", "blog", "post.md")] = new("# Post"),
            [Path.Combine(root, "sr", "blog", "post.md")] = new("# Objava")
        });
        var repository = CreateRepository(fileSystem, root);

        var items = await repository.GetAllAsync();

        var english = items.Single(item => item.Locale == "en");
        var serbian = items.Single(item => item.Locale == "sr");
        english.Id.ShouldBe("69908b3181847e5e");
        serbian.Id.ShouldNotBe(english.Id);
        english.ContentId.ShouldBe("2633825eeabd7824");
        serbian.ContentId.ShouldBe(english.ContentId);
    }

    private static FileSystemContentRepository CreateRepository(MockFileSystem fileSystem, string root)
    {
        var markdownProcessor = Substitute.For<IMarkdownProcessor>();
        markdownProcessor.ExtractFrontMatterAndContent(Arg.Any<string>())
            .Returns(call => ((FrontMatter?)null, call.Arg<string>()));
        markdownProcessor.RenderToHtmlAsync(Arg.Any<string>(), Arg.Any<bool>())
            .Returns(call => call.Arg<string>());

        var options = Options.Create(new FileSystemOptions
        {
            BasePath = root,
            ProviderId = ProviderId,
            WatchForChanges = false,
            EnableLocalization = true,
            SupportedLocales = ["en", "sr"]
        });

        return new FileSystemContentRepository(
            markdownProcessor,
            Substitute.For<IContentQueryFilter>(),
            options,
            NullLogger<FileSystemContentRepository>.Instance,
            fileSystem);
    }
}
