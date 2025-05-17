using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Components;

public partial class ContentView(IContentProviderManager contentProviderManager)
{
    [Parameter]
    public string Path { get; set; } = string.Empty;

    [Parameter]
    public string LoadingText { get; set; } = "Loading content...";

    [Parameter]
    public string NotFoundText { get; set; } = "Content not found.";

    [Parameter]
    public Func<string, string>? CategoryUrlFormatter { get; set; }

    [Parameter]
    public Func<string, string>? TagUrlFormatter { get; set; }

    [Parameter]
    public Func<ContentItem, string>? ContentUrlFormatter { get; set; }

    [Parameter]
    public ContentItem? Item { get; set; }

    [Parameter]
    public ContentItem? PreviousItem { get; set; }

    [Parameter]
    public ContentItem? NextItem { get; set; }

    [Parameter]
    public bool ShowNavigationLinks { get; set; } = false;

    private bool IsLoading { get; set; } = true;

    protected override async Task OnParametersSetAsync()
    {
        if (Item is null && !string.IsNullOrEmpty(Path))
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
        try
        {
            var provider = contentProviderManager.GetDefaultProvider();
            if (provider != null)
            {
                Item = await provider.GetItemByPathAsync(Path);

                if (Item != null && ShowNavigationLinks)
                {
                    // If we need previous and next items, load them
                    var allItems = await provider.GetItemsByQueryAsync(new ContentQuery
                    {
                        Directory = System.IO.Path.GetDirectoryName(Path)?.Replace('\\', '/'),
                        SortBy = SortField.Date,
                        SortDirection = SortDirection.Descending
                    });

                    // Find the index manually
                    int currentIndex = -1;
                    for (int i = 0; i < allItems.Count; i++)
                    {
                        if (allItems[i].Path == Item.Path)
                        {
                            currentIndex = i;
                            break;
                        }
                    }

                    if (currentIndex > 0)
                    {
                        PreviousItem = allItems[currentIndex - 1];
                    }

                    if (currentIndex >= 0 && currentIndex < allItems.Count - 1)
                    {
                        NextItem = allItems[currentIndex + 1];
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading content: {ex.Message}");
            Item = null;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private string GetContentViewClass()
    {
        return $"osirion-content-view {CssClass}".Trim();
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
