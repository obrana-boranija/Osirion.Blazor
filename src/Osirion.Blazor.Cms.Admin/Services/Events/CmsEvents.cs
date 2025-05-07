using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Services.Events;

// Base event interface
public interface ICmsEvent { }

// Repository events
public record RepositorySelectedEvent(GitHubRepository Repository) : ICmsEvent;
public record BranchSelectedEvent(GitHubBranch Branch) : ICmsEvent;
public record RepositoryChangedEvent(List<GitHubRepository> Repositories) : ICmsEvent;

// Content events
public record ContentSelectedEvent(GitHubItem Item) : ICmsEvent;
public record ContentSavedEvent(string Path) : ICmsEvent;
public record ContentDeletedEvent(string Path) : ICmsEvent;
public record ContentChangedEvent(string Path) : ICmsEvent;

// Authentication events
public record AuthenticationChangedEvent(bool IsAuthenticated) : ICmsEvent;
public record UserProfileLoadedEvent(string Username) : ICmsEvent;

// Navigation events
public record NavigationRequestedEvent(string Route) : ICmsEvent;
public record StateResetRequestedEvent() : ICmsEvent;

// UI events
public record ThemeChangedEvent(string Theme) : ICmsEvent;
public record ErrorOccurredEvent(string Message, Exception? Exception = null) : ICmsEvent;
public record StatusNotificationEvent(string Message, StatusType Type = StatusType.Info) : ICmsEvent;

public enum StatusType
{
    Info,
    Success,
    Warning,
    Error
}