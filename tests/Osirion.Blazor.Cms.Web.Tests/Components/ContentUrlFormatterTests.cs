using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

// The formatter tests fail before the fix: the list section linked every post to item.Url and ignored both
// formatters. The last test pins the default link, now built by ContentUrls.ItemUrl like ContentList's.
public class ContentUrlFormatterTests : CmsWebComponentTestBase
{
    private static readonly ContentItem Post = CreatePost();

    [Fact]
    public void ContentList_ContentUrlFormatter_SetsThePostLinks()
    {
        var provider = Substitute.For<IContentProvider>();
        provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>()).Returns(Task.FromResult<IReadOnlyList<ContentItem>?>([Post]));
        Services.GetRequiredService<IContentProviderManager>().GetDefaultProvider().Returns(provider);
        Services.GetRequiredService<NavigationManager>().NavigateTo("http://localhost/blog");

        var cut = RenderStatic<ContentList>(p => p
            .Add(c => c.EnableSeoMetadata, false)
            .Add(c => c.ContentUrlFormatter, item => $"/en/articles/{item.Slug}"));

        cut.Find("a.osirion-featured-post-title-link").GetAttribute("href").ShouldBe("/en/articles/first-post");
    }

    [Fact]
    public void OsirionContentListSection_PostUrlFormatter_SetsTitleAndReadMoreLinks()
    {
        var cut = RenderStatic<OsirionContentListSection>(p => p
            .Add(c => c.Items, new List<ContentItem> { Post })
            .Add(c => c.ShowReadMoreLink, true)
            .Add(c => c.PostUrlFormatter, item => $"/posts/{item.Slug}"));

        cut.Find("a.osirion-featured-post-title-link").GetAttribute("href").ShouldBe("/posts/first-post");
        cut.Find("a.osirion-featured-post-read-more").GetAttribute("href").ShouldBe("/posts/first-post");
    }

    [Fact]
    public void OsirionContentListSection_WithoutAFormatter_LinksToTheItemUrlFromTheRoot()
    {
        var cut = RenderStatic<OsirionContentListSection>(p => p.Add(c => c.Items, new List<ContentItem> { Post }));

        cut.Find("a.osirion-featured-post-title-link").GetAttribute("href").ShouldBe("/blog/first-post");
    }

    private static ContentItem CreatePost()
    {
        var item = ContentItem.Create("first-post", "First post", "<p>Body</p>", "blog/first-post.md", "test");
        item.SetSlug("first-post");
        item.SetUrl("blog/first-post");
        return item;
    }
}
