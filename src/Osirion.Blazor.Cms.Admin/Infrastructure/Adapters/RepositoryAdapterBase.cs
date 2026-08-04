using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using System.Runtime.CompilerServices;

namespace Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;

/// <summary>Provides the base contract and logging support for content repository adapters.</summary>
public abstract class RepositoryAdapterBase : IContentRepositoryAdapter
{
    /// <summary>Performs the _logger operation.</summary>
    protected readonly ILogger _logger;

    /// <summary>Performs the RepositoryAdapterBase operation.</summary>
    protected RepositoryAdapterBase(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>Gets repositories available to the current account.</summary>
    public abstract Task<List<GitHubRepository>> GetRepositoriesAsync();

    /// <summary>Gets branches for a repository.</summary>
    public abstract Task<List<GitHubBranch>> GetBranchesAsync(string repositoryName);

    /// <summary>Gets repository contents at a path.</summary>
    public abstract Task<List<GitHubItem>> GetContentsAsync(string path);

    /// <summary>Gets a blog post by path.</summary>
    public abstract Task<ContentItem> GetBlogPostAsync(string path);

    /// <summary>Saves content to the repository.</summary>
    public abstract Task<GitHubFileCommitResponse> SaveContentAsync(
        string path, string content, string message, string? sha = null);

    /// <summary>Deletes a file from the repository.</summary>
    public abstract Task<GitHubFileCommitResponse> DeleteFileAsync(
        string path, string message, string sha);

    /// <summary>Creates a branch from a base branch.</summary>
    public abstract Task<GitHubBranch> CreateBranchAsync(string name, string baseBranch);

    /// <summary>Sets the repository used by the adapter.</summary>
    public abstract void SetRepository(string repositoryName);

    /// <summary>Sets the branch used by the adapter.</summary>
    public abstract void SetBranch(string branchName);

    /// <summary>Sets the access token used by the adapter.</summary>
    public abstract Task SetAccessTokenAsync(string token);

    /// <summary>Gets or sets the LogOperation value.</summary>
    protected void LogOperation(string operation, string? details = null, [CallerMemberName] string? methodName = null)
    {
        var message = $"{GetType().Name}: {methodName} - {operation}";
        if (!string.IsNullOrWhiteSpace(details))
        {
            message += $" ({details})";
        }
        _logger.LogDebug(message);
    }

    /// <summary>Creates a pull request.</summary>
    public abstract Task<GitHubPullRequest> CreatePullRequestAsync(string title, string body, string head, string baseBranch);

    /// <summary>Searches repository files.</summary>
    public abstract Task<List<GitHubItem>> SearchFilesAsync(string query);
}
