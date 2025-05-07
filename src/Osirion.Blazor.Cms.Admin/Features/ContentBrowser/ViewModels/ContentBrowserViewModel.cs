using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Services.State;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.ContentBrowser.ViewModels;

public class ContentBrowserViewModel
{
    private readonly ContentBrowserService _contentService;
    private readonly CmsApplicationState _appState;
    private readonly CmsEventMediator _eventMediator;

    public List<GitHubItem> Contents { get; private set; } = new();
    public string CurrentPath => _appState.CurrentPath;
    public bool IsLoading { get; private set; }
    public string? ErrorMessage { get; private set; }
    public GitHubItem? SelectedItem { get; private set; }

    public event Action? StateChanged;

    public ContentBrowserViewModel(
        ContentBrowserService contentService,
        CmsApplicationState appState,
        CmsEventMediator eventMediator)
    {
        _contentService = contentService;
        _appState = appState;
        _eventMediator = eventMediator;

        // Subscribe to relevant events
        _appState.StateChanged += OnAppStateChanged;
    }

    public async Task RefreshContentsAsync()
    {
        if (!IsValidState())
            return;

        IsLoading = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            Contents = await _contentService.GetContentsAsync(CurrentPath);
            _appState.SetCurrentPath(CurrentPath, Contents);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load contents: {ex.Message}";
            _appState.SetErrorMessage(ErrorMessage);
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    public async Task NavigateToPathAsync(string path)
    {
        _appState.SetCurrentPath(path, new List<GitHubItem>());
        await RefreshContentsAsync();
    }

    public async Task SelectItemAsync(GitHubItem item)
    {
        SelectedItem = item;

        if (item.IsDirectory)
        {
            await NavigateToPathAsync(item.Path);
        }
        else if (item.IsMarkdownFile)
        {
            // Publish event for content selection
            _eventMediator.Publish(new ContentSelectedEvent(item));
        }

        NotifyStateChanged();
    }

    public async Task DeleteFileAsync(GitHubItem item)
    {
        if (!item.IsFile)
            return;

        IsLoading = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            await _contentService.DeleteFileAsync(item.Path, item.Sha);
            await RefreshContentsAsync();

            // Publish content deleted event
            _eventMediator.Publish(new ContentDeletedEvent(item.Path));

            _appState.SetStatusMessage($"File {item.Name} deleted successfully");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to delete file: {ex.Message}";
            _appState.SetErrorMessage(ErrorMessage);
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    public bool IsItemSelected(GitHubItem item)
    {
        return SelectedItem?.Path == item.Path;
    }

    public bool IsValidState()
    {
        return _appState.SelectedRepository != null && _appState.SelectedBranch != null;
    }

    private void OnAppStateChanged()
    {
        NotifyStateChanged();
    }

    protected void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}