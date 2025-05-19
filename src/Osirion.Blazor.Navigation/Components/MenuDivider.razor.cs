// MenuDivider.razor.cs
using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Navigation.Components;

public partial class MenuDivider
{
    /// <summary>
    /// Gets or sets the divider label.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// Gets the CSS class for the menu divider.
    /// </summary>
    protected string GetDividerCssClass()
    {
        var classes = new List<string> { "osirion-menu-divider" };

        if (!string.IsNullOrEmpty(Label))
            classes.Add("osirion-menu-divider-with-label");

        if (!string.IsNullOrEmpty(CssClass))
            classes.Add(CssClass);

        return string.Join(" ", classes);
    }
}