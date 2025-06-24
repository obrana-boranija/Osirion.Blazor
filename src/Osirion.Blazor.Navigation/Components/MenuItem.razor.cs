using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;

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
    /// Gets the unique identifier for the menu item.
    /// </summary>
    private string ItemId => Id ?? $"osirion-menu-item-{Guid.NewGuid():N}";

    /// <summary>
    /// Gets the submenu identifier.
    /// </summary>
    private string SubmenuId => $"{ItemId}-submenu";

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Attributes is null)
        {
            Attributes = new Dictionary<string, object>();
        }

        // Add CSS classes for state
        var cssClasses = new List<string>();

        if (IsActive)
            cssClasses.Add("osirion-menu-item-active");

        if (Disabled)
            cssClasses.Add("osirion-menu-item-disabled");

        // Add role and accessibility attributes
        if (!Attributes.ContainsKey("id"))
            Attributes["id"] = ItemId;

        if (!Attributes.ContainsKey("role"))
            Attributes["role"] = "menuitem";

        if (HasSubmenu && !Attributes.ContainsKey("aria-haspopup"))
            Attributes["aria-haspopup"] = "true";

        if (HasSubmenu && !Attributes.ContainsKey("aria-expanded"))
            Attributes["aria-expanded"] = IsActive.ToString().ToLowerInvariant();

        if (HasSubmenu && !Attributes.ContainsKey("aria-controls"))
            Attributes["aria-controls"] = SubmenuId;

        if (Disabled && !Attributes.ContainsKey("aria-disabled"))
            Attributes["aria-disabled"] = "true";

        if (Target == "_blank" && !Attributes.ContainsKey("rel"))
            Attributes["rel"] = "noopener noreferrer";
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