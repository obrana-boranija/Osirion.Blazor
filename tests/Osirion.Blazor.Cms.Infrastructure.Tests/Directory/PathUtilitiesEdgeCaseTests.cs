using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.Directory;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Directory;

public class PathUtilitiesEdgeCaseTests
{
    private static PathUtilities Create(bool enableLocalization, params string[] supportedLocales) =>
        new(Options.Create(new FileSystemOptions
        {
            ContentRoot = "content",
            EnableLocalization = enableLocalization,
            DefaultLocale = "en",
            SupportedLocales = supportedLocales.ToList()
        }));

    [Fact]
    public void Constructor_NullOptions_Throws()
    {
        Should.Throw<ArgumentNullException>(() => new PathUtilities(null!));
    }

    [Fact]
    public void RemoveLocaleFromPath_LocalizationDisabled_ReturnsThePathUnchanged()
    {
        var utilities = Create(enableLocalization: false, "en");

        utilities.RemoveLocaleFromPath("content/en/blog").ShouldBe("content/en/blog");
    }

    [Fact]
    public void RemoveLocaleFromPath_FirstSegmentIsNotALocale_ReturnsThePathWithoutTheContentRoot()
    {
        var utilities = Create(enableLocalization: true, "en");

        utilities.RemoveLocaleFromPath("content/blog/post").ShouldBe("blog/post");
    }

    [Fact]
    public void RemoveLocaleFromPath_PathIsTheContentRoot_ReturnsEmpty()
    {
        var utilities = Create(enableLocalization: true, "en");

        utilities.RemoveLocaleFromPath("content").ShouldBeEmpty();
    }

    [Fact]
    public void RemoveLocaleFromPath_PathOnlySharesThePrefixWithTheContentRoot_KeepsIt()
    {
        var utilities = Create(enableLocalization: true, "en");

        utilities.RemoveLocaleFromPath("contents/en").ShouldBe("contents/en");
    }

    [Fact]
    public void GenerateContentUrl_FileAtTheTop_ReturnsOnlyTheSlug()
    {
        var utilities = Create(enableLocalization: false, "en");

        utilities.GenerateContentUrl("post.md", "my-post").ShouldBe("my-post");
    }

    [Fact]
    public void GenerateContentUrl_LocalizedFileAtTheTop_ReturnsOnlyTheSlug()
    {
        var utilities = Create(enableLocalization: true, "en");

        utilities.GenerateContentUrl("content/en/post.md", "my-post").ShouldBe("my-post");
    }

    [Fact]
    public void GenerateDirectoryUrl_LocalizationDisabled_KeepsTheLocaleSegment()
    {
        var utilities = Create(enableLocalization: false, "en");

        utilities.GenerateDirectoryUrl("content\\en\\blog\\").ShouldBe("en/blog");
    }

    [Theory]
    [InlineData("fr/blog", "fr")]
    [InlineData("pt-BR/blog", "pt-BR")]
    [InlineData("blog/post", "en")]
    [InlineData("f1/blog", "en")]
    [InlineData("pt_BR/blog", "en")]
    [InlineData("p1-BR/blog", "en")]
    [InlineData("pt-B1/blog", "en")]
    public void ExtractLocaleFromPath_NoSupportedLocalesConfigured_AcceptsLanguageAndRegionCodes(string path, string expected)
    {
        var utilities = Create(enableLocalization: true);

        utilities.ExtractLocaleFromPath(path).ShouldBe(expected);
    }
}
