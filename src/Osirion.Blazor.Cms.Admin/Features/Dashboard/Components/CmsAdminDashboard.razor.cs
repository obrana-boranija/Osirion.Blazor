using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Shared.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Admin.Features.Dashboard.Components;

/// <summary>Defines the CmsAdminDashboard type.</summary>
public partial class CmsAdminDashboard
{
    /// <summary>Gets or sets the ChildContent value.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets the Theme value.</summary>
    [Parameter]
    public new string Theme { get; set; } = "light";

    private bool IsEditing => DashboardState.IsEditing;
    private bool IsViewingContent => DashboardState.SelectedRepository is not null && DashboardState.SelectedBranch is not null;

    /// <summary>Initializes the component state and required services.</summary>
    protected override void OnInitialized()
    {
        DashboardState.StateChanged += StateHasChanged;
    }

    /// <summary>Releases resources held by the component or service.</summary>
    public void Dispose()
    {
        DashboardState.StateChanged -= StateHasChanged;
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
            Path = string.IsNullOrWhiteSpace(DashboardState.CurrentPath) ?
                "new-post.md" :
                $"{DashboardState.CurrentPath}/new-post.md"
        };

        DashboardState.SetEditingPost(newPost, true);
    }

    private async Task HandleSaveComplete(ContentItem post)
    {
        // Reload the directory contents
        if (DashboardState.SelectedRepository is not null && DashboardState.SelectedBranch is not null)
        {
            try
            {
                var contents = await GitHubService.GetRepositoryContentsAsync(DashboardState.CurrentPath);
                DashboardState.SetCurrentPath(DashboardState.CurrentPath, contents);
            }
            catch (Exception ex)
            {
                DashboardState.SetErrorMessage($"Failed to refresh directory: {ex.Message}");
            }
        }

        // Clear editing state
        DashboardState.ClearEditing();
    }

    private void HandleDiscardChanges()
    {
        DashboardState.ClearEditing();
    }

    private void ClearMessages()
    {
        DashboardState.ClearMessages();
    }

    private string GetAdminDashboardClass()
    {
        return $"osirion-admin-dashboard osirion-admin-theme-{Theme} {Class}".Trim();
    }
}
