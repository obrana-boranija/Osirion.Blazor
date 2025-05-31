using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Shared.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Admin.Features.Dashboard.Components;

public partial class CmsAdminDashboard
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string Theme { get; set; } = "light";

    private bool IsEditing => AdminState.IsEditing;
    private bool IsViewingContent => AdminState.SelectedRepository != null && AdminState.SelectedBranch != null;

    protected override void OnInitialized()
    {
        AdminState.StateChanged += StateHasChanged;
    }

    public void Dispose()
    {
        AdminState.StateChanged -= StateHasChanged;
    }

    private async Task HandleRepositoryChange(GitHubRepository repository)
    {
        // Nothing additional to do here, AdminState already updated
    }

    private async Task HandleBranchChange(GitHubBranch branch)
    {
        // Nothing additional to do here, AdminState already updated
    }

    private void HandleFileSelected(GitHubItem item)
    {
        // File already selected in AdminState
    }

    private void HandleCreateFile()
    {
        // Create new blog post with empty content
        var newPost = new ContentItem
        {
            Metadata = FrontMatter.Create("", "Enter description here", DateTime.Now),
            Content = "## New Post\n\nStart writing your content here...",
            Path = string.IsNullOrEmpty(AdminState.CurrentPath) ?
                "new-post.md" :
                $"{AdminState.CurrentPath}/new-post.md"
        };

        AdminState.SetEditingPost(newPost, true);
    }

    private async Task HandleSaveComplete(ContentItem post)
    {
        // Reload the directory contents
        if (AdminState.SelectedRepository != null && AdminState.SelectedBranch != null)
        {
            try
            {
                var contents = await GitHubService.GetRepositoryContentsAsync(AdminState.CurrentPath);
                AdminState.SetCurrentPath(AdminState.CurrentPath, contents);
            }
            catch (Exception ex)
            {
                AdminState.SetErrorMessage($"Failed to refresh directory: {ex.Message}");
            }
        }

        // Clear editing state
        AdminState.ClearEditing();
    }

    private void HandleDiscardChanges()
    {
        AdminState.ClearEditing();
    }

    private void ClearMessages()
    {
        AdminState.ClearMessages();
    }

    private string GetAdminDashboardClass()
    {
        return $"osirion-admin-dashboard osirion-admin-theme-{Theme} {CssClass}".Trim();
    }
}