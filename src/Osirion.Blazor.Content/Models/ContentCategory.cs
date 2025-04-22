namespace Osirion.Blazor.Content.Models;

/// <summary>
/// Represents a content category
/// </summary>
public class ContentCategory
{
    /// <summary>
    /// Gets or sets the name of the category
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL-friendly slug
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of content items in this category
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the description of the category
    /// </summary>
    public string? Description { get; set; }
}