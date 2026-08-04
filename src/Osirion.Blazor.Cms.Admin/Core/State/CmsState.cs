using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Admin.Core.State;

/// <summary>
/// Centralized state container for the CMS admin interface
/// </summary>
public class CmsState
{
    // Current selections
    /// <summary>Gets the selected repository.</summary>
    public GitHubRepository? SelectedRepository { get; protected set; }
    /// <summary>Gets the selected branch.</summary>
    public GitHubBranch? SelectedBranch { get; protected set; }
    /// <summary>Gets the selected item.</summary>
    public GitHubItem? SelectedItem { get; private set; }
    /// <summary>Gets the current repository path.</summary>
    public string CurrentPath { get; protected set; } = string.Empty;
    /// <summary>Gets the items at the current path.</summary>
    public List<GitHubItem> CurrentItems { get; private set; } = new();
    /// <summary>Gets the current theme.</summary>
    public string CurrentTheme { get; private set; } = "light";

    // Available options
    /// <summary>Gets the available repositories.</summary>
    public List<GitHubRepository> AvailableRepositories { get; private set; } = new();
    /// <summary>Gets the available branches.</summary>
    public List<GitHubBranch> AvailableBranches { get; private set; } = new();

    // Editing state
    /// <summary>Gets the post being edited.</summary>
    public ContentItem? EditingPost { get; private set; }
    /// <summary>Gets whether a post is being edited.</summary>
    public bool IsEditing { get; private set; }
    /// <summary>Gets whether a save is in progress.</summary>
    public bool IsSaving { get; private set; }
    /// <summary>Gets whether a new file is being created.</summary>
    public bool IsCreatingNewFile { get; private set; }

    // Status and errors
    /// <summary>Gets the current status message.</summary>
    public string? StatusMessage { get; private set; }
    /// <summary>Gets the current error message.</summary>
    public string? ErrorMessage { get; private set; }

    // State change events
    private Action? _stateChanged;
    /// <summary>Occurs when the state changes.</summary>
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
    public virtual void SelectRepository(GitHubRepository? repository)
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
    public virtual void SelectBranch(GitHubBranch? branch)
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
    public void SetEditingPost(ContentItem post, bool isNew = false)
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

    /// <summary>
    /// Serialize state to string for persistence
    /// </summary>
    public string Serialize()
    {
        var state = new Dictionary<string, object?>();

        if (SelectedRepository is not null)
            state["SelectedRepository"] = SelectedRepository;

        if (SelectedBranch is not null)
            state["SelectedBranch"] = SelectedBranch;

        state["CurrentPath"] = CurrentPath;
        state["CurrentTheme"] = CurrentTheme;

        return JsonSerializer.Serialize(state);
    }

    /// <summary>
    /// Deserialize state from string
    /// </summary>
    public void DeserializeFrom(string serializedState)
    {
        if (string.IsNullOrWhiteSpace(serializedState))
            return;

        try
        {
            var state = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(serializedState);
            if (state is null) return;

            if (state.TryGetValue("SelectedRepository", out var repoElement))
            {
                SelectedRepository = repoElement.Deserialize<GitHubRepository>();
            }

            if (state.TryGetValue("SelectedBranch", out var branchElement))
            {
                SelectedBranch = branchElement.Deserialize<GitHubBranch>();
            }

            if (state.TryGetValue("CurrentPath", out var pathElement) &&
                pathElement.ValueKind == JsonValueKind.String)
            {
                CurrentPath = pathElement.GetString() ?? string.Empty;
            }

            if (state.TryGetValue("CurrentTheme", out var themeElement) &&
                themeElement.ValueKind == JsonValueKind.String)
            {
                CurrentTheme = themeElement.GetString() ?? "light";
            }

            NotifyStateChanged();
        }
        catch
        {
            // If deserialization fails, just keep current state
        }
    }

    /// <summary>
    /// Notifies listeners that state has changed
    /// </summary>
    protected void NotifyStateChanged()
    {
        _stateChanged?.Invoke();
    }
}
