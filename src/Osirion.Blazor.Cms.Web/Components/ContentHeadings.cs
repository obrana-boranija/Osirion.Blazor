using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>
/// Helpers for the headings of rendered content.
/// </summary>
internal static partial class ContentHeadings
{
    // The content's first element when it is a top-level heading, the HTML of a Markdown "# Title" line.
    [GeneratedRegex(@"^\s*<h1\b[^>]*>.*?</h1>\s*", RegexOptions.IgnoreCase | RegexOptions.Singleline, matchTimeoutMilliseconds: 1000)]
    private static partial Regex LeadingTitleHeading();

    /// <summary>
    /// Removes the content's opening top-level heading. A page that shows the item's title as its own heading would
    /// otherwise show the title twice and contain two <c>h1</c> elements. A top-level heading further down is kept.
    /// </summary>
    /// <param name="html">The rendered content.</param>
    /// <returns>The content without its opening <c>h1</c>, or the content unchanged when it does not open with one.</returns>
    [return: NotNullIfNotNull(nameof(html))]
    public static string? WithoutLeadingTitle(string? html) =>
        string.IsNullOrEmpty(html) ? html : LeadingTitleHeading().Replace(html, string.Empty, 1);
}
