// MenuItem.razor.cs
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

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
    public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;

    /// <summary>
    /// Gets or sets how to open the link (useful for external links).
    /// </summary>
    [Parameter]
    public string? Target { get; set; }

    /// <summary>
    /// Gets or sets custom aria-expanded value.
    /// </summary>
    [Parameter]
    public bool? AriaExpanded { get; set; }

    /// <summary>
    /// Gets or sets the items identifier.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets the unique identifier for the menu item.
    /// </summary>
    private string ItemId => Id ?? $"osirion-menu-item-{Guid.NewGuid():N}";

    /// <summary>
    /// Gets the submenu identifier.
    /// </summary>
    private string SubmenuId => $"{ItemId}-submenu";

    /// <summary>
    /// Gets the CSS class for the menu item.
    /// </summary>
    protected string GetItemCssClass()
    {
        var classes = new List<string> { "osirion-menu-item" };

        if (!string.IsNullOrEmpty(CssClass))
            classes.Add(CssClass);

        if (IsActive)
            classes.Add("osirion-menu-item-active");

        if (Disabled)
            classes.Add("osirion-menu-item-disabled");

        if (HasSubmenu)
            classes.Add("osirion-menu-item-has-submenu");

        return string.Join(" ", classes);
    }

    /// <summary>
    /// Gets the CSS class for the submenu.
    /// </summary>
    protected string GetSubmenuCssClass()
    {
        return IsActive ? "osirion-submenu osirion-submenu-active" : "osirion-submenu";
    }

    /// <summary>
    /// Gets additional attributes for the menu item.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (AdditionalAttributes == null)
        {
            AdditionalAttributes = new Dictionary<string, object>();
        }

        // Set ID attribute
        if (!AdditionalAttributes.ContainsKey("id"))
            AdditionalAttributes["id"] = ItemId;

        // Add aria attributes
        if (Disabled && !AdditionalAttributes.ContainsKey("aria-disabled"))
            AdditionalAttributes["aria-disabled"] = "true";

        if (!AdditionalAttributes.ContainsKey("role"))
            AdditionalAttributes["role"] = "menuitem";

        // Handle aria-expanded for submenus
        if (HasSubmenu && !AdditionalAttributes.ContainsKey("aria-expanded") && AriaExpanded.HasValue)
            AdditionalAttributes["aria-expanded"] = AriaExpanded.Value.ToString().ToLowerInvariant();

        if (HasSubmenu && !AdditionalAttributes.ContainsKey("aria-controls"))
            AdditionalAttributes["aria-controls"] = SubmenuId;

        // Handle target for links
        if (!string.IsNullOrEmpty(Target) && !AdditionalAttributes.ContainsKey("target"))
            AdditionalAttributes["target"] = Target;

        // Add rel="noopener" for security when target="_blank"
        if (Target == "_blank" && !AdditionalAttributes.ContainsKey("rel"))
            AdditionalAttributes["rel"] = "noopener";

        // Set tabindex for disabled items
        if (Disabled && !AdditionalAttributes.ContainsKey("tabindex"))
            AdditionalAttributes["tabindex"] = "-1";
    }
}