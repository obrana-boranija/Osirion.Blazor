using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.ContentBrowser.ViewModels;

/// <summary>Manages repository content browsing and file selection.</summary>
public class ContentBrowserViewModel
{
    private readonly ContentBrowserService _contentService;
    private readonly CmsState _appState;
    private readonly CmsEventMediator _eventMediator;

    /// <summary>Gets the contents in the current directory.</summary>
    public List<GitHubItem> Contents { get; private set; } = new();
    /// <summary>Gets the current repository path.</summary>
    public string CurrentPath => _appState.CurrentPath;
    /// <summary>Gets whether content is loading.</summary>
    public bool IsLoading { get; private set; }
    /// <summary>Gets the current error message.</summary>
    public string? ErrorMessage { get; private set; }
    /// <summary>Gets the selected repository item.</summary>
    public GitHubItem? SelectedItem { get; private set; }

    /// <summary>Occurs when the view-model state changes.</summary>
    public event Action? StateChanged;

    /// <summary>Initializes a new content browser view-model.</summary>
    public ContentBrowserViewModel(
        ContentBrowserService contentService,
        CmsState appState,
        CmsEventMediator eventMediator)
    {
        _contentService = contentService;
        _appState = appState;
        _eventMediator = eventMediator;

        // Subscribe to relevant events
        _appState.StateChanged += OnAppStateChanged;
    }

    /// <summary>Refreshes the contents at the current path.</summary>
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

    /// <summary>Navigates to a repository path.</summary>
    public async Task NavigateToPathAsync(string path)
    {
        _appState.SetCurrentPath(path, new List<GitHubItem>());
        await RefreshContentsAsync();
    }

    /// <summary>Selects a file or directory.</summary>
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
            _eventMediator.Publish(new ContentSelectedEvent(item.Path));
        }

        NotifyStateChanged();
    }

    /// <summary>Deletes a file and refreshes the current directory.</summary>
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

    /// <summary>Determines whether an item is selected.</summary>
    public bool IsItemSelected(GitHubItem item)
    {
        return SelectedItem?.Path == item.Path;
    }

    /// <summary>Determines whether repository browsing state is valid.</summary>
    public bool IsValidState()
    {
        return _appState.SelectedRepository is not null && _appState.SelectedBranch is not null;
    }

    private void OnAppStateChanged()
    {
        NotifyStateChanged();
    }

    /// <summary>Performs the NotifyStateChanged operation.</summary>
    protected void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}
