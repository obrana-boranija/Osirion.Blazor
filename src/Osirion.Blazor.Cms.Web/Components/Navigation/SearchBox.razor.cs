using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Web.Components;

public partial class SearchBox
{
    [Parameter]
    public string ActionUrl { get; set; } = "/search";

    [Parameter]
    public string QueryParameterName { get; set; } = "q";

    [Parameter]
    public string SearchQuery { get; set; } = string.Empty;

    [Parameter]
    public string Placeholder { get; set; } = "Search content...";

    [Parameter]
    public string SearchButtonText { get; set; } = "Search";

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
}
