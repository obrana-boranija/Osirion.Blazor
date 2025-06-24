using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Components;

/// <summary>
/// DocumentPage component for displaying documentation with side navigation
/// </summary>
public partial class DocumentPage
{
    [Inject]
    private IContentProviderManager ContentProviderManager { get; set; } = default!;

    /// <summary>
    /// Gets or sets the path to the document content
    /// </summary>
    [Parameter]
    public string? ContentPath { get; set; }

    /// <summary>
    /// Gets or sets the content item to display
    /// </summary>
    [Parameter]
    public ContentItem? Content { get; set; }

    /// <summary>
    /// Gets or sets the previous content item for navigation
    /// </summary>
    [Parameter]
    public ContentItem? PreviousItem { get; set; }

    /// <summary>
    /// Gets or sets the next content item for navigation
    /// </summary>
    [Parameter]
    public ContentItem? NextItem { get; set; }

    /// <summary>
    /// Gets or sets whether to show side navigation
    /// </summary>
    [Parameter]
    public bool ShowSideNavigation { get; set; } = true;

    /// <summary>
    /// Gets or sets the navigation title
    /// </summary>
    [Parameter]
    public string? NavigationTitle { get; set; }

    /// <summary>
    /// Gets or sets the navigation items
    /// </summary>
    [Parameter]
    public IReadOnlyList<DocumentNavigationItem>? NavigationItems { get; set; }

    /// <summary>
    /// Gets or sets the current document path for navigation highlighting
    /// </summary>
    [Parameter]
    public string? CurrentPath { get; set; }

    /// <summary>
    /// Gets or sets whether to show breadcrumbs
    /// </summary>
    [Parameter]
    public bool ShowBreadcrumbs { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show the home link in breadcrumbs
    /// </summary>
    [Parameter]
    public bool ShowBreadcrumbHome { get; set; } = true;

    /// <summary>
    /// Gets or sets the home link URL in breadcrumbs
    /// </summary>
    [Parameter]
    public string BreadcrumbHomeUrl { get; set; } = "/";

    /// <summary>
    /// Gets or sets the home link text in breadcrumbs
    /// </summary>
    [Parameter]
    public string BreadcrumbHomeText { get; set; } = "Home";

    /// <summary>
    /// Gets or sets whether to show metadata
    /// </summary>
    [Parameter]
    public bool ShowMetadata { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show table of contents
    /// </summary>
    [Parameter]
    public bool ShowTableOfContents { get; set; } = true;

    /// <summary>
    /// Gets or sets the table of contents title
    /// </summary>
    [Parameter]
    public string TableOfContentsTitle { get; set; } = "On this page";

    /// <summary>
    /// Gets or sets the minimum heading level for table of contents
    /// </summary>
    [Parameter]
    public int TocMinLevel { get; set; } = 2;

    /// <summary>
    /// Gets or sets the maximum heading level for table of contents
    /// </summary>
    [Parameter]
    public int TocMaxLevel { get; set; } = 4;

    /// <summary>
    /// Gets or sets whether to show navigation links
    /// </summary>
    [Parameter]
    public bool ShowNavigationLinks { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show a back link when not found
    /// </summary>
    [Parameter]
    public bool ShowBackLink { get; set; } = true;

    /// <summary>
    /// Gets or sets the back link URL
    /// </summary>
    [Parameter]
    public string BackLinkUrl { get; set; } = "/";

    /// <summary>
    /// Gets or sets the back link text
    /// </summary>
    [Parameter]
    public string BackLinkText { get; set; } = "Back to Documentation";

    /// <summary>
    /// Gets or sets the schema.org type
    /// </summary>
    [Parameter]
    public string SchemaType { get; set; } = "TechArticle";

    /// <summary>
    /// Gets or sets custom footer content
    /// </summary>
    [Parameter]
    public string? CustomFooterContent { get; set; }

    /// <summary>
    /// Gets or sets the directory URL formatter
    /// </summary>
    [Parameter]
    public Func<DirectoryItem, string>? DirectoryUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets the content URL formatter
    /// </summary>
    [Parameter]
    public Func<ContentItem, string>? ContentUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets the navigation URL formatter
    /// </summary>
    [Parameter]
    public Func<DocumentNavigationItem, string>? NavigationUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets the loading text
    /// </summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading document...";

    /// <summary>
    /// Gets or sets the not found title
    /// </summary>
    [Parameter]
    public string NotFoundTitle { get; set; } = "Document Not Found";

    /// <summary>
    /// Gets or sets the not found text
    /// </summary>
    [Parameter]
    public string NotFoundText { get; set; } = "The requested document could not be found.";

    /// <summary>
    /// Gets or sets whether the component is loading
    /// </summary>
    private bool IsLoading { get; set; } = true;

    /// <inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        if (Content is null && !string.IsNullOrWhiteSpace(ContentPath))
        {
            await LoadContentAsync();
        }
        else if (Content is not null)
        {
            await LoadNavigationItemsAsync();
            IsLoading = false;
        }
        else
        {
            IsLoading = false;
        }

        // Set current path for navigation highlighting
        if (string.IsNullOrWhiteSpace(CurrentPath) && Content is not null)
        {
            CurrentPath = Content.Path;
        }
    }

    private async Task LoadContentAsync()
    {
        IsLoading = true;
        try
        {
            var provider = ContentProviderManager.GetDefaultProvider();
            if (provider is not null)
            {
                Content = await provider.GetItemByPathAsync(ContentPath);

                if (Content is not null)
                {
                    CurrentPath = Content.Path;
                    await LoadNavigationItemsAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading document content: {ex.Message}");
            Content = null;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadNavigationItemsAsync()
    {
        if (!ShowNavigationLinks || Content is null || (PreviousItem is not null && NextItem is not null))
            return;

        try
        {
            var provider = ContentProviderManager.GetDefaultProvider();
            if (provider is not null)
            {
                var query = new ContentQuery
                {
                    Directory = Content.Directory?.Path,
                    SortBy = SortField.Order,
                    SortDirection = SortDirection.Ascending
                };

                var items = await provider.GetItemsByQueryAsync(query);

                int currentIndex = -1;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Path == Content.Path)
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
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading navigation items: {ex.Message}");
        }
    }

    private string GetDocumentPageClass()
    {
        var classes = new List<string> { "osirion-document-page" };

        if (ShowSideNavigation)
        {
            classes.Add("osirion-document-with-sidenav");
        }

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        return string.Join(" ", classes);
    }

    private string GetNavigationUrl(DocumentNavigationItem item)
    {
        return NavigationUrlFormatter?.Invoke(item) ?? item.Url;
    }

    private string GetNavigationLinkClass(DocumentNavigationItem item)
    {
        var isActive = IsCurrentPage(item);
        var classes = new List<string> { "osirion-sidenav-link" };

        if (isActive)
        {
            classes.Add("osirion-sidenav-active");
        }

        return string.Join(" ", classes);
    }

    private bool IsCurrentPage(DocumentNavigationItem item)
    {
        if (string.IsNullOrWhiteSpace(CurrentPath))
            return false;

        return item.Path?.Equals(CurrentPath, StringComparison.OrdinalIgnoreCase) == true ||
               item.Url?.TrimStart('/').Equals(CurrentPath.TrimStart('/'), StringComparison.OrdinalIgnoreCase) == true;
    }

    private bool IsParentOfCurrentPage(DocumentNavigationItem item)
    {
        if (string.IsNullOrWhiteSpace(CurrentPath) || item.Children is null)
            return false;

        return item.Children.Any(child => IsCurrentPage(child));
    }

    private string GetContentUrl(ContentItem item)
    {
        return ContentUrlFormatter?.Invoke(item) ?? $"/{item.Path}";
    }
}

/// <summary>
/// Represents a navigation item in the document navigation
/// </summary>
public class DocumentNavigationItem
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Path { get; set; }
    public int Order { get; set; }
    public bool IsExpanded { get; set; }
    public IReadOnlyList<DocumentNavigationItem>? Children { get; set; }
}