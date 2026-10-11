using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Common.Constants;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Admin.Components.Pages;

    /// <summary>Defines the public member type.</summary>
public partial class EditContent : IDisposable
{
    [Inject]
    private IGitHubAdminService GitHubService { get; set; } = default!;

    [Inject]
    private ILogger<EditContent> Logger { get; set; } = default!;

    private string? _loadedPath;

    /// <summary>Gets or sets the Path value.</summary>
    [SupplyParameterFromQuery]
    public string? Path { get; set; }

    /// <summary>Gets or sets the IsLoading value.</summary>
    protected new bool IsLoading { get; set; }

    /// <summary>Initializes the component state and required services.</summary>
    protected override void OnInitialized()
    {
        // Subscribe to state changes from AdminState
        AdminState.StateChanged += StateHasChanged;
    }

    /// <summary>Releases resources held by the component or service.</summary>
    public void Dispose()
    {
        // Unsubscribe to prevent memory leaks
        AdminState.StateChanged -= StateHasChanged;
    }

    /// <summary>Loads the file once whenever <see cref="Path"/> changes.</summary>
    protected override async Task OnParametersSetAsync()
    {
        if (string.IsNullOrWhiteSpace(Path) || string.Equals(Path, _loadedPath, StringComparison.Ordinal))
        {
            return;
        }

        _loadedPath = Path;

        // The content browser may already hold this file in the shared state; do not fetch it again.
        if (AdminState.EditingPost is not null && AdminState.EditingPost.Path == Path)
        {
            return;
        }

        await LoadContentAsync();
    }

    /// <summary>Handles the cases without a Path parameter on the first render.</summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            RedirectToSelectionWhenNoPath();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Without a Path parameter, follows the item selected in the content browser or the post being edited.
    /// </summary>
    private void RedirectToSelectionWhenNoPath()
    {
        if (!string.IsNullOrWhiteSpace(Path))
        {
            return;
        }

        if (AdminState.SelectedItem is not null && AdminState.SelectedItem.IsMarkdownFile)
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

            // Set the post in AdminState (this triggers StateChanged event). The mounted editor picks it up
            // from there; publishing ContentSelectedEvent as well would make every subscriber fetch the file again.
            AdminState.SetEditingPost(blogPost);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Loading content {Path} failed", Path);
            AdminState.SetErrorMessage(UserMessages.LoadFailed);
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
