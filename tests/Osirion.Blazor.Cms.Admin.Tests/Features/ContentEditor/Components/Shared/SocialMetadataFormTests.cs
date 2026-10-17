using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentEditor.Components.Shared;

public class SocialMetadataFormTests : AdminComponentTestContext
{
    public SocialMetadataFormTests()
    {
        RenderInteractively();
    }

    [Fact]
    public void SocialMetadataForm_StaticRenderWithoutImage_OmitsPreview()
    {
        SetRendererInfo(new RendererInfo("Static", false));

        var cut = Render<SocialMetadataForm>();

        cut.Find("#ogTitle").ShouldNotBeNull();
        cut.Find("#twitterCard").ShouldNotBeNull();
        cut.FindAll("img.social-image-preview").ShouldBeEmpty();
    }

    [Fact]
    public void SocialMetadataForm_WithOgImage_ShowsPreview()
    {
        var seo = SeoMetadata.Create("t", "d");
        seo.OgImageUrl = "https://example.com/og.png";

        var cut = Render<SocialMetadataForm>(parameters => parameters.Add(p => p.SeoMetadata, seo));

        cut.Find("img.social-image-preview").GetAttribute("src").ShouldBe("https://example.com/og.png");
    }

    [Fact]
    public void SocialMetadataForm_NullMetadata_FallsBackToEmpty()
    {
        var cut = Render<SocialMetadataForm>(parameters => parameters.Add(p => p.SeoMetadata, null!));

        cut.Instance.SeoMetadata.ShouldNotBeNull();
        cut.Find("#ogTitle").GetAttribute("value").ShouldBe(string.Empty);
    }

    [Fact]
    public void SocialMetadataForm_EditingFields_RaisesChangedWithNewValues()
    {
        SeoMetadata? changed = null;
        var cut = Render<SocialMetadataForm>(parameters => parameters
            .Add(p => p.SeoMetadata, SeoMetadata.Create("t", "d"))
            .Add(p => p.SeoMetadataChanged, (SeoMetadata value) => changed = value));

        cut.Find("#ogTitle").Input("OG title");
        cut.Find("#ogDescription").Input("OG description");
        cut.Find("#ogImageUrl").Input("https://example.com/a.png");
        cut.Find("#ogType").Change("website");
        cut.Find("#twitterCard").Change("summary");
        cut.Find("#twitterTitle").Input("Tw title");
        cut.Find("#twitterDescription").Input("Tw description");
        cut.Find("#twitterImageUrl").Input("https://example.com/t.png");

        changed.ShouldNotBeNull();
        changed.OgTitle.ShouldBe("OG title");
        changed.OgDescription.ShouldBe("OG description");
        changed.OgImageUrl.ShouldBe("https://example.com/a.png");
        changed.OgType.ShouldBe("website");
        changed.TwitterCard.ShouldBe("summary");
        changed.TwitterTitle.ShouldBe("Tw title");
        changed.TwitterDescription.ShouldBe("Tw description");
        changed.TwitterImageUrl.ShouldBe("https://example.com/t.png");
    }

    [Fact]
    public void SocialMetadataForm_EditingWithoutListener_UpdatesBoundValue()
    {
        var seo = SeoMetadata.Create("t", "d");
        var cut = Render<SocialMetadataForm>(parameters => parameters.Add(p => p.SeoMetadata, seo));

        cut.Find("#ogTitle").Input("only local");

        seo.OgTitle.ShouldBe("only local");
    }

    [Fact]
    public void SocialMetadataForm_EmptyOgType_OffersADefaultOption()
    {
        // Fails before the fix: the select had no empty option, so an empty OgType showed "Article" and could not be
        // chosen again once a type was picked.
        var cut = Render<SocialMetadataForm>(parameters => parameters.Add(p => p.SeoMetadata, SeoMetadata.Create("t", "d")));

        var options = cut.FindAll("#ogType option");
        options[0].GetAttribute("value").ShouldBe(string.Empty);
        options[0].TextContent.ShouldContain("Default");
        cut.Find("#ogType").GetAttribute("value").ShouldBe(string.Empty);
    }
}
