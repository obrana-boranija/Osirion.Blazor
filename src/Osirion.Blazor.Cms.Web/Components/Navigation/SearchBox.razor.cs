using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>Defines the SearchBox type.</summary>
public partial class SearchBox
{
    /// <summary>Gets or sets the page the form submits to with GET. Defaults to <c>/search</c>.</summary>
    [Parameter]
    public string ActionUrl { get; set; } = "/search";

    /// <summary>
    /// Gets or sets the name of the query string parameter that carries the search text, which is also the name of
    /// the input. Defaults to <c>q</c>.
    /// </summary>
    [Parameter]
    public string QueryParameterName { get; set; } = "q";

    /// <summary>
    /// Gets or sets the text shown in the field, usually the query of the results page, so it stays in the field
    /// after a search.
    /// </summary>
    [Parameter]
    public string SearchQuery { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the placeholder of the field. It is also the field's <c>aria-label</c>, so screen readers
    /// announce it as the field's name.
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; } = "Search content...";

    /// <summary>
    /// Gets or sets the visible text of the submit button. The button's accessible name stays <c>Submit search</c>.
    /// </summary>
    [Parameter]
    public string SearchButtonText { get; set; } = "Search";

    /// <summary>Gets or sets the heading above the form, rendered as an <c>h2</c>. No heading when <c>null</c>.</summary>
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
