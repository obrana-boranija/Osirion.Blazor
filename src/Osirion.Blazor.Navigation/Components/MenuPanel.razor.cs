using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Navigation.Components;

/// <summary>
/// Renders a high-contrast promotional panel inside a mega menu,
/// typically used as an aside for featured links ("Platform highlights",
/// "Build your business case").
/// </summary>
public partial class MenuPanel
{
    /// <summary>Gets or sets the panel heading.</summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>Gets or sets optional supporting text below the heading.</summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>Gets or sets the accessibility label. Falls back to <see cref="Title"/>.</summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>Gets or sets the panel content, typically <see cref="MenuPanelLink"/> items.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
