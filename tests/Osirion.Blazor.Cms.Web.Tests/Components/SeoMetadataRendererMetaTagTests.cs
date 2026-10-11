using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Options;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

/// <summary>
/// Pins the meta and link tags the renderer writes for each parameter that changes them.
/// </summary>
public class SeoMetadataRendererMetaTagTests : CmsWebComponentTestBase
{
    private const string DefaultRobots = "index, follow, max-snippet:-1, max-image-preview:large, max-video-preview:-1";

    [Fact]
    public void SeoMetadataRenderer_NoContent_RendersNothing()
    {
        var cut = RenderPage(_ => { });

        cut.Markup.ShouldBeEmpty();
    }

    [Fact]
    public void SeoMetadataRenderer_EmptyContentItems_RendersNothing()
    {
        var cut = RenderPage(p => p.Add(c => c.ContentItems, Array.Empty<ContentItem>()));

        cut.Markup.ShouldBeEmpty();
    }

    [Fact]
    public void SeoMetadataRenderer_Article_RendersCoreOpenGraphAndTwitterTags()
    {
        var cut = RenderPage(p => p
            .Add(c => c.Content, SeoTestContent.Article())
            .Add(c => c.SchemaTypes, [SchemaType.Article]));

        Meta(cut, "description").ShouldBe(SeoTestContent.Description);
        Meta(cut, "keywords").ShouldBe("blazor, testing");
        Meta(cut, "author").ShouldBe("Jane Doe");
        Meta(cut, "generator").ShouldBe("Osirion Blazor CMS");
        Meta(cut, "language").ShouldBe("en");
        Meta(cut, "robots").ShouldBe(DefaultRobots);
        Property(cut, "og:title").ShouldBe("Guide to testing Blazor");
        Property(cut, "og:type").ShouldBe("article");
        Property(cut, "og:url").ShouldBe(SeoTestContent.PageUrl);
        Property(cut, "og:site_name").ShouldBe("localhost");
        Property(cut, "og:locale").ShouldBe("en");
        Property(cut, "og:image").ShouldBe("http://localhost/images/cover.png");
        Property(cut, "og:image:secure_url").ShouldBe("http://localhost/images/cover.png");
        Property(cut, "og:image:alt").ShouldBe("Guide to testing Blazor");
        Property(cut, "og:image:width").ShouldBe("1200");
        Property(cut, "og:image:height").ShouldBe("630");
        Property(cut, "article:modified_time").ShouldBe("2020-02-03T04:05:06Z");
        Property(cut, "article:author").ShouldBe("Jane Doe");
        Property(cut, "article:section").ShouldBe("Guides");
        cut.FindAll("meta[property='article:tag']").Select(tag => tag.GetAttribute("content")).ShouldBe(["blazor", "testing"]);
        Meta(cut, "twitter:card").ShouldBe("summary_large_image");
        Meta(cut, "twitter:title").ShouldBe("Guide to testing Blazor");
        Meta(cut, "twitter:image:alt").ShouldBe("Guide to testing Blazor");
        Meta(cut, "publish_date").ShouldBe("2020-01-02");
        Meta(cut, "last_modified").ShouldBe("2020-02-03");
        Meta(cut, "reading_time").ShouldBe("1 minutes");
        Meta(cut, "estimated_reading_time").ShouldBe("1");
        cut.Find("link[rel='preload']").GetAttribute("href").ShouldBe("http://localhost/images/cover.png");
        cut.Find("link[rel='dns-prefetch']").GetAttribute("href").ShouldBe(SeoTestContent.Origin);
    }

    [Fact]
    public void SeoMetadataRenderer_SocialParameters_RenderTwitterFacebookAndEncodedSiteName()
    {
        var cut = RenderPage(p => p
            .Add(c => c.Content, SeoTestContent.Article())
            .Add(c => c.TwitterSite, "@site")
            .Add(c => c.TwitterCreator, "@me")
            .Add(c => c.FacebookAppId, "123")
            .Add(c => c.SiteNameOverride, "My <Site>"));

        Meta(cut, "twitter:site").ShouldBe("@site");
        Meta(cut, "twitter:creator").ShouldBe("@me");
        Property(cut, "fb:app_id").ShouldBe("123");
        Property(cut, "og:site_name").ShouldBe("My <Site>");
        cut.Markup.ShouldContain("content=\"My &lt;Site&gt;\"");
    }

    [Fact]
    public void SeoMetadataRenderer_FrontMatterSeoProperties_OverrideTheContentValues()
    {
        var item = SeoTestContent.Article();
        item.Metadata = new FrontMatter
        {
            SeoProperties = new SeoMetadata
            {
                Title = "SEO title",
                Description = "SEO description",
                OgTitle = "OG title",
                OgDescription = "OG description",
                OgImageUrl = "https://cdn.example.com/og.png",
                OgType = "product",
                Type = "Product",
                TwitterCard = "summary",
                TwitterTitle = "TW title",
                TwitterDescription = "TW description",
                TwitterImageUrl = "/tw.png",
                Canonical = "https://example.com/canon",
            }
        };

        var cut = RenderPage(p => p.Add(c => c.Content, item));

        Meta(cut, "description").ShouldBe("SEO description");
        Meta(cut, "robots").ShouldBe("index, follow");
        Property(cut, "og:title").ShouldBe("OG title");
        Property(cut, "og:description").ShouldBe("OG description");
        Property(cut, "og:type").ShouldBe("product");
        Property(cut, "og:image").ShouldBe("https://cdn.example.com/og.png");
        Meta(cut, "twitter:card").ShouldBe("summary");
        Meta(cut, "twitter:title").ShouldBe("TW title");
        Meta(cut, "twitter:description").ShouldBe("TW description");
        Meta(cut, "twitter:image").ShouldBe("http://localhost/tw.png");
        Meta(cut, "content-type").ShouldBe("Product");
        cut.FindAll("meta[property='article:published_time']").ShouldBeEmpty();
    }

    [Fact]
    public void SeoMetadataRenderer_SearchIndexingNotAllowed_RendersNoIndexForEveryRobot()
    {
        var cut = RenderPage(p => p
            .Add(c => c.Content, SeoTestContent.Article())
            .Add(c => c.AllowSearchIndexing, false));

        Meta(cut, "robots").ShouldBe("noindex, nofollow");
        Meta(cut, "googlebot").ShouldBe("noindex, nofollow");
        Meta(cut, "bingbot").ShouldBe("noindex, nofollow");
    }

    [Fact]
    public void SeoMetadataRenderer_AiDiscoveryNotAllowed_RendersTheOptOutTags()
    {
        var cut = RenderPage(p => p
            .Add(c => c.Content, SeoTestContent.Article())
            .Add(c => c.AllowAiDiscovery, false));

        Meta(cut, "ai-crawling").ShouldBe("none");
        Meta(cut, "ai-training").ShouldBe("disallow");
        Meta(cut, "ChatGPT-User").ShouldBe("noindex");
        Meta(cut, "Google-Extended").ShouldBe("noindex");
        cut.FindAll("meta[name='ai-content-declaration']").ShouldBeEmpty();
    }

    [Fact]
    public void SeoMetadataRenderer_AiTrainingNotAllowed_AllowsDiscoveryButNotTraining()
    {
        var cut = RenderPage(p => p
            .Add(c => c.Content, SeoTestContent.Article())
            .Add(c => c.AllowAiTraining, false));

        Meta(cut, "ai-crawling").ShouldBe("all");
        Meta(cut, "ai-training").ShouldBe("disallow");
        Meta(cut, "Google-Extended").ShouldBe("noindex");
    }

    [Fact]
    public void SeoMetadataRenderer_Defaults_AllowAiDiscoveryAndTraining()
    {
        var cut = RenderPage(p => p.Add(c => c.Content, SeoTestContent.Article()));

        Meta(cut, "ai-content-declaration").ShouldBe("human-created");
        Meta(cut, "ai-training").ShouldBe("allow");
        Meta(cut, "Google-Extended").ShouldBe("allow");
    }

    [Fact]
    public void SeoMetadataRenderer_SeoOptions_FillTheParametersThatAreNotSet()
    {
        Services.AddSingleton(Microsoft.Extensions.Options.Options.Create(CreateOptions()));

        var cut = RenderPage(p => p.Add(c => c.Content, SeoTestContent.Article()));

        Property(cut, "og:site_name").ShouldBe("Opt Site");
        Property(cut, "og:image:width").ShouldBe("800");
        Property(cut, "og:image:height").ShouldBe("400");
        Property(cut, "og:type").ShouldBe("article");
        Property(cut, "fb:app_id").ShouldBe("999");
        Meta(cut, "twitter:site").ShouldBe("@opt");
        Meta(cut, "twitter:creator").ShouldBe("@optc");
        Meta(cut, "content-type").ShouldBe("BlogPosting");
        Meta(cut, "answers-to").ShouldBe("How to testing Blazor?");
        Meta(cut, "apple-mobile-web-app-title").ShouldBe("Opt Site");
        cut.FindAll("script[type='application/ld+json']").Count.ShouldBe(3);
    }

    [Fact]
    public void SeoMetadataRenderer_SeoOptions_DoNotReplaceExplicitParameters()
    {
        Services.AddSingleton(Microsoft.Extensions.Options.Options.Create(CreateOptions()));

        var cut = RenderPage(p => p
            .Add(c => c.Content, SeoTestContent.Article())
            .Add(c => c.SiteNameOverride, "Mine")
            .Add(c => c.BreadcrumbHomeText, " ")
            .Add(c => c.DefaultImageWidth, 640)
            .Add(c => c.DefaultImageHeight, 480)
            .Add(c => c.SchemaTypes, [SchemaType.Course]));

        Property(cut, "og:site_name").ShouldBe("Mine");
        Property(cut, "og:image:width").ShouldBe("640");
        Property(cut, "og:image:height").ShouldBe("480");
        Meta(cut, "content-type").ShouldBe("Course");
        cut.Markup.ShouldContain("\"name\": \"Start\"");
    }

    [Fact]
    public void SeoMetadataRenderer_LongTitleAndDescription_AreTruncatedAtWordBoundaries()
    {
        var item = SeoTestContent.Article(title: "This is a very long title that keeps going well beyond the sixty character limit for titles");
        item.Description = new string('x', 170) + " " + string.Join(" ", Enumerable.Repeat("word", 60))
            + ". End sentence here. More words follow " + new string('y', 300);

        var cut = RenderPage(p => p.Add(c => c.Content, item));

        Property(cut, "og:title").ShouldBe("This is a very long title that keeps going well beyond the...");
        Meta(cut, "twitter:title").ShouldBe("This is a very long title that keeps going well beyond the...");
        Meta(cut, "description").ShouldBe(new string('x', 160) + "...");
        Property(cut, "og:description").ShouldBe(new string('x', 160) + "...");
        Meta(cut, "entity-summary").ShouldBe(new string('x', 170) + " " + string.Join(" ", Enumerable.Repeat("word", 21)) + "...");
    }

    [Fact]
    public void SeoMetadataRenderer_DescriptionWithSentenceNearTheLimit_EndsAtTheSentence()
    {
        var item = SeoTestContent.Article();
        item.Description = new string('a', 140) + ". Then a second sentence that runs past the limit.";

        var cut = RenderPage(p => p.Add(c => c.Content, item));

        Meta(cut, "description").ShouldBe(new string('a', 140) + ".");
    }

    [Fact]
    public void SeoMetadataRenderer_AbsoluteImageUrl_IsKeptAsIs()
    {
        var item = SeoTestContent.Article();
        item.FeaturedImageUrl = "https://cdn.example.com/a.png";

        var cut = RenderPage(p => p.Add(c => c.Content, item));

        Property(cut, "og:image").ShouldBe("https://cdn.example.com/a.png");
    }

    [Fact]
    public void SeoMetadataRenderer_NoFeaturedImage_UsesTheDefaultImageMadeAbsolute()
    {
        var item = SeoTestContent.Article();
        item.FeaturedImageUrl = null;

        var cut = RenderPage(p => p
            .Add(c => c.Content, item)
            .Add(c => c.DefaultImageUrl, "img/default.png"));

        Property(cut, "og:image").ShouldBe("http://localhost/img/default.png");
    }

    [Fact]
    public void SeoMetadataRenderer_NoImageAtAll_RendersNoImageTags()
    {
        var item = SeoTestContent.Article();
        item.FeaturedImageUrl = null;

        var cut = RenderPage(p => p.Add(c => c.Content, item));

        cut.FindAll("meta[property^='og:image']").ShouldBeEmpty();
        cut.FindAll("meta[name='twitter:image']").ShouldBeEmpty();
        cut.FindAll("link[rel='preload']").ShouldBeEmpty();
    }

    [Fact]
    public void SeoMetadataRenderer_PaginationUrls_RenderPrevAndNextLinks()
    {
        var cut = RenderPage(p => p
            .Add(c => c.Content, SeoTestContent.Article())
            .Add(c => c.PrevPageUrl, "/p1")
            .Add(c => c.NextPageUrl, "/p3"));

        cut.Find("link[rel='prev']").GetAttribute("href").ShouldBe("/p1");
        cut.Find("link[rel='next']").GetAttribute("href").ShouldBe("/p3");
    }

    [Fact]
    public void SeoMetadataRenderer_AlternateLanguageUrls_RenderTrimmedHreflangLinks()
    {
        var cut = RenderPage(p => p
            .Add(c => c.Content, SeoTestContent.Article())
            .Add(c => c.AlternateLanguageUrls, ["sr|http://localhost/sr/x", "de | http://localhost/de/x "]));

        cut.FindAll("link[rel='alternate']")
            .Select(link => (link.GetAttribute("hreflang"), link.GetAttribute("href")))
            .ShouldBe([("sr", "http://localhost/sr/x"), ("de", "http://localhost/de/x")]);
    }

    [Fact]
    public void SeoMetadataRenderer_GeoOptimizationOff_RendersNoEntitySummary()
    {
        var cut = RenderPage(p => p
            .Add(c => c.Content, SeoTestContent.Article())
            .Add(c => c.EnableGeoOptimization, false));

        cut.FindAll("meta[name='entity-summary']").ShouldBeEmpty();
        cut.FindAll("meta[name='content-type']").ShouldBeEmpty();
    }

    [Fact]
    public void SeoMetadataRenderer_GeoOptimization_RendersEntityClassificationTags()
    {
        var cut = RenderPage(p => p.Add(c => c.Content, SeoTestContent.Article()));

        Meta(cut, "entity-summary").ShouldBe(SeoTestContent.Description);
        Meta(cut, "content-type").ShouldBe("WebPage");
        Meta(cut, "primary-category").ShouldBe("Guides");
        Meta(cut, "content-author").ShouldBe("Jane Doe");
        Meta(cut, "content-freshness").ShouldBe("evergreen");
    }

    [Fact]
    public void SeoMetadataRenderer_AeoOptimizationOff_IgnoresGeneratedFaq()
    {
        var cut = RenderPage(p => p
            .Add(c => c.Content, SeoTestContent.Article())
            .Add(c => c.EnableAeoOptimization, false)
            .Add(c => c.EnableGeneratedFaq, true));

        cut.FindAll("meta[name='answers-to']").ShouldBeEmpty();
        cut.Markup.ShouldNotContain("FAQPage");
    }

    [Fact]
    public void SeoMetadataRenderer_ContentWithoutAuthorOrDescription_LeavesOutThoseTags()
    {
        var item = SeoTestContent.Article();
        item.Author = string.Empty;
        item.Description = string.Empty;

        var cut = RenderPage(p => p.Add(c => c.Content, item));

        cut.FindAll("meta[name='author']").ShouldBeEmpty();
        cut.FindAll("meta[name='description']").ShouldBeEmpty();
        cut.FindAll("meta[name='content-author']").ShouldBeEmpty();
        cut.FindAll("meta[name='entity-summary']").ShouldBeEmpty();
    }

    [Fact]
    public void SeoMetadataRenderer_ContentLocale_SetsLanguageAndOpenGraphLocale()
    {
        var cut = RenderPage(p => p.Add(c => c.Content, SeoTestContent.Article(locale: "sr-Latn-RS")));

        Meta(cut, "language").ShouldBe("sr-Latn-RS");
        Property(cut, "og:locale").ShouldBe("sr_Latn_RS");
    }

    [Fact]
    public void SeoMetadataRenderer_EmptyLocale_DefaultsTheOpenGraphLocale()
    {
        var cut = RenderPage(p => p.Add(c => c.Content, SeoTestContent.Article(locale: string.Empty)));

        Property(cut, "og:locale").ShouldBe("en_US");
    }

    [Fact]
    public void SeoMetadataRenderer_WwwHost_SiteNameLeavesOutWww()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("http://www.localhost/blog/x");

        var cut = RenderStatic<SeoMetadataRenderer>(p => p.Add(c => c.Content, SeoTestContent.Article()));

        Property(cut, "og:site_name").ShouldBe("localhost");
        Property(cut, "og:image").ShouldBe("http://www.localhost/images/cover.png");
    }

    [Fact]
    public void SeoMetadataRenderer_EmptySchemaTypes_DerivesOgTypeFromTheDefaultSchemaType()
    {
        var cut = RenderPage(p => p
            .Add(c => c.Content, SeoTestContent.Article())
            .Add(c => c.SchemaTypes, []));

        Property(cut, "og:type").ShouldBe("article");
        Meta(cut, "content-type").ShouldBe("Article");
    }

    [Theory]
    [InlineData(SchemaType.Product, "product")]
    [InlineData(SchemaType.VideoObject, "video.other")]
    [InlineData(SchemaType.BlogPosting, "article")]
    [InlineData(SchemaType.Event, "website")]
    public void SeoMetadataRenderer_PrimarySchemaType_SetsTheOgType(SchemaType schemaType, string expectedOgType)
    {
        var cut = RenderPage(p => p
            .Add(c => c.Content, SeoTestContent.Article())
            .Add(c => c.SchemaTypes, [schemaType]));

        Property(cut, "og:type").ShouldBe(expectedOgType);
    }

    [Fact]
    public void SeoMetadataRenderer_CollectionPage_UsesTheCollectionValues()
    {
        var cut = RenderPage(p => p
            .Add(c => c.ContentItems, [SeoTestContent.Article(), SeoTestContent.BarePost()])
            .Add(c => c.CollectionTitle, "All posts")
            .Add(c => c.CollectionDescription, "Every post on the site."));

        Meta(cut, "description").ShouldBe("Every post on the site.");
        Meta(cut, "keywords").ShouldBe("blazor, testing");
        Meta(cut, "author").ShouldBe("Jane Doe");
        Property(cut, "og:title").ShouldBe("All posts");
        Property(cut, "og:type").ShouldBe("website");
        Property(cut, "og:locale").ShouldBe("en_US");
        Property(cut, "og:image").ShouldBe("http://localhost/images/cover.png");
        Property(cut, "og:image:alt").ShouldBe("All posts");
        Meta(cut, "twitter:title").ShouldBe("All posts");
        Meta(cut, "last_modified").ShouldBe("2020-02-03");
        Meta(cut, "primary-category").ShouldBe("Guides");
        cut.FindAll("meta[name='reading_time']").ShouldBeEmpty();
    }

    [Fact]
    public void SeoMetadataRenderer_CollectionWithoutTitle_FallsBackToTheSiteName()
    {
        var cut = RenderPage(p => p
            .Add(c => c.ContentItems, [SeoTestContent.BarePost()])
            .Add(c => c.DefaultImageUrl, "/default.png"));

        Property(cut, "og:title").ShouldBe("localhost");
        Property(cut, "og:image").ShouldBe("http://localhost/default.png");
        Property(cut, "og:image:alt").ShouldBe("localhost");
        cut.FindAll("meta[name='description']").ShouldBeEmpty();
    }

    private IRenderedComponent<SeoMetadataRenderer> RenderPage(Action<ComponentParameterCollectionBuilder<SeoMetadataRenderer>> parameters)
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo(SeoTestContent.PageUrl);
        return RenderStatic(parameters);
    }

    private static string? Meta(IRenderedComponent<SeoMetadataRenderer> cut, string name) =>
        cut.FindAll($"meta[name='{name}']").ShouldHaveSingleItem().GetAttribute("content");

    private static string? Property(IRenderedComponent<SeoMetadataRenderer> cut, string property) =>
        cut.FindAll($"meta[property='{property}']").ShouldHaveSingleItem().GetAttribute("content");

    private static SeoMetadataOptions CreateOptions() => new()
    {
        SiteName = "Opt Site",
        SiteDescription = "desc",
        SiteLogoUrl = "https://x/logo.png",
        TwitterSite = "@opt",
        TwitterCreator = "@optc",
        FacebookAppId = "999",
        DefaultImageUrl = "/default.png",
        OrganizationName = "Org",
        BreadcrumbHomeText = "Start",
        DefaultImageWidth = 800,
        DefaultImageHeight = 400,
        EmitMobileMetaTags = true,
        EnableGeneratedFaq = true,
        DefaultSchemaTypes = [SchemaType.BlogPosting, SchemaType.WebSite],
    };
}
