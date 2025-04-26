using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Models;

/// <summary>
/// Represents a content item from any provider with enhanced metadata handling
/// </summary>
public class ContentItem : IEquatable<ContentItem>
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
    /// Gets or sets the original markdown content if available
    /// </summary>
    public string? OriginalMarkdown { get; set; }

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
    /// Gets or sets the status of the content item (draft, published, archived)
    /// </summary>
    public ContentStatus Status { get; set; } = ContentStatus.Published;

    /// <summary>
    /// Gets or sets the tags associated with this content item
    /// </summary>
    private readonly List<string> _tags = new();
    public IReadOnlyList<string> Tags => _tags.AsReadOnly();

    /// <summary>
    /// Adds a tag to the content item
    /// </summary>
    public void AddTag(string tag)
    {
        if (!string.IsNullOrWhiteSpace(tag) && !_tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
        {
            _tags.Add(tag);
        }
    }

    /// <summary>
    /// Gets or sets the categories associated with this content item
    /// </summary>
    private readonly List<string> _categories = new();
    public IReadOnlyList<string> Categories => _categories.AsReadOnly();

    /// <summary>
    /// Adds a category to the content item
    /// </summary>
    public void AddCategory(string category)
    {
        if (!string.IsNullOrWhiteSpace(category) && !_categories.Contains(category, StringComparer.OrdinalIgnoreCase))
        {
            _categories.Add(category);
        }
    }

    // Private dictionary for metadata with proper JsonExtensionData attribute
    private readonly Dictionary<string, object> _metadata = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the metadata dictionary for serialization
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, object> Metadata => _metadata;

    /// <summary>
    /// Gets a strongly-typed metadata value
    /// </summary>
    public T? GetMetadata<T>(string key, T? defaultValue = default)
    {
        if (_metadata.TryGetValue(key, out var value))
        {
            if (value is T typedValue)
            {
                return typedValue;
            }

            // Try to convert if possible
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        return defaultValue;
    }

    /// <summary>
    /// Sets a metadata value with strong typing
    /// </summary>
    public void SetMetadata<T>(string key, T value)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Metadata key cannot be null or empty", nameof(key));
        }

        if (value == null)
        {
            _metadata.Remove(key);
        }
        else
        {
            _metadata[key] = value;
        }
    }

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

    /// <summary>
    /// Creates a deep clone of this content item
    /// </summary>
    public ContentItem Clone()
    {
        var clone = new ContentItem
        {
            Id = Id,
            Title = Title,
            Author = Author,
            DateCreated = DateCreated,
            LastModified = LastModified,
            Content = Content,
            OriginalMarkdown = OriginalMarkdown,
            Locale = Locale,
            ContentId = ContentId,
            Description = Description,
            Slug = Slug,
            Url = Url,
            Path = Path,
            FeaturedImageUrl = FeaturedImageUrl,
            IsFeatured = IsFeatured,
            Status = Status,
            ProviderId = ProviderId,
            ProviderSpecificId = ProviderSpecificId,
            Directory = Directory,
            Seo = new SeoMetadata
            {
                MetaTitle = Seo.MetaTitle,
                MetaDescription = Seo.MetaDescription,
                CanonicalUrl = Seo.CanonicalUrl,
                Robots = Seo.Robots,
                OgTitle = Seo.OgTitle,
                OgDescription = Seo.OgDescription,
                OgImageUrl = Seo.OgImageUrl,
                OgType = Seo.OgType,
                JsonLd = Seo.JsonLd,
                SchemaType = Seo.SchemaType,
                TwitterCard = Seo.TwitterCard,
                TwitterTitle = Seo.TwitterTitle,
                TwitterDescription = Seo.TwitterDescription,
                TwitterImageUrl = Seo.TwitterImageUrl
            }
        };

        // Clone collections
        foreach (var tag in _tags)
        {
            clone.AddTag(tag);
        }

        foreach (var category in _categories)
        {
            clone.AddCategory(category);
        }

        // Clone metadata
        foreach (var kvp in _metadata)
        {
            clone._metadata[kvp.Key] = kvp.Value;
        }

        return clone;
    }

    private int CalculateReadTime()
    {
        const int wordsPerMinute = 200;
        var words = Content.Split(new[] { ' ', '\n', '\r', '\t' },
            StringSplitOptions.RemoveEmptyEntries).Length;
        return Math.Max(1, (int)Math.Ceiling(words / (double)wordsPerMinute));
    }

    /// <summary>
    /// Equality implementation for content items
    /// </summary>
    public bool Equals(ContentItem? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Id == other.Id &&
               ProviderId == other.ProviderId &&
               Path == other.Path;
    }

    /// <summary>
    /// Override equals for object comparison
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is ContentItem item && Equals(item);
    }

    /// <summary>
    /// Get a stable hash code for the content item
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, ProviderId, Path);
    }
}

/// <summary>
/// Defines the status of a content item
/// </summary>
public enum ContentStatus
{
    /// <summary>
    /// Item is a draft and not publicly visible
    /// </summary>
    Draft,

    /// <summary>
    /// Item is published and publicly visible
    /// </summary>
    Published,

    /// <summary>
    /// Item is archived and may have limited visibility
    /// </summary>
    Archived
}