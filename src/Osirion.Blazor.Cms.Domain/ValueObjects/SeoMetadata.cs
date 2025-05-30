using Osirion.Blazor.Cms.Domain.Common;

namespace Osirion.Blazor.Cms.Domain.ValueObjects;

/// <summary>
/// Represents SEO metadata for content items
/// </summary>
public class SeoMetadata : ValueObject
{
    /// <summary>
    /// Gets the meta title (SEO title)
    /// </summary>
    public string MetaTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets the meta description
    /// </summary>
    public string MetaDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets the canonical URL
    /// </summary>
    public string CanonicalUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets the robots meta directive
    /// </summary>
    public string Robots { get; set; } = "index, follow";

    /// <summary>
    /// Gets the Open Graph title
    /// </summary>
    public string OgTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets the Open Graph description
    /// </summary>
    public string OgDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets the Open Graph image URL
    /// </summary>
    public string OgImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets the Open Graph type
    /// </summary>
    public string OgType { get; set; } = "article";

    /// <summary>
    /// Gets the JSON-LD structured data
    /// </summary>
    public string JsonLd { get; set; } = string.Empty;

    /// <summary>
    /// Gets the schema.org type
    /// </summary>
    public string SchemaType { get; set; } = "Article";

    /// <summary>
    /// Gets the Twitter card type
    /// </summary>
    public string TwitterCard { get; set; } = "summary_large_image";

    /// <summary>
    /// Gets the Twitter title
    /// </summary>
    public string TwitterTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets the Twitter description
    /// </summary>
    public string TwitterDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets the Twitter image URL
    /// </summary>
    public string TwitterImageUrl { get; set; } = string.Empty;

    // Factory method for creating a populated instance
    public static SeoMetadata Create(
        string metaTitle,
        string metaDescription,
        string? canonicalUrl = null,
        string? robots = null,
        string? ogTitle = null,
        string? ogDescription = null,
        string? ogImageUrl = null,
        string? ogType = null,
        string? jsonLd = null,
        string? schemaType = null,
        string? twitterCard = null,
        string? twitterTitle = null,
        string? twitterDescription = null,
        string? twitterImageUrl = null)
    {
        var metadata = new SeoMetadata
        {
            MetaTitle = metaTitle,
            MetaDescription = metaDescription,
            CanonicalUrl = canonicalUrl ?? string.Empty,
            Robots = robots ?? "index, follow",
            OgTitle = ogTitle ?? metaTitle,
            OgDescription = ogDescription ?? metaDescription,
            OgImageUrl = ogImageUrl ?? string.Empty,
            OgType = ogType ?? "article",
            JsonLd = jsonLd ?? string.Empty,
            SchemaType = schemaType ?? "Article",
            TwitterCard = twitterCard ?? "summary_large_image",
            TwitterTitle = twitterTitle ?? metaTitle,
            TwitterDescription = twitterDescription ?? metaDescription,
            TwitterImageUrl = twitterImageUrl ?? ogImageUrl ?? string.Empty
        };

        return metadata;
    }

    // Builder methods for fluent API
    public SeoMetadata WithMetaTitle(string metaTitle)
    {
        var clone = Clone();
        clone.MetaTitle = metaTitle;
        return clone;
    }

    public SeoMetadata WithMetaDescription(string metaDescription)
    {
        var clone = Clone();
        clone.MetaDescription = metaDescription;
        return clone;
    }

    public SeoMetadata WithCanonicalUrl(string canonicalUrl)
    {
        var clone = Clone();
        clone.CanonicalUrl = canonicalUrl;
        return clone;
    }

    public SeoMetadata WithRobots(string robots)
    {
        var clone = Clone();
        clone.Robots = robots;
        return clone;
    }

    public SeoMetadata WithOpenGraph(string title, string description, string imageUrl, string type = "article")
    {
        var clone = Clone();
        clone.OgTitle = title;
        clone.OgDescription = description;
        clone.OgImageUrl = imageUrl;
        clone.OgType = type;
        return clone;
    }

    public SeoMetadata WithTwitterCard(string title, string description, string imageUrl, string cardType = "summary_large_image")
    {
        var clone = Clone();
        clone.TwitterTitle = title;
        clone.TwitterDescription = description;
        clone.TwitterImageUrl = imageUrl;
        clone.TwitterCard = cardType;
        return clone;
    }

    public SeoMetadata WithJsonLd(string jsonLd, string schemaType = "Article")
    {
        var clone = Clone();
        clone.JsonLd = jsonLd;
        clone.SchemaType = schemaType;
        return clone;
    }

    /// <summary>
    /// Creates a deep clone of this SEO metadata
    /// </summary>
    public SeoMetadata Clone()
    {
        return new SeoMetadata
        {
            MetaTitle = MetaTitle,
            MetaDescription = MetaDescription,
            CanonicalUrl = CanonicalUrl,
            Robots = Robots,
            OgTitle = OgTitle,
            OgDescription = OgDescription,
            OgImageUrl = OgImageUrl,
            OgType = OgType,
            JsonLd = JsonLd,
            SchemaType = SchemaType,
            TwitterCard = TwitterCard,
            TwitterTitle = TwitterTitle,
            TwitterDescription = TwitterDescription,
            TwitterImageUrl = TwitterImageUrl
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return MetaTitle;
        yield return MetaDescription;
        yield return CanonicalUrl;
        yield return Robots;
        yield return OgTitle;
        yield return OgDescription;
        yield return OgImageUrl;
        yield return OgType;
        yield return JsonLd;
        yield return SchemaType;
        yield return TwitterCard;
        yield return TwitterTitle;
        yield return TwitterDescription;
        yield return TwitterImageUrl;
    }
}