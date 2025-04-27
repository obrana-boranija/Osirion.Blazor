using System;
using System.Collections.Generic;
using System.Linq;
using Osirion.Blazor.Cms.Domain.Common;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Domain.Entities;

/// <summary>
/// Represents a content item in the CMS
/// </summary>
public class ContentItem : Entity<string>
{
    // Private backing fields for collections
    private readonly List<string> _tags = new();
    private readonly List<string> _categories = new();
    private readonly MetadataContainer _metadata = new();

    /// <summary>
    /// Gets or sets the title of the content item
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the author of the content item
    /// </summary>
    public string Author { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the publication date of the content item
    /// </summary>
    public DateTime DateCreated { get; private set; }

    /// <summary>
    /// Gets or sets the last modified date of the content item
    /// </summary>
    public DateTime? LastModified { get; private set; }

    /// <summary>
    /// Gets or sets the content body (HTML)
    /// </summary>
    public string Content { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the original markdown content if available
    /// </summary>
    public string? OriginalMarkdown { get; private set; }

    /// <summary>
    /// Gets or sets the locale/language code
    /// </summary>
    public string Locale { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content ID that is shared across all localizations
    /// </summary>
    public string ContentId { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description or summary of the content item
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL-friendly slug
    /// </summary>
    public string Slug { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full URL to the content item
    /// </summary>
    public string Url { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full path to the content item
    /// </summary>
    public string Path { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the featured image URL
    /// </summary>
    public string? FeaturedImageUrl { get; private set; }

    /// <summary>
    /// Gets or sets whether this content item is featured
    /// </summary>
    public bool IsFeatured { get; private set; }

    /// <summary>
    /// Gets or sets the status of the content item
    /// </summary>
    public ContentStatus Status { get; private set; } = ContentStatus.Published;

    /// <summary>
    /// Gets the tags associated with this content item
    /// </summary>
    public IReadOnlyList<string> Tags => _tags.AsReadOnly();

    /// <summary>
    /// Gets the categories associated with this content item
    /// </summary>
    public IReadOnlyList<string> Categories => _categories.AsReadOnly();

    /// <summary>
    /// Gets the metadata dictionary for serialization
    /// </summary>
    public IReadOnlyDictionary<string, object> Metadata => _metadata.Values;

    /// <summary>
    /// Gets or sets the provider identifier that created this content item
    /// </summary>
    public string ProviderId { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider-specific identifier
    /// </summary>
    public string? ProviderSpecificId { get; private set; }

    /// <summary>
    /// Gets the estimated reading time in minutes
    /// </summary>
    public int ReadTimeMinutes => CalculateReadTime();

    /// <summary>
    /// Gets or sets the parent directory of this content item
    /// </summary>
    public DirectoryItem? Directory { get; private set; }

    /// <summary>
    /// Gets or sets the SEO metadata for this content item
    /// </summary>
    public SeoMetadata Seo { get; private set; } = new SeoMetadata();

    /// <summary>
    /// Gets the publish date (scheduled or actual)
    /// </summary>
    public DateTime PublishDate => GetMetadata<DateTime>("publish_date", DateCreated);

    /// <summary>
    /// Gets or sets the ordering index (for manual sorting)
    /// </summary>
    public int OrderIndex { get; private set; }

    // Constructor with required fields
    private ContentItem() { }

    public static ContentItem Create(
        string id,
        string title,
        string content,
        string path,
        string providerId)
    {
        var contentItem = new ContentItem
        {
            Id = id,
            Title = title,
            Content = content,
            Path = path,
            ProviderId = providerId,
            DateCreated = DateTime.UtcNow
        };

        // Generate slug from title if not provided
        contentItem.Slug = contentItem.GenerateSlug(title);

        return contentItem;
    }

    // Domain methods
    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ContentValidationException("Title", "Title cannot be empty");

        Title = title;
        LastModified = DateTime.UtcNow;
    }

    public void SetContent(string content, string? markdown = null)
    {
        Content = content ?? throw new ContentValidationException("Content", "Content cannot be null");
        OriginalMarkdown = markdown;
        LastModified = DateTime.UtcNow;
    }

    public void SetDescription(string description)
    {
        Description = description;
        LastModified = DateTime.UtcNow;
    }

    public void SetSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ContentValidationException("Slug", "Slug cannot be empty");

        // Validate slug format
        if (!IsValidSlug(slug))
            throw new ContentValidationException("Slug", "Slug must contain only lowercase letters, numbers, and hyphens");

        Slug = slug;
        LastModified = DateTime.UtcNow;
    }

    public void SetUrl(string url)
    {
        Url = url;
    }

    public void SetFeaturedImage(string? url)
    {
        FeaturedImageUrl = url;
        LastModified = DateTime.UtcNow;
    }

    public void SetFeatured(bool isFeatured)
    {
        IsFeatured = isFeatured;
        LastModified = DateTime.UtcNow;
    }

    public void SetStatus(ContentStatus status)
    {
        Status = status;
        LastModified = DateTime.UtcNow;
    }

    public void SetAuthor(string author)
    {
        Author = author;
        LastModified = DateTime.UtcNow;
    }

    public void SetLocale(string locale)
    {
        Locale = locale;
        LastModified = DateTime.UtcNow;
    }

    public void SetContentId(string contentId)
    {
        ContentId = contentId;
    }

    public void SetProviderSpecificId(string? id)
    {
        ProviderSpecificId = id;
    }

    public void SetOrderIndex(int orderIndex)
    {
        OrderIndex = orderIndex;
    }

    public void SetDirectory(DirectoryItem? directory)
    {
        Directory = directory;
    }

    public void SetSeoMetadata(SeoMetadata seo)
    {
        Seo = seo ?? throw new ArgumentNullException(nameof(seo));
    }

    public void AddTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            throw new ContentValidationException("Tag", "Tag cannot be empty");

        if (!_tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
        {
            _tags.Add(tag);
            LastModified = DateTime.UtcNow;
        }
    }

    public void RemoveTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return;

        var matchingTag = _tags.FirstOrDefault(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));
        if (matchingTag != null)
        {
            _tags.Remove(matchingTag);
            LastModified = DateTime.UtcNow;
        }
    }

    public void ClearTags()
    {
        if (_tags.Count > 0)
        {
            _tags.Clear();
            LastModified = DateTime.UtcNow;
        }
    }

    public void AddCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ContentValidationException("Category", "Category cannot be empty");

        if (!_categories.Contains(category, StringComparer.OrdinalIgnoreCase))
        {
            _categories.Add(category);
            LastModified = DateTime.UtcNow;
        }
    }

    public void RemoveCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return;

        var matchingCategory = _categories.FirstOrDefault(c => c.Equals(category, StringComparison.OrdinalIgnoreCase));
        if (matchingCategory != null)
        {
            _categories.Remove(matchingCategory);
            LastModified = DateTime.UtcNow;
        }
    }

    public void ClearCategories()
    {
        if (_categories.Count > 0)
        {
            _categories.Clear();
            LastModified = DateTime.UtcNow;
        }
    }

    public T? GetMetadata<T>(string key, T? defaultValue = default)
    {
        return _metadata.GetValue(key, defaultValue);
    }

    public void SetMetadata<T>(string key, T value)
    {
        _metadata.SetValue(key, value);
        LastModified = DateTime.UtcNow;
    }

    // Helper methods
    private string GenerateSlug(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "untitled";

        // Convert to lowercase
        var slug = text.ToLowerInvariant();

        // Remove special characters
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");

        // Replace spaces with hyphens
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");

        // Remove consecutive hyphens
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-{2,}", "-");

        // Trim hyphens from ends
        slug = slug.Trim('-');

        return slug;
    }

    private bool IsValidSlug(string slug)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(slug, "^[a-z0-9-]+$");
    }

    private int CalculateReadTime()
    {
        const int wordsPerMinute = 200;

        // Count words in content
        var wordCount = 0;
        if (!string.IsNullOrWhiteSpace(Content))
        {
            wordCount = Content.Split(new[] { ' ', '\n', '\r', '\t' },
                StringSplitOptions.RemoveEmptyEntries).Length;
        }
        else if (!string.IsNullOrWhiteSpace(OriginalMarkdown))
        {
            wordCount = OriginalMarkdown.Split(new[] { ' ', '\n', '\r', '\t' },
                StringSplitOptions.RemoveEmptyEntries).Length;
        }

        // Minimum reading time is 1 minute
        return Math.Max(1, (int)Math.Ceiling(wordCount / (double)wordsPerMinute));
    }

    // Create a deep clone of this content item
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
            OrderIndex = OrderIndex,
            Directory = Directory,
            Seo = Seo.Clone()
        };

        // Clone collections
        foreach (var tag in _tags)
        {
            clone._tags.Add(tag);
        }

        foreach (var category in _categories)
        {
            clone._categories.Add(category);
        }

        // Clone metadata
        clone._metadata = _metadata.Clone();

        return clone;
    }
}