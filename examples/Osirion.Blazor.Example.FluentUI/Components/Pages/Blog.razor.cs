using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Example.FluentUI.Components.Pages;

public partial class Blog
{
    [Parameter]
    public string? Locale { get; set; } = "en";

    [Parameter]
    public string? Slug { get; set; }
}
