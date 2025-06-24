namespace Osirion.Blazor.Cms.Domain.Models;

/// <summary>
/// Represents a text selection within a content item.
/// </summary>
public class TextSelection
{
    /// <summary>
    /// Gets or sets the text of the selection.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the start and end positions of the selection within the text.
    /// </summary>
    public int Start { get; set; }

    /// <summary>
    /// Gets or sets the end position of the selection within the text.
    /// </summary>
    public int End { get; set; }
}