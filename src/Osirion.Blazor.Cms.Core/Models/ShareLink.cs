namespace Osirion.Blazor.Cms.Models;

/// <summary>
/// Represents a share link for content.
/// </summary>
public class ShareLink
{
    /// <summary>
    /// Gets or sets the name of the sharing platform.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display label.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the SVG icon markup.
    /// </summary>
    public string Icon { get; set; } = string.Empty;
}