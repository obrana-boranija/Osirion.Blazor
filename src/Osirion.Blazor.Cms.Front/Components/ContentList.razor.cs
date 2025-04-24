using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Interfaces;
using Osirion.Blazor.Cms.Models;

namespace Osirion.Blazor.Cms.Components;
public partial class ContentList(IContentProviderManager contentProviderManager)
{
    [Parameter]
    public string? Directory { get; set; }

    [Parameter]
    public string? Category { get; set; }

    [Parameter]
    public string? Tag { get; set; }

    [Parameter]
    public int? FeaturedCount { get; set; }

    [Parameter]
    public string LoadingText { get; set; } = "Loading content...";

    [Parameter]
    public string NoContentText { get; set; } = "No content available.";

    [Parameter]
    public string ReadMoreText { get; set; } = "Read more";

    [Parameter]
    public Func<ContentItem, string>? ContentUrlFormatter { get; set; }

    [Parameter]
    public bool ShowPagination { get; set; } = false;

    [Parameter]
    public int ItemsPerPage { get; set; } = 10;

    [Parameter]
    public int CurrentPage { get; set; } = 1;

    [Parameter]
    public EventCallback<int> PageChanged { get; set; }

    [Parameter]
    public Func<int, string>? PaginationUrlFormatter { get; set; }

    [Parameter]
    public string? Locale { get; set; }

    [Parameter]
    public string? DirectoryId { get; set; }

    private IReadOnlyList<ContentItem>? ContentItems { get; set; }
    private bool IsLoading { get; set; } = true;
    private int TotalPages => ContentItems != null ? (int)Math.Ceiling(ContentItems.Count / (double)ItemsPerPage) : 0;

    protected override async Task OnInitializedAsync()
    {
        await LoadContentAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        await LoadContentAsync();
    }

    private async Task LoadContentAsync()
    {
        IsLoading = true;
        try
        {
            var provider = contentProviderManager.GetDefaultProvider();
            if (provider != null)
            {
                var query = new ContentQuery();

                if (!string.IsNullOrEmpty(Directory))
                    query.Directory = Directory;

                if (!string.IsNullOrEmpty(Category))
                    query.Category = Category;

                if (!string.IsNullOrEmpty(Tag))
                    query.Tag = Tag;

                if (!string.IsNullOrEmpty(Locale))
                    query.Locale = Locale;

                if (!string.IsNullOrEmpty(DirectoryId))
                    query.DirectoryId = DirectoryId;

                if (FeaturedCount.HasValue)
                {
                    query.IsFeatured = true;
                    query.Take = FeaturedCount.Value;
                }
                else if (ShowPagination)
                {
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
        }
        finally
        {
            IsLoading = false;
        }
    }

    private string GetContentListClass()
    {
        return $"osirion-content-list {CssClass}".Trim();
    }

    private string GetContentUrl(ContentItem item)
    {
        return ContentUrlFormatter?.Invoke(item) ?? $"/{item.Path}";
    }

    private string GetPaginationUrl(int page)
    {
        return PaginationUrlFormatter?.Invoke(page) ?? $"?page={page}";
    }

    private async Task OnPageChanged(int page)
    {
        if (page != CurrentPage)
        {
            CurrentPage = page;
            await PageChanged.InvokeAsync(page);
        }
    }
}
