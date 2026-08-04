using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>Defines the SearchBox type.</summary>
public partial class SearchBox
{
    /// <summary>Gets or sets the ActionUrl value.</summary>
    [Parameter]
    public string ActionUrl { get; set; } = "/search";

    /// <summary>Gets or sets the QueryParameterName value.</summary>
    [Parameter]
    public string QueryParameterName { get; set; } = "q";

    /// <summary>Gets or sets the SearchQuery value.</summary>
    [Parameter]
    public string SearchQuery { get; set; } = string.Empty;

    /// <summary>Gets or sets the Placeholder value.</summary>
    [Parameter]
    public string Placeholder { get; set; } = "Search content...";

    /// <summary>Gets or sets the SearchButtonText value.</summary>
    [Parameter]
    public string SearchButtonText { get; set; } = "Search";

    /// <summary>Gets or sets the Title value.</summary>
    [Parameter]
    public string? Title { get; set; }

    private string GetWrapperClass()
    {
        return $"osirion-search-container {Class}".Trim();
    }

    private string GetSearchBoxClass()
    {
        return "osirion-search-box";
    }

    private new string GetButtonClass()
    {
        return "osirion-search-button";
    }
}
