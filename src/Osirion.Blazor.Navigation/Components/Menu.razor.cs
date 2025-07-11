using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Navigation.Components;

public partial class Menu
{
    /// <summary>
    /// Gets or sets the menu items to be displayed.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets href
    /// </summary>
    [Parameter]
    public string? Href { get; set; } = "/";

    /// <summary>
    /// Gets or sets the branding logo to display in the navigation header.
    /// </summary>
    [Parameter]
    public string? BrandLogo { get; set; }

    /// <summary>
    /// Gets or sets the branding text to display in the navigation header.
    /// </summary>
    [Parameter]
    public string? BrandText { get; set; }

    /// <summary>
    /// Gets or sets a custom branding template.
    /// </summary>
    [Parameter]
    public RenderFragment? BrandingTemplate { get; set; }

    /// <summary>
    /// Gets or sets whether the menu should stick to the top when scrolling.
    /// Only applies to horizontal menus.
    /// </summary>
    [Parameter]
    public bool Sticky { get; set; } = true;

    /// <summary>
    /// Gets or sets the z-index to use when the menu is sticky.
    /// </summary>
    [Parameter]
    public int StickyZIndex { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the menu orientation: Horizontal or Vertical.
    /// </summary>
    [Parameter]
    public MenuOrientation Orientation { get; set; } = MenuOrientation.Horizontal;

    /// <summary>
    /// Gets or sets the alignment of horizontal menu items.
    /// </summary>
    [Parameter]
    public MenuAlignment Alignment { get; set; } = MenuAlignment.Center;

    /// <summary>
    /// Gets or sets whether to collapse the menu on mobile devices.
    /// </summary>
    [Parameter]
    public bool CollapseOnMobile { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to auto-expand active items in vertical mode.
    /// </summary>
    [Parameter]
    public bool AutoExpandActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the menu accessibility label.
    /// </summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Navigation menu";

    /// <summary>
    /// Gets or sets the mobile toggle button's accessibility label.
    /// </summary>
    [Parameter]
    public string ToggleAriaLabel { get; set; } = "Toggle navigation menu";

    /// <summary>
    /// Gets or sets the menu identifier (useful for multiple menus on same page).
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets the unique identifier for the menu component.
    /// </summary>
    private string MenuId => Id ?? $"osirion-menu-{Orientation.ToString().ToLowerInvariant()}";

    /// <summary>
    /// Gets the menu toggle ID.
    /// </summary>
    private string ToggleId => $"{MenuId}-toggle";

    protected override void OnInitialized()
    {
        base.OnInitialized();

        // Set menu orientation class
        var orientationClass = $"osirion-menu-{Orientation.ToString().ToLowerInvariant()}";

        // Set alignment class for horizontal orientation
        var alignmentClass = Orientation == MenuOrientation.Horizontal
            ? $"osirion-menu-align-{Alignment.ToString().ToLowerInvariant()}"
            : string.Empty;

        // Set auto-expand class for vertical orientation
        var expandClass = Orientation == MenuOrientation.Vertical && AutoExpandActive
            ? "osirion-menu-auto-expand"
            : string.Empty;

        // Combine all classes
        var classes = new List<string> { orientationClass };

        if (!string.IsNullOrWhiteSpace(alignmentClass))
            classes.Add(alignmentClass);

        if (!string.IsNullOrWhiteSpace(expandClass))
            classes.Add(expandClass);

        // Add collapsible class if needed - this is critical for mobile behavior
        if (CollapseOnMobile)
            classes.Add("osirion-menu-collapsible");

        // Add sticky class if enabled (only for horizontal menus)
        if (Sticky && Orientation == MenuOrientation.Horizontal)
            classes.Add("osirion-menu-sticky");

        // Combine with existing CssClass
        Class = string.IsNullOrWhiteSpace(Class)
            ? string.Join(" ", classes)
            : $"{Class} {string.Join(" ", classes)}";

        // Apply z-index as inline style for sticky menu
        if (Sticky && Orientation == MenuOrientation.Horizontal && !Attributes.ContainsKey("style"))
        {
            Attributes["style"] = $"z-index: {StickyZIndex};";
        }
        else if (Sticky && Orientation == MenuOrientation.Horizontal)
        {
            Attributes["style"] = $"{Attributes["style"]} z-index: {StickyZIndex};";
        }

        // Set ID attribute
        if (!Attributes.ContainsKey("id"))
            Attributes["id"] = MenuId;
    }
}

/// <summary>
/// Defines the menu orientation options.
/// </summary>
public enum MenuOrientation
{
    /// <summary>
    /// Horizontal menu layout
    /// </summary>
    Horizontal,

    /// <summary>
    /// Vertical menu layout
    /// </summary>
    Vertical
}

/// <summary>
/// Defines the menu alignment options for horizontal layout.
/// </summary>
public enum MenuAlignment
{
    /// <summary>
    /// Left-aligned items
    /// </summary>
    Left,

    /// <summary>
    /// Center-aligned items
    /// </summary>
    Center,

    /// <summary>
    /// Right-aligned items
    /// </summary>
    Right
}