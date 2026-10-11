using Osirion.Blazor.Components;

namespace Osirion.Blazor.Navigation.Options;

/// <summary>
/// Configuration options for scroll to top functionality
/// </summary>
public class ScrollToTopOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string Section = "Osirion:Navigation:ScrollToTop";

    /// <summary>
    /// Gets or sets the position of the button
    /// </summary>
    public Position Position { get; set; } = Position.BottomRight;

    /// <summary>
    /// Gets or sets the scroll behavior
    /// </summary>
    public ScrollBehavior Behavior { get; set; } = ScrollBehavior.Smooth;

    /// <summary>
    /// Gets or sets the visibility threshold in pixels
    /// </summary>
    public int VisibilityThreshold { get; set; } = 300;

    /// <summary>
    /// Gets or sets the button text
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the button title
    /// </summary>
    public string Title { get; set; } = "Scroll to top";

    /// <summary>
    /// Gets or sets additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    /// <summary>
    /// Gets or sets custom icon markup. It is sanitized with the Osirion HTML sanitizer before it is rendered,
    /// which removes <c>svg</c> elements; use <see cref="IconCssClass"/> for an icon font or CSS mask icon.
    /// </summary>
    public string? CustomIcon { get; set; }

    /// <summary>
    /// Gets or sets the CSS class of an icon element rendered inside the button (for example an icon font
    /// class). Used when <see cref="CustomIcon"/> is not set or yields no markup after sanitizing.
    /// </summary>
    public string? IconCssClass { get; set; }

    /// <summary>
    /// Gets or sets whether the scroll to top button is rendered. Defaults to <see langword="true"/>.
    /// </summary>
    public bool Enabled { get; set; } = true;
}
