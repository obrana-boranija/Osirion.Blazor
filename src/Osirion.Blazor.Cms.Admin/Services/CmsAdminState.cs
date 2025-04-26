using Osirion.Blazor.Cms.Admin.Models;
using Osirion.Blazor.Cms.Core.Models;

namespace Osirion.Blazor.Cms.Admin.Services;

/// <summary>
/// Service for managing the state of the CMS admin interface
/// </summary>
public class CmsAdminState
{
    // Current selections
    public GitHubRepository? SelectedRepository { get; protected set; }
    public GitHubBranch? SelectedBranch { get; protected set; }
    public GitHubItem? SelectedItem { get; private set; }
    public string CurrentPath { get; protected set; } = string.Empty;
    public List<GitHubItem> CurrentItems { get; private set; } = new();

    // Available options
    public List<GitHubRepository> AvailableRepositories { get; private set; } = new();
    public List<GitHubBranch> AvailableBranches { get; private set; } = new();

    // Editing state
    public BlogPost? EditingPost { get; private set; }
    public bool IsEditing { get; private set; }
    public bool IsSaving { get; private set; }
    public bool IsCreatingNewFile { get; private set; }

    // Status and errors
    public string? StatusMessage { get; private set; }
    public string? ErrorMessage { get; private set; }

    // State change events - make it protected so derived classes can access it
    protected Action? _stateChanged;

    // Public event accessor
    public event Action StateChanged
    {
        add => _stateChanged += value;
        remove => _stateChanged -= value;
    }

    /// <summary>
    /// Sets the available repositories
    /// </summary>
    public void SetRepositories(List<GitHubRepository> repositories)
    {
        AvailableRepositories = repositories;
        NotifyStateChanged();
    }

    /// <summary>
    /// Sets the selected repository
    /// </summary>
    public virtual void SelectRepository(GitHubRepository repository)
    {
        SelectedRepository = repository;
        SelectedBranch = null;
        AvailableBranches.Clear();
        CurrentItems.Clear();
        CurrentPath = string.Empty;
        NotifyStateChanged();
    }

    /// <summary>
    /// Sets the available branches
    /// </summary>
    public void SetBranches(List<GitHubBranch> branches)
    {
        AvailableBranches = branches;
        NotifyStateChanged();
    }

    /// <summary>
    /// Sets the selected branch
    /// </summary>
    public virtual void SelectBranch(GitHubBranch branch)
    {
        SelectedBranch = branch;
        CurrentItems.Clear();
        CurrentPath = string.Empty;
        NotifyStateChanged();
    }

    /// <summary>
    /// Sets the current path and items
    /// </summary>
    public virtual void SetCurrentPath(string path, List<GitHubItem> items)
    {
        CurrentPath = path;
        CurrentItems = items;
        NotifyStateChanged();
    }

    /// <summary>
    /// Sets the selected item
    /// </summary>
    public void SelectItem(GitHubItem item)
    {
        SelectedItem = item;
        NotifyStateChanged();
    }

    /// <summary>
    /// Sets the editing post
    /// </summary>
    public void SetEditingPost(BlogPost post, bool isNew = false)
    {
        EditingPost = post;
        IsEditing = true;
        IsCreatingNewFile = isNew;
        NotifyStateChanged();
    }

    /// <summary>
    /// Clears the editing state
    /// </summary>
    public void ClearEditing()
    {
        EditingPost = null;
        IsEditing = false;
        IsCreatingNewFile = false;
        NotifyStateChanged();
    }

    /// <summary>
    /// Sets the saving state
    /// </summary>
    public void SetSaving(bool isSaving)
    {
        IsSaving = isSaving;
        NotifyStateChanged();
    }

    /// <summary>
    /// Sets a status message
    /// </summary>
    public void SetStatusMessage(string message)
    {
        StatusMessage = message;
        ErrorMessage = null;
        NotifyStateChanged();
    }

    /// <summary>
    /// Sets an error message
    /// </summary>
    public void SetErrorMessage(string message)
    {
        ErrorMessage = message;
        StatusMessage = null;
        NotifyStateChanged();
    }

    /// <summary>
    /// Clears status and error messages
    /// </summary>
    public void ClearMessages()
    {
        StatusMessage = null;
        ErrorMessage = null;
        NotifyStateChanged();
    }

    /// <summary>
    /// Reset the entire state
    /// </summary>
    public virtual void Reset()
    {
        SelectedRepository = null;
        SelectedBranch = null;
        SelectedItem = null;
        CurrentPath = string.Empty;
        AvailableRepositories.Clear();
        AvailableBranches.Clear();
        CurrentItems.Clear();
        EditingPost = null;
        IsEditing = false;
        IsSaving = false;
        IsCreatingNewFile = false;
        StatusMessage = null;
        ErrorMessage = null;
        NotifyStateChanged();
    }

    protected void NotifyStateChanged()
    {
        _stateChanged?.Invoke();
    }
}