using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Services;
using Osirion.Blazor.Cms.Core.Models;
using Osirion.Blazor.Cms.Models;

namespace Osirion.Blazor.Example.Components.Pages.CmsAdmin;

public partial class EditContent
{
    [SupplyParameterFromQuery]
    public string? Path { get; set; }

    private bool IsLoading { get; set; }

    protected override void OnInitialized()
    {
        // Don't try to load content yet - wait for OnAfterRenderAsync
        // We can still initialize other state that doesn't require JSInterop
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
            // Initialize state persistence if available
            if (AdminState is CmsAdminStatePersistent persistentState)
            {
                await persistentState.InitializeAsync();

                // Now we can load content based on parameters or state
                await LoadContentBasedOnParameters();

                // Force a re-render to reflect the updated state
                StateHasChanged();
            }
            else
            {
                // Even if we don't have the persistent state, still try to load content
                await LoadContentBasedOnParameters();
            }
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
            NavigationManager.NavigateTo($"/admin/content/edit/{AdminState.SelectedItem.Path}");
        }
        else if (AdminState.EditingPost != null && AdminState.IsEditing)
        {
            // We already have a post being edited (likely from content browser)
            // Just ensure we're on the right path
            if (!string.IsNullOrEmpty(AdminState.EditingPost.FilePath) &&
                !AdminState.IsCreatingNewFile)
            {
                NavigationManager.NavigateTo($"/admin/content/edit/{AdminState.EditingPost.FilePath}");
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

        try
        {
            var blogPost = await GitHubService.GetBlogPostAsync(Path);
            AdminState.SetEditingPost(blogPost);
        }
        catch (Exception ex)
        {
            AdminState.SetErrorMessage($"Failed to load content: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void CreateNewPost()
    {
        // Create new blog post with empty content
        var newPost = new BlogPost
        {
            Metadata = new FrontMatter
            {
                Title = "New Post",
                Description = "Enter description here",
                Author = string.Empty,
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                Categories = new List<string>(),
                Tags = new List<string>()
            },
            Content = "## New Post\n\nStart writing your content here...",
            FilePath = string.IsNullOrEmpty(AdminState.CurrentPath) ?
                "new-post.md" :
                $"{AdminState.CurrentPath}/new-post.md"
        };

        AdminState.SetEditingPost(newPost, true);
    }

    private void GoToContentBrowser()
    {
        NavigationManager.NavigateTo("/admin/content");
    }

    private async Task HandleSaveComplete(BlogPost post)
    {
        // Navigate to content listing
        NavigationManager.NavigateTo("/admin/content");
    }

    private void HandleDiscardChanges()
    {
        // Navigate to content listing
        NavigationManager.NavigateTo("/admin/content");
    }

    private string GetPageTitle()
    {
        if (AdminState.EditingPost == null)
        {
            return "Edit Content";
        }

        return AdminState.IsCreatingNewFile
            ? "New Post"
            : $"Edit: {AdminState.EditingPost.Metadata.Title}";
    }
}
