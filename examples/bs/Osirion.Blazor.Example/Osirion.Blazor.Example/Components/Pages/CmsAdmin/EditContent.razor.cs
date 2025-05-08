using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Example.Components.Pages.CmsAdmin;

public partial class EditContent(NavigationManager navigationManager) : OsirionComponentBase, IDisposable
{
    [Inject]
    private CmsState AdminState { get; set; } = default!;

    [Inject]
    private IGitHubAdminService GitHubService { get; set; } = default!;

    [Inject]
    private IEventPublisher EventPublisher { get; set; } = default!;

    [SupplyParameterFromQuery]
    public string? Path { get; set; }

    private bool IsLoading { get; set; }

    protected override void OnInitialized()
    {
        // Subscribe to state changes to trigger re-render
        AdminState.StateChanged += StateHasChanged;
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
            navigationManager.NavigateTo($"/admin/content/edit/{AdminState.SelectedItem.Path}");
        }
        else if (AdminState.EditingPost != null && AdminState.IsEditing)
        {
            // We already have a post being edited (likely from content browser)
            // Just ensure we're on the right path
            if (!string.IsNullOrEmpty(AdminState.EditingPost.FilePath) &&
                !AdminState.IsCreatingNewFile)
            {
                navigationManager.NavigateTo($"/admin/content/edit/{AdminState.EditingPost.FilePath}");
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
        StateHasChanged();

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

        // Update state
        AdminState.SetEditingPost(newPost, true);

        // Publish event
        EventPublisher.Publish(new CreateNewContentEvent(AdminState.CurrentPath));
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
        // Navigate to content listing
        navigationManager.NavigateTo("/admin/content");
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
            return "Create or edit content";
        }

        if (AdminState.IsCreatingNewFile)
        {
            return "Create a new post";
        }

        // Get the file path relative to the repository root
        var filePath = AdminState.EditingPost.FilePath;
        return $"Editing: {filePath}";
    }

    public void Dispose()
    {
        // Unsubscribe from state changes when component is disposed
        AdminState.StateChanged -= StateHasChanged;
    }
}