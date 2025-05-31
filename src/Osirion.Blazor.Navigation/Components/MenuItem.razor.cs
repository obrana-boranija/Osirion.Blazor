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

        if (AdditionalAttributes is null)
        {
            AdditionalAttributes = new Dictionary<string, object>();
        }

        // Add CSS classes for state
        var cssClasses = new List<string>();

        if (IsActive)
            cssClasses.Add("osirion-menu-item-active");

        if (Disabled)
            cssClasses.Add("osirion-menu-item-disabled");

        // Add role and accessibility attributes
        if (!AdditionalAttributes.ContainsKey("id"))
            AdditionalAttributes["id"] = ItemId;

        if (!AdditionalAttributes.ContainsKey("role"))
            AdditionalAttributes["role"] = "menuitem";

        if (HasSubmenu && !AdditionalAttributes.ContainsKey("aria-haspopup"))
            AdditionalAttributes["aria-haspopup"] = "true";

        if (HasSubmenu && !AdditionalAttributes.ContainsKey("aria-expanded"))
            AdditionalAttributes["aria-expanded"] = IsActive.ToString().ToLowerInvariant();

        if (HasSubmenu && !AdditionalAttributes.ContainsKey("aria-controls"))
            AdditionalAttributes["aria-controls"] = SubmenuId;

        if (Disabled && !AdditionalAttributes.ContainsKey("aria-disabled"))
            AdditionalAttributes["aria-disabled"] = "true";

        if (Target == "_blank" && !AdditionalAttributes.ContainsKey("rel"))
            AdditionalAttributes["rel"] = "noopener noreferrer";
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