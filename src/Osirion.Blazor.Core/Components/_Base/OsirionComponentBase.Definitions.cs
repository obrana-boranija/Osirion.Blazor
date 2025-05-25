using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace Osirion.Blazor.Components;

public abstract partial class OsirionComponentBase : ComponentBase
{
    /// <summary>
    /// Gets or sets the CSS class(es) for the component.
    /// </summary>
    [Parameter]
    public string? CssClass { get; set; }

    /// <summary>
    /// Gets or sets the theming options.
    /// </summary>
    [Inject]
    protected IOptions<ThemingOptions> ThemingOptions { get; set; } = default!;

    /// <summary>
    /// Gets the current theme mode.
    /// </summary>
    protected CssFramework Framework => ThemingOptions.Value.Framework;

    protected virtual string GetButtonClass()
    {
        switch (Framework)
        {
            case CssFramework.Bootstrap:
                return "btn btn-primary";
            case CssFramework.FluentUI:
                return "osirion-fluent-button";
            case CssFramework.MudBlazor:
                return "mud-button mud-button-filled";
            case CssFramework.Radzen:
                return "rz-button rz-button-primary";
            default:
                return "osirion-search-button";
        }
    }


    /// <summary>
    /// Combines the base CSS class with any additional classes provided.
    /// </summary>
    /// <param name="baseClass">The base CSS class for the component</param>
    /// <returns>Combined CSS class string</returns>
    protected string CombineCssClasses(string baseClass)
    {
        return string.IsNullOrWhiteSpace(CssClass)
            ? baseClass
            : $"{baseClass} {CssClass}";
    }
}