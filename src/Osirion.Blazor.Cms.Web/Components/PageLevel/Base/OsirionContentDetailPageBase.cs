using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Web.Components;

public abstract partial class OsirionContentDetailPageBase : OsirionContentPageBase
{
    #region Content

    /// <summary>
    /// Gets or sets the content item to display
    /// </summary>
    [Parameter]
    public ContentItem? Item { get; set; }

    /// <summary>
    /// Gets or sets the path to the article content (eg. "en/articles/my-article.md")
    /// </summary>
    [Parameter]
    public string? ItemPath { get; set; }

    /// <summary>
    /// Gets or sets the url to the article content (eg. "articles/my-article")
    /// </summary>
    [Parameter]
    public string? ItemUrl { get; set; }

    /// <summary>
    /// Gets or sets the slug to the article content (eg. "my-article")
    /// </summary>
    [Parameter]
    public string? ItemSlug { get; set; }

    /// <summary>
    /// Gets or sets the content URL formatter
    /// </summary>
    [Parameter]
    public Func<ContentItem, string>? ItemUrlFormatter { get; set; }

    #endregion

    #region Content Navigation 

    /// <summary>
    /// Gets or sets whether to show navigation links
    /// </summary>
    [Parameter]
    public bool ShowNavigation { get; set; } = true;

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

    #endregion

    #region Related Items

    /// <summary>
    /// Gets or sets the related content items
    /// </summary>
    [Parameter]
    public IReadOnlyList<ContentItem>? RelatedItems { get; set; }

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

    #endregion

    /// <summary>
    /// Initializes the component and loads content asynchronously based on the provided parameters.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        await LoadContentAsync();
    }

    /// <summary>
    /// Loads the content asynchronously based on the provided parameters.
    /// </summary>
    protected async Task LoadContentAsync()
    {
        if (Item is null)
        {
            IsLoading = true;
            try
            {
                var result = await ContentProviderManager
                    .GetContentByQueryAsync(Query ?? new ContentQuery
                    {
                        Directory = DirectoryName,
                        Url = ItemUrl,
                        Slug = ItemSlug,
                        Locale = Locale,
                    });

                Item = result?.FirstOrDefault();
            }
            catch (Exception)
            {
                Item = null;
            }
            finally
            {
                IsLoading = false;
                await LoadRelatedContentAsync();
            }
        }
    }

    private async Task LoadRelatedContentAsync()
    {
        if (!ShowRelatedContent || RelatedItems is not null || Item is null)
            return;

        try
        {
            var query = new ContentQuery
            {
                Locale = Locale,
                Directory = Item.Directory?.Name,
                SortBy = SortField.Date,
                SortDirection = SortDirection.Descending
            };

            var items = await ContentProviderManager.GetContentByQueryAsync(query);

            if (items is not null)
            {
                var related = items
                .Where(i => i.Path != Item.Path)
                .Where(i =>
                    i.Tags.Intersect(Item.Tags, StringComparer.OrdinalIgnoreCase).Any() ||
                    i.Categories.Intersect(Item.Categories, StringComparer.OrdinalIgnoreCase).Any())
                .OrderByDescending(i =>
                    i.Tags.Intersect(Item.Tags, StringComparer.OrdinalIgnoreCase).Count() +
                    i.Categories.Intersect(Item.Categories, StringComparer.OrdinalIgnoreCase).Count())
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
}
