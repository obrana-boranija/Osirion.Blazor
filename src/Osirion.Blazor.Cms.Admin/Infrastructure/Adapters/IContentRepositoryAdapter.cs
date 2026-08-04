using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;

/// <summary>
/// Adapter for GitHub admin operations that bridges with domain repositories
/// </summary>
public interface IContentRepositoryAdapter
{
    /// <summary>Gets repositories available to the authenticated user.</summary>
    Task<List<GitHubRepository>> GetRepositoriesAsync();
    /// <summary>Gets branches for a repository.</summary>
    /// <param name="repositoryName">The repository name.</param>
    Task<List<GitHubBranch>> GetBranchesAsync(string repositoryName);
    /// <summary>Gets repository contents at a path.</summary>
    /// <param name="path">The repository path.</param>
    Task<List<GitHubItem>> GetContentsAsync(string path);
    /// <summary>Gets a blog post from a repository path.</summary>
    /// <param name="path">The repository path.</param>
    Task<ContentItem> GetBlogPostAsync(string path);
    /// <summary>Saves content to a repository.</summary>
    /// <param name="path">The repository path.</param>
    /// <param name="content">The content to save.</param>
    /// <param name="message">The commit message.</param>
    /// <param name="sha">The existing file revision, when updating.</param>
    Task<GitHubFileCommitResponse> SaveContentAsync(string path, string content, string message, string? sha = null);
    /// <summary>Deletes a file from a repository.</summary>
    /// <param name="path">The repository path.</param>
    /// <param name="message">The commit message.</param>
    /// <param name="sha">The file revision.</param>
    Task<GitHubFileCommitResponse> DeleteFileAsync(string path, string message, string sha);
    /// <summary>Creates a branch from a base branch.</summary>
    /// <param name="name">The new branch name.</param>
    /// <param name="baseBranch">The source branch.</param>
    Task<GitHubBranch> CreateBranchAsync(string name, string baseBranch);
    /// <summary>Creates a pull request.</summary>
    /// <param name="title">The pull request title.</param>
    /// <param name="body">The pull request body.</param>
    /// <param name="head">The source branch.</param>
    /// <param name="baseBranch">The target branch.</param>
    Task<GitHubPullRequest> CreatePullRequestAsync(string title, string body, string head, string baseBranch);
    /// <summary>Searches repository files.</summary>
    /// <param name="query">The search query.</param>
    Task<List<GitHubItem>> SearchFilesAsync(string query);
    /// <summary>Selects the repository used by subsequent operations.</summary>
    /// <param name="repositoryName">The repository name.</param>
    void SetRepository(string repositoryName);
    /// <summary>Selects the branch used by subsequent operations.</summary>
    /// <param name="branchName">The branch name.</param>
    void SetBranch(string branchName);
    /// <summary>Sets the access token used for repository operations.</summary>
    /// <param name="token">The access token.</param>
    Task SetAccessTokenAsync(string token);
}
