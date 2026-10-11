using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>
/// Shows where an item stands in its series ("Part 2 of 5"), lists the parts and links the previous and next one.
/// The series comes from the item's <c>series</c> and <c>series_order</c> front matter; nothing renders for an item
/// that is not part of a series of at least two published items.
/// </summary>
public partial class ContentSeriesNavigation
{
    [Inject]
    private IContentProviderManager ContentProviderManager { get; set; } = default!;

    [Inject]
    private ILogger<ContentSeriesNavigation> Logger { get; set; } = default!;

    /// <summary>Gets or sets the item being shown.</summary>
    [Parameter, EditorRequired]
    public ContentItem? Item { get; set; }

    /// <summary>
    /// Gets or sets how an item's URL is built. Defaults to <c>/{item.Url}</c>, the path the content lists link to.
    /// </summary>
    [Parameter]
    public Func<ContentItem, string>? ContentUrlFormatter { get; set; }

    /// <summary>Gets or sets whether every part is listed, not only the previous and next one. Defaults to true.</summary>
    [Parameter]
    public bool ShowPartList { get; set; } = true;

    /// <summary>
    /// Gets or sets the position text, a format string where <c>{0}</c> is the part number and <c>{1}</c> the number
    /// of parts. Defaults to <c>Part {0} of {1}</c>.
    /// </summary>
    [Parameter]
    public string PositionFormat { get; set; } = "Part {0} of {1}";

    /// <summary>Gets or sets the label of the previous-part link. Defaults to <c>Previous</c>.</summary>
    [Parameter]
    public string PreviousText { get; set; } = "Previous";

    /// <summary>Gets or sets the label of the next-part link. Defaults to <c>Next</c>.</summary>
    [Parameter]
    public string NextText { get; set; } = "Next";

    private IReadOnlyList<ContentItem> Parts { get; set; } = [];

    private int CurrentIndex { get; set; } = -1;

    private string SeriesName { get; set; } = string.Empty;

    private string PositionText => string.Format(CultureInfo.CurrentCulture, PositionFormat, CurrentIndex + 1, Parts.Count);

    private string AriaLabel => $"{SeriesName}: {PositionText}";

    private (string? ItemId, string? Series) _loadedFor;

    /// <inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        var series = Item?.Metadata?.Series;
        if (Item is null || string.IsNullOrWhiteSpace(series))
        {
            Reset();
            return;
        }

        if (_loadedFor == (Item.Id, series))
        {
            return;
        }

        _loadedFor = (Item.Id, series);
        SeriesName = series;

        try
        {
            var query = new ContentQuery { Locale = string.IsNullOrWhiteSpace(Item.Locale) ? null : Item.Locale };
            var items = await ContentProviderManager.GetContentByQueryAsync(query);
            Parts = OrderParts(items.Where(candidate => string.Equals(candidate.Metadata?.Series, series, StringComparison.OrdinalIgnoreCase)));
            CurrentIndex = IndexOf(Parts, Item);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            // The article still renders, without its series links.
            Logger.LogError(exception, "Failed to load the parts of series {Series}", series);
            Parts = [];
            CurrentIndex = -1;
        }
    }

    /// <summary>
    /// Orders a series: numbered parts by <c>series_order</c>, then parts without a number, oldest first.
    /// </summary>
    internal static IReadOnlyList<ContentItem> OrderParts(IEnumerable<ContentItem> parts) =>
        parts
            .OrderBy(part => part.Metadata?.SeriesOrder is null)
            .ThenBy(part => part.Metadata?.SeriesOrder ?? 0)
            .ThenBy(part => part.DateCreated)
            .ThenBy(part => part.Title, StringComparer.Ordinal)
            .ToArray();

    private static int IndexOf(IReadOnlyList<ContentItem> parts, ContentItem item)
    {
        for (var index = 0; index < parts.Count; index++)
        {
            if (parts[index].Id == item.Id)
            {
                return index;
            }
        }

        return -1;
    }

    private void Reset()
    {
        _loadedFor = default;
        Parts = [];
        CurrentIndex = -1;
        SeriesName = string.Empty;
    }

    private string GetUrl(ContentItem item) => ContentUrlFormatter?.Invoke(item) ?? $"/{item.Url.TrimStart('/')}";

    private string GetNavClass() =>
        string.IsNullOrWhiteSpace(Class) ? "osirion-series-nav" : $"osirion-series-nav {Class}";
}
