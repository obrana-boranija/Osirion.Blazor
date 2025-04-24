namespace Osirion.Blazor.Cms.Models;

/// <summary>
/// Represents a content item from any provider
/// </summary>
public class ContentItem
{
    /// <summary>
    /// Gets or sets the unique identifier of the content item
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title of the content item
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the author of the content item
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the publication date of the content item
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Gets or sets the last modified date of the content item
    /// </summary>
    public DateTime? LastModified { get; set; }

    /// <summary>
    /// Gets or sets the content body (HTML or markdown)
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the locale/language code (e.g., "en", "de-DE", "sr-Latn-RS")
    /// </summary>
    public string Locale { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content ID that is shared across all localizations
    /// </summary>
    public string ContentId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description or summary of the content item
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL-friendly slug
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full url to the content item
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full path to the content item
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the featured image URL
    /// </summary>
    public string? FeaturedImageUrl { get; set; }

    /// <summary>
    /// Gets or sets whether this content item is featured
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Gets or sets the tags associated with this content item
    /// </summary>
    public IList<string> Tags { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the categories associated with this content item
    /// </summary>
    public IList<string> Categories { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets additional metadata for the content item
    /// </summary>
    public IDictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets or sets the provider identifier that created this content item
    /// </summary>
    public string ProviderId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider-specific identifier
    /// </summary>
    public string? ProviderSpecificId { get; set; }

    /// <summary>
    /// Gets the estimated reading time in minutes
    /// </summary>
    public int ReadTimeMinutes => CalculateReadTime();

    /// <summary>
    /// Gets or sets the parent directory of this content item
    /// </summary>
    public DirectoryItem? Directory { get; set; }

    /// <summary>
    /// Gets or sets the SEO metadata for this content item
    /// </summary>
    public SeoMetadata Seo { get; set; } = new SeoMetadata();

    private int CalculateReadTime()
    {
        const int wordsPerMinute = 200;
        var words = Content.Split(new[] { ' ', '\n', '\r', '\t' },
            StringSplitOptions.RemoveEmptyEntries).Length;
        return Math.Max(1, (int)Math.Ceiling(words / (double)wordsPerMinute));
    }
}