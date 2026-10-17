using System.Text.Json.Nodes;
using Bunit;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class SeoMetadataRendererPageTypeTests : CmsWebComponentTestBase
{
    [Fact]
    public void SeoMetadataRenderer_TechArticle_EmitsTheArticleSchemaWithTheTechArticleTypeAndOgArticle()
    {
        var cut = Render(CreatePage("Tabs"), SchemaType.TechArticle);

        var schema = FindSchema(cut, "TechArticle");
        schema["headline"]!.GetValue<string>().ShouldBe("Tabs");
        schema["datePublished"].ShouldNotBeNull();
    }

    // Fails before the fix: SchemaType.BlogPosting wrote "@type": "Article".
    [Fact]
    public void SeoMetadataRenderer_BlogPosting_EmitsTheBlogPostingType()
    {
        var cut = Render(CreatePage("Post"), SchemaType.BlogPosting);

        FindSchema(cut, "BlogPosting")["headline"]!.GetValue<string>().ShouldBe("Post");
    }

    [Fact]
    public void SeoMetadataRenderer_SoftwareApplication_UsesTheApplicationCategory()
    {
        var cut = RenderWithCategory(CreatePage("Osirion.Blazor"), SchemaType.SoftwareApplication, "BusinessApplication");

        var schema = FindSchema(cut, "SoftwareApplication");
        schema["name"]!.GetValue<string>().ShouldBe("Osirion.Blazor");
        schema["applicationCategory"]!.GetValue<string>().ShouldBe("BusinessApplication");
    }

    [Fact]
    public void SeoMetadataRenderer_SoftwareApplication_DefaultsToDeveloperApplication()
    {
        var cut = Render(CreatePage("Osirion.Blazor"), SchemaType.SoftwareApplication);

        FindSchema(cut, "SoftwareApplication")["applicationCategory"]!.GetValue<string>().ShouldBe("DeveloperApplication");
    }

    [Fact]
    public void SeoMetadataRenderer_ProfilePage_DescribesTheAuthorWithLinkTagsAsSameAs()
    {
        var item = CreatePage("About Ana");
        item.SetAuthor("Ana Author");
        item.AddTag("https://github.com/ana");
        item.AddTag("blazor");

        var cut = Render(item, SchemaType.ProfilePage);

        var person = FindSchema(cut, "ProfilePage")["mainEntity"]!;
        person["@type"]!.GetValue<string>().ShouldBe("Person");
        person["name"]!.GetValue<string>().ShouldBe("Ana Author");
        person["sameAs"]!.AsArray().Select(node => node!.GetValue<string>()).ShouldBe(["https://github.com/ana"]);
    }

    // Front matter SEO properties carry og:type (default "article"); without them the schema type decides.
    [Theory]
    [InlineData(SchemaType.ProfilePage, "profile")]
    [InlineData(SchemaType.TechArticle, "article")]
    [InlineData(SchemaType.ContactPage, "website")]
    public void SeoMetadataRenderer_NoSeoFrontMatter_OgTypeFollowsTheSchemaType(SchemaType type, string expected)
    {
        var item = CreatePage("Page");
        item.Metadata = null;

        var cut = Render(item, type);

        cut.Find("meta[property='og:type']").GetAttribute("content").ShouldBe(expected);
    }

    [Fact]
    public void SeoMetadataRenderer_AboutPage_HasTheSiteOrganizationAsMainEntity()
    {
        var cut = RenderStatic<SeoMetadataRenderer>(p => p
            .Add(c => c.Content, CreatePage("About"))
            .Add(c => c.SchemaTypes, [SchemaType.AboutPage])
            .Add(c => c.SiteNameOverride, "Osirion"));

        var organization = FindSchema(cut, "AboutPage")["mainEntity"]!;
        organization["@type"]!.GetValue<string>().ShouldBe("Organization");
        organization["name"]!.GetValue<string>().ShouldBe("Osirion");
    }

    [Fact]
    public void SeoMetadataRenderer_ContactPage_EmitsAContactPage()
    {
        var cut = Render(CreatePage("Contact"), SchemaType.ContactPage);

        FindSchema(cut, "ContactPage")["name"]!.GetValue<string>().ShouldBe("Contact");
    }

    [Fact]
    public void SeoMetadataRenderer_TypeFromFrontMatter_IsParsed()
    {
        var item = CreatePage("Install");
        item.Metadata!.SeoProperties!.Type = "TechArticle";

        var cut = RenderStatic<SeoMetadataRenderer>(p => p.Add(c => c.Content, item));

        FindSchema(cut, "TechArticle").ShouldNotBeNull();
    }

    // Fails before the fix: the front matter type was parsed with a case-sensitive Enum.TryParse, so
    // "techarticle" silently became a WebPage.
    [Theory]
    [InlineData("techarticle")]
    [InlineData("TECHARTICLE")]
    public void SeoMetadataRenderer_TypeFromFrontMatterInAnyCase_IsParsed(string declaredType)
    {
        var item = CreatePage("Install");
        item.Metadata!.SeoProperties!.Type = declaredType;

        var cut = RenderStatic<SeoMetadataRenderer>(p => p.Add(c => c.Content, item));

        FindSchema(cut, "TechArticle").ShouldNotBeNull();
    }

    private IRenderedComponent<SeoMetadataRenderer> Render(ContentItem item, SchemaType type) =>
        RenderStatic<SeoMetadataRenderer>(p => p
            .Add(c => c.Content, item)
            .Add(c => c.SchemaTypes, [type]));

    private IRenderedComponent<SeoMetadataRenderer> RenderWithCategory(ContentItem item, SchemaType type, string category) =>
        RenderStatic<SeoMetadataRenderer>(p => p
            .Add(c => c.Content, item)
            .Add(c => c.SchemaTypes, [type])
            .Add(c => c.ApplicationCategory, category));

    private static JsonNode FindSchema(IRenderedComponent<SeoMetadataRenderer> cut, string type) =>
        cut.FindAll("script[type='application/ld+json']")
            .Select(script => JsonNode.Parse(script.TextContent))
            .Single(node => node?["@type"]?.GetValue<string>() == type)!;

    private static ContentItem CreatePage(string title)
    {
        var item = ContentItem.Create($"id-{title}", title, "<p>Body</p>", $"docs/{title}.md", "test");
        item.SetUrl($"/docs/{title.ToLowerInvariant()}");
        item.SetDescription("Summary");
        item.Metadata = new FrontMatter
        {
            Title = title,
            SeoProperties = new SeoMetadata()
        };

        return item;
    }
}
