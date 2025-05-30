using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;

public partial class SeoMetadataForm
{
    [Parameter]
    public SeoMetadata SeoMetadata { get; set; } = SeoMetadata.Create("", "");

    [Parameter]
    public EventCallback<SeoMetadata> SeoMetadataChanged { get; set; }

    protected override void OnParametersSet()
    {
        SeoMetadata ??= SeoMetadata.Create("", "");
    }

    private async Task NotifySeoMetadataChanged()
    {
        if (SeoMetadataChanged.HasDelegate)
        {
            await SeoMetadataChanged.InvokeAsync(SeoMetadata);
        }
    }

    private string GetCounterClass(int current, int max, int warning)
    {
        if (current > max) return "danger";
        if (current > warning) return "warning";
        return "";
    }

    private async Task GenerateJsonLd()
    {
        // Generate basic JSON-LD based on schema type and available metadata
        var jsonLd = SeoMetadata.SchemaType switch
        {
            "Article" or "BlogPosting" => GenerateArticleJsonLd(),
            "Product" => GenerateProductJsonLd(),
            "Event" => GenerateEventJsonLd(),
            "Organization" => GenerateOrganizationJsonLd(),
            "Person" => GeneratePersonJsonLd(),
            _ => GenerateWebPageJsonLd()
        };

        SeoMetadata = SeoMetadata.WithJsonLd(jsonLd);
        await NotifySeoMetadataChanged();
    }

    private string GenerateArticleJsonLd()
    {
        var article = new
        {
            @context = "https://schema.org",
            @type = SeoMetadata.SchemaType,
            headline = SeoMetadata.MetaTitle,
            description = SeoMetadata.MetaDescription,
            image = SeoMetadata.OgImageUrl,
            url = SeoMetadata.CanonicalUrl,
            datePublished = DateTime.Now.ToString("yyyy-MM-dd"),
            dateModified = DateTime.Now.ToString("yyyy-MM-dd"),
            author = new
            {
                @type = "Person",
                name = "Author Name" // This should come from FrontMatter
            },
            publisher = new
            {
                @type = "Organization",
                name = "Your Site Name",
                logo = new
                {
                    @type = "ImageObject",
                    url = "https://example.com/logo.png"
                }
            }
        };

        return JsonSerializer.Serialize(article, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private string GenerateProductJsonLd()
    {
        var product = new
        {
            @context = "https://schema.org",
            @type = "Product",
            name = SeoMetadata.MetaTitle,
            description = SeoMetadata.MetaDescription,
            image = SeoMetadata.OgImageUrl,
            url = SeoMetadata.CanonicalUrl,
            offers = new
            {
                @type = "Offer",
                price = "0.00",
                priceCurrency = "USD",
                availability = "https://schema.org/InStock"
            }
        };

        return JsonSerializer.Serialize(product, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private string GenerateEventJsonLd()
    {
        var eventData = new
        {
            @context = "https://schema.org",
            @type = "Event",
            name = SeoMetadata.MetaTitle,
            description = SeoMetadata.MetaDescription,
            image = SeoMetadata.OgImageUrl,
            url = SeoMetadata.CanonicalUrl,
            startDate = DateTime.Now.ToString("yyyy-MM-dd"),
            endDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
            location = new
            {
                @type = "Place",
                name = "Event Location",
                address = new
                {
                    @type = "PostalAddress",
                    streetAddress = "123 Main St",
                    addressLocality = "City",
                    addressRegion = "State",
                    postalCode = "12345",
                    addressCountry = "US"
                }
            }
        };

        return JsonSerializer.Serialize(eventData, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private string GenerateOrganizationJsonLd()
    {
        var organization = new
        {
            @context = "https://schema.org",
            @type = "Organization",
            name = SeoMetadata.MetaTitle,
            description = SeoMetadata.MetaDescription,
            url = SeoMetadata.CanonicalUrl,
            logo = SeoMetadata.OgImageUrl,
            sameAs = new[]
            {
                "https://facebook.com/yourpage",
                "https://twitter.com/yourhandle",
                "https://linkedin.com/company/yourcompany"
            }
        };

        return JsonSerializer.Serialize(organization, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private string GeneratePersonJsonLd()
    {
        var person = new
        {
            @context = "https://schema.org",
            @type = "Person",
            name = SeoMetadata.MetaTitle,
            description = SeoMetadata.MetaDescription,
            url = SeoMetadata.CanonicalUrl,
            image = SeoMetadata.OgImageUrl,
            sameAs = new[]
            {
                "https://twitter.com/yourhandle",
                "https://linkedin.com/in/yourprofile"
            }
        };

        return JsonSerializer.Serialize(person, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private string GenerateWebPageJsonLd()
    {
        var webPage = new
        {
            @context = "https://schema.org",
            @type = "WebPage",
            name = SeoMetadata.MetaTitle,
            description = SeoMetadata.MetaDescription,
            url = SeoMetadata.CanonicalUrl
        };

        return JsonSerializer.Serialize(webPage, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}