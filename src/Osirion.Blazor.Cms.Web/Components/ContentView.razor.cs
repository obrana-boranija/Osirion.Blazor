using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>Displays a content item and optional previous and next navigation links.</summary>
public partial class ContentView(IContentProviderManager contentProviderManager)
{
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

    /// <summary>Performs the OnParametersSet operation asynchronously.</summary>
    protected override async Task OnParametersSetAsync()
    {
        if (Item is null && !string.IsNullOrWhiteSpace(Path))
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
            if (provider is not null)
            {
                Item = await provider.GetItemByPathAsync(Path);

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
        return $"osirion-content-view {Class}".Trim();
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
