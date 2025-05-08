using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.ContentBrowser.ViewModels;

public class FileExplorerViewModel
{
    private readonly ContentBrowserService _browserService;
    private readonly CmsState _state;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<FileExplorerViewModel> _logger;
    private readonly CmsEventMediator _eventMediator;

    public List<GitHubItem> Contents => _state.CurrentItems;
    public string CurrentPath => _state.CurrentPath;
    public GitHubItem? SelectedItem { get; private set; }
    public bool IsLoading { get; private set; }
    public string? ErrorMessage { get; private set; }

    public bool IsValidRepositoryAndBranch =>
        _state.SelectedRepository != null && _state.SelectedBranch != null;

    // For delete confirmation
    public bool IsShowingDeleteConfirmation { get; private set; }
    public GitHubItem? FileToDelete { get; private set; }
    public bool IsDeletingFile { get; private set; }

    public event Action? StateChanged;

    public FileExplorerViewModel(
        ContentBrowserService browserService,
        CmsState state,
        NavigationManager navigationManager,
        CmsEventMediator eventMediator,
        ILogger<FileExplorerViewModel> logger)
    {
        _browserService = browserService;
        _state = state;
        _navigationManager = navigationManager;
        _eventMediator = eventMediator;
        _logger = logger;

        _state.StateChanged += OnStateChanged;
    }

    private void OnStateChanged()
    {
        NotifyStateChanged();
    }

    public async Task LoadContentsAsync()
    {
        if (!IsValidRepositoryAndBranch)
            return;

        IsLoading = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            _logger.LogInformation("Loading contents for path: {Path}", CurrentPath);
            var contents = await _browserService.GetContentsAsync(CurrentPath);
            _state.SetCurrentPath(CurrentPath, contents);
            _logger.LogInformation("Loaded {Count} items for path: {Path}", contents.Count, CurrentPath);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load contents: {ex.Message}";
            _logger.LogError(ex, "Failed to load contents for path: {Path}", CurrentPath);
            _state.SetErrorMessage(ErrorMessage);
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    public async Task NavigateToPathAsync(string path)
    {
        _state.SetCurrentPath(path, new List<GitHubItem>());
        await LoadContentsAsync();
    }

    public async Task NavigateToRootAsync()
    {
        await NavigateToPathAsync(string.Empty);
    }

    /// <summary>
    /// Navigates to the parent directory of the current path
    /// </summary>
    public async Task NavigateToParentDirectoryAsync()
    {
        if (string.IsNullOrEmpty(CurrentPath))
            return;

        string parentPath = string.Empty;

        // Get the parent directory path
        int lastSlashIndex = CurrentPath.LastIndexOf('/');
        if (lastSlashIndex > 0)
        {
            parentPath = CurrentPath.Substring(0, lastSlashIndex);
        }

        // Navigate to the parent directory
        await NavigateToPathAsync(parentPath);
    }

    /// <summary>
    /// Handles item click - navigates into directories or opens files
    /// </summary>
    public async Task HandleItemClickAsync(GitHubItem item)
    {
        if (item == null)
            return;

        SelectItem(item);

        if (item.IsDirectory)
        {
            // Navigate into the directory
            await NavigateToPathAsync(item.Path);
        }
        else if (item.IsFile)
        {
            await OpenFileAsync(item);
        }
    }

    /// <summary>
    /// Opens a file for editing
    /// </summary>
    public async Task OpenFileAsync(GitHubItem item)
    {
        if (item == null || !item.IsFile)
            return;

        IsLoading = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            _logger.LogInformation("Opening file: {Path}", item.Path);

            // For markdown files, open in editor
            if (item.IsMarkdownFile)
            {
                // Publish content selected event to open it in the editor
                _eventMediator.Publish(new ContentSelectedEvent(item.Path));

                // Navigate directly to the edit page with the path
                _navigationManager.NavigateTo($"/admin/content/edit?Path={item.Path}");

                _logger.LogInformation("Markdown file opened in editor: {Path}", item.Path);
            }
            else
            {
                // For other files, we could implement different handling
                // For now, just select the item
                _logger.LogInformation("Non-markdown file selected: {Path}", item.Path);
                _eventMediator.Publish(new StatusNotificationEvent($"Selected file: {item.Name}", StatusType.Info));
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to open file: {ex.Message}";
            _logger.LogError(ex, "Failed to open file: {Path}", item.Path);
            _state.SetErrorMessage(ErrorMessage);
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    public void SelectItem(GitHubItem item)
    {
        SelectedItem = item;
        NotifyStateChanged();
    }

    public bool IsItemSelected(GitHubItem item)
    {
        return SelectedItem?.Path == item.Path;
    }

    public void ShowDeleteConfirmation(GitHubItem item)
    {
        FileToDelete = item;
        IsShowingDeleteConfirmation = true;
        NotifyStateChanged();
    }

    public void CancelDelete()
    {
        FileToDelete = null;
        IsShowingDeleteConfirmation = false;
        NotifyStateChanged();
    }

    public async Task DeleteFileAsync()
    {
        if (FileToDelete == null)
        {
            CancelDelete();
            return;
        }

        IsDeletingFile = true;
        NotifyStateChanged();

        try
        {
            _logger.LogInformation("Deleting file: {Path}", FileToDelete.Path);
            await _browserService.DeleteFileAsync(FileToDelete.Path, FileToDelete.Sha);
            _logger.LogInformation("File deleted successfully: {Path}", FileToDelete.Path);

            // Set success message
            _state.SetStatusMessage($"File {FileToDelete.Name} deleted successfully");

            // Refresh the current directory
            await LoadContentsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to delete file: {ex.Message}";
            _logger.LogError(ex, "Failed to delete file: {Path}", FileToDelete?.Path);
            _state.SetErrorMessage(ErrorMessage);
        }
        finally
        {
            IsDeletingFile = false;
            IsShowingDeleteConfirmation = false;
            FileToDelete = null;
            NotifyStateChanged();
        }
    }

    protected void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}