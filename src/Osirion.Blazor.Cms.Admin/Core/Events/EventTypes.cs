using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Core.Events;

/// <summary>
/// Base event interface
/// </summary>
public interface ICmsEvent { }

/// <summary>
/// Event triggered when content is selected in the CMS
/// </summary>
/// <param name="Path"></param>
public record ContentSelectedEvent(string Path) : ICmsEvent;

/// <summary>
/// Event triggered when content is saved or updated
/// </summary>
/// <param name="Path"></param>
public record ContentSavedEvent(string Path) : ICmsEvent;

/// <summary>
/// Event triggered when content is deleted
/// </summary>
/// <param name="Path"></param>
public record ContentDeletedEvent(string Path) : ICmsEvent;

/// <summary>
/// Event triggered when a new content item is created
/// </summary>
/// <param name="Directory"></param>
public record CreateNewContentEvent(string Directory) : ICmsEvent;

/// <summary>
/// Event triggered when a repository is selected
/// </summary>
/// <param name="Repository"></param>
public record RepositorySelectedEvent(GitHubRepository Repository) : ICmsEvent;

/// <summary>
/// Event triggered when a branch is selected
/// </summary>
/// <param name="Branch"></param>
public record BranchSelectedEvent(GitHubBranch Branch) : ICmsEvent;

// Navigation events
/// <summary>
/// Event triggered when navigation is requested
/// </summary>
/// <param name="Route"></param>
public record NavigationRequestedEvent(string Route) : ICmsEvent;

/// <summary>
/// Event triggered when the state needs to be reset
/// </summary>
public record StateResetRequestedEvent() : ICmsEvent;

// UI events
/// <summary>
/// Event triggered when the theme is changed
/// </summary>
/// <param name="Theme"></param>
public record ThemeChangedEvent(string Theme) : ICmsEvent;

/// <summary>
/// Event triggered when an error occurs in the CMS
/// </summary>
/// <param name="Message"></param>
/// <param name="Exception"></param>
public record ErrorOccurredEvent(string Message, Exception? Exception = null) : ICmsEvent;

/// <summary>
/// Event for displaying status notifications in the CMS
/// </summary>
public enum StatusType { Info, Success, Warning, Error }

/// <summary>
/// Event for displaying status notifications in the CMS
/// </summary>
/// <param name="Message"></param>
/// <param name="Type"></param>
public record StatusNotificationEvent(string Message, StatusType Type = StatusType.Info) : ICmsEvent;

// Authentication events
/// <summary>
/// Event triggered when authentication state changes
/// </summary>
/// <param name="IsAuthenticated"></param>
public record AuthenticationChangedEvent(bool IsAuthenticated) : ICmsEvent;

/// <summary>
/// Event triggered when a user profile is loaded
/// </summary>
/// <param name="Username"></param>
public record UserProfileLoadedEvent(string Username) : ICmsEvent;