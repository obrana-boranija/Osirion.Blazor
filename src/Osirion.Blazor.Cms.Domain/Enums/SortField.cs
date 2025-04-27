namespace Osirion.Blazor.Cms.Domain.Enums;

/// <summary>
/// Defines the fields available for sorting content items
/// </summary>
public enum SortField
{
    /// <summary>
    /// Sort by publication date
    /// </summary>
    Date,

    /// <summary>
    /// Sort by title
    /// </summary>
    Title,

    /// <summary>
    /// Sort by author
    /// </summary>
    Author,

    /// <summary>
    /// Sort by last modified date
    /// </summary>
    LastModified,

    /// <summary>
    /// Sort by manual order index
    /// </summary>
    Order,

    /// <summary>
    /// Sort by created date
    /// </summary>
    Created,

    /// <summary>
    /// Sort by scheduled publish date
    /// </summary>
    PublishDate,

    /// <summary>
    /// Sort by slug
    /// </summary>
    Slug,

    /// <summary>
    /// Sort by read time
    /// </summary>
    ReadTime
}