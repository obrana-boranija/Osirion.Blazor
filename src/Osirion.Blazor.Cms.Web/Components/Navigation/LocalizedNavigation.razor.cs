using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.Web;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>Displays localized directory and content navigation.</summary>
public partial class LocalizedNavigation
{
    [Inject]
    private ILogger<LocalizedNavigation> Logger { get; set; } = default!;

    [Inject] private IContentProviderManager ContentProviderManager { get; set; } = default!;

    /// <summary>Gets or sets the heading above the tree, rendered as an <c>h3</c>. No heading when <c>null</c>.</summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>Gets or sets the text shown while the tree loads.</summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading navigation...";

    /// <summary>
    /// Gets or sets the text shown when there are no directories, when no default content provider is registered or
    /// when loading failed.
    /// </summary>
    [Parameter]
    public string NoContentText { get; set; } = "No navigation available.";

    /// <summary>
    /// Gets or sets the locale whose directories are shown: the top-level directories of that locale, directly below
    /// the locale folder. Their subdirectories are reached through each directory's children. When <c>null</c>, or
    /// when <see cref="EnableLocalization"/> is <c>false</c>, the root directories are shown, which are the locale
    /// folders when the content sits in them.
    /// </summary>
    [Parameter]
    public string? CurrentLocale { get; set; }

    /// <summary>
    /// Gets or sets the callback run when the locale changes. Not used in 4.0: the locale buttons never render,
    /// because the component does not load the available locales.
    /// </summary>
    [Parameter]
    public EventCallback<string> OnLocaleChanged { get; set; }

    /// <summary>
    /// Gets or sets the callback run with the directory when its link is clicked in an interactive render mode. When
    /// it is set, the click no longer navigates. Under static SSR it is not called.
    /// </summary>
    [Parameter]
    public EventCallback<DirectoryItem> OnDirectorySelected { get; set; }

    /// <summary>
    /// Gets or sets the callback run with the content item when its link is clicked in an interactive render mode.
    /// When it is set, the click no longer navigates. Under static SSR it is not called.
    /// </summary>
    [Parameter]
    public EventCallback<ContentItem> OnContentSelected { get; set; }

    /// <summary>Gets or sets whether every directory is expanded, which shows the whole tree.</summary>
    [Parameter]
    public bool ExpandAllDirectories { get; set; } = false;

    /// <summary>
    /// Gets or sets the <c>Id</c> of the directory to expand when <see cref="ExpandAllDirectories"/> is
    /// <c>false</c>.
    /// </summary>
    [Parameter]
    public string? ExpandedDirectoryId { get; set; }

    /// <summary>
    /// Gets or sets whether the content items of an expanded directory are listed, sorted by title, before its
    /// subdirectories.
    /// </summary>
    [Parameter]
    public bool ShowContentItems { get; set; } = true;

    /// <summary>
    /// Gets or sets a function that builds each directory link. Without it, a link goes to the address the
    /// directory's <c>_index.md</c> declares in <c>permalink</c>, otherwise to the directory's path without the locale
    /// folder.
    /// </summary>
    [Parameter]
    public Func<DirectoryItem, string>? DirectoryUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets a function that builds each content link. Without it, a link goes to the item's URL from the site
    /// root, the address its page is served at.
    /// </summary>
    [Parameter]
    public Func<ContentItem, string>? ContentUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets a function that turns a locale code into the text of its button. Not used in 4.0: the locale
    /// buttons never render.
    /// </summary>
    [Parameter]
    public Func<string, string>? LocaleNameFormatter { get; set; }

    /// <summary>
    /// Gets or sets whether <see cref="CurrentLocale"/> is used. When <c>false</c>, the root directories are loaded.
    /// </summary>
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
                var directories = EnableLocalization
                    ? await provider.GetDirectoriesAsync(CurrentLocale)
                    : await provider.GetDirectoriesAsync();
                Directories = TopLevelOnly(directories);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load navigation directories");
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

    // A locale's list holds its directories at every depth, and each one is also reachable through its parent's
    // Children, so keeping the nested ones would show them twice.
    private static IReadOnlyList<DirectoryItem> TopLevelOnly(IReadOnlyList<DirectoryItem> directories)
    {
        var listed = directories.ToHashSet();
        return directories.Where(directory => directory.Parent is null || !listed.Contains(directory.Parent)).ToList();
    }

    private static bool IsIndexPage(ContentItem item) =>
        string.Equals(System.IO.Path.GetFileName(item.Path), "_index.md", StringComparison.OrdinalIgnoreCase);

    private string GetDirectoryUrl(DirectoryItem directory)
    {
        return DirectoryUrlFormatter?.Invoke(directory) ?? ContentUrls.DirectoryUrl(directory);
    }

    private string GetContentUrl(ContentItem item)
    {
        return ContentUrls.ItemUrl(item, ContentUrlFormatter);
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
            if (OnDirectorySelected.HasDelegate)
            {
                builder.AddAttribute(5, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => SelectDirectory(directory)));
                builder.AddAttribute(6, "onclick:preventDefault", true);
            }

            builder.AddContent(7, directory.Name);

            builder.CloseElement(); // a

            // Content items and subdirectories
            bool shouldExpand = ExpandAllDirectories || directory.Id == ExpandedDirectoryId;
            if (shouldExpand && (directory.Children.Any() || (ShowContentItems && directory.Items.Any(i => !IsIndexPage(i)))))
            {
                builder.OpenElement(8, "ul");
                builder.AddAttribute(9, "class", "osirion-nav-sublist");

                // Show content items
                if (ShowContentItems)
                {
                    foreach (var item in directory.Items.Where(i => !IsIndexPage(i)).OrderBy(i => i.Title))
                    {
                        builder.OpenElement(10, "li");
                        builder.AddAttribute(11, "class", "osirion-nav-content-item");

                        builder.OpenElement(12, "a");
                        builder.AddAttribute(13, "href", GetContentUrl(item));
                        builder.AddAttribute(14, "class", "osirion-nav-content-link");
                        if (OnContentSelected.HasDelegate)
                        {
                            builder.AddAttribute(15, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => SelectContent(item)));
                            builder.AddAttribute(16, "onclick:preventDefault", true);
                        }

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
