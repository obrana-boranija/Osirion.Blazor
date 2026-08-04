using Osirion.Blazor.Cms.Domain.Common;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Extensions;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using System.Text;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Domain.Entities;

/// <summary>
/// Represents a content item in the CMS
/// </summary>
public class ContentItem : DomainEntity<string>
{
    // Private backing fields for collections
    private readonly List<string> _tags = new();
    private readonly List<string> _categories = new();
    private readonly Dictionary<string, object> _metadataValues = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Gets or sets the content title.</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>Gets or sets the content author.</summary>
    public string Author { get; set; } = string.Empty;
    /// <summary>Gets or sets when the content was created.</summary>
    public DateTime DateCreated { get; set; }
    /// <summary>Gets or sets when the content was last modified.</summary>
    public DateTime? LastModified { get; set; }
    /// <summary>Gets or sets the rendered content.</summary>
    public string Content { get; set; } = string.Empty;
    /// <summary>Gets or sets the original markdown source.</summary>
    public string? OriginalMarkdown { get; set; }
    /// <summary>Gets or sets the content locale.</summary>
    public string Locale { get; set; } = string.Empty;
    /// <summary>Gets or sets the provider content identifier.</summary>
    public string ContentId { get; set; } = string.Empty;
    /// <summary>Gets or sets the content description.</summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>Gets or sets the URL-friendly slug.</summary>
    public string Slug { get; set; } = string.Empty;
    /// <summary>Gets or sets the content URL.</summary>
    public string Url { get; set; } = string.Empty;
    /// <summary>Gets or sets the source path.</summary>
    public string Path { get; set; } = string.Empty;
    /// <summary>Gets or sets the featured image URL.</summary>
    public string? FeaturedImageUrl { get; set; }
    /// <summary>Gets or sets whether the item is featured.</summary>
    public bool IsFeatured { get; set; }
    /// <summary>Gets or sets whether the item is published.</summary>
    public bool IsPublished { get; set; }
    /// <summary>Gets or sets the publication status.</summary>
    public ContentStatus Status { get; set; } = ContentStatus.Published;
    /// <summary>Gets or sets the display order.</summary>
    public int OrderIndex { get; set; }
    /// <summary>Gets or sets the provider revision identifier.</summary>
    public string Sha { get; set; } = string.Empty;
    /// <summary>Gets or sets the containing directory.</summary>
    public DirectoryItem? Directory { get; set; }

    // Collections
    /// <summary>Gets the content tags.</summary>
    public IReadOnlyList<string> Tags => _tags.AsReadOnly();
    /// <summary>Gets the content categories.</summary>
    public IReadOnlyList<string> Categories => _categories.AsReadOnly();
    /// <summary>Gets or sets the parsed front matter.</summary>
    public FrontMatter? Metadata { get; set; }

    // Value objects
    //public SeoMetadata Seo { get; set; } = new SeoMetadata();

    // Computed properties
    /// <summary>Gets the estimated reading time in minutes.</summary>
    public int ReadTimeMinutes => CalculateReadTime();
    /// <summary>Gets the publication date from metadata or the creation date.</summary>
    public DateTime PublishDate => GetMetadata("publish_date", DateCreated);
    /// <summary>Initializes an empty content item.</summary>
    public ContentItem() { }

    /// <summary>
    /// Creates a new content item
    /// </summary>
    public static ContentItem Create(
        string id,
        string title,
        string content,
        string path,
        string providerId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ContentValidationException("Title", "Title cannot be empty");

        if (string.IsNullOrWhiteSpace(path))
            throw new ContentValidationException("Path", "Path cannot be empty");

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
        contentItem.Slug = title.GenerateSlug();

        return contentItem;
    }

    /// <summary>Changes the title.</summary>
    /// <param name="title">The new title.</param>
    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ContentValidationException("Title", "Title cannot be empty");

        Title = title;
        LastModified = DateTime.UtcNow;
    }

    /// <summary>Changes the rendered content and optional markdown source.</summary>
    /// <param name="content">The rendered content.</param>
    /// <param name="markdown">The original markdown source.</param>
    public void SetContent(string content, string? markdown = null)
    {
        Content = content ?? throw new ContentValidationException("Content", "Content cannot be null");
        OriginalMarkdown = markdown;
        LastModified = DateTime.UtcNow;
    }

    /// <summary>Changes the original markdown source.</summary>
    /// <param name="markdown">The markdown source.</param>
    public void SetOriginalMarkdown(string? markdown)
    {
        OriginalMarkdown = markdown;
    }

    /// <summary>Changes the description.</summary>
    /// <param name="description">The new description.</param>
    public void SetDescription(string description)
    {
        Description = description;
        LastModified = DateTime.UtcNow;
    }

    /// <summary>Changes the URL-friendly slug.</summary>
    /// <param name="slug">The new slug.</param>
    public void SetSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ContentValidationException("Slug", "Slug cannot be empty");

        // Validate slug format
        if (!slug.IsValidSlug())
            throw new ContentValidationException("Slug", "Slug must contain only lowercase letters, numbers, and hyphens");

        Slug = slug;
        LastModified = DateTime.UtcNow;
    }

    /// <summary>Changes the content URL.</summary>
    /// <param name="url">The new URL.</param>
    public void SetUrl(string url)
    {
        Url = url;
    }

    /// <summary>Changes the source path.</summary>
    /// <param name="path">The new path.</param>
    public void SetPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ContentValidationException("Path", "Path cannot be empty");

        Path = path;
    }

    /// <summary>Changes the featured image URL.</summary>
    /// <param name="url">The image URL.</param>
    public void SetFeaturedImage(string? url)
    {
        FeaturedImageUrl = url;
        LastModified = DateTime.UtcNow;
    }

    /// <summary>Changes whether the content is featured.</summary>
    /// <param name="isFeatured">Whether the item is featured.</param>
    public void SetFeatured(bool isFeatured)
    {
        IsFeatured = isFeatured;
        LastModified = DateTime.UtcNow;
    }

    /// <summary>Changes the author.</summary>
    /// <param name="author">The new author.</param>
    public void SetAuthor(string author)
    {
        Author = author;
        LastModified = DateTime.UtcNow;
    }

    /// <summary>Changes the locale.</summary>
    /// <param name="locale">The new locale.</param>
    public void SetLocale(string locale)
    {
        Locale = locale;
        LastModified = DateTime.UtcNow;
    }

    /// <summary>Changes the provider content identifier.</summary>
    /// <param name="contentId">The provider identifier.</param>
    public void SetContentId(string contentId)
    {
        ContentId = contentId;
    }

    /// <summary>Changes the creation date.</summary>
    /// <param name="date">The new creation date.</param>
    public void SetCreatedDate(DateTime date)
    {
        DateCreated = date;
    }

    /// <summary>Changes the last-modified date.</summary>
    /// <param name="date">The new last-modified date.</param>
    public void SetLastModifiedDate(DateTime date)
    {
        LastModified = date;
    }

    /// <summary>Changes the display order.</summary>
    /// <param name="orderIndex">The new order.</param>
    public void SetOrderIndex(int orderIndex)
    {
        OrderIndex = orderIndex;
    }

    /// <summary>Changes the containing directory.</summary>
    /// <param name="directory">The containing directory.</param>
    public void SetDirectory(DirectoryItem? directory)
    {
        Directory = directory;
    }

    /// <summary>Sets SEO metadata for the content item.</summary>
    /// <param name="seo">The SEO metadata.</param>
    public void SetSeoMetadata(SeoMetadata seo)
    {
        Metadata ??= new FrontMatter();
        Metadata.SeoProperties = seo ?? throw new ArgumentNullException(nameof(seo));
    }

    /// <summary>Adds a tag when it is not already present.</summary>
    /// <param name="tag">The tag to add.</param>
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

    /// <summary>Removes a tag when it is present.</summary>
    /// <param name="tag">The tag to remove.</param>
    public void RemoveTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return;

        var matchingTag = _tags.FirstOrDefault(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));
        if (matchingTag is not null)
        {
            _tags.Remove(matchingTag);
            LastModified = DateTime.UtcNow;
        }
    }

    /// <summary>Removes all tags.</summary>
    public void ClearTags()
    {
        if (_tags.Count > 0)
        {
            _tags.Clear();
            LastModified = DateTime.UtcNow;
        }
    }

    /// <summary>Adds a category when it is not already present.</summary>
    /// <param name="category">The category to add.</param>
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

    /// <summary>Removes a category when it is present.</summary>
    /// <param name="category">The category to remove.</param>
    public void RemoveCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return;

        var matchingCategory = _categories.FirstOrDefault(c => c.Equals(category, StringComparison.OrdinalIgnoreCase));
        if (matchingCategory is not null)
        {
            _categories.Remove(matchingCategory);
            LastModified = DateTime.UtcNow;
        }
    }

    /// <summary>Removes all categories.</summary>
    public void ClearCategories()
    {
        if (_categories.Count > 0)
        {
            _categories.Clear();
            LastModified = DateTime.UtcNow;
        }
    }

    /// <summary>Gets a typed metadata value.</summary>
    /// <typeparam name="T">The metadata value type.</typeparam>
    /// <param name="key">The metadata key.</param>
    /// <param name="defaultValue">The value to return when the key is unavailable.</param>
    public T? GetMetadata<T>(string key, T? defaultValue = default)
    {
        if (_metadataValues.TryGetValue(key, out var value))
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

    /// <summary>Sets or removes a metadata value.</summary>
    /// <typeparam name="T">The metadata value type.</typeparam>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The value to store.</param>
    public void SetMetadata<T>(string key, T value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Metadata key cannot be empty", nameof(key));

        if (value is null)
        {
            _metadataValues.Remove(key);
        }
        else
        {
            _metadataValues[key] = value;
        }

        LastModified = DateTime.UtcNow;
    }

    // Helper methods
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

    /// <summary>Creates a deep clone of this content item.</summary>
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
            Metadata = Metadata?.Clone()
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
        foreach (var kvp in _metadataValues)
        {
            clone._metadataValues[kvp.Key] = kvp.Value;
        }

        return clone;
    }

    /// <summary>
    /// Converts the blog post to markdown with frontmatter
    /// </summary>
    /// <returns>The full markdown content with frontmatter</returns>
    public string ToMarkdown()
    {
        var markdown = new StringBuilder();

        // Add frontmatter
        markdown.Append(Metadata?.ToYaml());

        // Add content
        markdown.AppendLine(Content);

        return markdown.ToString();
    }

    /// <summary>Raises a content-created domain event.</summary>
    public void RaiseCreatedEvent()
    {
        AddDomainEvent(new ContentCreatedEvent(
            Id,
            Title,
            Path,
            ProviderId));
    }

    /// <summary>Raises a content-updated domain event.</summary>
    public void RaiseUpdatedEvent()
    {
        AddDomainEvent(new ContentUpdatedEvent(
            Id,
            Title,
            Path,
            ProviderId));
    }

    /// <summary>Raises a content-deleted domain event.</summary>
    public void RaiseDeletedEvent()
    {
        AddDomainEvent(new ContentDeletedEvent(
            Id,
            Path,
            ProviderId));
    }

    /// <summary>Raises a status-changed domain event.</summary>
    /// <param name="previousStatus">The previous status.</param>
    public void RaiseStatusChangedEvent(ContentStatus previousStatus)
    {
        AddDomainEvent(new ContentStatusChangedEvent(
            Id,
            Title,
            previousStatus,
            Status,
            ProviderId));
    }

    /// <summary>Changes the publication status.</summary>
    /// <param name="status">The new status.</param>
    public void SetStatus(ContentStatus status)
    {
        var previousStatus = Status;
        Status = status;
        LastModified = DateTime.UtcNow;

        // Raise event only if status actually changed
        if (previousStatus != status)
        {
            RaiseStatusChangedEvent(previousStatus);
        }
    }
}
