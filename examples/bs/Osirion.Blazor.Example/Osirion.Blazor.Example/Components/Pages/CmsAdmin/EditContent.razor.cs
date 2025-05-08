using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Example.Components.Pages.CmsAdmin;

public partial class EditContent(NavigationManager navigationManager, CmsState adminState, IGitHubAdminService gitHubService, IEventPublisher eventPublisher)
{
    [SupplyParameterFromQuery]
    public string? Path { get; set; }

    private bool IsLoading { get; set; }

    protected override void OnInitialized()
    {
        // Subscribe to state changes from AdminState
        adminState.StateChanged += StateHasChanged;
    }

    public void Dispose()
    {
        // Unsubscribe to prevent memory leaks
        adminState.StateChanged -= StateHasChanged;
    }

    protected override async Task OnParametersSetAsync()
    {
        // Reload content if parameters change and we're already rendered
        if (!string.IsNullOrEmpty(Path) &&
        (adminState.EditingPost == null || adminState.EditingPost.FilePath != Path))
        {
            await LoadContentAsync();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Load content based on parameters or state on first render
            await LoadContentBasedOnParameters();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Loads content based on the page parameters and state
    /// </summary>
    private async Task LoadContentBasedOnParameters()
    {
        if (!string.IsNullOrEmpty(Path))
        {
            // Path parameter takes precedence
            await LoadContentAsync();
        }
        else if (adminState.SelectedItem != null && adminState.SelectedItem.IsMarkdownFile)
        {
            // Redirect to proper route if we have a selected item
            navigationManager.NavigateTo($"/admin/content/edit?Path={adminState.SelectedItem.Path}");
        }
        else if (adminState.EditingPost != null && adminState.IsEditing)
        {
            // We already have a post being edited (likely from content browser)
            // Just ensure we're on the right path
            if (!string.IsNullOrEmpty(adminState.EditingPost.FilePath) &&
                !adminState.IsCreatingNewFile)
            {
                navigationManager.NavigateTo($"/admin/content/edit?Path={adminState.EditingPost.FilePath}");
            }
        }
    }

    private async Task LoadContentAsync()
    {
        if (string.IsNullOrEmpty(Path))
        {
            return;
        }

        IsLoading = true;
        StateHasChanged(); // Important: Update UI to show loading state

        try
        {
            var blogPost = await gitHubService.GetBlogPostAsync(Path);

            // Set the post in AdminState (this triggers StateChanged event)
            adminState.SetEditingPost(blogPost);

            // Also publish a content selected event
            eventPublisher.Publish(new ContentSelectedEvent(Path));
        }
        catch (Exception ex)
        {
            adminState.SetErrorMessage($"Failed to load content: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private void CreateNewPost()
    {
        // Create new blog post with empty content
        var newPost = new BlogPost
        {
            Metadata = FrontMatter.Create("New Post", "Enter description here", DateTime.Now),
            Content = "## New Post\n\nStart writing your content here...",
            FilePath = string.IsNullOrEmpty(adminState.CurrentPath) ?
                "new-post.md" :
                $"{adminState.CurrentPath}/new-post.md"
        };

        adminState.SetEditingPost(newPost, true);

        // Also publish a create new content event
        eventPublisher.Publish(new CreateNewContentEvent(adminState.CurrentPath));
    }

    private void GoToContentBrowser()
    {
        navigationManager.NavigateTo("/admin/content");
    }

    private async Task HandleSaveComplete(BlogPost post)
    {
        // Navigate to content listing
        navigationManager.NavigateTo("/admin/content");
    }

    private void HandleDiscardChanges()
    {
        // Clear editing state in AdminState
        adminState.ClearEditing();

        // Navigate to content listing
        navigationManager.NavigateTo("/admin/content");
    }

    private string GetPageTitle()
    {
        if (adminState.EditingPost == null)
        {
            return "Edit Content";
        }

        return adminState.IsCreatingNewFile
            ? "New Post"
            : $"Edit: {adminState.EditingPost.Metadata?.Title}";
    }

    private string GetPageSubtitle()
    {
        if (adminState.EditingPost == null)
        {
            return "Edit Content";
        }

        return adminState.IsCreatingNewFile
            ? "New Description"
            : $"Edit: {adminState.EditingPost.Metadata?.Description}";
    }
}