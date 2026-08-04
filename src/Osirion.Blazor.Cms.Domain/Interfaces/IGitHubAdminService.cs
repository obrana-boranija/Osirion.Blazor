using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>Defines the IGitHubAdminService API contract.</summary>
public interface IGitHubAdminService
{
    /// <summary>Gets the current GitHub branch.</summary>
    string CurrentBranch { get; }
    /// <summary>Gets the current GitHub repository.</summary>
    string CurrentRepository { get; }
    /// <summary>Gets the current content provider.</summary>
    string CurrentProvider { get; }

    /// <summary>
    /// Sets the current provider to use
    /// </summary>
    /// <param name="providerName">Name of the provider</param>
    void SetProvider(string providerName);

    /// <summary>Creates a branch from an existing branch.</summary>
    Task<GitHubBranch> CreateBranchAsync(string branchName, string fromBranch);
    /// <summary>Creates or updates a file in the repository.</summary>
    Task<GitHubFileCommitResponse> CreateOrUpdateFileAsync(string path, string content, string commitMessage, string? existingSha = null);
    /// <summary>Creates a pull request.</summary>
    Task<GitHubPullRequest> CreatePullRequestAsync(string title, string body, string head, string baseBranch);
    /// <summary>Deletes a file from the repository.</summary>
    Task<GitHubFileCommitResponse> DeleteFileAsync(string path, string commitMessage, string sha);
    /// <summary>Gets a blog post by path.</summary>
    Task<ContentItem> GetBlogPostAsync(string path);
    /// <summary>Gets branches for a repository.</summary>
    Task<List<GitHubBranch>> GetBranchesAsync(string repository);
    /// <summary>Gets file content by path.</summary>
    Task<GitHubFileContent> GetFileContentAsync(string path);
    /// <summary>Gets repositories available to the authenticated user.</summary>
    Task<List<GitHubRepository>> GetRepositoriesAsync();
    /// <summary>Gets repository contents at a path.</summary>
    Task<List<GitHubItem>> GetRepositoryContentsAsync(string path = "");
    /// <summary>Searches repository files.</summary>
    Task<List<GitHubItem>> SearchFilesAsync(string query);
    /// <summary>Sets the GitHub authentication token.</summary>
    Task SetAuthTokenAsync(string token);
    /// <summary>Sets the current branch.</summary>
    void SetBranch(string branch);
    /// <summary>Sets the current repository.</summary>
    void SetRepository(string repository);
}
