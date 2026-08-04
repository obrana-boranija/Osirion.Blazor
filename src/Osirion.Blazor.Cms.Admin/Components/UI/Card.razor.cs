using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Admin.Components.UI;
/// <summary>Defines the Card type.</summary>
public partial class Card
{
    /// <summary>Gets or sets the Title value.</summary>
    [Parameter]
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the ChildContent value.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets the HeaderContent value.</summary>
    [Parameter]
    public RenderFragment? HeaderContent { get; set; }

    /// <summary>Gets or sets the FooterContent value.</summary>
    [Parameter]
    public RenderFragment? FooterContent { get; set; }

    /// <summary>Gets or sets the CssClass value.</summary>
    [Parameter]
    public string CssClass { get; set; } = string.Empty;

    /// <summary>Gets or sets the BodyCssClass value.</summary>
    [Parameter]
    public string BodyCssClass { get; set; } = string.Empty;
}
