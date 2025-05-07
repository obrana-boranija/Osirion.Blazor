using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.ContentBrowser.ViewModels;

public class FileExplorerViewModel
{
    private readonly ContentBrowserService _browserService;
    private readonly CmsState _state;
    private readonly ILogger<FileExplorerViewModel> _logger;

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
        ILogger<FileExplorerViewModel> logger)
    {
        _browserService = browserService;
        _state = state;
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