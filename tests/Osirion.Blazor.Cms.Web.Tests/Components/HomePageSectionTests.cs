using Bunit;
using NSubstitute;
using Osirion.Blazor.Cms.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class HomePageSectionTests : CmsPageComponentTestBase
{
    // Fails before the fix: the featured section loaded the newest items of any locale, featured or not (the query
    // had no IsFeatured and no Locale).
    [Fact]
    public void HomePage_FeaturedSection_AsksForFeaturedItemsOfTheLocale()
    {
        ReturnFor(q => q.IsFeatured == true, Doc("star"));

        var cut = RenderStatic<HomePage>(p => p
            .Add(c => c.Content, Doc("home"))
            .Add(c => c.Locale, "de")
            .Add(c => c.FeaturedCount, 2)
            .Add(c => c.ShowRecentContent, false));

        Manager.Received(1).GetContentByQueryAsync(
            Arg.Is<ContentQuery>(q => q.IsFeatured == true && q.Locale == "de" && q.Take == 2 && q.SortBy == SortField.Date),
            Arg.Any<CancellationToken>());
        cut.Find(".osirion-featured-post-title").TextContent.Trim().ShouldBe("star");
    }

    // Without featured items the section would render the list's "not found" block under the home page's heading.
    [Fact]
    public void HomePage_NoFeaturedItems_RendersNoFeaturedSection()
    {
        var cut = RenderStatic<HomePage>(p => p
            .Add(c => c.Content, Doc("home"))
            .Add(c => c.ShowRecentContent, false));

        cut.FindAll(".osirion-featured-posts-section").ShouldBeEmpty();
        cut.FindAll(".osirion-content-not-found").ShouldBeEmpty();
    }

    // Fails before the fix: the recent section never rendered because nothing set its items, so these parameters did
    // nothing.
    [Fact]
    public void HomePage_RecentSection_RendersTheNewestItemsWithItsTitleAndReadMoreText()
    {
        ReturnFor(q => q.IsFeatured != true, Doc("one"), Doc("two"));

        var cut = RenderStatic<HomePage>(p => p
            .Add(c => c.Content, Doc("home"))
            .Add(c => c.ShowFeaturedContent, false)
            .Add(c => c.RecentSectionTitle, "Latest")
            .Add(c => c.RecentCount, 2)
            .Add(c => c.ReadMoreText, "Keep reading")
            .Add(c => c.ContentUrlFormatter, item => "/read/" + item.Title));

        Manager.Received(1).GetContentByQueryAsync(
            Arg.Is<ContentQuery>(q => q.IsFeatured == null && q.Locale == "en" && q.Take == 2 && q.SortBy == SortField.Date
                && q.SortDirection == Osirion.Blazor.Cms.Domain.Enums.SortDirection.Descending),
            Arg.Any<CancellationToken>());
        cut.Find(".osirion-content-section-title").TextContent.ShouldBe("Latest");
        cut.FindAll(".osirion-featured-post-title").Select(t => t.TextContent.Trim()).ShouldBe(["one", "two"]);
        cut.FindAll("a.osirion-featured-post-read-more").Select(a => a.TextContent.Trim()).ShouldAllBe(text => text == "Keep reading");
        cut.FindAll("a.osirion-featured-post-read-more").Select(a => a.GetAttribute("href")).ShouldBe(["/read/one", "/read/two"]);
    }

    // Fails before the fix: the page ran an item lookup and a related items query that nothing rendered.
    [Fact]
    public void HomePage_WithContentAndNoSections_RunsNoQuery()
    {
        RenderStatic<HomePage>(p => p
            .Add(c => c.Content, Doc("home"))
            .Add(c => c.ShowFeaturedContent, false)
            .Add(c => c.ShowRecentContent, false));

        Manager.DidNotReceive().GetContentByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public void HomePage_RecentSectionOff_AsksForNoRecentItems()
    {
        RenderStatic<HomePage>(p => p
            .Add(c => c.Content, Doc("home"))
            .Add(c => c.ShowFeaturedContent, false)
            .Add(c => c.ShowRecentContent, false));

        Manager.DidNotReceive().GetContentByQueryAsync(Arg.Is<ContentQuery>(q => q.IsFeatured == null), Arg.Any<CancellationToken>());
    }

    [Fact]
    public void HomePage_SameParametersRenderAgain_DoNotQueryAgain()
    {
        var cut = RenderStatic<HomePage>(p => p.Add(c => c.Content, Doc("home")));

        cut.Render(p => p.Add(c => c.Content, Doc("home")).Add(c => c.Class, "again"));

        Manager.Received(2).GetContentByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public void HomePage_QueryThrows_LogsAndRendersThePageWithoutTheSection()
    {
        Manager.GetContentByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>())
            .Returns<Task<IReadOnlyList<ContentItem>>>(_ => throw new InvalidOperationException("down"));

        var cut = RenderStatic<HomePage>(p => p.Add(c => c.Content, Doc("home")));

        cut.Find(".osirion-hero-title").TextContent.ShouldBe("home");
        cut.FindAll(".osirion-featured-posts-section").ShouldBeEmpty();
        cut.Markup.ShouldNotContain("down");
    }

    // Fails before the fix: the hero always used the item's featured image and ignored HeroImageUrl.
    [Fact]
    public void HomePage_HeroImageUrl_IsTheHeroImage()
    {
        var content = Doc("home");
        content.SetFeaturedImage("/img/item.png");

        var cut = RenderStatic<HomePage>(p => p
            .Add(c => c.Content, content)
            .Add(c => c.HeroImageUrl, "/img/hero.png")
            .Add(c => c.ShowFeaturedContent, false)
            .Add(c => c.ShowRecentContent, false));

        cut.Find(".osirion-hero-section img").GetAttribute("src").ShouldBe("/img/hero.png");
    }

    [Fact]
    public void HomePage_WithoutHeroImageUrl_UsesTheFeaturedImageOfTheItem()
    {
        var content = Doc("home");
        content.SetFeaturedImage("/img/item.png");

        var cut = RenderStatic<HomePage>(p => p
            .Add(c => c.Content, content)
            .Add(c => c.ShowFeaturedContent, false)
            .Add(c => c.ShowRecentContent, false));

        cut.Find(".osirion-hero-section img").GetAttribute("src").ShouldBe("/img/item.png");
    }

    private void ReturnFor(Func<ContentQuery, bool> matches, params ContentItem[] items) =>
        Manager.GetContentByQueryAsync(Arg.Is<ContentQuery>(q => matches(q)), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<ContentItem>>(items));
}
