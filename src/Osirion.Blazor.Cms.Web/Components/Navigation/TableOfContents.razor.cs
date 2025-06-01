using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>
/// TableOfContents component for extracting and displaying headings from HTML content
/// </summary>
public partial class TableOfContents
{
    /// <summary>
    /// Gets or sets the HTML content to extract headings from
    /// </summary>
    [Parameter]
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the minimum heading level to include
    /// </summary>
    [Parameter]
    public int MinLevel { get; set; } = 2;

    /// <summary>
    /// Gets or sets the maximum heading level to include
    /// </summary>
    [Parameter]
    public int MaxLevel { get; set; } = 6;

    /// <summary>
    /// Gets or sets the maximum nesting depth
    /// </summary>
    [Parameter]
    public int MaxDepth { get; set; } = 3;

    /// <summary>
    /// Gets or sets the text to display when no headings are found
    /// </summary>
    [Parameter]
    public string EmptyText { get; set; } = "No headings found.";

    /// <summary>
    /// Gets the extracted headings
    /// </summary>
    private IReadOnlyList<HeadingItem> Headings { get; set; } = Array.Empty<HeadingItem>();

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        if (!string.IsNullOrWhiteSpace(Content))
        {
            Headings = ExtractHeadings(Content);
        }
        else
        {
            Headings = Array.Empty<HeadingItem>();
        }
    }

    /// <summary>
    /// Extracts headings from HTML content
    /// </summary>
    private IReadOnlyList<HeadingItem> ExtractHeadings(string htmlContent)
    {
        var headings = new List<HeadingItem>();
        var pattern = @"<h([1-6])(?:\s+id=[""']([^""']*)[""'])?[^>]*>(.*?)</h\1>";
        var matches = Regex.Matches(htmlContent, pattern, RegexOptions.IgnoreCase);

        foreach (Match match in matches)
        {
            var level = int.Parse(match.Groups[1].Value);

            if (level < MinLevel || level > MaxLevel)
                continue;

            var id = match.Groups[2].Value;
            var text = StripHtmlTags(match.Groups[3].Value);

            // Generate ID if not present
            if (string.IsNullOrWhiteSpace(id))
            {
                id = GenerateId(text);
            }

            headings.Add(new HeadingItem
            {
                Level = level,
                Id = id,
                Text = text,
                Children = new List<HeadingItem>()
            });
        }

        return BuildHierarchy(headings);
    }

    /// <summary>
    /// Builds a hierarchical structure from flat headings list
    /// </summary>
    private IReadOnlyList<HeadingItem> BuildHierarchy(List<HeadingItem> flatHeadings)
    {
        var result = new List<HeadingItem>();
        var stack = new Stack<HeadingItem>();

        foreach (var heading in flatHeadings)
        {
            // Remove items from stack that are at same or deeper level
            while (stack.Count > 0 && stack.Peek().Level >= heading.Level)
            {
                stack.Pop();
            }

            if (stack.Count == 0)
            {
                // Top level heading
                result.Add(heading);
            }
            else if (stack.Count < MaxDepth)
            {
                // Add as child to parent
                stack.Peek().Children.Add(heading);
            }

            stack.Push(heading);
        }

        return result;
    }

    /// <summary>
    /// Strips HTML tags from text
    /// </summary>
    private static string StripHtmlTags(string input)
    {
        return Regex.Replace(input, "<[^>]*>", string.Empty).Trim();
    }

    /// <summary>
    /// Generates a URL-friendly ID from text
    /// </summary>
    private static string GenerateId(string text)
    {
        // Convert to lowercase and replace non-alphanumeric characters with hyphens
        var id = Regex.Replace(text.ToLowerInvariant(), @"[^a-z0-9\s-]", "");
        id = Regex.Replace(id, @"\s+", "-");
        id = id.Trim('-');

        return string.IsNullOrWhiteSpace(id) ? "heading" : id;
    }

    /// <summary>
    /// Gets the CSS class for the table of contents
    /// </summary>
    private string GetTableOfContentsClass()
    {
        return $"osirion-table-of-contents {CssClass}".Trim();
    }
}

/// <summary>
/// Represents a heading item in the table of contents
/// </summary>
public class HeadingItem
{
    public int Level { get; set; }
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public List<HeadingItem> Children { get; set; } = new();
}