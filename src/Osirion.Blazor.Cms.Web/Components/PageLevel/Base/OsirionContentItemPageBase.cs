using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>
/// Represents the base class for a page that shows one content item: it takes the item, or finds it by path, URL or
/// slug, and loads it whenever those parameters change.
/// </summary>
public abstract class OsirionContentItemPageBase : OsirionContentPageBase
{
    #region Content

    /// <summary>
    /// The item to show. When you set it, the page shows it and loads nothing; otherwise the page finds the first
    /// match of <see cref="OsirionContentPageBase.Query"/> or of the <c>ItemPath</c>, <c>ItemUrl</c>,
    /// <c>ItemSlug</c>, locale and directory parameters, and sets this property. Used by <c>ArticlePage</c>,
    /// <c>AboutPage</c> and <c>ContactPage</c>; <c>HomePage</c> and <c>DocumentPage</c> take their item as
    /// <c>Content</c>.
    /// </summary>
    [Parameter]
    public ContentItem? Item { get; set; }

    /// <summary>
    /// The source path of the item to find (<c>ContentItem.Path</c>, for example <c>en/articles/my-article.md</c>). A
    /// change of it, of <c>ItemUrl</c>, <c>ItemSlug</c>, the locale or the directory makes the page look the item up
    /// again.
    /// </summary>
    [Parameter]
    public string? ItemPath { get; set; }

    /// <summary>
    /// The public URL of the item to find (<c>ContentItem.Url</c>, for example <c>articles/my-article</c>), without a
    /// locale folder.
    /// </summary>
    [Parameter]
    public string? ItemUrl { get; set; }

    /// <summary>
    /// The slug of the item to find (<c>ContentItem.Slug</c>, for example <c>my-article</c>). Combine it with
    /// <c>DirectoryName</c> when slugs repeat across directories.
    /// </summary>
    [Parameter]
    public string? ItemSlug { get; set; }

    /// <summary>
    /// Builds the links of the other parts of a series from the item. Passed on by <c>ArticlePage</c> to its series
    /// navigation; no other page reads it.
    /// </summary>
    [Parameter]
    public Func<ContentItem, string>? ItemUrlFormatter { get; set; }

    #endregion

    /// <summary>
    /// Initializes the component and loads content asynchronously based on the provided parameters.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        await LoadContentAsync();
    }

    // What this page loaded itself, so a change of ItemPath, ItemUrl, ItemSlug, Locale or Directory replaces it,
    // while an Item supplied by the parent is left alone.
    private string? _loadedKey;
    private ContentItem? _loadedItem;

    private string BuildLoadKey() =>
        string.Join('\u001f', ItemPath, ItemUrl, ItemSlug, Locale, DirectoryName);

    /// <summary>
    /// Loads the content asynchronously based on the provided parameters.
    /// </summary>
    protected async Task LoadContentAsync()
    {
        var key = BuildLoadKey();
        var itemSuppliedByParent = Item is not null && !ReferenceEquals(Item, _loadedItem);
        if (itemSuppliedByParent)
        {
            return;
        }

        if (Item is null || !string.Equals(key, _loadedKey, StringComparison.Ordinal))
        {
            OnLoadedItemReplaced();
            Item = null;
            _loadedKey = key;
            _loadedItem = null;
            ResetNotFoundSignal();
            IsLoading = true;
            try
            {
                var result = await ContentProviderManager
                    .GetContentByQueryAsync(Query ?? new ContentQuery
                    {
                        Path = ItemPath,
                        Directory = DirectoryName,
                        Url = ItemUrl,
                        Slug = ItemSlug,
                        Locale = Locale,
                    });

                Item = _loadedItem = result?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to load content (path {ItemPath}, url {ItemUrl}, slug {ItemSlug}, locale {Locale})",
                    ItemPath, ItemUrl, ItemSlug, Locale);
                Item = null;
            }
            finally
            {
                IsLoading = false;
                await OnItemLoadedAsync();
            }
        }
    }

    /// <summary>
    /// Called before the page loads a different item, so a derived page can drop what it loaded for the previous one.
    /// </summary>
    protected virtual void OnLoadedItemReplaced()
    {
    }

    /// <summary>
    /// Called after the page loaded its item, also when nothing was found (<see cref="Item"/> is then
    /// <see langword="null"/>), so a derived page can load what belongs with it.
    /// </summary>
    protected virtual Task OnItemLoadedAsync() => Task.CompletedTask;
}
