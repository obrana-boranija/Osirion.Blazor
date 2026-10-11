using System.Text.Json.Nodes;
using Bunit;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class SeoMetadataRendererBreadcrumbTests : CmsWebComponentTestBase
{
    // Fails before the fix: the last ListItem was named from the slug ("Pre Mvp Make It Right"), and search engines
    // show that name in the result's breadcrumb trail.
    [Fact]
    public void SeoMetadataRenderer_BreadcrumbList_NamesTheLastItemWithTheContentTitle()
    {
        var names = RenderBreadcrumbNames(CreatePost("Pre-MVP: make it right"));

        names.ShouldBe(["Home", "Blog", "Pre-MVP: make it right"]);
    }

    private List<string?> RenderBreadcrumbNames(ContentItem item)
    {
        var cut = RenderStatic<SeoMetadataRenderer>(parameters => parameters
            .Add(p => p.Content, item)
            .Add(p => p.SchemaTypes, [SchemaType.BreadcrumbList]));

        var breadcrumbList = cut.FindAll("script[type='application/ld+json']")
            .Select(script => JsonNode.Parse(script.TextContent))
            .Single(node => node?["@type"]?.GetValue<string>() == "BreadcrumbList")!;

        return breadcrumbList["itemListElement"]!.AsArray()
            .Select(element => element?["name"]?.GetValue<string>())
            .ToList();
    }

    private static ContentItem CreatePost(string title)
    {
        var item = ContentItem.Create("post-1", title, "<p>Body</p>", "blog/pre-mvp-make-it-right.md", "test");
        item.SetUrl("/blog/pre-mvp-make-it-right");
        return item;
    }
}
