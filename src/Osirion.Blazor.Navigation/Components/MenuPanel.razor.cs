using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Navigation.Components;

/// <summary>
/// Renders a high-contrast promotional panel inside a mega menu,
/// typically used as an aside for featured links ("Platform highlights",
/// "Build your business case").
/// </summary>
public partial class MenuPanel
{
    /// <summary>Gets or sets the heading of the panel. It is also the accessible name unless <see cref="AriaLabel"/> is set.</summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>Gets or sets supporting text under the heading.</summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>Gets or sets the accessible name of the <c>aside</c>. Falls back to <see cref="Title"/>.</summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>Gets or sets the links of the panel, typically <see cref="MenuPanelLink"/> items.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
