namespace Osirion.Blazor.Cms.Models;

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

    /// <summary>
    /// Gets or sets the parent category if this is a subcategory
    /// </summary>
    public ContentCategory? Parent { get; set; }

    /// <summary>
    /// Gets or sets the child categories if this is a parent category
    /// </summary>
    public List<ContentCategory> Children { get; set; } = new();

    /// <summary>
    /// Gets or sets additional metadata for the category
    /// </summary>
    public IDictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets or sets the URL for the category
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the color associated with this category
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Gets or sets the icon associated with this category
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the order for sorting
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets whether this category is featured
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Creates a deep clone of this category (without parent or children)
    /// </summary>
    public ContentCategory Clone()
    {
        var clone = new ContentCategory
        {
            Name = Name,
            Slug = Slug,
            Count = Count,
            Description = Description,
            Url = Url,
            Color = Color,
            Icon = Icon,
            Order = Order,
            IsFeatured = IsFeatured
        };

        // Clone metadata
        foreach (var kvp in Metadata)
        {
            clone.Metadata[kvp.Key] = kvp.Value;
        }

        return clone;
    }
}