using Bunit;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class ContentListPageTests : CmsWebComponentTestBase
{
    private readonly IContentProvider _provider = Substitute.For<IContentProvider>();

    public ContentListPageTests()
    {
        Services.GetRequiredService<IContentProviderManager>().GetDefaultProvider().Returns(_provider);
        ReturnItems(ContentItem.Create("id-1", "First post", "<p>Body</p>", "blog/first-post.md", "test"));
    }

    // Fails before the fix: list pages had no h1 at all (issue 39).
    [Fact]
    public void ContentListPage_Defaults_OpensWithTheDirectoryAsTheOnlyH1()
    {
        var cut = RenderStatic<ContentListPage>(p => p.Add(c => c.Directory, "blog"));

        cut.FindAll("h1").ShouldHaveSingleItem().TextContent.ShouldBe("Blog");
        cut.Find(".osirion-content-list-description").TextContent.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ContentListPage_TitleAndDescription_AreShown()
    {
        var cut = RenderStatic<ContentListPage>(p => p
            .Add(c => c.Directory, "blog")
            .Add(c => c.Title, "Articles")
            .Add(c => c.Description, "Guides and release notes."));

        cut.Find("h1").TextContent.ShouldBe("Articles");
        cut.Find(".osirion-content-list-description").TextContent.ShouldBe("Guides and release notes.");
    }

    [Fact]
    public void ContentListPage_NothingPublished_KeepsTheH1AndSaysSo()
    {
        ReturnItems();

        var cut = RenderStatic<ContentListPage>(p => p.Add(c => c.Tag, "blazor"));

        cut.Find("h1").TextContent.ShouldBe("Tag: blazor");
        cut.Find(".osirion-no-content").TextContent.ShouldContain("Nothing has been published here yet.");
    }

    [Fact]
    public void ContentListPage_FeedUrl_LinksTheFeedInTheHeadAndOnThePage()
    {
        var head = Render<HeadOutlet>();

        var cut = RenderStatic<ContentListPage>(p => p
            .Add(c => c.Directory, "blog")
            .Add(c => c.FeedUrl, "/feed.xml"));

        cut.Find(".osirion-content-list-page-feed a").GetAttribute("href").ShouldBe("/feed.xml");
        var link = head.Find("link[rel='alternate']");
        link.GetAttribute("type").ShouldBe("application/rss+xml");
        link.GetAttribute("href").ShouldBe("/feed.xml");
    }

    [Fact]
    public void ContentListPage_Defaults_HaveNoSidebarOrFeedLink()
    {
        var cut = RenderStatic<ContentListPage>(p => p.Add(c => c.Directory, "blog"));

        cut.FindAll("aside").ShouldBeEmpty();
        cut.FindAll(".osirion-content-list-page-feed").ShouldBeEmpty();
    }

    [Fact]
    public void ContentListPage_ShowTags_AddsANamedSidebar()
    {
        var cut = RenderStatic<ContentListPage>(p => p
            .Add(c => c.Directory, "blog")
            .Add(c => c.ShowTags, true));

        cut.Find("aside.osirion-content-list-page-sidebar").GetAttribute("aria-label").ShouldBe("Browse");
        cut.Find(".osirion-content-list-page").ClassList.ShouldContain("osirion-content-list-page-with-sidebar");
    }

    [Fact]
    public void ContentList_Defaults_RenderNoH1()
    {
        var cut = RenderStatic<ContentList>(p => p.Add(c => c.Directory, "blog"));

        cut.FindAll("h1").ShouldBeEmpty();
    }

    private void ReturnItems(params ContentItem[] items) =>
        _provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>())
            .Returns(Task.FromResult<IReadOnlyList<ContentItem>?>(items));
}
