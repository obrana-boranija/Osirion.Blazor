using Osirion.Blazor.Cms.Domain.Models.GitHub;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Admin.Core.State;

/// <summary>
/// Centralized state container for the CMS admin interface
/// </summary>
public class CmsState
{
    // State properties
    public GitHubRepository? SelectedRepository { get; private set; }
    public GitHubBranch? SelectedBranch { get; private set; }
    public string CurrentPath { get; private set; } = string.Empty;
    public List<GitHubItem> CurrentItems { get; private set; } = new();
    public string? StatusMessage { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string CurrentTheme { get; private set; } = "light";
    public bool IsEditing { get; private set; }
    public bool IsSaving { get; private set; }

    // Event for notifying state changes
    public event Action? StateChanged;

    // State modification methods
    public void SelectRepository(GitHubRepository? repository)
    {
        SelectedRepository = repository;
        SelectedBranch = null;
        CurrentPath = string.Empty;
        CurrentItems.Clear();
        NotifyStateChanged();
    }

    public void SelectBranch(GitHubBranch? branch)
    {
        SelectedBranch = branch;
        CurrentPath = string.Empty;
        CurrentItems.Clear();
        NotifyStateChanged();
    }

    public void SetCurrentPath(string path, List<GitHubItem> items)
    {
        CurrentPath = path;
        CurrentItems = items;
        NotifyStateChanged();
    }

    public void SetStatusMessage(string message)
    {
        StatusMessage = message;
        ErrorMessage = null;
        NotifyStateChanged();
    }

    public void SetErrorMessage(string message)
    {
        ErrorMessage = message;
        StatusMessage = null;
        NotifyStateChanged();
    }

    public void ClearMessages()
    {
        StatusMessage = null;
        ErrorMessage = null;
        NotifyStateChanged();
    }

    public void SetEditing(bool isEditing)
    {
        IsEditing = isEditing;
        NotifyStateChanged();
    }

    public void SetSaving(bool isSaving)
    {
        IsSaving = isSaving;
        NotifyStateChanged();
    }

    public void SetTheme(string theme)
    {
        CurrentTheme = theme;
        NotifyStateChanged();
    }

    public void Reset()
    {
        SelectedRepository = null;
        SelectedBranch = null;
        CurrentPath = string.Empty;
        CurrentItems.Clear();
        StatusMessage = null;
        ErrorMessage = null;
        IsEditing = false;
        IsSaving = false;
        NotifyStateChanged();
    }

    public string Serialize()
    {
        var state = new Dictionary<string, object?>();

        if (SelectedRepository != null)
            state["SelectedRepository"] = SelectedRepository;

        if (SelectedBranch != null)
            state["SelectedBranch"] = SelectedBranch;

        state["CurrentPath"] = CurrentPath;
        state["CurrentTheme"] = CurrentTheme;

        return JsonSerializer.Serialize(state);
    }

    public void DeserializeFrom(string serializedState)
    {
        if (string.IsNullOrEmpty(serializedState))
            return;

        try
        {
            var state = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(serializedState);
            if (state == null) return;

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

    protected void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}