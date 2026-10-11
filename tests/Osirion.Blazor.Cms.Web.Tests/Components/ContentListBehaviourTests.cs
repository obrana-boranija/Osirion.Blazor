using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class ContentListBehaviourTests : CmsWebComponentTestBase
{
    private readonly IContentProvider _provider = Substitute.For<IContentProvider>();
    private readonly IContentProviderManager _manager = Substitute.For<IContentProviderManager>();
    private readonly RecordingLoggerFactory _logs = new();

    public ContentListBehaviourTests()
    {
        Services.AddSingleton(_manager);
        _logs.Register(Services);
        _manager.GetDefaultProvider().Returns(_provider);
        ReturnItems(Items(3));
    }

    [Fact]
    public void ContentList_Query_PassesTheFiltersAndSortOrderToTheProvider()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("http://localhost/search?q=blazor");

        RenderStatic<ContentList>(p => p
            .Add(c => c.Directory, "blog")
            .Add(c => c.Category, "news")
            .Add(c => c.Tag, "dotnet")
            .Add(c => c.Locale, "de")
            .Add(c => c.DirectoryId, "dir-1")
            .Add(c => c.SortBy, SortField.Title)
            .Add(c => c.SortDirection, Osirion.Blazor.Cms.Domain.Enums.SortDirection.Ascending)
            .Add(c => c.EnableSeoMetadata, false));

        _provider.Received(1).GetItemsByQueryAsync(Arg.Is<ContentQuery>(q =>
            q.Directory == "blog" && q.Category == "news" && q.Tag == "dotnet" && q.SearchQuery == "blazor"
            && q.Locale == "de" && q.DirectoryId == "dir-1" && q.SortBy == SortField.Title
            && q.SortDirection == Osirion.Blazor.Cms.Domain.Enums.SortDirection.Ascending
            && q.IsFeatured == null && q.Take == null));
    }

    [Fact]
    public void ContentList_OnlyFeatured_AsksForFeaturedItems()
    {
        RenderStatic<ContentList>(p => p
            .Add(c => c.OnlyFeatured, true)
            .Add(c => c.EnableSeoMetadata, false));

        _provider.Received(1).GetItemsByQueryAsync(Arg.Is<ContentQuery>(q => q.IsFeatured == true && q.Take == null));
    }

    [Fact]
    public void ContentList_FeaturedCount_AsksForThatManyFeaturedItemsWithoutPaginating()
    {
        var cut = RenderStatic<ContentList>(p => p
            .Add(c => c.FeaturedCount, 2)
            .Add(c => c.ShowPagination, true)
            .Add(c => c.EnableSeoMetadata, false));

        _provider.Received(1).GetItemsByQueryAsync(Arg.Is<ContentQuery>(q => q.IsFeatured == true && q.Take == 2));
        cut.FindAll(".osirion-content-pagination").ShouldBeEmpty();
    }

    [Fact]
    public void ContentList_NoItems_ShowsTheNoContentText()
    {
        ReturnItems([]);

        var cut = RenderStatic<ContentList>(p => p
            .Add(c => c.NoContentText, "Nothing yet")
            .Add(c => c.Class, "mine"));

        cut.Find("div.osirion-content-list").ClassList.ShouldContain("mine");
        cut.Find(".osirion-no-content p").TextContent.ShouldBe("Nothing yet");
    }

    [Fact]
    public void ContentList_NoDefaultProvider_ShowsTheNoContentText()
    {
        _manager.GetDefaultProvider().Returns((IContentProvider?)null);

        var cut = RenderStatic<ContentList>(_ => { });

        cut.FindAll(".osirion-no-content").ShouldHaveSingleItem();
    }

    [Fact]
    public void ContentList_ProviderThrows_LogsAndShowsTheNoContentText()
    {
        _provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>())
            .Returns<Task<IReadOnlyList<ContentItem>?>>(_ => throw new InvalidOperationException("down"));

        var cut = RenderStatic<ContentList>(_ => { });

        _logs.Entries.ShouldContain(e => e.Level == LogLevel.Error && e.Exception is InvalidOperationException);
        cut.FindAll(".osirion-no-content").ShouldHaveSingleItem();
        cut.Markup.ShouldNotContain("down");
    }

    [Fact]
    public void ContentList_Items_RenderedThroughTheListSection()
    {
        var cut = RenderStatic<ContentList>(p => p.Add(c => c.EnableSeoMetadata, false));

        cut.FindAll(".osirion-featured-post-title").Select(t => t.TextContent.Trim()).ShouldBe(["Post 1", "Post 2", "Post 3"]);
    }

    [Fact]
    public void ContentList_InteractiveWhileLoading_ShowsTheLoadingText()
    {
        var pending = new TaskCompletionSource<IReadOnlyList<ContentItem>?>();
        _provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>()).Returns(pending.Task);
        SetRendererInfo(new RendererInfo("Server", true));

        var cut = Render<ContentList>(p => p
            .Add(c => c.LoadingText, "Fetching")
            .Add(c => c.EnableSeoMetadata, false));

        cut.Find(".osirion-content-loading p").TextContent.ShouldBe("Fetching");

        pending.SetResult(Items(1));

        cut.WaitForAssertion(() => cut.FindAll(".osirion-content-loading").ShouldBeEmpty());
    }

    [Fact]
    public void ContentList_SeoMetadataEnabled_WritesTheComputedTitleToTheHead()
    {
        var head = Render<HeadOutlet>();

        RenderStatic<ContentList>(p => p.Add(c => c.Category, "News"));

        head.WaitForAssertion(() => head.Markup.ShouldContain("Category: News"));
    }

    [Fact]
    public void ContentList_SecondPageWithSeoMetadata_LinksThePreviousAndNextPages()
    {
        ReturnItems(Items(25));
        var head = Render<HeadOutlet>();
        Services.GetRequiredService<NavigationManager>().NavigateTo("http://localhost/blog?page=2");

        RenderStatic<ContentList>(p => p
            .Add(c => c.ShowPagination, true)
            .Add(c => c.ItemsPerPage, 10));

        head.WaitForAssertion(() =>
        {
            head.Find("link[rel='prev']").GetAttribute("href").ShouldBe("/blog");
            head.Find("link[rel='next']").GetAttribute("href").ShouldBe("/blog?page=3");
        });
    }

    [Fact]
    public void ContentList_PageChangedDelegate_ReceivesTheClickedPageInsteadOfNavigating()
    {
        ReturnItems(Items(25));
        var navigation = Services.GetRequiredService<NavigationManager>();
        var changedTo = 0;

        var cut = RenderStatic<ContentList>(p => p
            .Add(c => c.ShowPagination, true)
            .Add(c => c.ItemsPerPage, 10)
            .Add(c => c.EnableSeoMetadata, false)
            .Add(c => c.PageChanged, (int page) => changedTo = page));
        var before = navigation.Uri;

        cut.Find("a.osirion-pagination-link:not(.osirion-active)").Click();

        changedTo.ShouldBe(2);
        navigation.Uri.ShouldBe(before);
    }

    [Fact]
    public void ContentList_ClickingTheActivePage_DoesNothing()
    {
        ReturnItems(Items(25));
        var navigation = Services.GetRequiredService<NavigationManager>();
        var raised = false;

        var cut = RenderStatic<ContentList>(p => p
            .Add(c => c.ShowPagination, true)
            .Add(c => c.ItemsPerPage, 10)
            .Add(c => c.EnableSeoMetadata, false)
            .Add(c => c.PageChanged, (int _) => raised = true));
        var before = navigation.Uri;

        cut.Find("a.osirion-pagination-link.osirion-active").Click();

        raised.ShouldBeFalse();
        navigation.Uri.ShouldBe(before);
    }

    [Fact]
    public void ContentList_PaginationUrlFormatter_BuildsThePageLinks()
    {
        ReturnItems(Items(25));

        var cut = RenderStatic<ContentList>(p => p
            .Add(c => c.ShowPagination, true)
            .Add(c => c.ItemsPerPage, 10)
            .Add(c => c.EnableSeoMetadata, false)
            .Add(c => c.PaginationUrlFormatter, page => $"/p/{page}"));

        cut.FindAll("a.osirion-pagination-link").Select(a => a.GetAttribute("href")).ShouldContain("/p/3");
    }

    [Theory]
    [InlineData(1, "1 2 3 4 5 ... 10 >")]
    [InlineData(6, "< 1 ... 4 5 6 7 8 ... 10 >")]
    [InlineData(10, "< 1 ... 6 7 8 9 10")]
    [InlineData(3, "< 1 2 3 4 5 ... 10 >")]
    [InlineData(4, "< 1 2 3 4 5 6 ... 10 >")]
    public void ContentList_ManyPages_ShowsAWindowAroundTheCurrentPage(int page, string expected)
    {
        ReturnItems(Items(100));
        Services.GetRequiredService<NavigationManager>().NavigateTo($"http://localhost/list?page={page}");

        var cut = RenderStatic<ContentList>(p => p
            .Add(c => c.ShowPagination, true)
            .Add(c => c.ItemsPerPage, 10)
            .Add(c => c.EnableSeoMetadata, false));

        var parts = cut.Find(".osirion-content-pagination").Children.Select(child =>
        {
            var text = child.TextContent.Trim();
            if (text.Length > 0)
            {
                return text;
            }

            return child.QuerySelector("polyline")!.GetAttribute("points") == "15 18 9 12 15 6" ? "<" : ">";
        });
        string.Join(" ", parts).ShouldBe(expected);
        ActivePage(cut).ShouldBe(page.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    [Theory]
    [InlineData(null, null, null, null, false, 1, "Blog")]
    [InlineData("Custom", "news", null, null, false, 1, "Custom")]
    [InlineData(null, "news", "dotnet", "blog", false, 1, "Category: news")]
    [InlineData(null, null, "dotnet", "blog", false, 1, "Tag: dotnet")]
    [InlineData(null, null, null, "/blog/getting-started_guide/", false, 1, "Getting Started Guide")]
    [InlineData(null, null, null, "docs/API", false, 1, "Api")]
    [InlineData(null, null, null, "a--b", false, 1, "A  B")]
    [InlineData(null, null, null, null, true, 3, "Blog - Page 3")]
    [InlineData(null, null, null, null, false, 3, "Blog")]
    public void ContentList_ComputedPageTitle_DescribesTheListing(
        string? pageTitle, string? category, string? tag, string? directory, bool paginated, int page, string expected)
    {
        var probe = RenderProbe(pageTitle, null, category, tag, directory, paginated, page);

        probe.Title().ShouldBe(expected);
    }

    [Theory]
    [InlineData("Custom text", "news", null, null, "Custom text")]
    [InlineData(null, "news", "dotnet", "blog", "Browse all articles in the news category.")]
    [InlineData(null, null, "dotnet", "blog", "Browse all articles tagged with dotnet.")]
    [InlineData(null, null, null, "blog/how-to", "Browse all articles in How To.")]
    [InlineData(null, null, null, null, "Browse our collection of articles and blog posts.")]
    public void ContentList_ComputedPageDescription_DescribesTheListing(
        string? pageDescription, string? category, string? tag, string? directory, string expected)
    {
        var probe = RenderProbe(null, pageDescription, category, tag, directory, false, 1);

        probe.Description().ShouldBe(expected);
    }

    [Fact]
    public void ContentList_ContentUrl_UsesTheFormatterOrTheItemUrl()
    {
        var item = ContentItem.Create("id", "Post", "<p>x</p>", "blog/post.md", "test");
        item.SetUrl("blog/post");

        RenderProbe(null, null, null, null, null, false, 1).ContentUrl(item).ShouldBe("/blog/post");

        var formatted = RenderStatic<ContentListProbe>(p => p
            .Add(c => c.EnableSeoMetadata, false)
            .Add(c => c.ContentUrlFormatter, i => $"/read/{i.Title}")).Instance;
        formatted.ContentUrl(item).ShouldBe("/read/Post");
    }

    private ContentListProbe RenderProbe(
        string? pageTitle, string? pageDescription, string? category, string? tag, string? directory, bool paginated, int page)
    {
        ReturnItems(Items(30));
        Services.GetRequiredService<NavigationManager>().NavigateTo($"http://localhost/list?page={page}");
        return RenderStatic<ContentListProbe>(p => p
            .Add(c => c.PageTitle, pageTitle)
            .Add(c => c.PageDescription, pageDescription)
            .Add(c => c.Category, category)
            .Add(c => c.Tag, tag)
            .Add(c => c.Directory, directory)
            .Add(c => c.ShowPagination, paginated)
            .Add(c => c.EnableSeoMetadata, false)).Instance;
    }

    private static string ActivePage(IRenderedComponent<ContentList> cut) =>
        cut.Find("a.osirion-pagination-link.osirion-active").TextContent.Trim();

    private void ReturnItems(IReadOnlyList<ContentItem> items) =>
        _provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>()).Returns(Task.FromResult<IReadOnlyList<ContentItem>?>(items));

    private static List<ContentItem> Items(int count) =>
        Enumerable.Range(1, count)
            .Select(i => ContentItem.Create($"id-{i}", $"Post {i}", "<p>Body</p>", $"blog/post-{i}.md", "test"))
            .ToList();

    private sealed class ContentListProbe : ContentList
    {
        public string Title() => GetComputedPageTitle();

        public string? Description() => GetComputedPageDescription();

        public string ContentUrl(ContentItem item) => GetContentUrl(item);
    }
}
