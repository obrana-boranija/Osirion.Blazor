using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Admin.Components.UI;
public partial class Card
{
    [Parameter]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public RenderFragment? HeaderContent { get; set; }

    [Parameter]
    public RenderFragment? FooterContent { get; set; }

    [Parameter]
    public string CssClass { get; set; } = string.Empty;

    [Parameter]
    public string BodyCssClass { get; set; } = string.Empty;
}
