using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Components;

/// <summary>
/// Breadcrumb component for displaying navigation paths
/// </summary>
public partial class OsirionBreadcrumbs
{
    /// <summary>
    /// Gets or sets the path to parse for breadcrumbs
    /// </summary>
    [Parameter]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to show the home link
    /// </summary>
    [Parameter]
    public bool ShowHome { get; set; } = true;

    /// <summary>
    /// Gets or sets the home link text
    /// </summary>
    [Parameter]
    public string HomeText { get; set; } = "Home";

    /// <summary>
    /// Gets or sets the home URL
    /// </summary>
    [Parameter]
    public string HomeUrl { get; set; } = "/";

    /// <summary>
    /// Gets or sets whether to make the last item a link
    /// </summary>
    [Parameter]
    public bool LinkLastItem { get; set; } = false;

    /// <summary>
    /// Gets or sets the URL prefix for breadcrumb links
    /// </summary>
    [Parameter]
    public string UrlPrefix { get; set; } = "/";

    /// <summary>
    /// Gets or sets the segment formatter function
    /// </summary>
    [Parameter]
    public Func<string, string>? SegmentFormatter { get; set; }

    /// <summary>
    /// Gets the breadcrumb path information
    /// </summary>
    private BreadcrumbPath BreadcrumbInfo => new(Path);

    /// <summary>
    /// Gets the CSS class for the breadcrumbs
    /// </summary>
    private string GetBreadcrumbsClass()
    {
        return $"osirion-breadcrumbs {CssClass}".Trim();
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