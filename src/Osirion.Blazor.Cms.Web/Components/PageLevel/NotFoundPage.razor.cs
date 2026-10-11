using Microsoft.AspNetCore.Components;
#if !NET10_0_OR_GREATER
using Microsoft.AspNetCore.Http;
#endif
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>
/// A whole "page not found" page: asks search engines not to index it and offers a way home, a search box and
/// suggested pages. Use it as the router's <c>NotFoundPage</c> on .NET 10, which answers with status 404, or inside
/// the router's <c>NotFound</c> template on .NET 9, where the page sets status 404 itself under static server-side
/// rendering. It renders without script.
/// </summary>
public partial class NotFoundPage
{
#if !NET10_0_OR_GREATER
    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }
#endif

    /// <summary>Gets or sets the title, the page's <c>h1</c> and title tag. Defaults to <c>Page not found</c>.</summary>
    [Parameter]
    public string Title { get; set; } = "Page not found";

    /// <summary>Gets or sets the sentence under the title.</summary>
    [Parameter]
    public string Message { get; set; } = "The page you are looking for does not exist or has moved.";

    /// <summary>Gets or sets the address of the home link. Defaults to <c>/</c>.</summary>
    [Parameter]
    public string HomeUrl { get; set; } = "/";

    /// <summary>Gets or sets the text of the home link. Defaults to <c>Back to the home page</c>.</summary>
    [Parameter]
    public string HomeText { get; set; } = "Back to the home page";

    /// <summary>Gets or sets pages to suggest, for example the documentation and the blog. None by default.</summary>
    [Parameter]
    public IReadOnlyList<ContentNotFoundSuggestion>? Suggestions { get; set; }

    /// <summary>Gets or sets the heading of the suggestions. Defaults to <c>These pages might help:</c>.</summary>
    [Parameter]
    public string SuggestionsTitle { get; set; } = "These pages might help:";

    /// <summary>
    /// Gets or sets whether a search box is shown. Defaults to <see langword="false"/>; turn it on when the site has a
    /// search page at <see cref="SearchUrl"/>.
    /// </summary>
    [Parameter]
    public bool ShowSearch { get; set; }

    /// <summary>Gets or sets the search page the box submits to. Defaults to <c>/search</c>.</summary>
    [Parameter]
    public string SearchUrl { get; set; } = "/search";

    /// <summary>Gets or sets the placeholder of the search box. Defaults to <c>Search the site</c>.</summary>
    [Parameter]
    public string SearchPlaceholder { get; set; } = "Search the site";

    /// <summary>Gets or sets content shown below, for example a contact link.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

#if !NET10_0_OR_GREATER
    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // The .NET 9 router renders its NotFound template with status 200, which search engines treat as a soft 404.
        // On .NET 10 the router's NotFoundPage sets the status, and setting it here would make it send an empty page.
        if (HttpContext is { Response.HasStarted: false } httpContext)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        }
    }
#endif
}
