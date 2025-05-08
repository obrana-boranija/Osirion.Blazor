using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Example.Components.Pages.CmsAdmin;

public partial class ContentBrowser(NavigationManager navigationManager) : OsirionComponentBase, IDisposable
{
    [Inject]
    private CmsState AdminState { get; set; } = default!;

    [Inject]
    private IEventPublisher EventPublisher { get; set; } = default!;

    [Inject]
    private IGitHubAdminService GitHubService { get; set; } = default!;

    [Inject]
    private ContentBrowserService BrowserService { get; set; } = default!;

    // State properties
    private string SearchQuery { get; set; } = string.Empty;
    private bool IsSearchActive { get; set; }
    private List<GitHubItem>? SearchResults { get; set; }
    private bool IsSearching { get; set; }

    /// <summary>
    /// Checks if both repository and branch are configured
    /// </summary>
    private bool IsRepositoryConfigured =>
        AdminState.SelectedRepository != null && AdminState.SelectedBranch != null;

    protected override void OnInitialized()
    {
        // Subscribe to state changes to trigger re-render
        AdminState.StateChanged += StateHasChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // If state persistence is available, it might have loaded a repository
            // and branch already, so we should check and load content if needed
            if (IsRepositoryConfigured && AdminState.CurrentItems.Count == 0)
            {
                await RefreshContentAsync();
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Creates a new post and navigates to edit screen
    /// </summary>
    private void CreateNewPost()
    {
        if (!IsRepositoryConfigured)
        {
            // Cannot create without repository and branch
            return;
        }

        // Create new blog post with empty content
        var newPost = new BlogPost
        {
            Metadata = FrontMatter.Create("New Post", "Enter description here", DateTime.Now),
            Content = "## New Post\n\nStart writing your content here...",
            FilePath = string.IsNullOrEmpty(AdminState.CurrentPath) ?
                "new-post.md" :
                $"{AdminState.CurrentPath}/new-post.md"
        };

        // Update state and publish event
        AdminState.SetEditingPost(newPost, true);
        EventPublisher.Publish(new CreateNewContentEvent(AdminState.CurrentPath));

        // Navigate to edit page
        navigationManager.NavigateTo("/admin/content/edit");
    }

    /// <summary>
    /// Handles file selection - navigates to edit page for MD files
    /// </summary>
    private async Task HandleFileSelected(GitHubItem item)
    {
        if (item.IsMarkdownFile)
        {
            try
            {
                // Load the blog post
                var blogPost = await GitHubService.GetBlogPostAsync(item.Path);

                // Navigate to edit page
                navigationManager.NavigateTo($"/admin/content/edit/{item.Path}");
            }
            catch (Exception ex)
            {
                AdminState.SetErrorMessage($"Failed to load file: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Refreshes the content in the current path
    /// </summary>
    private async Task RefreshContentAsync()
    {
        try
        {
            if (!IsRepositoryConfigured)
                return;

            var contents = await BrowserService.GetContentsAsync(AdminState.CurrentPath);
            AdminState.SetCurrentPath(AdminState.CurrentPath, contents);
        }
        catch (Exception ex)
        {
            AdminState.SetErrorMessage($"Failed to load content: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles key presses in the search input
    /// </summary>
    private async Task HandleSearchKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await SearchFiles();
        }
        else if (e.Key == "Escape")
        {
            ClearSearch();
        }
    }

    /// <summary>
    /// Searches for files matching the query
    /// </summary>
    private async Task SearchFiles()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery) || !IsRepositoryConfigured)
            return;

        IsSearching = true;
        SearchResults = null;
        IsSearchActive = true;
        StateHasChanged();

        try
        {
            // Search for files matching the query
            SearchResults = await BrowserService.SearchFilesAsync(SearchQuery);
        }
        catch (Exception ex)
        {
            AdminState.SetErrorMessage($"Search failed: {ex.Message}");
        }
        finally
        {
            IsSearching = false;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Clears the search and returns to normal browsing
    /// </summary>
    private void ClearSearch()
    {
        SearchQuery = string.Empty;
        SearchResults = null;
        IsSearchActive = false;
        StateHasChanged();

        // Return to current browsing
        RefreshContentAsync();
    }

    public void Dispose()
    {
        // Unsubscribe from state changes when component is disposed
        AdminState.StateChanged -= StateHasChanged;
    }
}