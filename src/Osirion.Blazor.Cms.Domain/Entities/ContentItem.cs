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

    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public DateTime? LastModified { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? OriginalMarkdown { get; set; }
    public string Locale { get; set; } = string.Empty;
    public string ContentId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string? FeaturedImageUrl { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; }
    public ContentStatus Status { get; set; } = ContentStatus.Published;
    public int OrderIndex { get; set; }
    public string Sha { get; set; } = string.Empty;
    public DirectoryItem? Directory { get; set; }

    // Collections
    public IReadOnlyList<string> Tags => _tags.AsReadOnly();
    public IReadOnlyList<string> Categories => _categories.AsReadOnly();
    public FrontMatter? Metadata { get; set; }

    // Value objects
    //public SeoMetadata Seo { get; set; } = new SeoMetadata();

    // Computed properties
    public int ReadTimeMinutes => CalculateReadTime();
    public DateTime PublishDate => GetMetadata("publish_date", DateCreated);
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

    // Modifier methods
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

    public void SetOriginalMarkdown(string? markdown)
    {
        OriginalMarkdown = markdown;
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
        if (!slug.IsValidSlug())
            throw new ContentValidationException("Slug", "Slug must contain only lowercase letters, numbers, and hyphens");

        Slug = slug;
        LastModified = DateTime.UtcNow;
    }

    public void SetUrl(string url)
    {
        Url = url;
    }

    public void SetPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ContentValidationException("Path", "Path cannot be empty");

        Path = path;
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

    public void SetCreatedDate(DateTime date)
    {
        DateCreated = date;
    }

    public void SetLastModifiedDate(DateTime date)
    {
        LastModified = date;
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
        Metadata.SeoProperties = seo ?? throw new ArgumentNullException(nameof(seo));
    }

    // Collection modifiers
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
        if (matchingTag is not null)
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
        if (matchingCategory is not null)
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

    // Metadata operations
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
            Metadata = Metadata.Clone()
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
        markdown.Append(Metadata.ToYaml());

        // Add content
        markdown.AppendLine(Content);

        return markdown.ToString();
    }

    public void RaiseCreatedEvent()
    {
        AddDomainEvent(new ContentCreatedEvent(
            Id,
            Title,
            Path,
            ProviderId));
    }

    public void RaiseUpdatedEvent()
    {
        AddDomainEvent(new ContentUpdatedEvent(
            Id,
            Title,
            Path,
            ProviderId));
    }

    public void RaiseDeletedEvent()
    {
        AddDomainEvent(new ContentDeletedEvent(
            Id,
            Path,
            ProviderId));
    }

    public void RaiseStatusChangedEvent(ContentStatus previousStatus)
    {
        AddDomainEvent(new ContentStatusChangedEvent(
            Id,
            Title,
            previousStatus,
            Status,
            ProviderId));
    }

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