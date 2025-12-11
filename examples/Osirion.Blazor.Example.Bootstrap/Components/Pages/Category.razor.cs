using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Example.Bootstrap.Components.Pages;

public partial class Category
{
    [Parameter]
    public string? Locale { get; set; } = "en";

    [Parameter]
    public string? CategorySlug { get; set; }
}
