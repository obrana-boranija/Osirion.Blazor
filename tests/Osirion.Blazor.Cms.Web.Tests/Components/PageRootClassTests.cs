using Bunit;
using NSubstitute;
using Osirion.Blazor.Cms.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class PageRootClassTests : CmsPageComponentTestBase
{
    // Fails before the fix: the root element of ArticlePage, HomePage and DocumentPage listed `Class` twice, once from
    // the page's class builder and once from the markup (`class="osirion-article-page ... custom custom"`).
    [Fact]
    public void ArticlePage_Class_IsWrittenOnce()
    {
        Manager.GetContentByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<ContentItem>>([Doc("one")]));

        var cut = RenderStatic<ArticlePage>(p => p.Add(c => c.ItemSlug, "one").Add(c => c.Class, "custom"));

        ClassTokens(cut.Find("div.osirion-article-page")).Count(token => token == "custom").ShouldBe(1);
    }

    // Fails before the fix: the same duplicate as on ArticlePage.
    [Fact]
    public void HomePage_Class_IsWrittenOnce()
    {
        var cut = RenderStatic<HomePage>(p => p
            .Add(c => c.Content, Doc("home"))
            .Add(c => c.ShowFeaturedContent, false)
            .Add(c => c.Class, "custom"));

        ClassTokens(cut.Find("div.osirion-homepage")).Count(token => token == "custom").ShouldBe(1);
    }

    // Fails before the fix: the same duplicate as on ArticlePage.
    [Fact]
    public void DocumentPage_Class_IsWrittenOnce()
    {
        var cut = RenderStatic<DocumentPage>(p => p
            .Add(c => c.Content, Doc("guide"))
            .Add(c => c.Class, "custom"));

        ClassTokens(cut.Find("div.osirion-document-page")).Count(token => token == "custom").ShouldBe(1);
    }

    private static string[] ClassTokens(AngleSharp.Dom.IElement element) =>
        element.GetAttribute("class")!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
}
