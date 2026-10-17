using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>
/// Navigation component for displaying previous/next content links
/// </summary>
public partial class OsirionContentNavigation(IContentProviderManager ContentProviderManager)
{
    [Inject]
    private ILogger<OsirionContentNavigation> Logger { get; set; } = default!;

    /// <summary>
    /// Gets or sets the current item. Required: nothing renders without it, even when both neighbours are passed. A
    /// missing neighbour is looked up from it.
    /// </summary>
    [Parameter]
    public ContentItem? Item { get; set; }

    /// <summary>Gets or sets the previous item. When <c>null</c>, it is looked up from <see cref="Item"/>.</summary>
    [Parameter]
    public ContentItem? PreviousItem { get; set; }

    /// <summary>Gets or sets the next item. When <c>null</c>, it is looked up from <see cref="Item"/>.</summary>
    [Parameter]
    public ContentItem? NextItem { get; set; }

    /// <summary>Gets or sets the label above the previous item's title.</summary>
    [Parameter]
    public string PreviousLabel { get; set; } = "Previous";

    /// <summary>Gets or sets the label above the next item's title.</summary>
    [Parameter]
    public string NextLabel { get; set; } = "Next";

    /// <summary>Gets or sets the accessible name of the <c>nav</c> element.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Content navigation";

    /// <summary>Gets or sets whether the arrow icons are shown.</summary>
    [Parameter]
    public bool ShowIcons { get; set; } = true;

    /// <summary>
    /// Gets or sets the icon of the previous link. A left chevron when not set and <see cref="ShowIcons"/> is
    /// <c>true</c>.
    /// </summary>
    [Parameter]
    public RenderFragment? PreviousIcon { get; set; }

    /// <summary>
    /// Gets or sets the icon of the next link. A right chevron when not set and <see cref="ShowIcons"/> is
    /// <c>true</c>.
    /// </summary>
    [Parameter]
    public RenderFragment? NextIcon { get; set; }

    /// <summary>Gets or sets whether each neighbour's description is shown under its title.</summary>
    [Parameter]
    public bool ShowDescription { get; set; } = false;

    /// <summary>
    /// Gets or sets whether an empty placeholder is rendered where a neighbour is missing, so the other link keeps
    /// its side.
    /// </summary>
    [Parameter]
    public bool ShowPlaceholder { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the navigation renders. When <c>false</c>, nothing renders and the neighbours are not
    /// looked up.
    /// </summary>
    [Parameter]
    public bool ShowNavigation { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the links are stopped from navigating in an interactive render mode. Leave it
    /// <c>false</c>: the click callbacks are not raised, so a prevented link does nothing.
    /// </summary>
    [Parameter]
    public bool PreventDefault { get; set; } = false;

    /// <summary>
    /// Gets or sets the layout: <c>Default</c>, <c>Compact</c> or <c>Card</c>. It is written as a class on the
    /// <c>nav</c>.
    /// </summary>
    [Parameter]
    public ContentNavigationVariant Variant { get; set; } = ContentNavigationVariant.Default;

    /// <summary>
    /// Gets or sets a function that builds each link. Without it, the link is the item's <c>Url</c>, a path without
    /// a leading slash that resolves against the page's address.
    /// </summary>
    [Parameter]
    public Func<ContentItem, string>? UrlGenerator { get; set; }

    /// <summary>
    /// Gets or sets a function that returns the text shown as each title. Without it, the item's <c>Title</c>, or
    /// <c>Untitled</c>.
    /// </summary>
    [Parameter]
    public Func<ContentItem, string>? TitleExtractor { get; set; }

    /// <summary>
    /// Gets or sets a function that returns the text shown as each description. Without it, the item's
    /// <c>Description</c>.
    /// </summary>
    [Parameter]
    public Func<ContentItem, string?>? DescriptionExtractor { get; set; }

    /// <summary>Gets or sets a callback for a click on the previous link. Declared but never raised in 4.0.</summary>
    [Parameter]
    public EventCallback<ContentItem> OnPreviousClick { get; set; }

    /// <summary>Gets or sets a callback for a click on the next link. Declared but never raised in 4.0.</summary>
    [Parameter]
    public EventCallback<ContentItem> OnNextClick { get; set; }

    /// <summary>
    /// Gets the CSS class for the navigation
    /// </summary>
    private string GetNavigationClass()
    {
        var classes = new List<string> { "osirion-content-navigation" };

        classes.Add($"osirion-content-navigation-{Variant.ToString().ToLowerInvariant()}");

        if (PreviousItem is null || NextItem is null)
        {
            classes.Add("osirion-content-navigation-single");
        }

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        return string.Join(" ", classes);
    }

    /// <summary>
    /// Gets the navigation URL for an item
    /// </summary>
    private string GetNavigationUrl(ContentItem item)
    {
        if (UrlGenerator is not null)
        {
            return UrlGenerator(item);
        }

        return item.Url ?? "#";
    }

    /// <summary>
    /// Gets the title for an item
    /// </summary>
    private string GetItemTitle(ContentItem item)
    {
        if (TitleExtractor is not null)
        {
            return TitleExtractor(item);
        }

        return item.Title ?? "Untitled";
    }

    /// <summary>
    /// Gets the description for an item
    /// </summary>
    private string? GetItemDescription(ContentItem item)
    {
        if (DescriptionExtractor is not null)
        {
            return DescriptionExtractor(item);
        }

        return item.Description;
    }

    /// <summary>Performs the OnParametersSet operation asynchronously.</summary>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        // Set default icons if not provided and ShowIcons is true
        if (ShowIcons)
        {
            PreviousIcon ??= builder =>
            {
                builder.OpenElement(0, "svg");
                builder.AddAttribute(1, "xmlns", "http://www.w3.org/2000/svg");
                builder.AddAttribute(2, "width", "16");
                builder.AddAttribute(3, "height", "16");
                builder.AddAttribute(4, "viewBox", "0 0 24 24");
                builder.AddAttribute(5, "fill", "none");
                builder.AddAttribute(6, "stroke", "currentColor");
                builder.AddAttribute(7, "stroke-width", "2");
                builder.AddAttribute(8, "stroke-linecap", "round");
                builder.AddAttribute(9, "stroke-linejoin", "round");
                builder.OpenElement(10, "polyline");
                builder.AddAttribute(11, "points", "15 18 9 12 15 6");
                builder.CloseElement();
                builder.CloseElement();
            };

            NextIcon ??= builder =>
            {
                builder.OpenElement(0, "svg");
                builder.AddAttribute(1, "xmlns", "http://www.w3.org/2000/svg");
                builder.AddAttribute(2, "width", "16");
                builder.AddAttribute(3, "height", "16");
                builder.AddAttribute(4, "viewBox", "0 0 24 24");
                builder.AddAttribute(5, "fill", "none");
                builder.AddAttribute(6, "stroke", "currentColor");
                builder.AddAttribute(7, "stroke-width", "2");
                builder.AddAttribute(8, "stroke-linecap", "round");
                builder.AddAttribute(9, "stroke-linejoin", "round");
                builder.OpenElement(10, "polyline");
                builder.AddAttribute(11, "points", "9 18 15 12 9 6");
                builder.CloseElement();
                builder.CloseElement();
            };
        }

        await LoadNavigationItemsAsync();
    }

    private async Task LoadNavigationItemsAsync()
    {
        if (ShowNavigation && (PreviousItem is null || NextItem is null) && Item is not null)
        {
            try
            {
                var query = new ContentQuery
                {
                    Locale = Item.Locale,
                    Directory = Item.Directory?.Name,
                    SortBy = SortField.Order,
                    SortDirection = SortDirection.Ascending
                };

                var items = await ContentProviderManager.GetContentByQueryAsync(query);

                int currentIndex = -1;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Path == Item.Path)
                    {
                        currentIndex = i;
                        break;
                    }
                }

                if (currentIndex > 0)
                {
                    PreviousItem = items[currentIndex - 1];
                }

                if (currentIndex >= 0 && currentIndex < items.Count - 1)
                {
                    NextItem = items[currentIndex + 1];
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to load previous and next items");
            }
        }
    }
}

/// <summary>
/// Content navigation display variants
/// </summary>
public enum ContentNavigationVariant
{
    /// <summary>
    /// Default navigation style
    /// </summary>
    Default,

    /// <summary>
    /// Compact navigation style
    /// </summary>
    Compact,

    /// <summary>
    /// Card-based navigation style
    /// </summary>
    Card
}
