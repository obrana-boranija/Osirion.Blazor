using Osirion.Blazor.Cms.Web.Components;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class ContentUrlsTests
{
    [Theory]
    [InlineData("Machine Learning", "machine-learning")]
    [InlineData("  spaced   out  ", "spaced-out")]
    [InlineData("C#", "c%23")]
    [InlineData("a/b", "a%2Fb")]
    [InlineData("ISTANBUL", "istanbul")]
    public void Slug_ProducesLowerCaseEncodedSegment(string value, string expected)
    {
        ContentUrls.Slug(value).ShouldBe(expected);
    }

    // Fails before the fix: ToLower() followed the current culture, and in Turkish "I" became a dotless "ı".
    [Fact]
    public void Slug_IgnoresTheCurrentCulture()
    {
        var original = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("tr-TR");

            ContentUrls.Slug("TITLE").ShouldBe("title");
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = original;
        }
    }

    [Fact]
    public void TagUrl_WithLocale_PrefixesTheLocaleAndStaysAbsolute()
    {
        ContentUrls.TagUrl("Machine Learning", "sr").ShouldBe("/sr/tag/machine-learning");
        ContentUrls.CategoryUrl("News", "de").ShouldBe("/de/category/news");
        ContentUrls.CategoryUrl("News").ShouldBe("/category/news");
    }

    [Fact]
    public void StableHash_IsTheFnv1aValue()
    {
        // Published FNV-1a 32-bit test vectors.
        ContentUrls.StableHash(string.Empty).ShouldBe(0x811C9DC5u);
        ContentUrls.StableHash("a").ShouldBe(0xE40C292Cu);
        ContentUrls.StableHash("foobar").ShouldBe(0xBF9CF968u);
    }

    [Theory]
    [InlineData("http://x/a?utm=1#f", "http://x/a")]
    [InlineData("http://x/a#f", "http://x/a")]
    [InlineData("http://x/a", "http://x/a")]
    public void StripQueryAndFragment_RemovesQueryAndFragment(string input, string expected)
    {
        ContentUrls.StripQueryAndFragment(input).ShouldBe(expected);
    }

    [Fact]
    public void WithQueryParameter_ReplacesOrRemovesOnlyThatParameter()
    {
        ContentUrls.WithQueryParameter("http://x/s?q=a%20b&page=2", "page", "3").ShouldBe("/s?q=a%20b&page=3");
        ContentUrls.WithQueryParameter("/s?q=a&page=2", "page", null).ShouldBe("/s?q=a");
        ContentUrls.WithQueryParameter("/s", "page", "2").ShouldBe("/s?page=2");
    }
}
