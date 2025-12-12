using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace Osirion.Blazor.Example.Bootstrap.Components.Pages;

public partial class Category
{
    [Parameter]
    public string? Locale { get; set; } = "en";

    [Parameter]
    public string? CategorySlug { get; set; }

    private string Title
    {
        get
        {
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(CategorySlug?.ToLower().Replace("-", " ").Replace("_", " ").Replace("\"", " ").Replace("'", " ") ?? "no category selected");
        }
    }
}
