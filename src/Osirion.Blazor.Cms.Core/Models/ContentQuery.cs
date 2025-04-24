namespace Osirion.Blazor.Cms.Models;

/// <summary>
/// Query parameters for filtering content items
/// </summary>
public class ContentQuery
{
    /// <summary>
    /// Gets or sets the directory to filter by
    /// </summary>
    public string? Directory { get; set; }

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
    /// Gets or sets the directory ID to filter by
    /// </summary>
    public string? DirectoryId { get; set; }

    /// <summary>
    /// Gets or sets the localization ID to filter by (to get all translations of the same content)
    /// </summary>
    public string? LocalizationId { get; set; }
}