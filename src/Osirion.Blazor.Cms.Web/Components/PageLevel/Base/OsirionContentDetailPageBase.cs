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
    /// Whether <c>ArticlePage</c> shows the previous and next articles under the body. The other pages do not read
    /// it.
    /// </summary>
    [Parameter]
    public bool ShowNavigation { get; set; } = true;

    /// <summary>
    /// The item of the previous link. Read by <c>DocumentPage</c>, which looks it up when it is null and keeps a value
    /// you pass; <c>ArticlePage</c> finds its own neighbors and ignores it.
    /// </summary>
    [Parameter]
    public ContentItem? PreviousItem { get; set; }

    /// <summary>
    /// The item of the next link. Read by <c>DocumentPage</c>, which looks it up when it is null and keeps a value you
    /// pass; <c>ArticlePage</c> finds its own neighbors and ignores it.
    /// </summary>
    [Parameter]
    public ContentItem? NextItem { get; set; }

    #endregion

    #region Related Items

    /// <summary>
    /// The items of the related section of <c>ArticlePage</c>. When it is null and <see cref="ShowRelatedContent"/>
    /// is on, the page picks items of the same locale and directory that share a tag or a category with the article,
    /// most shared first, up to <see cref="RelatedItemsCount"/>. An empty list shows no section.
    /// </summary>
    [Parameter]
    public IReadOnlyList<ContentItem>? RelatedItems { get; set; }

    /// <summary>
    /// Whether <c>ArticlePage</c> shows a related section. The other pages do not read it, although the page base
    /// still loads related items for them.
    /// </summary>
    [Parameter]
    public bool ShowRelatedContent { get; set; } = true;

    /// <summary>
    /// The heading of the related section of <c>ArticlePage</c>.
    /// </summary>
    [Parameter]
    public string RelatedContentTitle { get; set; } = "Related Articles";

    /// <summary>
    /// The most related items the page picks when it finds them itself.
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
