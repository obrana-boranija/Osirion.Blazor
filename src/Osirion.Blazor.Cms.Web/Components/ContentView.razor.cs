using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>Displays a content item and optional previous and next navigation links.</summary>
public partial class ContentView(IContentProviderManager contentProviderManager)
{
    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

    [Inject]
    private ILogger<ContentView> Logger { get; set; } = default!;

    /// <summary>Gets or sets the content path to load.</summary>
    [Parameter]
    public string Path { get; set; } = string.Empty;

    /// <summary>Gets or sets the loading message.</summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading content...";

    /// <summary>Gets or sets the message shown when content is not found.</summary>
    [Parameter]
    public string NotFoundText { get; set; } = "Content not found.";

    /// <summary>Gets or sets a formatter for category URLs.</summary>
    [Parameter]
    public Func<string, string>? CategoryUrlFormatter { get; set; }

    /// <summary>Gets or sets a formatter for tag URLs.</summary>
    [Parameter]
    public Func<string, string>? TagUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets how the author's page URL is built from the author's name. When set, the author's name links there
    /// with <c>rel="author"</c>.
    /// </summary>
    [Parameter]
    public Func<string, string>? AuthorUrlFormatter { get; set; }

    /// <summary>Gets or sets a formatter for content URLs.</summary>
    [Parameter]
    public Func<ContentItem, string>? ContentUrlFormatter { get; set; }

    /// <summary>Gets or sets the content item to display.</summary>
    [Parameter]
    public ContentItem? Item { get; set; }

    /// <summary>Gets or sets the previous content item.</summary>
    [Parameter]
    public ContentItem? PreviousItem { get; set; }

    /// <summary>Gets or sets the next content item.</summary>
    [Parameter]
    public ContentItem? NextItem { get; set; }

    /// <summary>Gets or sets a value indicating whether navigation links are shown.</summary>
    [Parameter]
    public bool ShowNavigationLinks { get; set; } = false;

    private bool IsLoading { get; set; } = true;

    // What this component loaded itself, so a later Path change can tell it apart from values the parent supplied.
    private string? _loadedPath;
    private ContentItem? _loadedItem;
    private ContentItem? _loadedPrevious;
    private ContentItem? _loadedNext;

    /// <summary>Performs the OnParametersSet operation asynchronously.</summary>
    protected override async Task OnParametersSetAsync()
    {
        var itemSuppliedByParent = Item is not null && !ReferenceEquals(Item, _loadedItem);
        if (!itemSuppliedByParent && !string.IsNullOrWhiteSpace(Path)
            && (Item is null || !string.Equals(Path, _loadedPath, StringComparison.Ordinal)))
        {
            await LoadContentAsync();
        }
        else
        {
            IsLoading = false;
        }
    }

    private async Task LoadContentAsync()
    {
        IsLoading = true;

        // The previous document's content and neighbours must not stay visible while, or after, the new one loads.
        Item = null;
        if (ReferenceEquals(PreviousItem, _loadedPrevious))
        {
            PreviousItem = null;
        }

        if (ReferenceEquals(NextItem, _loadedNext))
        {
            NextItem = null;
        }

        _loadedPath = Path;
        _loadedItem = _loadedPrevious = _loadedNext = null;
        try
        {
            var provider = contentProviderManager.GetDefaultProvider();
            if (provider is not null)
            {
                Item = await provider.GetItemByPathAsync(Path);
                _loadedItem = Item;

                if (Item is not null && ShowNavigationLinks)
                {
                    // If we need previous and next items, load them
                    var allItems = await provider.GetItemsByQueryAsync(new ContentQuery
                    {
                        Directory = System.IO.Path.GetDirectoryName(Path)?.Replace('\\', '/'),
                        SortBy = SortField.Date,
                        SortDirection = SortDirection.Descending
                    }) ?? [];

                    var item = Item;
                    if (item is null)
                    {
                        return;
                    }

                    // Find the index manually
                    int currentIndex = -1;
                    for (int i = 0; i < allItems.Count; i++)
                    {
                        if (allItems[i].Path == item.Path)
                        {
                            currentIndex = i;
                            break;
                        }
                    }

                    if (currentIndex > 0)
                    {
                        PreviousItem = _loadedPrevious = allItems[currentIndex - 1];
                    }

                    if (currentIndex >= 0 && currentIndex < allItems.Count - 1)
                    {
                        NextItem = _loadedNext = allItems[currentIndex + 1];
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load content {ContentPath}", Path);
            Item = null;
            _loadedItem = null;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private string GetContentViewClass()
    {
        return $"osirion-content-view {Class}".Trim();
    }

    private readonly SanitizedContentCache _sanitizedContent = new();

    private MarkupString GetSanitizedContent(string? content)
    {
        return _sanitizedContent.Get(ServiceProvider, content);
    }

    private string GetCategoryUrl(string category)
    {
        return CategoryUrlFormatter?.Invoke(category) ?? ContentUrls.CategoryUrl(category);
    }

    private string GetTagUrl(string tag)
    {
        return TagUrlFormatter?.Invoke(tag) ?? ContentUrls.TagUrl(tag);
    }

    private string GetContentUrl(ContentItem item)
    {
        return ContentUrlFormatter?.Invoke(item) ?? $"/{item.Path}";
    }
}
