using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Admin.Components.Pages;

public partial class EditContent : IDisposable
{
    [Inject]
    private IGitHubAdminService GitHubService { get; set; } = default!;

    [SupplyParameterFromQuery]
    public string? Path { get; set; }

    protected bool IsLoading { get; set; }

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
        if (!string.IsNullOrWhiteSpace(Path) &&
           (AdminState.EditingPost is null || AdminState.EditingPost.Path != Path))
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
        if (!string.IsNullOrWhiteSpace(Path))
        {
            // Path parameter takes precedence
            await LoadContentAsync();
        }
        else if (AdminState.SelectedItem is not null && AdminState.SelectedItem.IsMarkdownFile)
        {
            // Redirect to proper route if we have a selected item
            NavigationManager.NavigateTo($"/osirion/content/edit?Path={AdminState.SelectedItem.Path}");
        }
        else if (AdminState.EditingPost is not null && AdminState.IsEditing)
        {
            // We already have a post being edited (likely from content browser)
            // Just ensure we're on the right path
            if (!string.IsNullOrWhiteSpace(AdminState.EditingPost.Path) &&
                !AdminState.IsCreatingNewFile)
            {
                NavigationManager.NavigateTo($"/osirion/content/edit?Path={AdminState.EditingPost.Path}");
            }
        }
    }

    private async Task LoadContentAsync()
    {
        if (string.IsNullOrWhiteSpace(Path))
        {
            return;
        }

        IsLoading = true;
        StateHasChanged(); // Important: Update UI to show loading state

        try
        {
            var blogPost = await GitHubService.GetBlogPostAsync(Path);

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
        var newPost = new ContentItem
        {
            Metadata = FrontMatter.Create("New Post", "Enter description here", DateTime.Now),
            Content = "## New Post\n\nStart writing your content here...",
            Path = string.IsNullOrWhiteSpace(AdminState.CurrentPath) ?
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

    private async Task HandleSaveComplete(ContentItem post)
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
        if (AdminState.EditingPost is null)
        {
            return "Edit Content";
        }

        return AdminState.IsCreatingNewFile
            ? "New Post"
            : $"Edit: {AdminState.EditingPost.Metadata?.Title}";
    }

    private string GetPageSubtitle()
    {
        if (AdminState.EditingPost is null)
        {
            return "Edit your markdown content";
        }

        return AdminState.IsCreatingNewFile
            ? "Create a new markdown file"
            : AdminState.EditingPost.Metadata?.Description ?? "Edit content";
    }
}