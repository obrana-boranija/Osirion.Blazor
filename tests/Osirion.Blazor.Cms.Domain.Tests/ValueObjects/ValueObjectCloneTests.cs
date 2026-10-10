using Osirion.Blazor.Cms.Domain.ValueObjects;
using Shouldly;

namespace Osirion.Blazor.Cms.Domain.Tests.ValueObjects;

/// <summary>
/// The admin edits metadata through the With* methods, which copy through Clone. A field Clone leaves out is
/// silently reset on the next edit and then written back to the repository.
/// </summary>
public class ValueObjectCloneTests
{
    [Fact]
    public void FrontMatter_WithTags_KeepsIdentityOrderPermalinkAndLanguage()
    {
        var original = new FrontMatter
        {
            Id = "post-translations",
            Order = 3,
            Permalink = "/de/beitrag",
            Lang = "de",
            Title = "Beitrag"
        };

        var edited = original.WithTags(["news"]);

        edited.Id.ShouldBe("post-translations");
        edited.Order.ShouldBe(3);
        edited.Permalink.ShouldBe("/de/beitrag");
        edited.Lang.ShouldBe("de");
        edited.Tags.ShouldBe(["news"]);
    }

    [Fact]
    public void FrontMatter_Clone_DoesNotShareSeoProperties()
    {
        var original = new FrontMatter { SeoProperties = new SeoMetadata { Title = "Original" } };

        var copy = original.Clone();
        copy.SeoProperties.Title = "Changed";

        original.SeoProperties.Title.ShouldBe("Original");
    }

    [Fact]
    public void SeoMetadata_WithMetaTitle_KeepsImageLanguageAndMetaTags()
    {
        var original = new SeoMetadata
        {
            Image = "/images/cover.png",
            Lang = "sr",
            MetaTags = new Dictionary<string, string> { ["robots-extra"] = "noarchive" }
        };

        var edited = original.WithMetaTitle("New title");

        edited.Image.ShouldBe("/images/cover.png");
        edited.Lang.ShouldBe("sr");
        edited.MetaTags.ShouldContainKeyAndValue("robots-extra", "noarchive");
    }

    [Fact]
    public void SeoMetadata_Clone_DoesNotShareMetaTags()
    {
        var original = new SeoMetadata { MetaTags = new Dictionary<string, string> { ["a"] = "1" } };

        var copy = original.Clone();
        copy.MetaTags["b"] = "2";

        original.MetaTags.ShouldNotContainKey("b");
    }
}
