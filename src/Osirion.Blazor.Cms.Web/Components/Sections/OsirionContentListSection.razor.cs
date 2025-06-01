using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Components;
public partial class OsirionContentListSection(IContentProviderManager ContentProviderManager)
{
    /// <summary>
    /// Gets or sets the posts to display. If not set, the component will load posts based on the provided filters and query.
    /// </summary>
    [Parameter]
    public IReadOnlyList<ContentItem>? Items { get; set; }

    /// <summary>
    /// Gets or sets the section title.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the section description.
    /// </summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the layout style: "grid", "list", "carousel", or "featured-first".
    /// </summary>
    [Parameter]
    public string Layout { get; set; } = "grid";

    /// <summary>
    /// Content query. If defined, overrides all other filters.
    /// </summary>
    [Parameter]
    public ContentQuery? Query { get; set; }

    /// <summary>
    /// Only featured items. Can be overwritten by Query filter.
    /// </summary>
    [Parameter]
    public bool FeaturedOnly { get; set; }

    /// <summary>
    /// Gets or sets the sort direction for the posts. Can be overwritten by Query filter.
    /// </summary>
    [Parameter]
    public SortDirection SortDirection { get; set; } = SortDirection.Descending;

    /// <summary>
    /// Gets or sets the sort field for the posts. Can be overwritten by Query filter.
    /// </summary>
    [Parameter]
    public SortField SortField { get; set; } = SortField.Date;

    /// <summary>
    /// Gets or sets the number of posts to display.
    /// </summary>
    [Parameter]
    public int PostCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the number of columns for grid layout.
    /// </summary>
    [Parameter]
    public int Columns { get; set; } = 3;

    /// <summary>
    /// Gets or sets the tag filter.
    /// </summary>
    [Parameter]
    public string? Tag { get; set; }

    /// <summary>
    /// Gets or sets the category filter.
    /// </summary>
    [Parameter]
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the directory path filter.
    /// </summary>
    [Parameter]
    public string? Directory { get; set; }

    /// <summary>
    /// Gets or sets the post URL formatter.
    /// </summary>
    [Parameter]
    public Func<ContentItem, string>? PostUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets the category URL formatter.
    /// </summary>
    [Parameter]
    public Func<string, string>? CategoryUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets the locale.
    /// </summary>
    [Parameter]
    public string? Locale { get; set; }

    /// <summary>
    /// Gets or sets the date format.
    /// </summary>
    [Parameter]
    public string DateFormat { get; set; } = "MMM d, yyyy";

    /// <summary>
    /// Gets or sets the loading text.
    /// </summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading posts...";

    /// <summary>
    /// Gets or sets the no content text.
    /// </summary>
    [Parameter]
    public string NoContentText { get; set; } = "No posts found.";

    /// <summary>
    /// Gets or sets the read more text.
    /// </summary>
    [Parameter]
    public string ReadMoreText { get; set; } = "Read more";

    /// <summary>
    /// Gets or sets the view all text.
    /// </summary>
    [Parameter]
    public string ViewAllText { get; set; } = "View all posts";

    /// <summary>
    /// Gets or sets the view all URL.
    /// </summary>
    [Parameter]
    public string? ViewAllUrl { get; set; }

    /// <summary>
    /// Gets or sets whether to show the excerpt.
    /// </summary>
    [Parameter]
    public bool ShowExcerpt { get; set; } = true;

    /// <summary>
    /// Gets or sets the excerpt length.
    /// </summary>
    [Parameter]
    public int ExcerptLength { get; set; } = 160;

    /// <summary>
    /// Gets or sets whether to show the author.
    /// </summary>
    [Parameter]
    public bool ShowAuthor { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show categories.
    /// </summary>
    [Parameter]
    public bool ShowCategories { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show the read more link.
    /// </summary>
    [Parameter]
    public bool ShowReadMoreLink { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to show the view all link.
    /// </summary>
    [Parameter]
    public bool ShowViewAllLink { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show post metadata.
    /// </summary>
    [Parameter]
    public bool ShowMetadata { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the component is loading.
    /// </summary>
    private bool IsLoading { get; set; } = true;

    /// <summary>
    /// Gets the CSS class for the current layout.
    /// </summary>
    private string LayoutClass => Layout switch
    {
        "list" => "osirion-layout-list",
        "carousel" => "osirion-layout-carousel",
        "featured-first" => "osirion-layout-featured-first",
        _ => $"osirion-layout-grid osirion-cols-{Columns}"
    };

    /// <summary>
    /// Called when the component is initialized.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        if (Items is null)
        {
            await LoadPostsAsync();
        }
        else
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Loads posts from the content provider.
    /// </summary>
    private async Task LoadPostsAsync()
    {
        IsLoading = true;
        try
        {
            var query = Query ?? new ContentQuery
            {
                IsFeatured = FeaturedOnly ? true : null,
                Directory = Directory,
                Category = Category,
                Tag = Tag,
                Locale = Locale,
                Take = PostCount,
                SortBy = SortField,
                SortDirection = SortDirection
            };

            Items = await ContentProviderManager.GetContentByQueryAsync(query);
        }
        catch (Exception)
        {
            Items = null;
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Gets the CSS class for the section.
    /// </summary>
    private string GetSectionClass()
    {
        return $"osirion-featured-posts-section {CssClass}".Trim();
    }

    /// <summary>
    /// Gets the URL for a category.
    /// </summary>
    private string GetCategoryUrl(string category)
    {
        return CategoryUrlFormatter?.Invoke(category) ?? $"/category/{category.ToLower().Replace(' ', '-')}";
    }

    /// <summary>
    /// Gets an excerpt from HTML content.
    /// </summary>
    private string GetExcerpt(string content, int length)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;

        // Strip HTML tags
        var text = Regex.Replace(content, "<.*?>", string.Empty);

        // Trim whitespace
        text = text.Trim();

        // Truncate to length
        if (text.Length <= length)
            return text;

        // Find the last space before the cutoff
        var lastSpace = text.LastIndexOf(' ', length);
        if (lastSpace == -1)
            lastSpace = length;

        return text.Substring(0, lastSpace) + "...";
    }
}
