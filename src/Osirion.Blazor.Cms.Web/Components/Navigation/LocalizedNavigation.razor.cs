using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>Displays localized directory and content navigation.</summary>
public partial class LocalizedNavigation
{
    [Inject] private IContentProviderManager ContentProviderManager { get; set; } = default!;

    /// <summary>Gets or sets the navigation title.</summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>Gets or sets the loading message.</summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading navigation...";

    /// <summary>Gets or sets the empty-navigation message.</summary>
    [Parameter]
    public string NoContentText { get; set; } = "No navigation available.";

    /// <summary>Gets or sets the current locale.</summary>
    [Parameter]
    public string? CurrentLocale { get; set; }

    /// <summary>Gets or sets the callback invoked when the locale changes.</summary>
    [Parameter]
    public EventCallback<string> OnLocaleChanged { get; set; }

    /// <summary>Gets or sets the callback invoked when a directory is selected.</summary>
    [Parameter]
    public EventCallback<DirectoryItem> OnDirectorySelected { get; set; }

    /// <summary>Gets or sets the callback invoked when content is selected.</summary>
    [Parameter]
    public EventCallback<ContentItem> OnContentSelected { get; set; }

    /// <summary>Gets or sets whether all directories are expanded.</summary>
    [Parameter]
    public bool ExpandAllDirectories { get; set; } = false;

    /// <summary>Gets or sets the identifier of the expanded directory.</summary>
    [Parameter]
    public string? ExpandedDirectoryId { get; set; }

    /// <summary>Gets or sets whether content items are shown.</summary>
    [Parameter]
    public bool ShowContentItems { get; set; } = true;

    /// <summary>Gets or sets the directory URL formatter.</summary>
    [Parameter]
    public Func<DirectoryItem, string>? DirectoryUrlFormatter { get; set; }

    /// <summary>Gets or sets the content URL formatter.</summary>
    [Parameter]
    public Func<ContentItem, string>? ContentUrlFormatter { get; set; }

    /// <summary>Gets or sets the locale display-name formatter.</summary>
    [Parameter]
    public Func<string, string>? LocaleNameFormatter { get; set; }

    /// <summary>Gets or sets whether localization controls are enabled.</summary>
    [Parameter]
    public bool EnableLocalization { get; set; } = true;

    private IReadOnlyList<DirectoryItem>? Directories { get; set; }
    private List<string> AvailableLocales { get; set; } = new();
    private bool IsLoading { get; set; } = true;
    private bool HasMultipleLocales => AvailableLocales.Count > 1;
    private bool ShowLocaleSelector => EnableLocalization && HasMultipleLocales;

    /// <summary>Initializes the component state and required services.</summary>
    protected override async Task OnInitializedAsync()
    {
        await LoadNavigationAsync();
    }

    /// <summary>Performs the OnParametersSet operation asynchronously.</summary>
    protected override async Task OnParametersSetAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentLocale) && AvailableLocales.Any())
        {
            CurrentLocale = AvailableLocales.First();
        }

        await LoadNavigationAsync();
    }

    private async Task LoadNavigationAsync()
    {
        IsLoading = true;
        try
        {
            var provider = ContentProviderManager.GetDefaultProvider();
            if (provider is not null)
            {
                // Get available locales
                //var localizationInfo = await provider.GetLocalizationInfoAsync();
                //AvailableLocales = EnableLocalization
                //    ? localizationInfo.AvailableLocales.Select(locale => locale.Code).ToList()
                //    : new List<string> { localizationInfo.DefaultLocale };

                //if (string.IsNullOrWhiteSpace(CurrentLocale) && AvailableLocales.Any())
                //{
                //    CurrentLocale = localizationInfo.DefaultLocale;
                //}

                // Get directories for the current locale or all directories if localization is disabled
                Directories = EnableLocalization
                    ? await provider.GetDirectoriesAsync(CurrentLocale)
                    : await provider.GetDirectoriesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading navigation: {ex.Message}");
            Directories = new List<DirectoryItem>();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private string GetNavClass()
    {
        return $"osirion-localized-navigation {Class}".Trim();
    }

    private string GetDirectoryUrl(DirectoryItem directory)
    {
        return DirectoryUrlFormatter?.Invoke(directory) ?? $"/{directory.Path}";
    }

    private string GetContentUrl(ContentItem item)
    {
        return ContentUrlFormatter?.Invoke(item) ?? $"/{item.Path}";
    }

    private string GetLocaleName(string locale)
    {
        return LocaleNameFormatter?.Invoke(locale) ?? locale.ToUpperInvariant();
    }

    private string GetLocaleButtonClass(string locale)
    {
        return locale == CurrentLocale
            ? "osirion-locale-button osirion-active"
            : "osirion-locale-button";
    }

    private async Task SwitchLocale(string locale)
    {
        if (locale != CurrentLocale)
        {
            CurrentLocale = locale;
            await OnLocaleChanged.InvokeAsync(locale);
            await LoadNavigationAsync();
        }
    }

    private async Task SelectDirectory(DirectoryItem directory)
    {
        await OnDirectorySelected.InvokeAsync(directory);
    }

    private async Task SelectContent(ContentItem item)
    {
        await OnContentSelected.InvokeAsync(item);
    }

    private RenderFragment RenderDirectories(IEnumerable<DirectoryItem> directories) => builder =>
    {
        foreach (var directory in directories.OrderBy(d => d.Order))
        {
            builder.OpenElement(0, "li");
            builder.AddAttribute(1, "class", "osirion-nav-item");

            // Directory link
            builder.OpenElement(2, "a");
            builder.AddAttribute(3, "href", GetDirectoryUrl(directory));
            builder.AddAttribute(4, "class", "osirion-nav-link");
            builder.AddAttribute(5, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => SelectDirectory(directory)));
            builder.AddAttribute(6, "onclick:preventDefault", true);

            builder.AddContent(7, directory.Name);

            builder.CloseElement(); // a

            // Content items and subdirectories
            bool shouldExpand = ExpandAllDirectories || directory.Id == ExpandedDirectoryId;
            if (shouldExpand && (directory.Children.Any() || (ShowContentItems && directory.Items.Any())))
            {
                builder.OpenElement(8, "ul");
                builder.AddAttribute(9, "class", "osirion-nav-sublist");

                // Show content items
                if (ShowContentItems)
                {
                    foreach (var item in directory.Items.OrderBy(i => i.Title))
                    {
                        builder.OpenElement(10, "li");
                        builder.AddAttribute(11, "class", "osirion-nav-content-item");

                        builder.OpenElement(12, "a");
                        builder.AddAttribute(13, "href", GetContentUrl(item));
                        builder.AddAttribute(14, "class", "osirion-nav-content-link");
                        builder.AddAttribute(15, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => SelectContent(item)));
                        builder.AddAttribute(16, "onclick:preventDefault", true);

                        builder.AddContent(17, item.Title);

                        builder.CloseElement(); // a

                        builder.CloseElement(); // li
                    }
                }

                // Render subdirectories recursively
                if (directory.Children.Any())
                {
                    builder.AddContent(18, RenderDirectories(directory.Children));
                }

                builder.CloseElement(); // ul
            }

            builder.CloseElement(); // li
        }
    };
}
