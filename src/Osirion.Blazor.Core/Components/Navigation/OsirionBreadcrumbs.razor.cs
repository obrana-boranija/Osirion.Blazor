using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Components;

/// <summary>
/// Renders a breadcrumb trail from a URL path: an optional home link, one link per path segment, and the current page
/// last. The trail is a <c>nav</c> landmark with an ordered list and needs no script.
/// </summary>
public partial class OsirionBreadcrumbs
{
    /// <summary>
    /// Gets or sets the path to split into crumbs, for example <c>/docs/components/navigation</c>. Leading and trailing
    /// slashes are ignored, and each segment becomes one crumb. Pass the path only: a query string would end up in the
    /// last segment.
    /// </summary>
    [Parameter]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether a home link is rendered before the path segments. <see cref="HomeText"/> and
    /// <see cref="HomeUrl"/> set its text and address.
    /// </summary>
    [Parameter]
    public bool ShowHome { get; set; } = true;

    /// <summary>
    /// Gets or sets the text of the home link.
    /// </summary>
    [Parameter]
    public string HomeText { get; set; } = "Home";

    /// <summary>
    /// Gets or sets the address of the home link, written as given.
    /// </summary>
    [Parameter]
    public string HomeUrl { get; set; } = "/";

    /// <summary>
    /// Gets or sets whether the last crumb, the current page, is a link instead of text. It carries
    /// <c>aria-current="page"</c> either way.
    /// </summary>
    [Parameter]
    public bool LinkLastItem { get; set; } = false;

    /// <summary>
    /// Gets or sets the text put before each segment's cumulative path to form its link, for example <c>/en/</c> for a
    /// localized site. With <c>Path="/a/b/c"</c> and the default prefix the crumbs link to <c>/a</c> and <c>/a/b</c>,
    /// and to <c>/a/b/c</c> when <see cref="LinkLastItem"/> is set. The home link does not use it.
    /// </summary>
    [Parameter]
    public string UrlPrefix { get; set; } = "/";

    /// <summary>
    /// Gets or sets a function that returns the text of a segment from its raw slug. Without it, hyphens become spaces
    /// and each word starts with a capital letter. It does not apply to the last crumb while
    /// <see cref="CurrentPageText"/> is set.
    /// </summary>
    [Parameter]
    public Func<string, string>? SegmentFormatter { get; set; }

    /// <summary>
    /// Gets or sets the text of the last crumb, the current page. Pass the page title: a slug turned into words
    /// loses its punctuation and casing ("Pre Mvp" for "Pre-MVP"). When empty, the last segment is formatted like the
    /// others. Available since 4.0.
    /// </summary>
    [Parameter]
    public string? CurrentPageText { get; set; }

    /// <summary>
    /// Gets the breadcrumb path information
    /// </summary>
    private BreadcrumbPath BreadcrumbInfo => new(Path);

    /// <summary>
    /// Gets the CSS class for the breadcrumbs
    /// </summary>
    private string GetBreadcrumbsClass()
    {
        return $"osirion-breadcrumbs {Class}".Trim();
    }

    private string GetSegmentText(BreadcrumbSegment segment)
    {
        if (segment.IsLast && !string.IsNullOrWhiteSpace(CurrentPageText))
        {
            return CurrentPageText;
        }

        return SegmentFormatter?.Invoke(segment.Name) ?? FormatSegmentName(segment.Name);
    }

    /// <summary>
    /// Default formatter for segment names - converts slug-case to Title Case
    /// </summary>
    private string FormatSegmentName(string segmentName)
    {
        // Replace hyphens with spaces and title case
        return Regex.Replace(segmentName, "-", " ", RegexOptions.Compiled)
            .Split(' ')
            .Select(word => word.Length > 0
                ? char.ToUpperInvariant(word[0]) + word[1..]
                : word)
            .Aggregate((a, b) => $"{a} {b}");
    }
}
