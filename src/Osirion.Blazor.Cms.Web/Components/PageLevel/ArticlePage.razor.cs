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

    /// <inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (Item is null && !IsLoading)
        {
            SignalNotFound();
        }
        else
        {
            ResetNotFoundSignal();
        }
    }

    private string GetCategoryUrl(string category)
    {
        return CategoryUrlFormatter?.Invoke(category) ?? ContentUrls.CategoryUrl(category);
    }

    private string GetTagUrl(string tag)
    {
        return TagUrlFormatter?.Invoke(tag) ?? ContentUrls.TagUrl(tag);
    }

    /// <summary>Number of colour variants in <c>ArticlePage.razor.css</c> (<c>osirion-tag-badge-0</c> and up).</summary>
    private const int TagBadgeColorCount = 8;

    // Stable across processes and restarts, so a tag keeps its colour (and cached pages stay identical).
    private static string GetTagBadgeClass(string tag) =>
        $"osirion-tag-badge osirion-tag-badge-{ContentUrls.StableHash(tag.Trim().ToLowerInvariant()) % TagBadgeColorCount}";
}
