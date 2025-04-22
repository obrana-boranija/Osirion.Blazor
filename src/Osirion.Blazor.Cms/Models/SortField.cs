namespace Osirion.Blazor.Cms.Models;

/// <summary>
/// Fields available for sorting content items
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
    LastModified
}