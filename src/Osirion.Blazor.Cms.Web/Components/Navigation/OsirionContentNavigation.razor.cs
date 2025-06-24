using Microsoft.AspNetCore.Components;
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
    /// <summary>
    /// Gets or sets a default item to search for navigation items
    /// </summary>
    [Parameter]
    public ContentItem? Item { get; set; }

    /// <summary>
    /// Gets or sets the previous navigation item
    /// </summary>
    [Parameter]
    public ContentItem? PreviousItem { get; set; }

    /// <summary>
    /// Gets or sets the next navigation item
    /// </summary>
    [Parameter]
    public ContentItem? NextItem { get; set; }

    /// <summary>
    /// Gets or sets the label for the previous link
    /// </summary>
    [Parameter]
    public string PreviousLabel { get; set; } = "Previous";

    /// <summary>
    /// Gets or sets the label for the next link
    /// </summary>
    [Parameter]
    public string NextLabel { get; set; } = "Next";

    /// <summary>
    /// Gets or sets the aria-label for the navigation
    /// </summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Content navigation";

    /// <summary>
    /// Gets or sets whether to show icons
    /// </summary>
    [Parameter]
    public bool ShowIcons { get; set; } = true;

    /// <summary>
    /// Gets or sets the previous icon content
    /// </summary>
    [Parameter]
    public RenderFragment? PreviousIcon { get; set; }

    /// <summary>
    /// Gets or sets the next icon content
    /// </summary>
    [Parameter]
    public RenderFragment? NextIcon { get; set; }

    /// <summary>
    /// Gets or sets whether to show descriptions
    /// </summary>
    [Parameter]
    public bool ShowDescription { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to show placeholders for missing items
    /// </summary>
    [Parameter]
    public bool ShowPlaceholder { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to show the navigation
    /// </summary>
    [Parameter]
    public bool ShowNavigation { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to prevent default link behavior
    /// </summary>
    [Parameter]
    public bool PreventDefault { get; set; } = false;

    /// <summary>
    /// Gets or sets the navigation variant: "default", "compact", "card"
    /// </summary>
    [Parameter]
    public ContentNavigationVariant Variant { get; set; } = ContentNavigationVariant.Default;

    /// <summary>
    /// Gets or sets the URL generator function
    /// </summary>
    [Parameter]
    public Func<ContentItem, string>? UrlGenerator { get; set; }

    /// <summary>
    /// Gets or sets the title extractor function
    /// </summary>
    [Parameter]
    public Func<ContentItem, string>? TitleExtractor { get; set; }

    /// <summary>
    /// Gets or sets the description extractor function
    /// </summary>
    [Parameter]
    public Func<ContentItem, string?>? DescriptionExtractor { get; set; }

    /// <summary>
    /// Event callback when previous item is clicked
    /// </summary>
    [Parameter]
    public EventCallback<ContentItem> OnPreviousClick { get; set; }

    /// <summary>
    /// Event callback when next item is clicked
    /// </summary>
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
                Console.Error.WriteLine($"Error loading navigation items: {ex.Message}");
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