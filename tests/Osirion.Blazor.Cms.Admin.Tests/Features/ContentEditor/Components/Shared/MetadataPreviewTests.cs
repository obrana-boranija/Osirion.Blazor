using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentEditor.Components.Shared;

public class MetadataPreviewTests : AdminComponentTestContext
{
    public MetadataPreviewTests()
    {
        RenderInteractively();
    }

    private IRenderedComponent<MetadataPreview> RenderPreview(FrontMatter metadata, SeoMetadata seo) =>
        Render<MetadataPreview>(parameters => parameters
            .Add(p => p.Metadata, metadata)
            .Add(p => p.SeoMetadata, seo));

    private static string MetaTags(IRenderedComponent<MetadataPreview> cut) =>
        cut.FindAll("pre.code-preview")[1].TextContent;

    [Fact]
    public void MetadataPreview_StaticRenderWithEmptySeo_FallsBackToFrontMatter()
    {
        SetRendererInfo(new RendererInfo("Static", false));
        var metadata = FrontMatter.Create("My Post", "About it", new DateTime(2024, 1, 15));

        var cut = RenderPreview(metadata, SeoMetadata.Create("", ""));

        cut.FindAll("pre.code-preview")[0].TextContent.ShouldContain("My Post");
        var tags = MetaTags(cut);
        tags.ShouldContain("<title>My Post</title>");
        tags.ShouldContain("<meta name=\"description\" content=\"About it\" />");
        tags.ShouldContain("<meta property=\"og:title\" content=\"My Post\" />");
        tags.ShouldContain("<meta name=\"twitter:title\" content=\"My Post\" />");
        tags.ShouldContain("<meta property=\"og:url\" content=\"https://example.com/my-post\" />");
        tags.ShouldNotContain("canonical");
        tags.ShouldNotContain("og:image");
        tags.ShouldNotContain("twitter:image");
        tags.ShouldNotContain("ld+json");
    }

    [Fact]
    public void MetadataPreview_WithFullSeo_RendersEveryTag()
    {
        var metadata = FrontMatter.Create("Post");
        var seo = SeoMetadata.Create("Seo Title", "Seo Desc");
        seo.Canonical = "https://site.test/post";
        seo.OgTitle = "Og Title";
        seo.OgDescription = "Og Desc";
        seo.OgImageUrl = "https://site.test/og.png";
        seo.TwitterTitle = "Tw Title";
        seo.TwitterDescription = "Tw Desc";
        seo.TwitterImageUrl = "https://site.test/tw.png";
        seo.JsonLd = "{\"a\":1}";

        var tags = MetaTags(RenderPreview(metadata, seo));

        tags.ShouldContain("<title>Seo Title</title>");
        tags.ShouldContain("<link rel=\"canonical\" href=\"https://site.test/post\" />");
        tags.ShouldContain("<meta property=\"og:url\" content=\"https://site.test/post\" />");
        tags.ShouldContain("og:title\" content=\"Og Title\"");
        tags.ShouldContain("og:description\" content=\"Og Desc\"");
        tags.ShouldContain("og:image\" content=\"https://site.test/og.png\"");
        tags.ShouldContain("twitter:title\" content=\"Tw Title\"");
        tags.ShouldContain("twitter:description\" content=\"Tw Desc\"");
        tags.ShouldContain("twitter:image\" content=\"https://site.test/tw.png\"");
        tags.ShouldContain("<script type=\"application/ld+json\">");
        tags.ShouldContain("{\"a\":1}");
    }

    [Fact]
    public void MetadataPreview_FeaturedImageOnly_UsedForOgAndTwitter()
    {
        var metadata = FrontMatter.Create("Post");
        metadata.FeaturedImage = "https://site.test/f.png";
        metadata.Slug = "custom-slug";

        var tags = MetaTags(RenderPreview(metadata, SeoMetadata.Create("", "")));

        tags.ShouldContain("og:image\" content=\"https://site.test/f.png\"");
        tags.ShouldContain("twitter:image\" content=\"https://site.test/f.png\"");
        tags.ShouldContain("og:url\" content=\"https://example.com/custom-slug\"");
    }

    [Fact]
    public void MetadataPreview_LongTitleAndDescription_AreTruncated()
    {
        var seo = SeoMetadata.Create(new string('t', 61), new string('d', 161));

        var tags = MetaTags(RenderPreview(FrontMatter.Create("Post"), seo));

        tags.ShouldContain($"<title>{new string('t', 57)}...</title>");
        tags.ShouldContain($"content=\"{new string('d', 157)}...\"");
    }

    [Fact]
    public void MetadataPreview_NullParameters_FallBackToDefaults()
    {
        var cut = RenderPreview(null!, null!);

        MetaTags(cut).ShouldContain("<title>New Post</title>");
    }

    [Fact]
    public void MetadataPreview_RefreshButton_RaisesRefresh()
    {
        var refreshed = false;
        var cut = Render<MetadataPreview>(parameters => parameters.Add(p => p.OnRefresh, () => refreshed = true));

        cut.Find("button[title='Refresh preview']").Click();

        refreshed.ShouldBeTrue();
    }

    [Fact]
    public void MetadataPreview_RefreshWithoutListener_KeepsPreview()
    {
        var cut = Render<MetadataPreview>();

        cut.Find("button[title='Refresh preview']").Click();

        cut.Find(".metadata-preview").ShouldNotBeNull();
    }

    [Fact]
    public void MetadataPreview_WithoutActions_HidesRefreshButton()
    {
        var cut = Render<MetadataPreview>(parameters => parameters.Add(p => p.ShowActions, false));

        cut.FindAll("button[title='Refresh preview']").ShouldBeEmpty();
    }

    [Fact]
    public void MetadataPreview_EmptyRobotsAndOgType_LeavesTheirTagsOut()
    {
        // Fails before the fix: the preview printed content="" for both, tags no page would carry.
        var cut = RenderPreview(FrontMatter.Create("Post"), SeoMetadata.Create("", ""));

        var tags = MetaTags(cut);
        tags.ShouldNotContain("name=\"robots\"");
        tags.ShouldNotContain("og:type");
    }

    [Fact]
    public void MetadataPreview_RobotsAndOgTypeSet_RendersTheirTags()
    {
        var seo = SeoMetadata.Create("", "");
        seo.Robots = "noindex, follow";
        seo.OgType = "website";

        var cut = RenderPreview(FrontMatter.Create("Post"), seo);

        var tags = MetaTags(cut);
        tags.ShouldContain("<meta name=\"robots\" content=\"noindex, follow\" />");
        tags.ShouldContain("<meta property=\"og:type\" content=\"website\" />");
    }
}
