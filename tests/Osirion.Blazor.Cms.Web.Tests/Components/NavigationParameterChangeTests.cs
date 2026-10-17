using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class NavigationParameterChangeTests : CmsWebComponentTestBase
{
    private readonly IContentProvider _provider = Substitute.For<IContentProvider>();
    private readonly IContentProviderManager _manager = Substitute.For<IContentProviderManager>();

    public NavigationParameterChangeTests()
    {
        Services.AddSingleton(_manager);
        _manager.GetDefaultProvider().Returns(_provider);
        _provider.GetTagsAsync().Returns(Task.FromResult<IReadOnlyList<ContentTag>>(
        [
            ContentTag.Create("beta", "beta", 1),
            ContentTag.Create("alpha", "alpha", 5),
            ContentTag.Create("gamma", "gamma", 3)
        ]));
    }

    // Fails before the fix: the first load was stored in the Tags parameter, so a later MaxTags change was ignored and the old list stayed.
    [Fact]
    public void TagCloud_MaxTagsChangesAfterTheLoad_ShowsTheNewLimit()
    {
        var cut = RenderStatic<TagCloud>(p => p.Add(c => c.MaxTags, 3));
        cut.FindAll("a.osirion-tag-link").Count.ShouldBe(3);

        cut.Render(p => p.Add(c => c.MaxTags, 1));

        cut.FindAll("a.osirion-tag-link").Select(a => a.TextContent.Trim()).ShouldHaveSingleItem().ShouldStartWith("alpha");
    }

    // Fails before the fix: a SortByCount change after the load was ignored for the same reason.
    [Fact]
    public void TagCloud_SortChangesAfterTheLoad_ReordersTheTags()
    {
        var cut = RenderStatic<TagCloud>(p => p.Add(c => c.ShowCount, false));

        cut.Render(p => p.Add(c => c.SortByCount, false));

        cut.FindAll("a.osirion-tag-link").Select(a => a.TextContent.Trim()).ShouldBe(["alpha", "beta", "gamma"]);
    }

    // Fails before the fix: the neighbours looked up for the first item were stored in PreviousItem and NextItem, so after Item changed they stayed, and the lookup was skipped because both were set.
    [Fact]
    public void OsirionContentNavigation_ItemChanges_LooksUpTheNeighboursAgain()
    {
        var first = Page("first");
        var middle = Page("middle");
        var last = Page("last");
        _manager.GetContentByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<ContentItem>>([first, middle, last]));
        var cut = RenderStatic<OsirionContentNavigation>(p => p.Add(c => c.Item, middle));
        cut.Find("a.osirion-content-previous .osirion-content-navigation-title").TextContent.ShouldBe("first");
        cut.Find("a.osirion-content-next .osirion-content-navigation-title").TextContent.ShouldBe("last");

        cut.Render(p => p.Add(c => c.Item, last));

        cut.Find("a.osirion-content-previous .osirion-content-navigation-title").TextContent.ShouldBe("middle");
        cut.FindAll("a.osirion-content-next").ShouldBeEmpty();
    }

    // Fails before the fix: the looked-up neighbours were stored in the parameters, so a neighbour the parent stopped passing never fell back to the lookup.
    [Fact]
    public void OsirionContentNavigation_ParentNeighbourRemoved_FallsBackToTheLookup()
    {
        var first = Page("first");
        var middle = Page("middle");
        var last = Page("last");
        var other = Page("other");
        _manager.GetContentByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<ContentItem>>([first, middle, last]));
        var cut = RenderStatic<OsirionContentNavigation>(p => p
            .Add(c => c.Item, middle)
            .Add(c => c.NextItem, other));
        cut.Find("a.osirion-content-next .osirion-content-navigation-title").TextContent.ShouldBe("other");

        cut.Render(p => p.Add(c => c.NextItem, null));

        cut.Find("a.osirion-content-next .osirion-content-navigation-title").TextContent.ShouldBe("last");
    }

    private static ContentItem Page(string name)
    {
        var item = ContentItem.Create($"id-{name}", name, "<p>Body</p>", $"docs/{name}.md", "test");
        item.SetUrl($"/docs/{name}");
        return item;
    }
}
