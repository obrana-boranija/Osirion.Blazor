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

/// <summary>
/// A section's index page (<c>_index.md</c>) has the section's own URL. The section's list must not link to it, or
/// the reader gets a card for the page they are already on.
/// </summary>
public class ContentListCurrentPageTests : CmsWebComponentTestBase
{
    private const string Origin = "http://localhost";

    private readonly IContentProvider _provider = Substitute.For<IContentProvider>();

    public ContentListCurrentPageTests()
    {
        var items = new List<ContentItem>
        {
            CreateItem("blog-index", "Blog", "blog/_index.md", "blog"),
            CreateItem("post-1", "First post", "blog/first-post.md", "blog/first-post"),
            CreateItem("post-2", "Second post", "blog/second-post.md", "blog/second-post")
        };
        _provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>()).Returns(Task.FromResult<IReadOnlyList<ContentItem>?>(items));
        Services.GetRequiredService<IContentProviderManager>().GetDefaultProvider().Returns(_provider);
    }

    private NavigationManager Navigation => Services.GetRequiredService<NavigationManager>();

    private IRenderedComponent<ContentList> RenderList(string relativeUrl, bool paginate, string? locale = null)
    {
        Navigation.NavigateTo(Origin + relativeUrl);
        return RenderStatic<ContentList>(p => p
            .Add(c => c.ShowPagination, paginate)
            .Add(c => c.ItemsPerPage, 10)
            .Add(c => c.Locale, locale)
            .Add(c => c.EnableSeoMetadata, false));
    }

    // The card title links, as site paths: the list section writes the item URL without a leading slash.
    private static List<string> Links(IRenderedComponent<ContentList> cut) =>
        cut.FindAll("a.osirion-featured-post-title-link").Select(a => "/" + a.GetAttribute("href")!.TrimStart('/')).ToList();

    // Fails before the fix: the blog list showed a "Blog" card that linked to /blog.
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ContentList_OnTheSectionPage_LeavesOutTheSectionsOwnIndexPage(bool paginate)
    {
        var cut = RenderList("/blog", paginate);

        var links = Links(cut);
        links.ShouldNotContain("/blog");
        links.ShouldContain("/blog/first-post");
        links.ShouldContain("/blog/second-post");
    }

    [Theory]
    [InlineData("/blog/?page=1")]
    [InlineData("/BLOG#top")]
    public void ContentList_IgnoresQueryFragmentSlashesAndCase_WhenMatchingTheCurrentPage(string relativeUrl)
    {
        var cut = RenderList(relativeUrl, paginate: false);

        Links(cut).ShouldNotContain("/blog");
    }

    [Fact]
    public void ContentList_OnALocalizedRoute_LeavesOutTheIndexPageToo()
    {
        var cut = RenderList("/en/blog", paginate: false, locale: "en");

        Links(cut).ShouldNotContain("/blog");
    }

    [Fact]
    public void ContentList_ElsewhereOnTheSite_KeepsTheSectionIndexPage()
    {
        var cut = RenderList("/search?q=blog", paginate: false);

        Links(cut).ShouldContain("/blog");
    }

    private static ContentItem CreateItem(string id, string title, string path, string url)
    {
        var item = ContentItem.Create(id, title, "<p>Body</p>", path, "test");
        item.SetUrl(url);
        return item;
    }
}
