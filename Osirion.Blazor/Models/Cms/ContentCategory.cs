namespace Osirion.Blazor.Models.Cms;

/// <summary>
/// Represents a content category
/// </summary>
public class ContentCategory
{
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the slug URL
    /// </summary>
    public string SlugUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content count
    /// </summary>
    public int ContentCount { get; set; }
}