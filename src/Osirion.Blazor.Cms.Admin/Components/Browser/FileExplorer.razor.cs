using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Services;
using Osirion.Blazor.Cms.Core.Models;
using Osirion.Blazor.Cms.Core.Providers.Interfaces;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Infrastructure.GitHub.Models;

namespace Osirion.Blazor.Cms.Admin.Components.Browser;

public partial class FileExplorer(IGitHubAdminService gitHubService, CmsAdminState adminState, NavigationManager navigationManager)
{
    [Parameter]
    public string Title { get; set; } = "Files";

    [Parameter]
    public bool CanCreateFile { get; set; } = true;

    [Parameter]
    public bool CanDeleteFile { get; set; } = true;

    [Parameter]
    public EventCallback<GitHubItem> OnFileSelected { get; set; }

    [Parameter]
    public EventCallback<GitHubItem> OnDirectorySelected { get; set; }

    [Parameter]
    public EventCallback OnCreateFile { get; set; }

    [Parameter]
    public bool NavigateToEditor { get; set; } = true;

    private List<GitHubItem> Contents { get; set; } = new();
    private string CurrentPath => adminState.CurrentPath;
    private bool IsLoading { get; set; }
    private string? ErrorMessage { get; set; }

    private bool IsShowingDeleteConfirmation { get; set; }
    private GitHubItem? FileToDelete { get; set; }
    private bool IsDeletingFile { get; set; }

    private bool IsValidRepositoryAndBranch =>
        adminState.SelectedRepository != null &&
        adminState.SelectedBranch != null;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (IsValidRepositoryAndBranch && Contents.Count == 0)
            {
                await RefreshContents();
                StateHasChanged();
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task OnParametersSetAsync()
    {
        if (IsValidRepositoryAndBranch && Contents.Count == 0 && !IsLoading)
        {
            await RefreshContents();
        }
    }

    public void Dispose()
    {
        adminState.StateChanged -= StateHasChanged;
    }

    private async Task RefreshContents()
    {
        if (!IsValidRepositoryAndBranch)
        {
            return;
        }

        IsLoading = true;
        ErrorMessage = null;

        try
        {
            Contents = await gitHubService.GetRepositoryContentsAsync(CurrentPath);
            adminState.SetCurrentPath(CurrentPath, Contents);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load contents: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task NavigateToRoot()
    {
        adminState.SetCurrentPath(string.Empty, new List<GitHubItem>());
        await RefreshContents();
    }

    private async Task NavigateToPath(string path)
    {
        adminState.SetCurrentPath(path, new List<GitHubItem>());
        await RefreshContents();
    }

    private async Task SelectItem(GitHubItem item)
    {
        if (item.IsDirectory)
        {
            // Navigate to directory
            adminState.SetCurrentPath(item.Path, new List<GitHubItem>());
            await RefreshContents();

            if (OnDirectorySelected.HasDelegate)
            {
                await OnDirectorySelected.InvokeAsync(item);
            }
        }
        else if (item.IsMarkdownFile)
        {
            // For markdown files, navigate to editor
            await EditFile(item);
        }
        else
        {
            // For other file types, just select
            adminState.SelectItem(item);

            if (OnFileSelected.HasDelegate)
            {
                await OnFileSelected.InvokeAsync(item);
            }
        }
    }

    private async Task EditFile(GitHubItem item)
    {
        if (!item.IsFile || !item.IsMarkdownFile)
        {
            return;
        }

        try
        {
            IsLoading = true;
            var blogPost = await gitHubService.GetBlogPostAsync(item.Path);

            // Set file for editing
            adminState.SelectItem(item);
            adminState.SetEditingPost(blogPost);

            if (OnFileSelected.HasDelegate)
            {
                await OnFileSelected.InvokeAsync(item);
            }

            // Navigate to editor if configured to do so
            if (NavigateToEditor)
            {
                navigationManager.NavigateTo($"/admin/content/edit/?path={item.Path}");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load file: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CreateNewFile()
    {
        if (OnCreateFile.HasDelegate)
        {
            await OnCreateFile.InvokeAsync();
        }
        else
        {
            // Create new blog post with empty content
            var newPost = new BlogPost
            {
                Metadata = FrontMatter.Create("", "Enter description here", DateTime.Now),
                Content = "## New Post\n\nStart writing your content here...",
                FilePath = string.IsNullOrEmpty(CurrentPath) ?
                        "new-post.md" :
                        $"{CurrentPath}/new-post.md"
            };

            // Set file for editing
            adminState.SetEditingPost(newPost, true);

            // Navigate to editor
            if (NavigateToEditor)
            {
                navigationManager.NavigateTo("/admin/content/edit");
            }
        }
    }

    private void ShowDeleteConfirmation(GitHubItem item)
    {
        FileToDelete = item;
        IsShowingDeleteConfirmation = true;
    }

    private void CancelDelete()
    {
        FileToDelete = null;
        IsShowingDeleteConfirmation = false;
    }

    private async Task ConfirmDeleteFile()
    {
        if (FileToDelete == null)
        {
            CancelDelete();
            return;
        }

        IsDeletingFile = true;

        try
        {
            await gitHubService.DeleteFileAsync(
                FileToDelete.Path,
                $"Delete {FileToDelete.Name}",
                FileToDelete.Sha);

            // Refresh contents
            await RefreshContents();

            // Close confirmation
            CancelDelete();

            // Show success message
            adminState.SetStatusMessage($"File {FileToDelete.Name ?? string.Empty} deleted successfully");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to delete file: {ex.Message}";
            adminState.SetErrorMessage(ErrorMessage);
        }
        finally
        {
            IsDeletingFile = false;
        }
    }

    private bool IsItemSelected(GitHubItem item)
    {
        return adminState.SelectedItem?.Path == item.Path;
    }

    private string GetFileExplorerClass()
    {
        return $"osirion-admin-file-explorer {CssClass}".Trim();
    }
}
