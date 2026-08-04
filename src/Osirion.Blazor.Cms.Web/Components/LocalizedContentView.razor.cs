using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>Displays localized content with optional translation and item navigation.</summary>
public partial class LocalizedContentView
{
    [Inject] private IContentProviderManager ContentProviderManager { get; set; } = default!;

    /// <summary>Gets or sets the content localization identifier.</summary>
    [Parameter]
    public string? LocalizationId { get; set; }

    /// <summary>Gets or sets the content path to load.</summary>
    [Parameter]
    public string? Path { get; set; }

    /// <summary>Gets or sets the current locale.</summary>
    [Parameter]
    public string CurrentLocale { get; set; } = "en";

    /// <summary>Gets or sets the callback invoked when the locale changes.</summary>
    [Parameter]
    public EventCallback<string> OnLocaleChanged { get; set; }

    /// <summary>Gets or sets the loading message.</summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading content...";

    /// <summary>Gets or sets the not-found message.</summary>
    [Parameter]
    public string NotFoundText { get; set; } = "Content not found.";

    /// <summary>Gets or sets the locale display-name formatter.</summary>
    [Parameter]
    public Func<string, string>? LocaleNameFormatter { get; set; }

    /// <summary>Gets or sets the translation URL formatter.</summary>
    [Parameter]
    public Func<string, string, string>? TranslationUrlFormatter { get; set; }

    /// <summary>Gets or sets the category URL formatter.</summary>
    [Parameter]
    public Func<string, string>? CategoryUrlFormatter { get; set; }

    /// <summary>Gets or sets the tag URL formatter.</summary>
    [Parameter]
    public Func<string, string>? TagUrlFormatter { get; set; }

    /// <summary>Gets or sets the content URL formatter.</summary>
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

    /// <summary>Gets or sets whether previous and next navigation links are shown.</summary>
    [Parameter]
    public bool ShowNavigationLinks { get; set; } = false;

    /// <summary>Gets or sets whether the jumbotron is shown.</summary>
    [Parameter]
    public bool ShowJumbotron { get; set; } = true;

    /// <summary>Gets or sets whether localization controls are enabled.</summary>
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
    }

    /// <summary>Gets or sets the OnAfterRender value.</summary>
    protected override void OnAfterRender(bool firstRender)
    {
        if(firstRender)
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
            Console.Error.WriteLine($"Error loading content: {ex.Message}");
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
            Console.Error.WriteLine($"Error loading translations: {ex.Message}");
        }
    }

    private string GetContentViewClass()
    {
        return $"osirion-localized-content-view {Class}".Trim();
    }

    private string GetLocaleName(string locale)
    {
        return LocaleNameFormatter?.Invoke(locale) ?? locale.ToUpperInvariant();
    }

    private string GetTranslationUrl(string locale)
    {
        return TranslationUrlFormatter?.Invoke(LocalizationId ?? string.Empty, locale) ?? $"/{AvailableTranslations[locale]}";
    }

    private string GetTranslationClass(string locale)
    {
        return locale == CurrentLocale
            ? "osirion-translation-link osirion-active"
            : "osirion-translation-link";
    }

    private string GetCategoryUrl(string category)
    {
        var url = (CategoryUrlFormatter?.Invoke(category) ?? $"/{CurrentLocale}/category/{category.ToLower().Replace(' ', '-')}").Trim('/');
        return url;
    }

    private string GetTagUrl(string tag)
    {
        return TagUrlFormatter?.Invoke(tag) ?? $"/{CurrentLocale}/tag/{tag.ToLower().Replace(' ', '-')}";
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
