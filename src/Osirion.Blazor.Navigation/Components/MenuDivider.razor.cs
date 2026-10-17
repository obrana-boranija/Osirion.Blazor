using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Navigation.Components;

/// <summary>
/// Renders a separator (<c>role="separator"</c>) between menu items: a horizontal rule in lists and the drawer, and a
/// short vertical rule between the items of a horizontal bar.
/// </summary>
public partial class MenuDivider
{
    /// <summary>
    /// Gets or sets an optional text shown with the separator.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }
}
