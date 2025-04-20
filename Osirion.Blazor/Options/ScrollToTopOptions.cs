using Osirion.Blazor.Components.Navigation;

namespace Osirion.Blazor.Options;

/// <summary>
/// Configuration options for the ScrollToTop component
/// </summary>
public class ScrollToTopOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string Section = "ScrollToTop";

    /// <summary>
    /// Gets or sets the behavior of the scroll animation
    /// </summary>
    public ScrollBehavior Behavior { get; set; } = ScrollBehavior.Smooth;

    /// <summary>
    /// Gets or sets the visibility threshold in pixels
    /// </summary>
    public int VisibilityThreshold { get; set; } = 300;

    /// <summary>
    /// Gets or sets the position of the button
    /// </summary>
    public ButtonPosition Position { get; set; } = ButtonPosition.BottomRight;

    /// <summary>
    /// Gets or sets the title attribute for the button
    /// </summary>
    public string Title { get; set; } = "Scroll to top";

    /// <summary>
    /// Gets or sets the text to display on the button
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the custom CSS class for the button
    /// </summary>
    public string? CssClass { get; set; }

    /// <summary>
    /// Gets or sets the custom SVG icon markup
    /// </summary>
    public string? CustomIcon { get; set; }

    /// <summary>
    /// Gets or sets whether to use the default styles
    /// </summary>
    public bool UseStyles { get; set; } = true;

    /// <summary>
    /// Gets or sets custom CSS variables to override the default values
    /// </summary>
    public string? CustomVariables { get; set; }

    /// <summary>
    /// Gets the behavior string for the script
    /// </summary>
    public string BehaviorString => Behavior switch
    {
        ScrollBehavior.Smooth => "smooth",
        ScrollBehavior.Instant => "instant",
        ScrollBehavior.Auto => "auto",
        _ => "smooth"
    };
}