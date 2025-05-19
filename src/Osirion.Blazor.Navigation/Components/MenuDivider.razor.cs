using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Navigation.Components;

public partial class MenuDivider
{
    /// <summary>
    /// Gets or sets the divider label.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }
}