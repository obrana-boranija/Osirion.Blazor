using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Internal;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Navigation.Components;

/// <summary>
/// Renders a site menu: a <c>nav</c> with an optional brand link, your items and an optional actions slot. Submenus and
/// the mobile drawer open with CSS only, from visually hidden checkboxes, so it works under static SSR without script.
/// </summary>
public partial class Menu
{
    /// <summary>
    /// Gets or sets the menu items: <see cref="MenuItem"/>, <see cref="MenuDivider"/> and the other menu parts.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the destination of the brand link. A scheme other than a relative URL, <c>http</c>, <c>https</c>,
    /// <c>mailto</c> or <c>tel</c> leaves the link without an <c>href</c>.
    /// </summary>
    [Parameter]
    public string? Href { get; set; } = "/";

    /// <summary>
    /// Gets or sets the address of the logo image, shown before <see cref="BrandText"/> inside the brand link. The image
    /// is rendered 60 by 60 pixels, and its alternative text is the brand text followed by <c>brand logo</c>.
    /// </summary>
    [Parameter]
    public string? BrandLogo { get; set; }

    /// <summary>
    /// Gets or sets the brand name, rendered as the text of the brand link. Without a logo it is the whole link.
    /// </summary>
    [Parameter]
    public string? BrandText { get; set; }

    /// <summary>
    /// Gets or sets your own brand markup. It replaces the brand link, <see cref="BrandLogo"/> and
    /// <see cref="BrandText"/> and is not wrapped in a link.
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
    /// Gets or sets whether a horizontal menu stays at the top of the page while it scrolls. It has no effect on a
    /// vertical menu. Turn it off for a menu inside a page.
    /// </summary>
    [Parameter]
    public bool Sticky { get; set; } = true;

    /// <summary>
    /// Gets or sets the <c>z-index</c> written to the container's inline style while the menu is sticky.
    /// </summary>
    [Parameter]
    public int StickyZIndex { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the layout: <c>Horizontal</c>, a bar with dropdown submenus, or <c>Vertical</c>, a stack whose
    /// submenus open in place.
    /// </summary>
    [Parameter]
    public MenuOrientation Orientation { get; set; } = MenuOrientation.Horizontal;

    /// <summary>
    /// Gets or sets where a horizontal bar places its items: <c>Left</c>, <c>Center</c> or <c>Right</c>. A vertical
    /// menu ignores it.
    /// </summary>
    [Parameter]
    public MenuAlignment Alignment { get; set; } = MenuAlignment.Center;

    /// <summary>
    /// Gets or sets whether the items collapse behind a hamburger toggle and a drawer below the breakpoint in
    /// <see cref="CollapseBreakpoint"/>.
    /// </summary>
    [Parameter]
    public bool CollapseOnMobile { get; set; } = true;

    /// <summary>
    /// Gets or sets the viewport width below which a collapsible menu shows the toggle: <c>Mobile</c> (768px and below),
    /// <c>Large</c> (below 1200px) or <c>ExtraLarge</c> (below 1440px). Pick the narrowest width at which the bar still
    /// fits on one line. It applies only while <see cref="CollapseOnMobile"/> is on.
    /// </summary>
    [Parameter]
    public MenuCollapseBreakpoint CollapseBreakpoint { get; set; } = MenuCollapseBreakpoint.Mobile;

    /// <summary>
    /// Gets or sets whether a vertical menu gets the <c>osirion-menu-auto-expand</c> class on its container. The
    /// package's own styles do not use that class, so it only gives your CSS a hook.
    /// </summary>
    [Parameter]
    public bool AutoExpandActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the accessible name of the <c>nav</c> landmark. Give each menu on a page a different one.
    /// </summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Navigation menu";

    /// <summary>
    /// Gets or sets the accessible name of the hamburger toggle.
    /// </summary>
    [Parameter]
    public string ToggleAriaLabel { get; set; } = "Toggle navigation menu";

    /// <summary>
    /// Gets or sets the id of the container; the toggle and navigation ids derive from it. Without it each menu
    /// gets a unique generated id, so two menus on one page do not clash.
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
