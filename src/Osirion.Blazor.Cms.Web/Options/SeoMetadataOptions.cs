using Osirion.Blazor.Cms.Web.Components;

namespace Osirion.Blazor.Cms.Web.Options;

/// <summary>
/// Configuration options for SEO metadata rendering across the site.
/// These settings provide default values that can be overridden on a per-page basis.
/// </summary>
public class SeoMetadataOptions
{
    /// <summary>
    /// Gets or sets the site name used in Open Graph and general metadata.
    /// If not set, will be extracted from the domain name.
    /// </summary>
    public string? SiteName { get; set; }

    /// <summary>
    /// Gets or sets the site-wide description used for organization schema.
    /// </summary>
    public string? SiteDescription { get; set; }

    /// <summary>
    /// Gets or sets the URL to the site logo (recommended: square format, min 112x112px).
    /// </summary>
    public string? SiteLogoUrl { get; set; }

    /// <summary>
    /// Gets or sets the Twitter handle for the site (e.g., "@yoursite").
    /// </summary>
    public string? TwitterSite { get; set; }

    /// <summary>
    /// Gets or sets the default Twitter handle for content creators (e.g., "@author").
    /// </summary>
    public string? TwitterCreator { get; set; }

    /// <summary>
    /// Gets or sets the Facebook App ID for Facebook Insights.
    /// </summary>
    public string? FacebookAppId { get; set; }

    /// <summary>
    /// Gets or sets whether to allow AI systems to discover and index content by default.
    /// </summary>
    public bool AllowAiDiscovery { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to allow AI systems to use content for training purposes by default.
    /// </summary>
    public bool AllowAiTraining { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to allow traditional search engines to index content by default.
    /// </summary>
    public bool AllowSearchIndexing { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to enable Generative Engine Optimization (GEO) meta tags by default.
    /// </summary>
    public bool EnableGeoOptimization { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to enable Answer Engine Optimization (AEO) structured answers by default.
    /// </summary>
    public bool EnableAeoOptimization { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to generate multiple schema types when applicable by default.
    /// </summary>
    public bool GenerateMultipleSchemas { get; set; } = true;

    /// <summary>
    /// Gets or sets the default fallback image URL when content has no featured image.
    /// </summary>
    public string? DefaultImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the default image width for Open Graph (recommended: 1200px).
    /// </summary>
    public int DefaultImageWidth { get; set; } = 1200;

    /// <summary>
    /// Gets or sets the default image height for Open Graph (recommended: 630px).
    /// </summary>
    public int DefaultImageHeight { get; set; } = 630;

    /// <summary>
    /// Gets or sets the organization name for structured data.
    /// </summary>
    public string? OrganizationName { get; set; }

    /// <summary>
    /// Gets or sets the default text label for the home breadcrumb.
    /// </summary>
    public string BreadcrumbHomeText { get; set; } = "Home";

    /// <summary>
    /// Gets or sets the default schema types to generate for content pages.
    /// </summary>
    public SchemaType[]? DefaultSchemaTypes { get; set; }
}
