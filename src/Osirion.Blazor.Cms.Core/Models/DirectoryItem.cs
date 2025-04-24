namespace Osirion.Blazor.Cms.Models;

/// <summary>
/// Represents a directory in the content structure
/// </summary>
public class DirectoryItem
{
    /// <summary>
    /// Gets or sets the unique identifier of the directory
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path of the directory
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the directory
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the directory
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the locale/language code (e.g., "en", "es", "de")
    /// </summary>
    public string Locale { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the order for sorting
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the parent directory
    /// </summary>
    public DirectoryItem? Parent { get; set; }

    /// <summary>
    /// Gets or sets the child directories
    /// </summary>
    public List<DirectoryItem> Children { get; set; } = new();

    /// <summary>
    /// Gets or sets the content items in this directory
    /// </summary>
    public List<ContentItem> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets additional metadata for the directory
    /// </summary>
    public IDictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
}