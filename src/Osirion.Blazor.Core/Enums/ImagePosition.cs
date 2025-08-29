namespace Osirion.Blazor.Components;

/// <summary>
/// Defines the position of the image relative to the content in the feature card.
/// This affects the layout direction and content alignment.
/// </summary>
public enum ImagePosition
{
    /// <summary>
    /// Image positioned at the top of the card.
    /// Layout: Column direction with image first, then content centered.
    /// Card width is constrained by CardSize, height adjusts to content.
    /// </summary>
    Top,

    /// <summary>
    /// Image positioned at the bottom of the card.
    /// Layout: Column direction with content first (centered), then image.
    /// Card width is constrained by CardSize, height adjusts to content.
    /// </summary>
    Bottom,

    /// <summary>
    /// Image positioned on the left side of the card (default).
    /// Layout: Row direction with image first, then content (left-aligned).
    /// Card height is constrained by CardSize, width adjusts to content.
    /// </summary>
    Left,

    /// <summary>
    /// Image positioned on the right side of the card.
    /// Layout: Row direction with content first (right-aligned), then image.
    /// Card height is constrained by CardSize, width adjusts to content.
    /// </summary>
    Right
}