using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Navigation.Components;

/// <summary>Defines the MenuDivider type.</summary>
public partial class MenuDivider
{
    /// <summary>
    /// Gets or sets the divider label.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }
}
