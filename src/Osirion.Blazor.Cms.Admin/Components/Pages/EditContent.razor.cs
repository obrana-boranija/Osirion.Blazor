using Microsoft.AspNetCore.Components;
using Octokit;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Admin.Components.Pages;

public partial class EditContent(IGitHubAdminService gitHubService)
{
    [SupplyParameterFromQuery]
    public string? Path { get; set; }

    protected override void OnInitialized()
    {
        // Subscribe to state changes from AdminState
        AdminState.StateChanged += StateHasChanged;
    }

    public void Dispose()
    {
        // Unsubscribe to prevent memory leaks
        AdminState.StateChanged -= StateHasChanged;
    }

    protected override async Task OnParametersSetAsync()
    {
        // Reload content if parameters change and we're already rendered
        if (!string.IsNullOrEmpty(Path) &&
        (AdminState.EditingPost == null || AdminState.EditingPost.FilePath != Path))
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
        else if (AdminState.SelectedItem != null && AdminState.SelectedItem.IsMarkdownFile)
        {
            // Redirect to proper route if we have a selected item
            NavigationManager.NavigateTo($"/osirion/content/edit?Path={AdminState.SelectedItem.Path}");
        }
        else if (AdminState.EditingPost != null && AdminState.IsEditing)
        {
            // We already have a post being edited (likely from content browser)
            // Just ensure we're on the right path
            if (!string.IsNullOrEmpty(AdminState.EditingPost.FilePath) &&
            !AdminState.IsCreatingNewFile)
            {
                NavigationManager.NavigateTo($"/osirion/content/edit?Path={AdminState.EditingPost.FilePath}");
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
            AdminState.SetEditingPost(blogPost);

            // Also publish a content selected event
            EventPublisher.Publish(new ContentSelectedEvent(Path));
        }
        catch (Exception ex)
        {
            AdminState.SetErrorMessage($"Failed to load content: {ex.Message}");
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
            FilePath = string.IsNullOrEmpty(AdminState.CurrentPath) ?
            "new-post.md" :
                $"{AdminState.CurrentPath}/new-post.md"
        };

        AdminState.SetEditingPost(newPost, true);

        // Also publish a create new content event
        EventPublisher.Publish(new CreateNewContentEvent(AdminState.CurrentPath));
    }

    private void GoToContentBrowser()
    {
        NavigationManager.NavigateTo("/osirion/content");
    }

    private async Task HandleSaveComplete(BlogPost post)
    {
        // Navigate to content listing
        NavigationManager.NavigateTo("/osirion/content");
    }

    private void HandleDiscardChanges()
    {
        // Clear editing state in AdminState
        AdminState.ClearEditing();

        // Navigate to content listing
        NavigationManager.NavigateTo("/osirion/content");
    }

    private string GetPageTitle()
    {
        if (AdminState.EditingPost == null)
        {
            return "Edit Content";
        }

        return AdminState.IsCreatingNewFile
        ? "New Post"
            : $"Edit: {AdminState.EditingPost.Metadata?.Title}";
    }

    private string GetPageSubtitle()
    {
        if (AdminState.EditingPost == null)
        {
            return "Edit Content";
        }

        return AdminState.IsCreatingNewFile
        ? "New Description"
            : $"Edit: {AdminState.EditingPost.Metadata?.Description}";
    }
}
