// src/Osirion.Blazor.Cms.Core/Models/SeoMetadata.cs
namespace Osirion.Blazor.Cms.Models;

/// <summary>
/// Represents SEO metadata for content items
/// </summary>
public class SeoMetadata
{
    /// <summary>
    /// Gets or sets the meta title (SEO title)
    /// </summary>
    public string MetaTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the meta description
    /// </summary>
    public string MetaDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the canonical URL
    /// </summary>
    public string CanonicalUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the robots meta directive
    /// </summary>
    public string Robots { get; set; } = "index, follow";

    /// <summary>
    /// Gets or sets the Open Graph title
    /// </summary>
    public string OgTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Open Graph description
    /// </summary>
    public string OgDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Open Graph image URL
    /// </summary>
    public string OgImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Open Graph type
    /// </summary>
    public string OgType { get; set; } = "article";

    /// <summary>
    /// Gets or sets the JSON-LD structured data
    /// </summary>
    public string JsonLd { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the schema.org type
    /// </summary>
    public string SchemaType { get; set; } = "Article";

    /// <summary>
    /// Gets or sets the Twitter card type
    /// </summary>
    public string TwitterCard { get; set; } = "summary_large_image";

    /// <summary>
    /// Gets or sets the Twitter title
    /// </summary>
    public string TwitterTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Twitter description
    /// </summary>
    public string TwitterDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Twitter image URL
    /// </summary>
    public string TwitterImageUrl { get; set; } = string.Empty;
}