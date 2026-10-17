using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

/// <summary>
/// Pins what the renderer does with the <c>image</c>, <c>lang</c> and <c>meta_tags</c> keys of the front matter
/// <c>seo_properties</c> block.
/// </summary>
public class SeoMetadataRendererFrontMatterTests : CmsWebComponentTestBase
{
    // Fails before the fix: image was parsed but never read, so the featured image was the social image.
    [Fact]
    public void SeoMetadataRenderer_SeoImage_IsTheSocialImageBeforeTheFeaturedImage()
    {
        var item = CreateItem(new SeoMetadata { Image = "/images/social.png" });

        var cut = RenderPage(item);

        Property(cut, "og:image").ShouldBe("http://localhost/images/social.png");
        Meta(cut, "twitter:image").ShouldBe("http://localhost/images/social.png");
    }

    [Fact]
    public void SeoMetadataRenderer_SeoImageAndOgImageUrl_OgImageUrlWins()
    {
        var item = CreateItem(new SeoMetadata { Image = "/images/social.png", OgImageUrl = "https://cdn.example.com/og.png" });

        var cut = RenderPage(item);

        Property(cut, "og:image").ShouldBe("https://cdn.example.com/og.png");
    }

    [Fact]
    public void SeoMetadataRenderer_NoSeoImage_FallsBackToTheFeaturedImage()
    {
        var cut = RenderPage(CreateItem(new SeoMetadata()));

        Property(cut, "og:image").ShouldBe("http://localhost/images/cover.png");
    }

    // Fails before the fix: lang was parsed but never read, so og:locale came from the item locale alone.
    [Fact]
    public void SeoMetadataRenderer_SeoLang_IsTheOpenGraphLocale()
    {
        var item = CreateItem(new SeoMetadata { Lang = "de-DE" });

        var cut = RenderPage(item);

        Property(cut, "og:locale").ShouldBe("de_DE");
    }

    [Fact]
    public void SeoMetadataRenderer_NoSeoLang_UsesTheItemLocale()
    {
        var cut = RenderPage(CreateItem(new SeoMetadata(), locale: "fr-FR"));

        Property(cut, "og:locale").ShouldBe("fr_FR");
    }

    // Fails before the fix: meta_tags was parsed but nothing wrote it.
    [Fact]
    public void SeoMetadataRenderer_MetaTags_AreWrittenAsMetaElements()
    {
        var item = CreateItem(new SeoMetadata
        {
            MetaTags = new Dictionary<string, string>
            {
                ["theme-color"] = "#102030",
                ["dc.creator"] = "Jane Doe",
            }
        });

        var cut = RenderPage(item);

        Meta(cut, "theme-color").ShouldBe("#102030");
        Meta(cut, "dc.creator").ShouldBe("Jane Doe");
    }

    // Fails before the fix: nothing was written, so the check for a hostile value could not even be made. The
    // value must stay text inside the content attribute and the hostile names must not become markup.
    [Fact]
    public void SeoMetadataRenderer_HostileMetaTags_AreEncodedOrSkipped()
    {
        var item = CreateItem(new SeoMetadata
        {
            MetaTags = new Dictionary<string, string>
            {
                ["safe-name"] = "\"><script>alert(1)</script><meta name=\"x",
                ["\"><script>alert(2)</script>"] = "name breakout",
                ["has space onmouseover=alert(3)"] = "attribute injection",
                ["robots"] = "noindex",
                ["empty-value"] = "",
            }
        });

        var cut = RenderPage(item);

        cut.FindAll("script").ShouldAllBe(script => script.GetAttribute("type") == "application/ld+json");
        cut.Markup.ShouldNotContain("alert(2)");
        cut.Markup.ShouldNotContain("alert(3)");
        Meta(cut, "safe-name").ShouldBe("\"><script>alert(1)</script><meta name=\"x");
        cut.Markup.ShouldNotContain("name breakout");
        cut.Markup.ShouldNotContain("attribute injection");
        cut.FindAll("meta[name='empty-value']").ShouldBeEmpty();
        cut.FindAll("meta[name='robots']").Count.ShouldBe(1);
        Meta(cut, "robots").ShouldNotBe("noindex");
    }

    [Fact]
    public void SeoMetadataRenderer_NoMetaTags_WritesNoExtraMetaElement()
    {
        var withoutTags = RenderPage(CreateItem(new SeoMetadata()));
        var withEmptyTags = RenderPage(CreateItem(new SeoMetadata { MetaTags = [] }));

        withEmptyTags.FindAll("meta").Count.ShouldBe(withoutTags.FindAll("meta").Count);
    }

    private IRenderedComponent<SeoMetadataRenderer> RenderPage(ContentItem item)
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo(SeoTestContent.PageUrl);
        return RenderStatic<SeoMetadataRenderer>(parameters => parameters.Add(c => c.Content, item));
    }

    private static ContentItem CreateItem(SeoMetadata seo, string locale = "en")
    {
        var item = SeoTestContent.Article(locale: locale);
        item.Metadata = new FrontMatter { SeoProperties = seo };
        return item;
    }

    private static string? Meta(IRenderedComponent<SeoMetadataRenderer> cut, string name) =>
        cut.FindAll($"meta[name='{name}']").ShouldHaveSingleItem().GetAttribute("content");

    private static string? Property(IRenderedComponent<SeoMetadataRenderer> cut, string property) =>
        cut.FindAll($"meta[property='{property}']").ShouldHaveSingleItem().GetAttribute("content");
}
