using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Example.Bootstrap.Components.Pages;

public partial class Tag
{
    [Parameter]
    public string? Locale { get; set; } = "en";

    [Parameter]
    public string? TagSlug { get; set; }
}
