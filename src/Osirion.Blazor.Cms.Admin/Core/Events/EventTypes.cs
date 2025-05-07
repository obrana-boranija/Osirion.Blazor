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

// Notification events
public enum StatusType { Info, Success, Warning, Error }
public record StatusNotificationEvent(string Message, StatusType Type = StatusType.Info) : ICmsEvent;
public record ErrorOccurredEvent(string Message, Exception? Exception = null) : ICmsEvent;