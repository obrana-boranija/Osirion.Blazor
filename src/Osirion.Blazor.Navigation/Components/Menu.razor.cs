using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Internal;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Navigation.Components;

/// <summary>Defines the Menu type.</summary>
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
    /// Gets or sets content shown at the end of the bar, after the menu items: a search box, a theme toggle or a
    /// call to action button. A horizontal menu aligns it to the trailing edge; a collapsed menu shows it at the
    /// bottom of its drawer.
    /// </summary>
    [Parameter]
    public RenderFragment? Actions { get; set; }

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
    /// Gets or sets the viewport breakpoint at which a collapsible menu is displayed as a toggle.
    /// </summary>
    [Parameter]
    public MenuCollapseBreakpoint CollapseBreakpoint { get; set; } = MenuCollapseBreakpoint.Mobile;

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
    /// Gets the brand link destination, or <see langword="null"/> when <see cref="Href"/> uses a scheme
    /// that is not safe (for example <c>javascript:</c>).
    /// </summary>
    private string? RenderedHref => SafeHref.Sanitize(Href);

    // Generated once per instance. A fixed per-orientation default would give two menus with the same
    // orientation the same id, and the toggle label would then switch the wrong menu.
    private readonly string _generatedId = Guid.NewGuid().ToString("N")[..8];

    // Derived render state, rebuilt from the parameters on every parameter update. It is never written
    // back into a parameter, so a parent re-render cannot wipe it.
    private string _containerClass = "osirion-menu-container";
    private Dictionary<string, object> _containerAttributes = [];

    /// <summary>
    /// Gets the unique identifier for the menu component.
    /// </summary>
    private string MenuId => Id ?? $"osirion-menu-{Orientation.ToString().ToLowerInvariant()}-{_generatedId}";

    /// <summary>
    /// Gets the menu toggle ID.
    /// </summary>
    private string ToggleId => $"{MenuId}-toggle";

    /// <summary>
    /// Gets the identifier of the navigation landmark the mobile toggle controls.
    /// </summary>
    private string NavId => $"{MenuId}-nav";

    /// <summary>
    /// Gets the component theme for the data attribute.
    /// Returns the component's specific theme if set, otherwise null to use global theme.
    /// </summary>
    private string? GetComponentTheme()
    {
        return Theme switch
        {
            ThemeMode.Light => "light",
            ThemeMode.Dark => "dark",
            ThemeMode.System => null, // Use global theme
            _ => null
        };
    }

    /// <summary>Recomputes the container classes and attributes from the current parameters.</summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        var classes = new List<string>
        {
            "osirion-menu-container",
            $"osirion-menu-{Orientation.ToString().ToLowerInvariant()}"
        };

        if (Orientation == MenuOrientation.Horizontal)
            classes.Add($"osirion-menu-align-{Alignment.ToString().ToLowerInvariant()}");

        if (Orientation == MenuOrientation.Vertical && AutoExpandActive)
            classes.Add("osirion-menu-auto-expand");

        // The collapsible class is critical for the mobile behavior.
        if (CollapseOnMobile)
            classes.Add("osirion-menu-collapsible");

        if (CollapseOnMobile)
            classes.Add(CollapseBreakpoint switch
            {
                MenuCollapseBreakpoint.Large => "osirion-menu-collapse-large",
                MenuCollapseBreakpoint.ExtraLarge => "osirion-menu-collapse-extra-large",
                _ => "osirion-menu-collapse-mobile"
            });

        if (Sticky && Orientation == MenuOrientation.Horizontal)
            classes.Add("osirion-menu-sticky");

        if (!string.IsNullOrWhiteSpace(Class))
            classes.Add(Class);

        _containerClass = string.Join(' ', classes);

        var attributes = new Dictionary<string, object>(Attributes ?? new Dictionary<string, object>());

        if (Sticky && Orientation == MenuOrientation.Horizontal)
        {
            var zIndex = $"z-index: {StickyZIndex};";
            attributes["style"] = attributes.TryGetValue("style", out var existingStyle)
                ? $"{existingStyle} {zIndex}"
                : zIndex;
        }

        if (!attributes.ContainsKey("id"))
            attributes["id"] = MenuId;

        _containerAttributes = attributes;
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
/// Defines the viewport breakpoints for a collapsible menu.
/// </summary>
public enum MenuCollapseBreakpoint
{
    /// <summary>
    /// Collapses at the mobile breakpoint (768px and below).
    /// </summary>
    Mobile,

    /// <summary>
    /// Collapses below the extra-large breakpoint (1440px).
    /// </summary>
    ExtraLarge,

    /// <summary>
    /// Collapses below the large breakpoint (1200px), which suits a bar with a few items and a search box or other
    /// actions.
    /// </summary>
    Large
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
