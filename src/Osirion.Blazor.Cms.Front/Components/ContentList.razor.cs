using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Core.Interfaces;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Components;

public partial class ContentList : IDisposable
{
    [Inject]
    private IContentProviderManager ContentProviderManager { get; set; } = default!;

    /// <summary>
    /// Gets or sets the content directory path.
    /// </summary>
    [Parameter]
    public string? Directory { get; set; }

    /// <summary>
    /// Gets or sets the category filter.
    /// </summary>
    [Parameter]
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the tag filter.
    /// </summary>
    [Parameter]
    public string? Tag { get; set; }

    /// <summary>
    /// Gets or sets whether to show only featured content.
    /// </summary>
    [Parameter]
    public bool? OnlyFeatured { get; set; }

    /// <summary>
    /// Gets or sets the number of featured items to display.
    /// </summary>
    [Parameter]
    public int? FeaturedCount { get; set; }

    /// <summary>
    /// Gets or sets the text to display when content is loading.
    /// </summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading content...";

    /// <summary>
    /// Gets or sets the text to display when no content is available.
    /// </summary>
    [Parameter]
    public string NoContentText { get; set; } = "No content available.";

    /// <summary>
    /// Gets or sets the text for the "Read more" link.
    /// </summary>
    [Parameter]
    public string ReadMoreText { get; set; } = "Read more";

    /// <summary>
    /// Gets or sets the content URL formatter function.
    /// </summary>
    [Parameter]
    public Func<ContentItem, string>? ContentUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets whether to show pagination.
    /// </summary>
    [Parameter]
    public bool ShowPagination { get; set; } = false;

    /// <summary>
    /// Gets or sets the number of items to display per page.
    /// </summary>
    [Parameter]
    public int ItemsPerPage { get; set; } = 10;

    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    [Parameter]
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// Gets or sets the event callback for page changes.
    /// </summary>
    [Parameter]
    public EventCallback<int> PageChanged { get; set; }

    /// <summary>
    /// Gets or sets the pagination URL formatter function.
    /// </summary>
    [Parameter]
    public Func<int, string>? PaginationUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets the locale for content filtering.
    /// </summary>
    [Parameter]
    public string? Locale { get; set; }

    /// <summary>
    /// Gets or sets the directory ID for content filtering.
    /// </summary>
    [Parameter]
    public string? DirectoryId { get; set; }

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
    /// Gets or sets the sort field for content items.
    /// </summary>
    [Parameter]
    public SortField SortBy { get; set; } = SortField.Date;

    /// <summary>
    /// Gets or sets the sort direction for content items.
    /// </summary>
    [Parameter]
    public SortDirection SortDirection { get; set; } = SortDirection.Descending;

    /// <summary>
    /// Gets or sets the callback when a content item is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<ContentItem> OnItemSelected { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the component is currently loading content.
    /// </summary>
    protected bool IsLoading { get; set; } = true;

    /// <summary>
    /// Gets or sets the list of content items to display.
    /// </summary>
    protected IReadOnlyList<ContentItem>? ContentItems { get; set; }

    /// <summary>
    /// Gets or sets the total number of content items (before pagination).
    /// </summary>
    protected int TotalItems { get; set; }

    /// <summary>
    /// Gets the total number of pages based on item count and page size.
    /// </summary>
    protected int TotalPages => (int)Math.Ceiling(TotalItems / (double)ItemsPerPage);

    /// <summary>
    /// Subscription for content provider events.
    /// </summary>
    private IDisposable? _contentProviderSubscription;

    /// <summary>
    /// Initializes the component.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await LoadContentAsync();
    }

    /// <summary>
    /// Processes changes to component parameters.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        await LoadContentAsync();
    }

    /// <summary>
    /// Loads content items based on current parameters.
    /// </summary>
    protected async Task LoadContentAsync()
    {
        IsLoading = true;
        try
        {
            var provider = ContentProviderManager.GetDefaultProvider();
            if (provider != null)
            {
                var query = new ContentQuery
                {
                    Directory = Directory,
                    Category = Category,
                    Tag = Tag,
                    Locale = Locale,
                    DirectoryId = DirectoryId,
                    SortBy = SortBy,
                    SortDirection = SortDirection
                };

                if (OnlyFeatured == true)
                {
                    query.IsFeatured = true;
                }

                if (FeaturedCount.HasValue)
                {
                    query.IsFeatured = true;
                    query.Take = FeaturedCount.Value;
                }
                else if (ShowPagination)
                {
                    // Get total count first (without pagination)
                    var allItems = await provider.GetItemsByQueryAsync(query);
                    TotalItems = allItems.Count;

                    // Then apply pagination
                    query.Skip = (CurrentPage - 1) * ItemsPerPage;
                    query.Take = ItemsPerPage;
                }

                ContentItems = await provider.GetItemsByQueryAsync(query);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading content: {ex.Message}");
            ContentItems = Array.Empty<ContentItem>();
            TotalItems = 0;
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Gets the CSS class for the component.
    /// </summary>
    protected string GetContentListClass()
    {
        return $"osirion-content-list {CssClass}".Trim();
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

        return $"/{item.Url}";
    }

    /// <summary>
    /// Gets the URL for a pagination link.
    /// </summary>
    protected string GetPaginationUrl(int page)
    {
        return PaginationUrlFormatter?.Invoke(page) ?? $"?page={page}";
    }

    /// <summary>
    /// Handles page change events.
    /// </summary>
    protected async Task OnPageChanged(int page)
    {
        if (page != CurrentPage)
        {
            CurrentPage = page;

            if (PageChanged.HasDelegate)
            {
                await PageChanged.InvokeAsync(page);
            }
            else
            {
                // If no external handler, reload content with the new page
                await LoadContentAsync();
            }
        }
    }

    /// <summary>
    /// Cleans up resources used by the component.
    /// </summary>
    public void Dispose()
    {
        _contentProviderSubscription?.Dispose();
    }
}