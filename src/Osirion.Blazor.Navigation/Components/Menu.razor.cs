using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Navigation.Components;

public partial class Menu
{
    /// <summary>
    /// Gets or sets the menu items to be displayed.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets whether the menu should stick to the top when scrolling.
    /// Only applies to horizontal menus.
    /// </summary>
    [Parameter]
    public bool IsSticky { get; set; } = true;

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
    /// Gets or sets whether to collapse the menu on mobile devices.
    /// </summary>
    [Parameter]
    public bool CollapseOnMobile { get; set; } = true;

    /// <summary>
    /// Gets or sets the menu accessibility label.
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (AdditionalAttributes == null)
        {
            AdditionalAttributes = new Dictionary<string, object>();
        }

        // Set the orientation class
        CssClass = string.IsNullOrEmpty(CssClass)
            ? $"osirion-menu-{Orientation.ToString().ToLowerInvariant()}"
            : $"{CssClass} osirion-menu-{Orientation.ToString().ToLowerInvariant()}";

        // Set collapse class if enabled
        if (CollapseOnMobile)
        {
            CssClass += " osirion-menu-collapsible";
        }

        // Set sticky class if enabled (only for horizontal menus)
        if (IsSticky && Orientation == MenuOrientation.Horizontal)
        {
            CssClass += " osirion-menu-sticky";

            // Apply z-index as inline style
            if (!AdditionalAttributes.ContainsKey("style"))
            {
                AdditionalAttributes["style"] = $"z-index: {StickyZIndex};";
            }
            else
            {
                AdditionalAttributes["style"] = $"{AdditionalAttributes["style"]} z-index: {StickyZIndex};";
            }
        }

        // Add aria-label for accessibility
        if (!string.IsNullOrEmpty(AriaLabel) && !AdditionalAttributes.ContainsKey("aria-label"))
        {
            AdditionalAttributes["aria-label"] = AriaLabel;
        }
    }
}

/// <summary>
/// Defines the menu orientation options.
/// </summary>
public enum MenuOrientation
{
    Horizontal,
    Vertical
}