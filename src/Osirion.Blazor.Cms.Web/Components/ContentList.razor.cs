using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Web.Components;

public partial class ContentList : IDisposable
{
    [Inject]
    private IContentProviderManager ContentProviderManager { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

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
    /// Gets or sets the search query for filtering content.
    /// </summary>
    [Parameter]
    [SupplyParameterFromQuery(Name = "q")]
    public string? SearchQuery { get; set; }

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
    [SupplyParameterFromQuery(Name = "page")]
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

    #region SEO Parameters

    /// <summary>
    /// Gets or sets the page title for SEO (used in CollectionPage schema).
    /// </summary>
    [Parameter]
    public string? PageTitle { get; set; }

    /// <summary>
    /// Gets or sets the page description for SEO.
    /// </summary>
    [Parameter]
    public string? PageDescription { get; set; }

    /// <summary>
    /// Gets or sets the website name override for SEO.
    /// </summary>
    [Parameter]
    public string? WebsiteName { get; set; }

    /// <summary>
    /// Gets or sets whether to enable SEO metadata rendering (default: true).
    /// </summary>
    [Parameter]
    public bool EnableSeoMetadata { get; set; } = true;

    /// <summary>
    /// Gets or sets custom schema types to generate. If not set, defaults to CollectionPage.
    /// </summary>
    [Parameter]
    public SchemaType[]? SchemaTypes { get; set; }

    #endregion

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

    private string CurrentUrl { get; set; } = "/";

    ///// <summary>
    ///// Initializes the component.
    ///// </summary>
    //protected override async Task OnInitializedAsync()
    //{
    //    await LoadContentAsync();
    //}

    /// <summary>
    /// Processes changes to component parameters.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        var uri = new Uri(NavigationManager.Uri);
        CurrentUrl = uri.AbsolutePath;

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
            if (provider is not null)
            {
                var query = new ContentQuery
                {
                    Directory = Directory,
                    Category = Category,
                    Tag = Tag,
                    SearchQuery = SearchQuery,
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
                    TotalItems = allItems?.Count ?? 0;

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
        return $"osirion-content-list {Class}".Trim();
    }

    /// <summary>
    /// Gets the URL for a content item.
    /// </summary>
    protected string GetContentUrl(ContentItem item)
    {
        if (ContentUrlFormatter is not null)
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

        return PaginationUrlFormatter?.Invoke(page) ?? $"{CurrentUrl}?page={page}";
    }

    /// <summary>
    /// Gets the computed page title for SEO based on filters and pagination.
    /// </summary>
    protected string GetComputedPageTitle()
    {
        if (!string.IsNullOrWhiteSpace(PageTitle))
            return PageTitle!;

        var title = "";
        
        if (!string.IsNullOrWhiteSpace(Category))
            title = $"Category: {Category}";
        else if (!string.IsNullOrWhiteSpace(Tag))
            title = $"Tag: {Tag}";
        else if (!string.IsNullOrWhiteSpace(Directory))
            title = FormatDirectoryName(Directory);
        else
            title = "Blog";

        // Add page number if paginated and not first page
        if (ShowPagination && CurrentPage > 1)
            title += $" - Page {CurrentPage}";

        return title;
    }

    /// <summary>
    /// Formats a directory name for display (converts slug-case to Title Case).
    /// </summary>
    private string FormatDirectoryName(string directory)
    {
        // Remove leading/trailing slashes
        directory = directory.Trim('/');
        
        // Get last segment
        var lastSegment = directory.Split('/').LastOrDefault() ?? directory;
        
        // Convert slug-case to Title Case
        var words = lastSegment.Split('-', '_');
        var titleCased = string.Join(" ", words.Select(word => 
        {
            if (string.IsNullOrEmpty(word)) return word;
            return char.ToUpper(word[0]) + (word.Length > 1 ? word[1..].ToLower() : "");
        }));
        
        return titleCased;
    }

    /// <summary>
    /// Gets the computed page description for SEO.
    /// </summary>
    protected string? GetComputedPageDescription()
    {
        if (!string.IsNullOrWhiteSpace(PageDescription))
            return PageDescription;

        if (!string.IsNullOrWhiteSpace(Category))
            return $"Browse all articles in the {Category} category.";
        
        if (!string.IsNullOrWhiteSpace(Tag))
            return $"Browse all articles tagged with {Tag}.";
        
        if (!string.IsNullOrWhiteSpace(Directory))
            return $"Browse all articles in {FormatDirectoryName(Directory)}.";

        return "Browse our collection of articles and blog posts.";
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