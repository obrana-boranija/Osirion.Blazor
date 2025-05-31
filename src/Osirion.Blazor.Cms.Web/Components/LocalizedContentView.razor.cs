using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Components;

public partial class LocalizedContentView
{
    [Inject] private IContentProviderManager ContentProviderManager { get; set; } = default!;

    [Parameter]
    public string? LocalizationId { get; set; }

    [Parameter]
    public string? Path { get; set; }

    [Parameter]
    public string CurrentLocale { get; set; } = "en";

    [Parameter]
    public EventCallback<string> OnLocaleChanged { get; set; }

    [Parameter]
    public string LoadingText { get; set; } = "Loading content...";

    [Parameter]
    public string NotFoundText { get; set; } = "Content not found.";

    [Parameter]
    public Func<string, string>? LocaleNameFormatter { get; set; }

    [Parameter]
    public Func<string, string, string>? TranslationUrlFormatter { get; set; }

    [Parameter]
    public Func<string, string>? CategoryUrlFormatter { get; set; }

    [Parameter]
    public Func<string, string>? TagUrlFormatter { get; set; }

    [Parameter]
    public Func<ContentItem, string>? ContentUrlFormatter { get; set; }

    [Parameter]
    public ContentItem? Item { get; set; }

    [Parameter]
    public ContentItem? PreviousItem { get; set; }

    [Parameter]
    public ContentItem? NextItem { get; set; }

    [Parameter]
    public bool ShowNavigationLinks { get; set; } = false;

    [Parameter]
    public bool ShowJumbotron { get; set; } = true;

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

    protected override async Task OnParametersSetAsync()
    {
        if (Item is null && !string.IsNullOrWhiteSpace(Path))
        {
            await LoadContentAsync();
        }
        else if (Item is not null && LocalizationId != Item.ContentId)
        {
            LocalizationId = Item.ContentId;
            await LoadContentAsync();
        }
    }

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
                if(Item is null)
                {
                    Item = await provider.GetItemByPathAsync(Path);
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
                        });

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
        return $"osirion-localized-content-view {CssClass}".Trim();
    }

    private string GetLocaleName(string locale)
    {
        return LocaleNameFormatter?.Invoke(locale) ?? locale.ToUpperInvariant();
    }

    private string GetTranslationUrl(string locale)
    {
        return TranslationUrlFormatter?.Invoke(LocalizationId, locale) ?? $"/{AvailableTranslations[locale]}";
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