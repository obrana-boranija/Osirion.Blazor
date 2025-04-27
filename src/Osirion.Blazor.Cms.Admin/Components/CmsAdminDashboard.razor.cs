using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Services;
using Osirion.Blazor.Cms.Core.Providers.Interfaces;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Infrastructure.GitHub.Models;

namespace Osirion.Blazor.Cms.Admin.Components;

public partial class CmsAdminDashboard(IGitHubAdminService gitHubService, CmsAdminState adminState)
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string Theme { get; set; } = "light";

    private bool IsEditing => adminState.IsEditing;
    private bool IsViewingContent => adminState.SelectedRepository != null && adminState.SelectedBranch != null;

    protected override void OnInitialized()
    {
        adminState.StateChanged += StateHasChanged;
    }

    public void Dispose()
    {
        adminState.StateChanged -= StateHasChanged;
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
        var newPost = new BlogPost
        {
            Metadata = FrontMatter.Create("", "Enter description here", DateTime.Now),
            Content = "## New Post\n\nStart writing your content here...",
            FilePath = string.IsNullOrEmpty(adminState.CurrentPath) ?
                "new-post.md" :
                $"{adminState.CurrentPath}/new-post.md"
        };

        adminState.SetEditingPost(newPost, true);
    }

    private async Task HandleSaveComplete(BlogPost post)
    {
        // Reload the directory contents
        if (adminState.SelectedRepository != null && adminState.SelectedBranch != null)
        {
            try
            {
                var contents = await gitHubService.GetRepositoryContentsAsync(adminState.CurrentPath);
                adminState.SetCurrentPath(adminState.CurrentPath, contents);
            }
            catch (Exception ex)
            {
                adminState.SetErrorMessage($"Failed to refresh directory: {ex.Message}");
            }
        }

        // Clear editing state
        adminState.ClearEditing();
    }

    private void HandleDiscardChanges()
    {
        adminState.ClearEditing();
    }

    private void ClearMessages()
    {
        adminState.ClearMessages();
    }

    private string GetAdminDashboardClass()
    {
        return $"osirion-admin-dashboard osirion-admin-theme-{Theme} {CssClass}".Trim();
    }
}
