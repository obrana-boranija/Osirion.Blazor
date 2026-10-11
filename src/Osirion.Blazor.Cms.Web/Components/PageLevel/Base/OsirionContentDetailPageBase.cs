using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>
/// Represents the base class for a content detail page in the Osirion CMS.
/// Provides properties and methods for displaying a single content item, navigation, and related content.
/// </summary>
public abstract partial class OsirionContentDetailPageBase : OsirionContentItemPageBase
{
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

    // Related items this page loaded itself, so loading another item replaces them, while related items supplied
    // by the parent are left alone.
    private IReadOnlyList<ContentItem>? _loadedRelated;

    /// <inheritdoc/>
    protected override void OnLoadedItemReplaced()
    {
        if (ReferenceEquals(RelatedItems, _loadedRelated))
        {
            RelatedItems = null;
        }

        _loadedRelated = null;
    }

    /// <inheritdoc/>
    protected override Task OnItemLoadedAsync() => LoadRelatedContentAsync();

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

                RelatedItems = _loadedRelated = related;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load related content for {ContentPath}", Item.Path);
        }
    }
}
