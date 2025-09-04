using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;

namespace Osirion.Blazor.Navigation.Components;

public partial class MenuItem
{
    /// <summary>
    /// Gets or sets the text content of the menu item.
    /// </summary>
    [Parameter]
    public string Text { get; set; } = string.Empty;

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

    /// <summary>
    /// Gets the unique identifier for the submenu.
    /// </summary>
    internal string SubmenuId => $"submenu-{Id ?? Guid.NewGuid().ToString("N")[..8]}";

    /// <summary>
    /// Gets whether this menu item should be considered active based on the current URL.
    /// </summary>
    private bool ShouldBeActive
    {
        get
        {
            // If IsActive is explicitly set, respect that setting
            if (IsActive)
                return true;

            // Skip auto-detection if disabled or if AutoDetectActive is false
            if (!AutoDetectActive || string.IsNullOrWhiteSpace(Href) || Href == "#")
                return false;

            var currentUri = NavigationManager.Uri;
            var baseUri = NavigationManager.BaseUri;
            
            // Get the relative path from current URI
            var relativePath = NavigationManager.ToBaseRelativePath(currentUri);
            
            // Normalize the href (remove leading slash if present)
            var normalizedHref = Href.TrimStart('/');
            
            // Handle different matching modes
            return Match switch
            {
                NavLinkMatch.All => string.Equals(relativePath, normalizedHref, StringComparison.OrdinalIgnoreCase),
                NavLinkMatch.Prefix => relativePath.StartsWith(normalizedHref, StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        // Update IsActive based on automatic detection
        if (AutoDetectActive && !IsActive)
        {
            IsActive = ShouldBeActive;
        }

        if (Attributes is null)
        {
            Attributes = new Dictionary<string, object>();
        }

        // Add rel attribute for external links
        if (Target == "_blank" && !Attributes.ContainsKey("rel"))
            Attributes["rel"] = "noopener noreferrer";
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        // Update active state on parameter changes (for example, when navigation occurs)
        if (AutoDetectActive && !IsActive)
        {
            IsActive = ShouldBeActive;
        }
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
}