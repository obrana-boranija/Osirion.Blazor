using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Components;

public partial class ContentPage
{
    [Inject]
    private IContentProviderManager ContentProviderManager { get; set; } = default!;

    /// <summary>
    /// Gets or sets the path of the content to display.
    /// </summary>
    [Parameter]
    public string? Path { get; set; }

    /// <summary>
    /// Gets or sets the content item to display.
    /// </summary>
    [Parameter]
    public ContentItem? ContentItem { get; set; }

    /// <summary>
    /// Gets or sets the previous content item for navigation.
    /// </summary>
    [Parameter]
    public ContentItem? PreviousItem { get; set; }

    /// <summary>
    /// Gets or sets the next content item for navigation.
    /// </summary>
    [Parameter]
    public ContentItem? NextItem { get; set; }

    /// <summary>
    /// Gets or sets the related content items to display.
    /// </summary>
    [Parameter]
    public IReadOnlyList<ContentItem>? RelatedItems { get; set; }

    /// <summary>
    /// Gets or sets the loading text.
    /// </summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading content...";

    /// <summary>
    /// Gets or sets the not found title.
    /// </summary>
    [Parameter]
    public string NotFoundTitle { get; set; } = "Content Not Found";

    /// <summary>
    /// Gets or sets the not found text.
    /// </summary>
    [Parameter]
    public string NotFoundText { get; set; } = "The requested content could not be found.";

    /// <summary>
    /// Gets or sets whether to show a back link when content is not found.
    /// </summary>
    [Parameter]
    public bool ShowBackLink { get; set; } = true;

    /// <summary>
    /// Gets or sets the back link URL.
    /// </summary>
    [Parameter]
    public string BackLinkUrl { get; set; } = "/";

    /// <summary>
    /// Gets or sets the back link text.
    /// </summary>
    [Parameter]
    public string BackLinkText { get; set; } = "Back to Home";

    /// <summary>
    /// Gets or sets whether to show breadcrumbs.
    /// </summary>
    [Parameter]
    public bool ShowBreadcrumbs { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show the home link in breadcrumbs.
    /// </summary>
    [Parameter]
    public bool ShowBreadcrumbHome { get; set; } = true;

    /// <summary>
    /// Gets or sets the home link URL in breadcrumbs.
    /// </summary>
    [Parameter]
    public string BreadcrumbHomeUrl { get; set; } = "/";

    /// <summary>
    /// Gets or sets the home link text in breadcrumbs.
    /// </summary>
    [Parameter]
    public string BreadcrumbHomeText { get; set; } = "Home";

    /// <summary>
    /// Gets or sets whether to show metadata.
    /// </summary>
    [Parameter]
    public bool ShowMetadata { get; set; } = true;

    /// <summary>
    /// Gets or sets the caption for the featured image.
    /// </summary>
    [Parameter]
    public string? FeaturedImageCaption { get; set; }

    /// <summary>
    /// Gets or sets whether to show the footer.
    /// </summary>
    [Parameter]
    public bool ShowFooter { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show categories.
    /// </summary>
    [Parameter]
    public bool ShowCategories { get; set; } = true;

    /// <summary>
    /// Gets or sets the categories title.
    /// </summary>
    [Parameter]
    public string CategoriesTitle { get; set; } = "Categories:";

    /// <summary>
    /// Gets or sets whether to show tags.
    /// </summary>
    [Parameter]
    public bool ShowTags { get; set; } = true;

    /// <summary>
    /// Gets or sets the tags title.
    /// </summary>
    [Parameter]
    public string TagsTitle { get; set; } = "Tags:";

    /// <summary>
    /// Gets or sets whether to show navigation links.
    /// </summary>
    [Parameter]
    public bool ShowNavigationLinks { get; set; } = true;

    /// <summary>
    /// Gets or sets the previous item label.
    /// </summary>
    [Parameter]
    public string PreviousItemLabel { get; set; } = "Previous";

    /// <summary>
    /// Gets or sets the next item label.
    /// </summary>
    [Parameter]
    public string NextItemLabel { get; set; } = "Next";

    /// <summary>
    /// Gets or sets whether to show share links.
    /// </summary>
    [Parameter]
    public bool ShowShareLinks { get; set; } = false;

    /// <summary>
    /// Gets or sets the share links title.
    /// </summary>
    [Parameter]
    public string ShareLinksTitle { get; set; } = "Share:";

    /// <summary>
    /// Gets or sets the share links.
    /// </summary>
    [Parameter]
    public List<ShareLink> ShareLinks { get; set; } = new();

    /// <summary>
    /// Gets or sets custom footer content (HTML).
    /// </summary>
    [Parameter]
    public string? FooterContent { get; set; }

    /// <summary>
    /// Gets or sets whether to show related content.
    /// </summary>
    [Parameter]
    public bool ShowRelatedContent { get; set; } = false;

    /// <summary>
    /// Gets or sets the related content title.
    /// </summary>
    [Parameter]
    public string RelatedContentTitle { get; set; } = "Related Content";

    /// <summary>
    /// Gets or sets the schema.org type.
    /// </summary>
    [Parameter]
    public string SchemaType { get; set; } = "Article";

    /// <summary>
    /// Gets or sets the directory URL formatter.
    /// </summary>
    [Parameter]
    public Func<DirectoryItem, string>? DirectoryUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets the category URL formatter.
    /// </summary>
    [Parameter]
    public Func<string, string>? CategoryUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets the tag URL formatter.
    /// </summary>
    [Parameter]
    public Func<string, string>? TagUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets the content URL formatter.
    /// </summary>
    [Parameter]
    public Func<ContentItem, string>? ContentUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets the path segment to ignore in URL construction.
    /// </summary>
    [Parameter]
    public string? IgnorePathSegment { get; set; }

    /// <summary>
    /// Gets or sets the custom path replacement pattern (regex).
    /// </summary>
    [Parameter]
    public string? PathReplacePattern { get; set; }

    /// <summary>
    /// Gets or sets the event callback for when the content item is loaded.
    /// </summary>
    [Parameter]
    public EventCallback<ContentItem> OnContentLoaded { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the component is loading.
    /// </summary>
    protected bool IsLoading { get; set; } = true;

    /// <summary>
    /// Processes changes to component parameters.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        if (ContentItem is null && !string.IsNullOrEmpty(Path))
        {
            await LoadContentAsync();
        }
        else if (Path is not null && ContentItem is not null && !ContentItem.Path.EndsWith(Path, StringComparison.OrdinalIgnoreCase))
        {
            await LoadContentAsync();
        }

        await base.OnParametersSetAsync();
    }

    /// <summary>
    /// Loads the content item based on the specified path.
    /// </summary>
    private async Task LoadContentAsync()
    {
        IsLoading = true;
        StateHasChanged();

        try
        {
            var provider = ContentProviderManager.GetDefaultProvider();
            if (provider != null)
            {
                // Clean up the path if needed
                var searchPath = Path;

                // If ignore path segment is specified, add it back before querying
                if (!string.IsNullOrEmpty(IgnorePathSegment) && !searchPath.Contains(IgnorePathSegment))
                {
                    searchPath = $"{IgnorePathSegment}/{searchPath}";
                }

                // Apply custom regex replacement if specified
                if (!string.IsNullOrEmpty(PathReplacePattern))
                {
                    var parts = PathReplacePattern.Split('|');
                    if (parts.Length == 2)
                    {
                        searchPath = Regex.Replace(searchPath, parts[0], parts[1]);
                    }
                }

                ContentItem = await provider.GetItemByUrlAsync(searchPath);

                if (ContentItem != null && ShowNavigationLinks && (PreviousItem == null || NextItem == null))
                {
                    await LoadNavigationItemsAsync(provider, ContentItem);
                }

                if (ContentItem != null && ShowRelatedContent && (RelatedItems == null || !RelatedItems.Any()))
                {
                    await LoadRelatedItemsAsync(provider, ContentItem);
                }

                if (ContentItem != null && OnContentLoaded.HasDelegate)
                {
                    await OnContentLoaded.InvokeAsync(ContentItem);
                }

                if (ShowShareLinks && !ShareLinks.Any())
                {
                    // Default share links if none provided and sharing is enabled
                    InitializeDefaultShareLinks();
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading content: {ex.Message}");
            ContentItem = null;
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Loads navigation items (previous and next) for the content item.
    /// </summary>
    private async Task LoadNavigationItemsAsync(IContentProvider provider, ContentItem item)
    {
        try
        {
            // Get items from the same directory and same locale for navigation
            var query = new ContentQuery
            {
                Directory = item.Directory?.Path,
                Locale = item.Locale,
                SortBy = SortField.Date,
                SortDirection = SortDirection.Descending
            };

            var items = await provider.GetItemsByQueryAsync(query);

            // Find the current item's index
            int currentIndex = -1;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Path == item.Path)
                {
                    currentIndex = i;
                    break;
                }
            }

            // Set previous and next items
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

    /// <summary>
    /// Loads related content items based on tags and categories.
    /// </summary>
    private async Task LoadRelatedItemsAsync(IContentProvider provider, ContentItem item)
    {
        try
        {
            if (!item.Tags.Any() && !item.Categories.Any())
            {
                return;
            }

            // Get items with matching tags or categories
            var query = new ContentQuery
            {
                Locale = item.Locale,
                SortBy = SortField.Date,
                SortDirection = SortDirection.Descending
            };

            var allItems = await provider.GetItemsByQueryAsync(query);

            // Find related items (excluding the current item)
            var related = allItems
                .Where(i => i.Path != item.Path)
                .Where(i =>
                    i.Tags.Intersect(item.Tags, StringComparer.OrdinalIgnoreCase).Any() ||
                    i.Categories.Intersect(item.Categories, StringComparer.OrdinalIgnoreCase).Any())
                .OrderByDescending(i =>
                    i.Tags.Intersect(item.Tags, StringComparer.OrdinalIgnoreCase).Count() +
                    i.Categories.Intersect(item.Categories, StringComparer.OrdinalIgnoreCase).Count())
                .Take(3)
                .ToList();

            RelatedItems = related;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading related items: {ex.Message}");
        }
    }

    /// <summary>
    /// Initializes default share links if none are provided.
    /// </summary>
    private void InitializeDefaultShareLinks()
    {
        var currentUrl = Uri.EscapeDataString($"https://{Path}");
        var title = Uri.EscapeDataString(ContentItem?.Title ?? "");

        ShareLinks = new List<ShareLink>
        {
            new ShareLink
            {
                Name = "Twitter",
                Label = "Twitter",
                Url = $"https://twitter.com/intent/tweet?url={currentUrl}&text={title}",
                Icon = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M22 4s-.7 2.1-2 3.4c1.6 10-9.4 17.3-18 11.6 2.2.1 4.4-.6 6-2C3 15.5.5 9.6 3 5c2.2 2.6 5.6 4.1 9 4-.9-4.2 4-6.6 7-3.8 1.1 0 3-1.2 3-1.2z\"></path></svg>"
            },
            new ShareLink
            {
                Name = "Facebook",
                Label = "Facebook",
                Url = $"https://www.facebook.com/sharer/sharer.php?u={currentUrl}",
                Icon = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M18 2h-3a5 5 0 0 0-5 5v3H7v4h3v8h4v-8h3l1-4h-4V7a1 1 0 0 1 1-1h3z\"></path></svg>"
            },
            new ShareLink
            {
                Name = "LinkedIn",
                Label = "LinkedIn",
                Url = $"https://www.linkedin.com/shareArticle?mini=true&url={currentUrl}&title={title}",
                Icon = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M16 8a6 6 0 0 1 6 6v7h-4v-7a2 2 0 0 0-2-2 2 2 0 0 0-2 2v7h-4v-7a6 6 0 0 1 6-6z\"></path><rect x=\"2\" y=\"9\" width=\"4\" height=\"12\"></rect><circle cx=\"4\" cy=\"4\" r=\"2\"></circle></svg>"
            }
        };
    }

    /// <summary>
    /// Gets the CSS class for the component.
    /// </summary>
    protected string GetContentPageClass()
    {
        return $"osirion-content-page-container {CssClass}".Trim();
    }

    /// <summary>
    /// Gets the URL for a directory.
    /// </summary>
    protected string GetDirectoryUrl(DirectoryItem directory)
    {
        return DirectoryUrlFormatter?.Invoke(directory) ?? $"/{directory.Path}";
    }

    /// <summary>
    /// Gets the URL for a category.
    /// </summary>
    protected string GetCategoryUrl(string category)
    {
        return CategoryUrlFormatter?.Invoke(category) ?? $"/category/{category.ToLower().Replace(' ', '-')}";
    }

    /// <summary>
    /// Gets the URL for a tag.
    /// </summary>
    protected string GetTagUrl(string tag)
    {
        return TagUrlFormatter?.Invoke(tag) ?? $"/tag/{tag.ToLower().Replace(' ', '-')}";
    }

    /// <summary>
    /// Gets the URL for a content item.
    /// </summary>
    protected string GetContentUrl(ContentItem item)
    {
        if (ContentUrlFormatter != null)
        {
            return ContentUrlFormatter(item);
        }

        var path = item.Path;

        // Remove the IgnorePathSegment if specified
        if (!string.IsNullOrEmpty(IgnorePathSegment) && path.Contains(IgnorePathSegment))
        {
            path = path.Replace($"{IgnorePathSegment}/", "");
        }

        // Apply custom regex replace pattern if specified
        if (!string.IsNullOrEmpty(PathReplacePattern))
        {
            var parts = PathReplacePattern.Split('|');
            if (parts.Length == 2)
            {
                path = Regex.Replace(path, parts[0], parts[1]);
            }
        }

        // Remove file extension if present
        if (path.EndsWith(".md") || path.EndsWith(".markdown"))
        {
            path = path.Substring(0, path.LastIndexOf('.'));
        }

        return $"/{path}";
    }
}