using Osirion.Blazor.Components.Navigation;

namespace Osirion.Blazor.Services;

/// <summary>
/// Service to manage the global ScrollToTop button
/// </summary>
public class ScrollToTopManager
{
    /// <summary>
    /// Gets or sets whether the ScrollToTop button is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// Gets or sets the position of the ScrollToTop button
    /// </summary>
    public ButtonPosition Position { get; set; } = ButtonPosition.BottomRight;

    /// <summary>
    /// Gets or sets the scroll behavior
    /// </summary>
    public ScrollBehavior Behavior { get; set; } = ScrollBehavior.Smooth;

    /// <summary>
    /// Gets or sets the visibility threshold in pixels
    /// </summary>
    public int VisibilityThreshold { get; set; } = 300;

    /// <summary>
    /// Gets or sets the text to display on the button
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the title/tooltip for the button
    /// </summary>
    public string Title { get; set; } = "Scroll to top";

    /// <summary>
    /// Gets or sets the custom CSS class
    /// </summary>
    public string? CssClass { get; set; }

    /// <summary>
    /// Gets or sets custom SVG icon markup
    /// </summary>
    public string? CustomIcon { get; set; }
}