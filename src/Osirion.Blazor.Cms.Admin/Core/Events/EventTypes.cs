using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Core.Events;

// Repository events
public class RepositorySelectedEvent
{
    public GitHubRepository Repository { get; }

    public RepositorySelectedEvent(GitHubRepository repository)
    {
        Repository = repository;
    }
}

public class BranchSelectedEvent
{
    public GitHubBranch Branch { get; }

    public BranchSelectedEvent(GitHubBranch branch)
    {
        Branch = branch;
    }
}

// Content events
public class ContentSelectedEvent
{
    public GitHubItem Item { get; }

    public ContentSelectedEvent(GitHubItem item)
    {
        Item = item;
    }
}

public class ContentSavedEvent
{
    public string Path { get; }

    public ContentSavedEvent(string path)
    {
        Path = path;
    }
}

public class ContentDeletedEvent
{
    public string Path { get; }

    public ContentDeletedEvent(string path)
    {
        Path = path;
    }
}

// Authentication events
public class AuthenticationChangedEvent
{
    public bool IsAuthenticated { get; }

    public AuthenticationChangedEvent(bool isAuthenticated)
    {
        IsAuthenticated = isAuthenticated;
    }
}

// Error events
public class ErrorOccurredEvent
{
    public string Message { get; }
    public Exception? Exception { get; }

    public ErrorOccurredEvent(string message, Exception? exception = null)
    {
        Message = message;
        Exception = exception;
    }
}

// Status notification events
public enum StatusType
{
    Info,
    Success,
    Warning,
    Error
}

public class StatusNotificationEvent
{
    public string Message { get; }
    public StatusType Type { get; }

    public StatusNotificationEvent(string message, StatusType type = StatusType.Info)
    {
        Message = message;
        Type = type;
    }
}