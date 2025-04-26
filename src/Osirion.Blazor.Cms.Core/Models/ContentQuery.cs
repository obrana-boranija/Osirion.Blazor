using Osirion.Blazor.Cms.Enums;

namespace Osirion.Blazor.Cms.Models;

/// <summary>
/// Query parameters for filtering content items
/// </summary>
public class ContentQuery
{
    /// <summary>
    /// Gets or sets the directory path to filter by
    /// </summary>
    public string? Directory { get; set; }

    /// <summary>
    /// Gets or sets the directory ID to filter by
    /// </summary>
    public string? DirectoryId { get; set; }

    /// <summary>
    /// Gets or sets the category to filter by
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the tag to filter by
    /// </summary>
    public string? Tag { get; set; }

    /// <summary>
    /// Gets or sets the search query
    /// </summary>
    public string? SearchQuery { get; set; }

    /// <summary>
    /// Gets or sets whether to filter by featured content
    /// </summary>
    public bool? IsFeatured { get; set; }

    /// <summary>
    /// Gets or sets the content status to filter by
    /// </summary>
    public ContentStatus? Status { get; set; }

    /// <summary>
    /// Gets or sets the author to filter by
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets the start date for filtering
    /// </summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>
    /// Gets or sets the end date for filtering
    /// </summary>
    public DateTime? DateTo { get; set; }

    /// <summary>
    /// Gets or sets the number of items to skip
    /// </summary>
    public int? Skip { get; set; }

    /// <summary>
    /// Gets or sets the number of items to take
    /// </summary>
    public int? Take { get; set; }

    /// <summary>
    /// Gets or sets the field to sort by
    /// </summary>
    public SortField SortBy { get; set; } = SortField.Date;

    /// <summary>
    /// Gets or sets the sort direction
    /// </summary>
    public SortDirection SortDirection { get; set; } = SortDirection.Descending;

    /// <summary>
    /// Gets or sets the locale to filter by
    /// </summary>
    public string? Locale { get; set; }

    /// <summary>
    /// Gets or sets the localization ID to filter by (to get all translations of the same content)
    /// </summary>
    public string? LocalizationId { get; set; }

    /// <summary>
    /// Gets or sets the provider ID to filter by
    /// </summary>
    public string? ProviderId { get; set; }

    /// <summary>
    /// Gets or sets whether to include unpublished content (drafts, scheduled, etc.)
    /// </summary>
    public bool IncludeUnpublished { get; set; }

    /// <summary>
    /// Gets or sets whether to include content items from subdirectories
    /// </summary>
    public bool IncludeSubdirectories { get; set; }

    /// <summary>
    /// Gets or sets a collection of content IDs to include (for targeted queries)
    /// </summary>
    public ICollection<string>? IncludeIds { get; set; }

    /// <summary>
    /// Gets or sets a collection of content IDs to exclude
    /// </summary>
    public ICollection<string>? ExcludeIds { get; set; }

    /// <summary>
    /// Gets or sets a collection of tags to filter by (AND logic - all tags must be present)
    /// </summary>
    public ICollection<string>? Tags { get; set; }

    /// <summary>
    /// Gets or sets a collection of categories to filter by (AND logic - all categories must be present)
    /// </summary>
    public ICollection<string>? Categories { get; set; }

    /// <summary>
    /// Creates a deep clone of this query
    /// </summary>
    public ContentQuery Clone()
    {
        var clone = new ContentQuery
        {
            Directory = Directory,
            DirectoryId = DirectoryId,
            Category = Category,
            Tag = Tag,
            SearchQuery = SearchQuery,
            IsFeatured = IsFeatured,
            Status = Status,
            Author = Author,
            DateFrom = DateFrom,
            DateTo = DateTo,
            Skip = Skip,
            Take = Take,
            SortBy = SortBy,
            SortDirection = SortDirection,
            Locale = Locale,
            LocalizationId = LocalizationId,
            ProviderId = ProviderId,
            IncludeUnpublished = IncludeUnpublished,
            IncludeSubdirectories = IncludeSubdirectories
        };

        if (IncludeIds != null)
        {
            clone.IncludeIds = new List<string>(IncludeIds);
        }

        if (ExcludeIds != null)
        {
            clone.ExcludeIds = new List<string>(ExcludeIds);
        }

        if (Tags != null)
        {
            clone.Tags = new List<string>(Tags);
        }

        if (Categories != null)
        {
            clone.Categories = new List<string>(Categories);
        }

        return clone;
    }
}