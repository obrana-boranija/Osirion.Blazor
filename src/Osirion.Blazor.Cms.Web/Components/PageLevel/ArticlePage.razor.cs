using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
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
    /// Gets or sets HTML shown after the article and its author, for example a call to subscribe. It is sanitized
    /// like the article itself.
    /// </summary>
    [Parameter]
    public string? CustomFooterContent { get; set; }

    /// <summary>
    /// Gets or sets whether an article that belongs to a series (<c>series</c> front matter) shows where it stands in
    /// it, with links to the other parts, above the body. Defaults to <see langword="true"/>; nothing renders for an
    /// article outside a series.
    /// </summary>
    [Parameter]
    public bool ShowSeriesNavigation { get; set; } = true;

    /// <summary>
    /// Gets or sets what is shown after the body about the article's author, for example an
    /// <c>OsirionAuthorCard</c> built from <c>context.Author</c>. Nothing is shown when <see langword="null"/>.
    /// </summary>
    [Parameter]
    public RenderFragment<ContentItem>? AuthorContent { get; set; }

    /// <summary>
    /// Gets or sets the article's header, shown instead of the hero. It must render the page's <c>h1</c>, usually from
    /// <c>context.Title</c>; a leading <c>h1</c> in the body is still left out, as it is under the hero. When
    /// <see langword="null"/>, the hero is shown as configured by <c>ShowHero</c>.
    /// </summary>
    [Parameter]
    public RenderFragment<ContentItem>? HeaderContent { get; set; }

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

    /// <summary>Number of color variants in <c>ArticlePage.razor.css</c> (<c>osirion-tag-badge-0</c> and up).</summary>
    private const int TagBadgeColorCount = 8;

    // Stable across processes and restarts, so a tag keeps its color (and cached pages stay identical).
    private static string GetTagBadgeClass(string tag) =>
        $"osirion-tag-badge osirion-tag-badge-{ContentUrls.StableHash(tag.Trim().ToLowerInvariant()) % TagBadgeColorCount}";
}
