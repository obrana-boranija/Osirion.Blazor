using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Services.Events;

// Base event interface
public interface ICmsEvent { }

// Repository events
public record RepositorySelectedEvent(GitHubRepository Repository) : ICmsEvent;
public record BranchSelectedEvent(GitHubBranch Branch) : ICmsEvent;

// Content events
public record ContentSelectedEvent(GitHubItem Item) : ICmsEvent;
public record ContentSavedEvent(string Path) : ICmsEvent;
public record ContentDeletedEvent(string Path) : ICmsEvent;