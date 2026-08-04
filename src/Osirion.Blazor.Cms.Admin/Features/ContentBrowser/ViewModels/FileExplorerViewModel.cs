using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.ContentBrowser.ViewModels;

/// <summary>Coordinates content browser navigation and file actions.</summary>
public class FileExplorerViewModel
{
    private readonly ContentBrowserService _browserService;
    private readonly CmsState _state;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<FileExplorerViewModel> _logger;
    private readonly CmsEventMediator _eventMediator;

    /// <summary>Gets the current directory contents.</summary>
    public List<GitHubItem> Contents => _state.CurrentItems;
    /// <summary>Gets the current repository path.</summary>
    public string CurrentPath => _state.CurrentPath;
    /// <summary>Gets the selected item.</summary>
    public GitHubItem? SelectedItem { get; private set; }
    /// <summary>Gets whether content is loading.</summary>
    public bool IsLoading { get; private set; }
    /// <summary>Gets the current error message.</summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>Gets whether a repository and branch are selected.</summary>
    public bool IsValidRepositoryAndBranch =>
        _state.SelectedRepository is not null && _state.SelectedBranch is not null;

    // For delete confirmation
    /// <summary>Gets whether delete confirmation is visible.</summary>
    public bool IsShowingDeleteConfirmation { get; private set; }
    /// <summary>Gets the file pending deletion.</summary>
    public GitHubItem? FileToDelete { get; private set; }
    /// <summary>Gets whether a file deletion is in progress.</summary>
    public bool IsDeletingFile { get; private set; }

    /// <summary>Raised when view-model state changes.</summary>
    public event Action? StateChanged;

    /// <summary>Initializes the file explorer view model.</summary>
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

    /// <summary>Loads the contents of the current path.</summary>
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

    /// <summary>Navigates to a repository path.</summary>
    /// <param name="path">The repository path.</param>
    public async Task NavigateToPathAsync(string path)
    {
        _state.SetCurrentPath(path, new List<GitHubItem>());
        await LoadContentsAsync();
    }

    /// <summary>Navigates to the repository root.</summary>
    public async Task NavigateToRootAsync()
    {
        await NavigateToPathAsync(string.Empty);
    }

    /// <summary>
    /// Navigates to the parent directory of the current path
    /// </summary>
    public async Task NavigateToParentDirectoryAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentPath))
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
        if (item is null)
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
        if (item is null || !item.IsFile)
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
                _navigationManager.NavigateTo($"/osirion/content/edit?Path={item.Path}");

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

    /// <summary>Selects an item.</summary>
    /// <param name="item">The item to select.</param>
    public void SelectItem(GitHubItem item)
    {
        SelectedItem = item;
        NotifyStateChanged();
    }

    /// <summary>Determines whether an item is selected.</summary>
    /// <param name="item">The item to check.</param>
    public bool IsItemSelected(GitHubItem item)
    {
        return SelectedItem?.Path == item.Path;
    }

    /// <summary>Shows the delete confirmation for an item.</summary>
    /// <param name="item">The file to delete.</param>
    public void ShowDeleteConfirmation(GitHubItem item)
    {
        FileToDelete = item;
        IsShowingDeleteConfirmation = true;
        NotifyStateChanged();
    }

    /// <summary>Cancels the pending deletion.</summary>
    public void CancelDelete()
    {
        FileToDelete = null;
        IsShowingDeleteConfirmation = false;
        NotifyStateChanged();
    }

    /// <summary>Deletes the file pending confirmation.</summary>
    public async Task DeleteFileAsync()
    {
        if (FileToDelete is null)
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

    /// <summary>Performs the NotifyStateChanged operation.</summary>
    protected void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}
