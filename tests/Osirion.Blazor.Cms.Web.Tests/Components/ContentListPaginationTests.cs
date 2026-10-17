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

public class ContentListPaginationTests : CmsWebComponentTestBase
{
    private const string Origin = "http://localhost";

    private readonly IContentProvider _provider = Substitute.For<IContentProvider>();

    public ContentListPaginationTests()
    {
        var items = Enumerable.Range(1, 25)
            .Select(i => ContentItem.Create($"id-{i}", $"Post {i}", "<p>Body</p>", $"blog/post-{i}.md", "test"))
            .ToList();
        _provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>()).Returns(Task.FromResult<IReadOnlyList<ContentItem>?>(items));
        Services.GetRequiredService<IContentProviderManager>().GetDefaultProvider().Returns(_provider);
    }

    private NavigationManager Navigation => Services.GetRequiredService<NavigationManager>();

    private IRenderedComponent<ContentList> RenderList(string relativeUrl)
    {
        Navigation.NavigateTo(Origin + relativeUrl);
        return RenderStatic<ContentList>(p => p
            .Add(c => c.ShowPagination, true)
            .Add(c => c.ItemsPerPage, 10)
            .Add(c => c.EnableSeoMetadata, false));
    }

    private static string ActivePage(IRenderedComponent<ContentList> cut) =>
        cut.Find("a.osirion-pagination-link.osirion-active").TextContent.Trim();

    // Fails before the fix: the previous and next links held only an icon, so they had no accessible name, and the
    // current page was marked only by a class.
    [Fact]
    public void ContentList_Pagination_NamesItsLinksAndMarksTheCurrentPage()
    {
        var cut = RenderList("/search?q=hello&page=2");

        cut.Find("nav.osirion-content-pagination").GetAttribute("aria-label").ShouldBe("Pagination");
        var links = cut.FindAll("a.osirion-pagination-link");
        links.First().GetAttribute("aria-label").ShouldBe("Previous page");
        links.Last().GetAttribute("aria-label").ShouldBe("Next page");
        cut.FindAll("a.osirion-pagination-link svg").ShouldAllBe(svg => svg.GetAttribute("aria-hidden") == "true");
        cut.Find("a.osirion-pagination-link.osirion-active").GetAttribute("aria-current").ShouldBe("page");
    }

    // Fails before the fix: the default pagination URL was "{path}?page=N" and dropped the search query.
    [Fact]
    public void ContentList_PaginationLinks_KeepOtherQueryParameters()
    {
        var cut = RenderList("/search?q=hello%20world&page=2");

        var hrefs = cut.FindAll("a.osirion-pagination-link").Select(a => a.GetAttribute("href")).ToList();
        hrefs.ShouldContain("/search?q=hello%20world&page=3");
        hrefs.ShouldContain("/search?q=hello%20world");
        hrefs.ShouldAllBe(href => href!.Contains("q=hello%20world"));
    }

    // Fails before the fix: a click assigned CurrentPage without navigating, and the next parameter update reset it.
    [Fact]
    public void ContentList_ClickingAPageLink_NavigatesToThatPage()
    {
        var cut = RenderList("/search?q=hello&page=1");

        cut.Find("a.osirion-pagination-link[href$='page=2']").Click();

        Navigation.Uri.ShouldBe($"{Origin}/search?q=hello&page=2");
    }

    // Fails before the fix: page=0 or a negative page produced a negative Skip.
    [Theory]
    [InlineData(0)]
    [InlineData(-7)]
    public void ContentList_PageBelowOne_ShowsTheFirstPage(int page)
    {
        var cut = RenderList($"/list?page={page}");

        ActivePage(cut).ShouldBe("1");
        cut.FindAll("article").Count.ShouldBeLessThanOrEqualTo(10);
    }

    // Fails before the fix: (int.MaxValue - 1) * ItemsPerPage overflowed.
    [Fact]
    public void ContentList_HugePage_ShowsTheLastPage()
    {
        var cut = RenderList($"/list?page={int.MaxValue}");

        ActivePage(cut).ShouldBe("3");
    }

    // Fails before the fix: the whole list was fetched once for the total and again for the page.
    [Fact]
    public async Task ContentList_Pagination_FetchesTheListOnce()
    {
        RenderList("/list?page=2");

        await _provider.Received(1).GetItemsByQueryAsync(Arg.Any<ContentQuery>());
    }

    [Fact]
    public void ContentList_PaginationUrlFormatter_StillWins()
    {
        Navigation.NavigateTo($"{Origin}/list?page=1");
        var cut = RenderStatic<ContentList>(p => p
            .Add(c => c.ShowPagination, true)
            .Add(c => c.ItemsPerPage, 10)
            .Add(c => c.EnableSeoMetadata, false)
            .Add(c => c.PaginationUrlFormatter, page => $"/insights?page={page}"));

        cut.FindAll("a.osirion-pagination-link").Select(a => a.GetAttribute("href")).ShouldContain("/insights?page=2");
    }
}
