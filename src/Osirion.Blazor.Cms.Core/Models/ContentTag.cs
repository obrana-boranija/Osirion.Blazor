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

    /// <summary>
    /// Gets or sets the URL for the tag
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the color associated with this tag
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Gets or sets whether this tag is featured
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Gets or sets the group this tag belongs to (for tag categorization)
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    /// Creates a deep clone of this tag
    /// </summary>
    public ContentTag Clone()
    {
        return new ContentTag
        {
            Name = Name,
            Slug = Slug,
            Count = Count,
            Url = Url,
            Color = Color,
            IsFeatured = IsFeatured,
            Group = Group
        };
    }
}