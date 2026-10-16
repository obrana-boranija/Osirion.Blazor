using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Examples.Shared.Pages;

public partial class Blog
{
    [Parameter]
    public string? Locale { get; set; } = "en";

    [Parameter]
    public string? Slug { get; set; }
}
