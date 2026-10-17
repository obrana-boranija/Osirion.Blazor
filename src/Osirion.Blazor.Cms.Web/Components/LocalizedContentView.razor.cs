using Microsoft.AspNetCore.Components;
#if !NET10_0_OR_GREATER
using Microsoft.AspNetCore.Http;
#endif
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>Displays localized content with optional translation and item navigation.</summary>
public partial class LocalizedContentView
{
    [Inject] private IContentProviderManager ContentProviderManager { get; set; } = default!;
    [Inject] private IServiceProvider ServiceProvider { get; set; } = default!;
    [Inject] private ILogger<LocalizedContentView> Logger { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    /// <summary>
    /// The id an item shares with its translations. The component sets it from the item's <c>ContentId</c>; leave it
    /// unset.
    /// </summary>
    [Parameter]
    public string? LocalizationId { get; set; }

    /// <summary>
    /// The source path of the item to load from the default provider while <see cref="Item"/> is not set. A later
    /// change of the path is ignored once an item is shown, so pass <see cref="Item"/> on a page that changes address
    /// while it is interactive.
    /// </summary>
    [Parameter]
    public string? Path { get; set; }

    /// <summary>
    /// The locale of the page. It is left out of the translation links, it is the locale of the default category and
    /// tag links (<c>/{locale}/category/{slug}</c>) and of the neighbor query.
    /// </summary>
    [Parameter]
    public string CurrentLocale { get; set; } = "en";

    /// <summary>
    /// Has no effect: translation links are plain links and nothing in the markup raises it.
    /// </summary>
    [Parameter]
    public EventCallback<string> OnLocaleChanged { get; set; }

    /// <summary>
    /// The text shown while the item loads, in interactive render modes only. A statically rendered response is sent
    /// once the item has loaded.
    /// </summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading content...";

    /// <summary>
    /// The message shown when no item is found, when loading fails (the error is logged) and when neither <see
    /// cref="Item"/> nor <see cref="Path"/> is set.
    /// </summary>
    [Parameter]
    public string NotFoundText { get; set; } = "Content not found.";

    /// <summary>
    /// Builds the link text of a translation from its locale code. Without it the text is the code in upper case,
    /// such as <c>SR</c>.
    /// </summary>
    [Parameter]
    public Func<string, string>? LocaleNameFormatter { get; set; }

    /// <summary>
    /// Builds a translation link from the localization id and the target locale. Without it the link is the
    /// translation's <c>Url</c> under a leading slash, which the built-in providers store without the locale folder,
    /// so add the locale here.
    /// </summary>
    [Parameter]
    public Func<string, string, string>? TranslationUrlFormatter { get; set; }

    /// <summary>
    /// Builds a category link from the category name. Without it the link is <c>/{CurrentLocale}/category/{slug}</c>.
    /// </summary>
    [Parameter]
    public Func<string, string>? CategoryUrlFormatter { get; set; }

    /// <summary>
    /// Builds a tag link from the tag name. Without it the link is <c>/{CurrentLocale}/tag/{slug}</c>.
    /// </summary>
    [Parameter]
    public Func<string, string>? TagUrlFormatter { get; set; }

    /// <summary>
    /// Builds the links of the previous and next items from the item. Without it a link is <c>/{item.Path}</c>, the
    /// source path, which is usually not the public address.
    /// </summary>
    [Parameter]
    public Func<ContentItem, string>? ContentUrlFormatter { get; set; }

    /// <summary>
    /// The item to render. When it is set, the component loads its translations (and the neighbors when <see
    /// cref="ShowNavigationLinks"/> is on) and never loads by <see cref="Path"/>.
    /// </summary>
    [Parameter]
    public ContentItem? Item { get; set; }

    /// <summary>
    /// The item the previous link points to. Loading with <see cref="ShowNavigationLinks"/> on replaces it when the
    /// neighbor query finds the item.
    /// </summary>
    [Parameter]
    public ContentItem? PreviousItem { get; set; }

    /// <summary>
    /// The item the next link points to. See <see cref="PreviousItem"/>.
    /// </summary>
    [Parameter]
    public ContentItem? NextItem { get; set; }

    /// <summary>
    /// Whether to show previous and next links to items in the same directory and locale, newest first. The component
    /// looks the neighbors up whenever it loads, also for an item you pass.
    /// </summary>
    [Parameter]
    public bool ShowNavigationLinks { get; set; } = false;

    /// <summary>
    /// Whether to render the header: the title as the page's <c>h1</c>, the author ("By ..."), the date, the reading
    /// time and the featured image. When it is on, the body leaves out its own leading <c>h1</c> so the title appears
    /// once; when it is off, the body is written as it is.
    /// </summary>
    [Parameter]
    public bool ShowJumbotron { get; set; } = true;

    /// <summary>
    /// Whether to look up the item's translations and list them. The links appear only when the item has more than
    /// one translation, and the current locale is left out.
    /// </summary>
    [Parameter]
    public bool EnableLocalization { get; set; } = true;

    private Dictionary<string, string> AvailableTranslations { get; set; } = new();
    private bool IsLoading { get; set; } = true;
    private bool HasMultipleTranslations => AvailableTranslations.Count > 1;
    private bool ShowTranslations => EnableLocalization && HasMultipleTranslations;

    //protected override async Task OnInitializedAsync()
    //{
    //    if (Item is null && !string.IsNullOrWhiteSpace(Path))
    //    {
    //        await LoadContentAsync();
    //    }
    //    else if (Item is not null)
    //    {
    //        LocalizationId = Item.ContentId;
    //        await LoadTranslationsAsync();
    //    }
    //}

    /// <summary>Performs the OnParametersSet operation asynchronously.</summary>
    protected override async Task OnParametersSetAsync()
    {
        if (Item is null && Path is { } path && !string.IsNullOrWhiteSpace(path))
        {
            await LoadContentAsync();
        }
        else if (Item is not null && LocalizationId != Item.ContentId)
        {
            LocalizationId = Item.ContentId;
            await LoadContentAsync();
        }

        SignalWhenNotFound(Item is null);
    }

#if !NET10_0_OR_GREATER
    /// <summary>The request context; only available during static server-side rendering.</summary>
    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }
#endif

    private readonly NotFoundSignal _notFound = new();

    // The not-found block is the page's answer, so the response says 404 with it instead of a soft 404.
    private void SignalWhenNotFound(bool notFound)
    {
        if (!notFound)
        {
            _notFound.Reset();
            return;
        }

#if NET10_0_OR_GREATER
        _notFound.Send(NavigationManager);
#else
        _notFound.Send(HttpContext);
#endif
    }

    /// <summary>Gets or sets the OnAfterRender value.</summary>
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            IsLoading = false;
            StateHasChanged();
        }

        base.OnAfterRender(firstRender);
    }

    private async Task LoadContentAsync()
    {
        IsLoading = true;
        try
        {
            var provider = ContentProviderManager.GetDefaultProvider();
            if (provider is not null)
            {
                if (Item is null && Path is { } path && !string.IsNullOrWhiteSpace(path))
                {
                    Item = await provider.GetItemByPathAsync(path);
                }


                if (Item is not null)
                {
                    LocalizationId = Item.ContentId;
                    await LoadTranslationsAsync();

                    if (ShowNavigationLinks)
                    {
                        // If we need previous and next items, load them
                        var allItems = await provider.GetItemsByQueryAsync(new ContentQuery
                        {
                            Directory = Item.Directory?.Name,
                            Locale = CurrentLocale,
                            SortBy = SortField.Date,
                            SortDirection = SortDirection.Descending
                        }) ?? [];

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
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load localized content {ContentPath}", Path);
            Item = null;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadTranslationsAsync()
    {
        if (string.IsNullOrWhiteSpace(LocalizationId) || !EnableLocalization)
        {
            AvailableTranslations.Clear();
            return;
        }

        try
        {
            var provider = ContentProviderManager.GetDefaultProvider();
            if (provider is not null)
            {
                var translations = await provider.GetContentTranslationsAsync(LocalizationId);

                AvailableTranslations.Clear();
                foreach (var translation in translations)
                {
                    AvailableTranslations[translation.Key] = translation.Value.Url;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load translations for localization {LocalizationId}", LocalizationId);
        }
    }

    private string GetContentViewClass()
    {
        return $"osirion-localized-content-view {Class}".Trim();
    }

    private readonly SanitizedContentCache _sanitizedContent = new();

    private MarkupString GetSanitizedContent(string? content)
    {
        return _sanitizedContent.Get(ServiceProvider, content);
    }

    private string GetLocaleName(string locale)
    {
        return LocaleNameFormatter?.Invoke(locale) ?? locale.ToUpperInvariant();
    }

    private string GetTranslationUrl(string locale)
    {
        // A leading slash on the stored URL must not produce "//", which browsers read as another host.
        return TranslationUrlFormatter?.Invoke(LocalizationId ?? string.Empty, locale)
            ?? "/" + AvailableTranslations[locale].TrimStart('/');
    }

    private string GetTranslationClass(string locale)
    {
        return locale == CurrentLocale
            ? "osirion-translation-link osirion-active"
            : "osirion-translation-link";
    }

    private string GetCategoryUrl(string category)
    {
        return CategoryUrlFormatter?.Invoke(category) ?? ContentUrls.CategoryUrl(category, CurrentLocale);
    }

    private string GetTagUrl(string tag)
    {
        return TagUrlFormatter?.Invoke(tag) ?? ContentUrls.TagUrl(tag, CurrentLocale);
    }

    private string GetContentUrl(ContentItem item)
    {
        return ContentUrlFormatter?.Invoke(item) ?? $"/{item.Path}";
    }

    private async Task SwitchTranslation(string locale)
    {
        if (locale != CurrentLocale && AvailableTranslations.TryGetValue(locale, out var path))
        {
            CurrentLocale = locale;
            Path = path;
            await OnLocaleChanged.InvokeAsync(locale);
            await LoadContentAsync();
        }
    }
}
