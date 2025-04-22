namespace Osirion.Blazor.Cms.Models;

/// <summary>
/// Represents a content tag
/// </summary>
public class ContentTag
{
    /// <summary>
    /// Gets or sets the name of the tag
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL-friendly slug
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of content items with this tag
    /// </summary>
    public int Count { get; set; }
}