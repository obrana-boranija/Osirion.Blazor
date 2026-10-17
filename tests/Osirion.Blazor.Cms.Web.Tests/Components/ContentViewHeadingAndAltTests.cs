using Bunit;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class ContentViewHeadingAndAltTests : CmsPageComponentTestBase
{
    [Fact]
    public void ContentView_DefaultHeadingLevel_IsAnH1()
    {
        var cut = RenderStatic<ContentView>(p => p.Add(c => c.Item, Doc("post")));

        cut.Find("h1.osirion-content-title").TextContent.ShouldBe("post");
    }

    // Fails before the fix: the title was always an h1, so a view embedded in a page added a second h1.
    [Theory]
    [InlineData(2, "h2")]
    [InlineData(3, "h3")]
    [InlineData(6, "h6")]
    public void ContentView_HeadingLevel_SetsTheTitleElement(int level, string tag)
    {
        var cut = RenderStatic<ContentView>(p => p
            .Add(c => c.Item, Doc("post"))
            .Add(c => c.HeadingLevel, level));

        cut.Find($"{tag}.osirion-content-title").TextContent.ShouldBe("post");
        cut.FindAll("h1").ShouldBeEmpty();
    }

    // Fails before the fix: a level outside 1 to 6 had no meaning; it now stays within the range.
    [Theory]
    [InlineData(0, "h1")]
    [InlineData(-4, "h1")]
    [InlineData(7, "h6")]
    public void ContentView_HeadingLevelOutsideTheRange_IsClamped(int level, string tag)
    {
        var cut = RenderStatic<ContentView>(p => p
            .Add(c => c.Item, Doc("post"))
            .Add(c => c.HeadingLevel, level));

        cut.FindAll($"{tag}.osirion-content-title").ShouldHaveSingleItem();
    }

    [Fact]
    public void LocalizedContentView_DefaultHeadingLevel_IsAnH1()
    {
        var cut = RenderStatic<LocalizedContentView>(p => p.Add(c => c.Item, Doc("post")));

        cut.Find(".osirion-content-header h1").TextContent.ShouldBe("post");
    }

    // Fails before the fix: the title was always an h1.
    [Theory]
    [InlineData(2, "h2")]
    [InlineData(4, "h4")]
    public void LocalizedContentView_HeadingLevel_SetsTheTitleElement(int level, string tag)
    {
        var cut = RenderStatic<LocalizedContentView>(p => p
            .Add(c => c.Item, Doc("post"))
            .Add(c => c.HeadingLevel, level));

        cut.Find($".osirion-content-header {tag}").TextContent.ShouldBe("post");
        cut.FindAll("h1").ShouldBeEmpty();
    }

    // Fails before the fix: the featured image used the title as alt text, which repeats the heading above it.
    [Fact]
    public void ContentView_FeaturedImageWithoutAltText_IsDecorative()
    {
        var item = Doc("post");
        item.SetFeaturedImage("/img/cover.png");

        var cut = RenderStatic<ContentView>(p => p.Add(c => c.Item, item));

        cut.Find("img.osirion-content-featured-image").GetAttribute("alt").ShouldBe(string.Empty);
    }

    // Fails before the fix: no front matter value reached the alt attribute.
    [Fact]
    public void ContentView_FeaturedImageAltFromFrontMatter_IsUsed()
    {
        var item = WithImageAlt(Doc("post"), "A bridge at dusk");

        var cut = RenderStatic<ContentView>(p => p.Add(c => c.Item, item));

        cut.Find("img.osirion-content-featured-image").GetAttribute("alt").ShouldBe("A bridge at dusk");
    }

    // Fails before the fix: the featured image used the title as alt text.
    [Fact]
    public void LocalizedContentView_FeaturedImageWithoutAltText_IsDecorative()
    {
        var item = Doc("post");
        item.SetFeaturedImage("/img/cover.png");

        var cut = RenderStatic<LocalizedContentView>(p => p.Add(c => c.Item, item));

        cut.Find("img.osirion-content-featured-image").GetAttribute("alt").ShouldBe(string.Empty);
    }

    // Fails before the fix: no front matter value reached the alt attribute.
    [Fact]
    public void LocalizedContentView_FeaturedImageAltFromFrontMatter_IsUsed()
    {
        var item = WithImageAlt(Doc("post"), "A bridge at dusk");

        var cut = RenderStatic<LocalizedContentView>(p => p.Add(c => c.Item, item));

        cut.Find("img.osirion-content-featured-image").GetAttribute("alt").ShouldBe("A bridge at dusk");
    }

    // Fails before the fix: the image of a related item used the item's title as alt text, next to the same title link.
    [Fact]
    public void ContentPage_RelatedItemImage_UsesTheFrontMatterAltOrNone()
    {
        var plain = Doc("plain");
        plain.SetFeaturedImage("/img/plain.png");
        var described = WithImageAlt(Doc("described"), "A lighthouse");
        var main = Doc("main");
        main.AddTag("a");
        plain.AddTag("a");
        described.AddTag("a");
        Provider.GetItemByUrlAsync("docs/main").Returns(main);
        Provider.GetItemsByQueryAsync(Arg.Any<Osirion.Blazor.Cms.Domain.Repositories.ContentQuery>())
            .Returns(Task.FromResult<IReadOnlyList<ContentItem>?>([main, plain, described]));

        var cut = RenderStatic<ContentPage>(p => p
            .Add(c => c.Path, "docs/main")
            .Add(c => c.ShowRelatedContent, true));

        cut.FindAll("img.osirion-content-related-image").Select(i => i.GetAttribute("alt")).ShouldBe([string.Empty, "A lighthouse"], ignoreOrder: true);
    }

    private static ContentItem WithImageAlt(ContentItem item, string alt)
    {
        item.SetFeaturedImage("/img/cover.png");
        item.Metadata = FrontMatter.Create(item.Title).WithCustomField("featured_image_alt", alt);
        return item;
    }
}
