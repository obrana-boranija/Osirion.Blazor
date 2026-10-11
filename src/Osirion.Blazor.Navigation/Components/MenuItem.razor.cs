using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Osirion.Blazor.Navigation.Internal;

namespace Osirion.Blazor.Navigation.Components;

/// <summary>Defines the MenuItem type.</summary>
public partial class MenuItem : IDisposable
{
    /// <summary>
    /// Gets or sets the text content of the menu item.
    /// </summary>
    [Parameter]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets optional supporting text displayed below the menu item text.
    /// </summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the icon name for the menu item.
    /// </summary>
    [Parameter]
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets a custom icon template.
    /// </summary>
    [Parameter]
    public RenderFragment? IconTemplate { get; set; }

    /// <summary>
    /// Gets or sets the URL for the menu item.
    /// </summary>
    [Parameter]
    public string Href { get; set; } = "#";

    /// <summary>
    /// Gets or sets whether this item is active.
    /// </summary>
    [Parameter]
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets whether this item is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets whether this item has a submenu.
    /// </summary>
    [Parameter]
    public bool HasSubmenu { get; set; }

    /// <summary>
    /// Gets or sets the submenu content.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets optional aside content for mega submenus, rendered as a
    /// dedicated right-hand column. Typically a <see cref="MenuPanel"/> or
    /// <see cref="MenuImageCard"/>. Only used when <see cref="SubmenuVariant"/> is Mega.
    /// </summary>
    [Parameter]
    public RenderFragment? SubmenuAside { get; set; }

    /// <summary>
    /// Gets or sets the visual presentation of the submenu.
    /// </summary>
    [Parameter]
    public SubmenuVariant SubmenuVariant { get; set; } = SubmenuVariant.Regular;

    /// <summary>
    /// Gets or sets the match behavior for automatic active state detection.
    /// </summary>
    [Parameter]
    public NavLinkMatch Match { get; set; } = NavLinkMatch.All;

    /// <summary>
    /// Gets or sets how to open the link (useful for external links).
    /// </summary>
    [Parameter]
    public string? Target { get; set; }

    /// <summary>
    /// Event callback for when the menu item is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Gets or sets the identifier for the menu item.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets whether to enable automatic active state detection.
    /// When true, the component will automatically detect if the current URL matches this menu item's href.
    /// </summary>
    [Parameter]
    public bool AutoDetectActive { get; set; } = true;

    /// <summary>
    /// Navigation manager for URL matching and navigation.
    /// </summary>
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    // The icon names the markup has an SVG for. Any other name renders no icon at all, rather than an empty
    // icon slot that indents the text.
    private static readonly HashSet<string> BuiltInIcons = new(StringComparer.OrdinalIgnoreCase)
    {
        "home", "blog", "document", "docs", "documentation", "user", "profile", "contact", "about", "info",
        "settings", "gear", "dashboard"
    };

    private readonly string _generatedId = Guid.NewGuid().ToString("N")[..8];

    // Derived render state. [Parameter] properties are never written by the component, so a parent
    // re-render cannot be overridden by a stale computed value.
    private bool _isActive;
    private string? _renderedHref;
    private bool _isToggleOnly;
    private bool _isSubscribed;

    /// <summary>
    /// Gets whether <see cref="Icon"/> names one of the built-in icons.
    /// </summary>
    private bool HasBuiltInIcon => Icon is not null && BuiltInIcons.Contains(Icon);

    /// <summary>
    /// Gets the unique identifier for the submenu.
    /// </summary>
    internal string SubmenuId => $"submenu-{Id ?? _generatedId}";

    /// <summary>
    /// Gets the identifier for the submenu toggle control.
    /// </summary>
    internal string SubmenuToggleId => $"{SubmenuId}-toggle";

    internal string SubmenuVariantClass => SubmenuVariant switch
    {
        SubmenuVariant.Mega => "osirion-submenu-mega",
        _ => "osirion-submenu-regular"
    };

    /// <summary>Initializes the component state and required services.</summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        // Static SSR renders once per request, so only an interactive item can see the location change.
        if (IsInteractive && AutoDetectActive)
        {
            NavigationManager.LocationChanged += OnLocationChanged;
            _isSubscribed = true;
        }
    }

    /// <summary>Recomputes the effective link target and active state from the current parameters.</summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        RefreshRenderState();
    }

    private void RefreshRenderState()
    {
        // A disabled item and a rejected URL (for example a javascript: scheme) render no href at all.
        _renderedHref = Disabled ? null : SafeHref.Sanitize(Href);
        _isActive = IsActive || (AutoDetectActive && MatchesCurrentLocation());

        // A parent that only groups its submenu has nowhere to go, so the whole item toggles the submenu.
        _isToggleOnly = HasSubmenu && !Disabled && !OnClick.HasDelegate
            && (string.IsNullOrWhiteSpace(Href) || Href == "#");
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        var wasActive = _isActive;
        RefreshRenderState();

        if (wasActive != _isActive)
        {
            _ = InvokeAsync(StateHasChanged);
        }
    }

    private bool MatchesCurrentLocation()
    {
        if (string.IsNullOrWhiteSpace(Href) || Href == "#" || Disabled)
        {
            return false;
        }

        var currentPath = TrimToPath(NavigationManager.ToBaseRelativePath(NavigationManager.Uri));
        var targetPath = TrimToPath(Href);

        return Match switch
        {
            NavLinkMatch.All => string.Equals(currentPath, targetPath, StringComparison.OrdinalIgnoreCase),
            NavLinkMatch.Prefix => targetPath.Length == 0
                || string.Equals(currentPath, targetPath, StringComparison.OrdinalIgnoreCase)
                || currentPath.StartsWith(targetPath + "/", StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }

    // Compares paths only: a query string or fragment must not change the match, and neither must
    // a leading or trailing slash.
    private static string TrimToPath(string value)
    {
        var end = value.AsSpan().IndexOfAny('?', '#');
        var path = end >= 0 ? value[..end] : value;
        return path.Trim('/');
    }

    /// <summary>
    /// Handles the click event on the menu item.
    /// </summary>
    /// <param name="args">Mouse event arguments</param>
    private async Task OnClickHandler(MouseEventArgs args)
    {
        if (!Disabled && OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync(args);
        }
    }

    /// <summary>Releases the location change subscription.</summary>
    public void Dispose()
    {
        if (_isSubscribed)
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
            _isSubscribed = false;
        }
    }
}
