using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Core.Events;

#region Repository Events

/// <summary>
/// Event raised when a repository is selected
/// </summary>
public record RepositorySelectedEvent(GitHubRepository Repository);

/// <summary>
/// Event raised when a branch is selected
/// </summary>
public record BranchSelectedEvent(GitHubBranch Branch);

/// <summary>
/// Event raised when repositories are loaded
/// </summary>
public record RepositoriesLoadedEvent(List<GitHubRepository> Repositories);

/// <summary>
/// Event raised when branches are loaded
/// </summary>
public record BranchesLoadedEvent(string RepositoryName, List<GitHubBranch> Branches);

#endregion

#region Content Events

/// <summary>
/// Event raised when a content item is selected
/// </summary>
public record ContentSelectedEvent(string Path);

/// <summary>
/// Event raised when a content item is saved
/// </summary>
public record ContentSavedEvent(string Path);

/// <summary>
/// Event raised when a content item is deleted
/// </summary>
public record ContentDeletedEvent(string Path);

/// <summary>
/// Event raised to request creating new content
/// </summary>
public record CreateNewContentEvent(string Directory);

/// <summary>
/// Event raised when a directory is selected
/// </summary>
public record DirectorySelectedEvent(string Path);

/// <summary>
/// Event raised when directory contents are loaded
/// </summary>
public record DirectoryContentsLoadedEvent(string Path, List<GitHubItem> Items);

#endregion

#region Authentication Events

/// <summary>
/// Event raised when authentication state changes
/// </summary>
public record AuthenticationChangedEvent(bool IsAuthenticated, string? Username = null);

/// <summary>
/// Event raised when login is required
/// </summary>
public record LoginRequiredEvent(string ReturnUrl);

/// <summary>
/// Event raised when logout is requested
/// </summary>
public record LogoutRequestedEvent();

#endregion

#region Notification Events

/// <summary>
/// Status type for notifications
/// </summary>
public enum StatusType
{
    Info,
    Success,
    Warning,
    Error
}

/// <summary>
/// Event raised when a notification should be displayed
/// </summary>
public record StatusNotificationEvent(string Message, StatusType Type = StatusType.Info);

/// <summary>
/// Event raised when an error occurs
/// </summary>
public record ErrorOccurredEvent(string Message, Exception? Exception = null);

#endregion

#region UI Events

/// <summary>
/// Event raised when the theme is changed
/// </summary>
public record ThemeChangedEvent(string Theme);

/// <summary>
/// Event raised when a UI dialog should be shown
/// </summary>
public record ShowDialogEvent(string DialogId, object? Parameters = null);

/// <summary>
/// Event raised when a UI dialog should be closed
/// </summary>
public record CloseDialogEvent(string DialogId);

#endregion