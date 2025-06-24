using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>
/// ArticlePage component for displaying blog articles and similar content
/// </summary>
public partial class ArticlePage
{
    /// <summary>
    /// Gets or sets whether to show the sidebar
    /// </summary>
    [Parameter]
    public bool ShowSidebar { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show table of contents
    /// </summary>
    [Parameter]
    public bool ShowTableOfContents { get; set; } = true;

    /// <summary>
    /// Gets or sets the table of contents title
    /// </summary>
    [Parameter]
    public string TableOfContentsTitle { get; set; } = "Table of Contents";

    /// <summary>
    /// Gets or sets custom footer content
    /// </summary>
    [Parameter]
    public string? CustomFooterContent { get; set; }

    /// <summary>
    /// Gets or sets additional sidebar content
    /// </summary>
    [Parameter]
    public RenderFragment? SidebarContent { get; set; }

    IReadOnlyList<ContentNotFoundSuggestion> _contentNotFoundSuggestion = new List<ContentNotFoundSuggestion> { new ContentNotFoundSuggestion
    {
        Text = "Article Not Found",
        Description = "The article you're looking for does not exist or has been moved.",
        Url = "true"
    }};

    private string GetArticlePageClass()
    {
        var classes = new List<string> { "osirion-article-page" };

        if (ShowSidebar)
        {
            classes.Add("osirion-article-with-sidebar");
        }

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        return string.Join(" ", classes);
    }

    private string GetCategoryUrl(string category)
    {
        return CategoryUrlFormatter?.Invoke(category) ?? $"/category/{category.ToLower().Replace(' ', '-')}";
    }

    private string GetTagUrl(string tag)
    {
        return TagUrlFormatter?.Invoke(tag) ?? $"/tag/{tag.ToLower().Replace(' ', '-')}";
    }

    private string GetShieldsIoUrl(string tag)
    {
        // Generate a deterministic color based on the tag name
        var tagHash = tag.GetHashCode();
        var colorIndex = Math.Abs(tagHash % TagColors.Length);
        var color = TagColors[colorIndex];

        // URL encode the tag
        var encodedTag = Uri.EscapeDataString(tag);

        return $"https://img.shields.io/badge/{encodedTag}-{color}?style=flat-square&logo=tag&logoColor=white";
    }

    // Array of colors for the shields.io tags
    private static readonly string[] TagColors = new[]
    {
        "blue", "green", "red", "orange", "yellow", "purple", "lightgrey",
        "success", "important", "critical", "informational", "inactive",
        "blueviolet", "ff69b4", "9cf", "brightgreen", "yellowgreen", "orange"
    };
}