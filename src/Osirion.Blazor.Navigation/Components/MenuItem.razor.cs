using Microsoft.AspNetCore.Components;

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
        {
            classes.Add("osirion-menu-item-disabled");

            // Add aria-disabled attribute
            if (AdditionalAttributes == null)
            {
                AdditionalAttributes = new Dictionary<string, object>();
            }

            if (!AdditionalAttributes.ContainsKey("aria-disabled"))
            {
                AdditionalAttributes["aria-disabled"] = "true";
            }

            if (!AdditionalAttributes.ContainsKey("tabindex"))
            {
                AdditionalAttributes["tabindex"] = "-1";
            }
        }

        if (HasSubmenu)
            classes.Add("osirion-menu-item-has-submenu");

        return string.Join(" ", classes);
    }
}