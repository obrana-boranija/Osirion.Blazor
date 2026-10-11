using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class ContentSeriesNavigationTests : CmsWebComponentTestBase
{
    private const string Series = "Build a blog";

    private readonly ContentItem _part1 = Part("p1", "Set up", 1, new DateTime(2026, 3, 1));
    private readonly ContentItem _part2 = Part("p2", "Write posts", 2, new DateTime(2026, 2, 1));
    private readonly ContentItem _part3 = Part("p3", "Deploy", 3, new DateTime(2026, 1, 1));
    private readonly ContentItem _bonus = Part("p4", "Bonus", null, new DateTime(2025, 1, 1));
    private readonly ContentItem _other = Part("x", "Elsewhere", 1, new DateTime(2026, 1, 1), "Another series");

    private ContentQuery? _lastQuery;

    public ContentSeriesNavigationTests()
    {
        Services.GetRequiredService<IContentProviderManager>()
            .GetContentByQueryAsync(Arg.Do<ContentQuery>(q => _lastQuery = q), Arg.Any<CancellationToken>())
            .Returns(new List<ContentItem> { _bonus, _part3, _other, _part1, _part2 });
    }

    [Fact]
    public void ContentSeriesNavigation_MiddlePart_ShowsPositionPartsAndBothNeighbours()
    {
        var cut = RenderStatic<ContentSeriesNavigation>(p => p.Add(c => c.Item, _part2));

        cut.Find(".osirion-series-nav-position").TextContent.ShouldBe("Part 2 of 4");
        cut.Find(".osirion-series-nav-name").TextContent.ShouldBe(Series);
        cut.Find("nav").GetAttribute("aria-label").ShouldBe("Build a blog: Part 2 of 4");
        cut.FindAll(".osirion-series-nav-parts a").Select(a => a.TextContent).ShouldBe(["Set up", "Write posts", "Deploy", "Bonus"]);
        cut.Find(".osirion-series-nav-parts a[aria-current='page']").TextContent.ShouldBe("Write posts");
        var previous = cut.Find("a[rel='prev']");
        previous.GetAttribute("href").ShouldBe("/blog/p1");
        previous.QuerySelector(".osirion-series-nav-title")!.TextContent.ShouldBe("Set up");
        cut.Find("a[rel='next']").GetAttribute("href").ShouldBe("/blog/p3");
    }

    [Fact]
    public void ContentSeriesNavigation_FirstAndLastParts_HaveOnlyOneNeighbour()
    {
        var first = RenderStatic<ContentSeriesNavigation>(p => p.Add(c => c.Item, _part1));
        first.FindAll("a[rel='prev']").ShouldBeEmpty();
        first.FindAll("a[rel='next']").Count.ShouldBe(1);

        var last = RenderStatic<ContentSeriesNavigation>(p => p.Add(c => c.Item, _bonus));
        last.Find(".osirion-series-nav-position").TextContent.ShouldBe("Part 4 of 4");
        last.FindAll("a[rel='next']").ShouldBeEmpty();
    }

    [Fact]
    public void ContentSeriesNavigation_ItemWithoutSeries_RendersNothingAndDoesNotQuery()
    {
        var item = ContentItem.Create("solo", "Solo", "<p>Body</p>", "blog/solo.md", "test");

        var cut = RenderStatic<ContentSeriesNavigation>(p => p.Add(c => c.Item, item));

        cut.Markup.Trim().ShouldBeEmpty();
        _lastQuery.ShouldBeNull();
    }

    [Fact]
    public void ContentSeriesNavigation_OnlyPartPublished_RendersNothing()
    {
        var cut = RenderStatic<ContentSeriesNavigation>(p => p.Add(c => c.Item, _other));

        cut.Markup.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void ContentSeriesNavigation_QueriesTheItemsLocale()
    {
        _part2.SetLocale("sr");

        RenderStatic<ContentSeriesNavigation>(p => p.Add(c => c.Item, _part2));

        _lastQuery!.Locale.ShouldBe("sr");
    }

    [Fact]
    public void ContentSeriesNavigation_ProviderFails_RendersNothing()
    {
        Services.GetRequiredService<IContentProviderManager>()
            .GetContentByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>())
            .Returns<IReadOnlyList<ContentItem>>(_ => throw new InvalidOperationException("GitHub is down"));

        var cut = RenderStatic<ContentSeriesNavigation>(p => p.Add(c => c.Item, _part2));

        cut.Markup.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void ContentSeriesNavigation_FormatterAndTexts_AreApplied()
    {
        var cut = RenderStatic<ContentSeriesNavigation>(p => p
            .Add(c => c.Item, _part2)
            .Add(c => c.ContentUrlFormatter, item => $"/tutorials/{item.Id}")
            .Add(c => c.PositionFormat, "Deo {0} od {1}")
            .Add(c => c.NextText, "Sledeće")
            .Add(c => c.ShowPartList, false));

        cut.Find(".osirion-series-nav-position").TextContent.ShouldBe("Deo 2 od 4");
        cut.Find("a[rel='next']").GetAttribute("href").ShouldBe("/tutorials/p3");
        cut.Find("a[rel='next'] .osirion-series-nav-direction").TextContent.ShouldBe("Sledeće");
        cut.FindAll(".osirion-series-nav-parts").ShouldBeEmpty();
    }

    private static ContentItem Part(string id, string title, int? order, DateTime date, string series = Series)
    {
        var item = ContentItem.Create(id, title, "<p>Body</p>", $"blog/{id}.md", "test");
        item.Url = $"blog/{id}";
        item.DateCreated = date;
        item.Metadata = new FrontMatter { Title = title, Series = series, SeriesOrder = order };
        return item;
    }
}
