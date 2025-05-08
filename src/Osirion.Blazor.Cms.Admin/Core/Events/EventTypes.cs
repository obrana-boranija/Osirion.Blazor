using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Core.Events;

// Base event interface
public interface ICmsEvent { }

// Content-related events
public record ContentSelectedEvent(string Path) : ICmsEvent;
public record ContentSavedEvent(string Path) : ICmsEvent;
public record ContentDeletedEvent(string Path) : ICmsEvent;
public record CreateNewContentEvent(string Directory) : ICmsEvent;

// Repository-related events
public record RepositorySelectedEvent(GitHubRepository Repository) : ICmsEvent;
public record BranchSelectedEvent(GitHubBranch Branch) : ICmsEvent;

// Navigation events
public record NavigationRequestedEvent(string Route) : ICmsEvent;
public record StateResetRequestedEvent() : ICmsEvent;

// UI events
public record ThemeChangedEvent(string Theme) : ICmsEvent;
public record ErrorOccurredEvent(string Message, Exception? Exception = null) : ICmsEvent;

// Notification events
public enum StatusType { Info, Success, Warning, Error }
public record StatusNotificationEvent(string Message, StatusType Type = StatusType.Info) : ICmsEvent;

// Authentication events
public record AuthenticationChangedEvent(bool IsAuthenticated) : ICmsEvent;
public record UserProfileLoadedEvent(string Username) : ICmsEvent;