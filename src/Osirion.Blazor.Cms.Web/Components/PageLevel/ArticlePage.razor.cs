using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Components;

/// <summary>
/// ArticlePage component for displaying blog articles and similar content
/// </summary>
public partial class ArticlePage
{
    [Inject]
    private IContentProviderManager ContentProviderManager { get; set; } = default!;

    /// <summary>
    /// Gets or sets the path to the article content
    /// </summary>
    [Parameter]
    public string? ContentPath { get; set; }

    /// <summary>
    /// Gets or sets the content item to display
    /// </summary>
    [Parameter]
    public ContentItem? Content { get; set; }

    /// <summary>
    /// Gets or sets the previous content item for navigation
    /// </summary>
    [Parameter]
    public ContentItem? PreviousItem { get; set; }

    /// <summary>
    /// Gets or sets the next content item for navigation
    /// </summary>
    [Parameter]
    public ContentItem? NextItem { get; set; }

    /// <summary>
    /// Gets or sets the related content items
    /// </summary>
    [Parameter]
    public IReadOnlyList<ContentItem>? RelatedItems { get; set; }

    /// <summary>
    /// Gets or sets whether to show breadcrumbs
    /// </summary>
    [Parameter]
    public bool ShowBreadcrumbs { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show the home link in breadcrumbs
    /// </summary>
    [Parameter]
    public bool ShowBreadcrumbHome { get; set; } = true;

    /// <summary>
    /// Gets or sets the home link URL in breadcrumbs
    /// </summary>
    [Parameter]
    public string BreadcrumbHomeUrl { get; set; } = "/";

    /// <summary>
    /// Gets or sets the home link text in breadcrumbs
    /// </summary>
    [Parameter]
    public string BreadcrumbHomeText { get; set; } = "Home";

    /// <summary>
    /// Gets or sets whether to show navigation links
    /// </summary>
    [Parameter]
    public bool ShowNavigationLinks { get; set; } = true;

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
    /// Gets or sets whether to show categories in sidebar
    /// </summary>
    [Parameter]
    public bool ShowCategoriesSidebar { get; set; } = true;

    /// <summary>
    /// Gets or sets the categories section title
    /// </summary>
    [Parameter]
    public string CategoriesSectionTitle { get; set; } = "Categories";

    /// <summary>
    /// Gets or sets whether to show tags in sidebar
    /// </summary>
    [Parameter]
    public bool ShowTagsSidebar { get; set; } = true;

    /// <summary>
    /// Gets or sets the tags section title
    /// </summary>
    [Parameter]
    public string TagsSectionTitle { get; set; } = "Tags";

    /// <summary>
    /// Gets or sets whether to show author info in sidebar
    /// </summary>
    [Parameter]
    public bool ShowAuthorSidebar { get; set; } = true;

    /// <summary>
    /// Gets or sets the author section title
    /// </summary>
    [Parameter]
    public string AuthorSectionTitle { get; set; } = "About the Author";

    /// <summary>
    /// Gets or sets whether to show related content
    /// </summary>
    [Parameter]
    public bool ShowRelatedContent { get; set; } = true;

    /// <summary>
    /// Gets or sets the related content title
    /// </summary>
    [Parameter]
    public string RelatedContentTitle { get; set; } = "Related Articles";

    /// <summary>
    /// Gets or sets the number of related items to show
    /// </summary>
    [Parameter]
    public int RelatedItemsCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets whether to show a back link when not found
    /// </summary>
    [Parameter]
    public bool ShowBackLink { get; set; } = true;

    /// <summary>
    /// Gets or sets the back link URL
    /// </summary>
    [Parameter]
    public string BackLinkUrl { get; set; } = "/";

    /// <summary>
    /// Gets or sets the back link text
    /// </summary>
    [Parameter]
    public string BackLinkText { get; set; } = "Back to Home";

    /// <summary>
    /// Gets or sets the schema.org type
    /// </summary>
    [Parameter]
    public string SchemaType { get; set; } = "Article";

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

    /// <summary>
    /// Gets or sets the directory URL formatter
    /// </summary>
    [Parameter]
    public Func<DirectoryItem, string>? DirectoryUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets the category URL formatter
    /// </summary>
    [Parameter]
    public Func<string, string>? CategoryUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets the tag URL formatter
    /// </summary>
    [Parameter]
    public Func<string, string>? TagUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets the content URL formatter
    /// </summary>
    [Parameter]
    public Func<ContentItem, string>? ContentUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets the loading text
    /// </summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading article...";

    /// <summary>
    /// Gets or sets the not found title
    /// </summary>
    [Parameter]
    public string NotFoundTitle { get; set; } = "Article Not Found";

    /// <summary>
    /// Gets or sets the not found text
    /// </summary>
    [Parameter]
    public string NotFoundText { get; set; } = "The requested article could not be found.";

    /// <summary>
    /// Gets or sets whether the component is loading
    /// </summary>
    private bool IsLoading { get; set; } = true;

    /// <inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        if (Content == null && !string.IsNullOrEmpty(ContentPath))
        {
            await LoadContentAsync();
        }
        else if (Content != null)
        {
            await LoadRelatedContentAsync();
            IsLoading = false;
        }
        else
        {
            IsLoading = false;
        }
    }

    private async Task LoadContentAsync()
    {
        IsLoading = true;
        try
        {
            var provider = ContentProviderManager.GetDefaultProvider();
            if (provider != null)
            {
                Content = await provider.GetItemByPathAsync(ContentPath);

                if (Content != null)
                {
                    await LoadNavigationItemsAsync(provider, Content);
                    await LoadRelatedContentAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading article content: {ex.Message}");
            Content = null;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadNavigationItemsAsync(IContentProvider provider, ContentItem item)
    {
        if (!ShowNavigationLinks || PreviousItem != null || NextItem != null)
            return;

        try
        {
            var query = new ContentQuery
            {
                Directory = item.Directory?.Path,
                SortBy = SortField.Date,
                SortDirection = SortDirection.Descending
            };

            var items = await provider.GetItemsByQueryAsync(query);

            int currentIndex = -1;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Path == item.Path)
                {
                    currentIndex = i;
                    break;
                }
            }

            if (currentIndex > 0)
            {
                PreviousItem = items[currentIndex - 1];
            }

            if (currentIndex >= 0 && currentIndex < items.Count - 1)
            {
                NextItem = items[currentIndex + 1];
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading navigation items: {ex.Message}");
        }
    }

    private async Task LoadRelatedContentAsync()
    {
        if (!ShowRelatedContent || RelatedItems != null || Content == null)
            return;

        try
        {
            var provider = ContentProviderManager.GetDefaultProvider();
            if (provider != null)
            {
                if (!Content.Tags.Any() && !Content.Categories.Any())
                {
                    return;
                }

                var query = new ContentQuery
                {
                    SortBy = SortField.Date,
                    SortDirection = SortDirection.Descending
                };

                var allItems = await provider.GetItemsByQueryAsync(query);

                var related = allItems
                    .Where(i => i.Path != Content.Path)
                    .Where(i =>
                        i.Tags.Intersect(Content.Tags, StringComparer.OrdinalIgnoreCase).Any() ||
                        i.Categories.Intersect(Content.Categories, StringComparer.OrdinalIgnoreCase).Any())
                    .OrderByDescending(i =>
                        i.Tags.Intersect(Content.Tags, StringComparer.OrdinalIgnoreCase).Count() +
                        i.Categories.Intersect(Content.Categories, StringComparer.OrdinalIgnoreCase).Count())
                    .Take(RelatedItemsCount)
                    .ToList();

                RelatedItems = related;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading related content: {ex.Message}");
        }
    }

    private string GetArticlePageClass()
    {
        var classes = new List<string> { "osirion-article-page" };

        if (ShowSidebar)
        {
            classes.Add("osirion-article-with-sidebar");
        }

        if (!string.IsNullOrEmpty(CssClass))
        {
            classes.Add(CssClass);
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

    private string GetContentUrl(ContentItem item)
    {
        return ContentUrlFormatter?.Invoke(item) ?? $"/{item.Path}";
    }
}