using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class ContentPageStateTests : CmsWebComponentTestBase
{
    private readonly IContentProvider _provider = Substitute.For<IContentProvider>();
    private readonly ContentItem _first = Item("first");
    private readonly ContentItem _second = Item("second");
    private readonly ContentItem _third = Item("third");

    public ContentPageStateTests()
    {
        Services.GetRequiredService<IContentProviderManager>().GetDefaultProvider().Returns(_provider);
        _provider.GetItemByPathAsync("first").Returns(_first);
        _provider.GetItemByPathAsync("second").Returns(_second);
        _provider.GetItemByUrlAsync("first").Returns(_first);
        _provider.GetItemByUrlAsync("second").Returns(_second);
        _provider.GetItemByUrlAsync("third").Returns(_third);
        _provider.GetItemByUrlAsync("missing").Returns((ContentItem?)null);
        _provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>())
            .Returns(Task.FromResult<IReadOnlyList<ContentItem>?>([_first, _second, _third]));
    }

    // Fails before the fix: Item kept the first document after Path changed.
    [Fact]
    public void ContentView_PathChanges_ShowsTheNewDocument()
    {
        var cut = RenderStatic<ContentView>(p => p.Add(c => c.Path, "first"));
        cut.Find(".osirion-content-title").TextContent.ShouldBe("first");

        cut.Render(p => p.Add(c => c.Path, "second"));

        cut.Find(".osirion-content-title").TextContent.ShouldBe("second");
    }

    [Fact]
    public void ContentView_PathChangesToMissingContent_ShowsNotFoundInsteadOfTheOldDocument()
    {
        var cut = RenderStatic<ContentView>(p => p.Add(c => c.Path, "first"));

        _provider.GetItemByPathAsync("gone").Returns((ContentItem?)null);
        cut.Render(p => p.Add(c => c.Path, "gone"));

        cut.FindAll(".osirion-content-title").ShouldBeEmpty();
        cut.FindAll(".osirion-content-not-found").ShouldHaveSingleItem();
    }

    [Fact]
    public void ContentView_ItemSuppliedByTheParent_IsNotReplacedByPathLoading()
    {
        var cut = RenderStatic<ContentView>(p => p
            .Add(c => c.Item, _third)
            .Add(c => c.Path, "first"));

        cut.Find(".osirion-content-title").TextContent.ShouldBe("third");
    }

    // Fails before the fix: previous and next stayed from the first document because they were only loaded while null.
    [Fact]
    public void ContentPage_PathChanges_ReplacesNeighboursLoadedForThePreviousPath()
    {
        var cut = RenderStatic<ContentPage>(p => p
            .Add(c => c.Path, "second")
            .Add(c => c.ShowBreadcrumbs, false)
            .Add(c => c.ShowCategories, false)
            .Add(c => c.ShowTags, false));
        cut.Instance.PreviousItem.ShouldBe(_first);
        cut.Instance.NextItem.ShouldBe(_third);

        cut.Render(p => p.Add(c => c.Path, "first"));

        cut.Instance.ContentItem.ShouldBe(_first);
        cut.Instance.PreviousItem.ShouldBeNull();
        cut.Instance.NextItem.ShouldBe(_second);
    }

    // Fails before the fix: the share URL was "https://" + Path, for example https://blog/my-post.
    [Fact]
    public void ContentPage_DefaultShareLinks_UseTheAbsolutePageUrl()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("http://localhost/blog/first?utm=1#top");

        var cut = RenderStatic<ContentPage>(p => p
            .Add(c => c.Path, "first")
            .Add(c => c.ShowShareLinks, true)
            .Add(c => c.ShowBreadcrumbs, false));

        var expected = Uri.EscapeDataString("http://localhost/blog/first");
        var hrefs = cut.FindAll("a.osirion-content-share-link").Select(a => a.GetAttribute("href")).ToList();
        hrefs.Count.ShouldBe(3);
        hrefs.ShouldAllBe(href => href!.Contains(expected));
    }

    // Fails before the fix: ContentPage never passed its locale to LocalizedContentView.
    [Fact]
    public void ContentPage_ItemWithLocale_BuildsTagLinksBelowThatLocale()
    {
        var item = Item("localized");
        item.SetLocale("sr");
        item.AddTag("C#");
        _provider.GetItemByUrlAsync("localized").Returns(item);

        var cut = RenderStatic<ContentPage>(p => p
            .Add(c => c.Path, "localized")
            .Add(c => c.ShowBreadcrumbs, false)
            .Add(c => c.ShowTags, false));

        cut.Find("a.osirion-content-tag").GetAttribute("href").ShouldBe("/sr/tag/c%23");
    }

    [Fact]
    public void ContentPage_TagUrlFormatter_AppliesToTheContentTags()
    {
        var item = Item("tagged");
        item.AddTag("C#");
        _provider.GetItemByUrlAsync("tagged").Returns(item);

        var cut = RenderStatic<ContentPage>(p => p
            .Add(c => c.Path, "tagged")
            .Add(c => c.ShowBreadcrumbs, false)
            .Add(c => c.ShowTags, false)
            .Add(c => c.TagUrlFormatter, tag => $"/topics/{tag}"));

        cut.Find("a.osirion-content-tag").GetAttribute("href").ShouldBe("/topics/C#");
    }

    private static ContentItem Item(string name) =>
        ContentItem.Create($"id-{name}", name, "<p>Body</p>", $"blog/{name}.md", "test");
}
