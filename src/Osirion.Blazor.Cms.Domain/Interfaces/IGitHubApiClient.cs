using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>
/// Interface for GitHub API client operations
/// </summary>
public interface IGitHubApiClient
{
    /// <summary>
    /// Gets a list of repositories the authenticated user has access to
    /// </summary>
    /// <returns>List of GitHub repositories</returns>
    Task<List<GitHubRepository>?> GetRepositoriesAsync(CancellationToken cancellationToken = default!);

    /// <summary>
    /// Gets the branches for the current repository
    /// </summary>
    /// <returns>List of GitHub branches</returns>
    Task<List<GitHubBranch>?> GetBranchesAsync(CancellationToken cancellationToken = default!);

    /// <summary>
    /// Gets the contents of the repository at the specified path
    /// </summary>
    /// <param name="path">Path in the repository</param>
    /// <returns>List of GitHub items (files and directories)</returns>
    Task<List<GitHubItem>?> GetRepositoryContentsAsync(string path, CancellationToken cancellationToken = default!);

    /// <summary>
    /// Gets the content of a file in the repository
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <returns>File content</returns>
    Task<GitHubFileContent?> GetFileContentAsync(string path, CancellationToken cancellationToken = default!);

    /// <summary>
    /// Creates or updates a file in the repository
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <param name="content">Content of the file</param>
    /// <param name="message">Commit message</param>
    /// <param name="sha">SHA of the file to update (null for new files)</param>
    /// <returns>Commit response</returns>
    Task<GitHubFileCommitResponse?> CreateOrUpdateFileAsync(string path, string content, string message, string? sha = null, CancellationToken cancellationToken = default!);

    /// <summary>
    /// Deletes a file from the repository
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <param name="message">Commit message</param>
    /// <param name="sha">SHA of the file to delete</param>
    /// <returns>Commit response</returns>
    Task<GitHubFileCommitResponse> DeleteFileAsync(string path, string message, string sha, CancellationToken cancellationToken = default!);

    /// <summary>
    /// Creates a new branch in the repository
    /// </summary>
    /// <param name="branchName">Name of the new branch</param>
    /// <param name="baseBranch">Name of the base branch</param>
    /// <returns>The created branch</returns>
    Task<GitHubBranch?> CreateBranchAsync(string branchName, string baseBranch, CancellationToken cancellationToken = default!);

    /// <summary>
    /// Creates a new pull request in the repository
    /// </summary>
    /// <param name="title">Title of the pull request</param>
    /// <param name="body">Body of the pull request</param>
    /// <param name="head">Head branch name</param>
    /// <param name="baseBranch">Base branch name</param>
    /// <returns>The created pull request</returns>
    Task<GitHubPullRequest?> CreatePullRequestAsync(string title, string body, string head, string baseBranch, CancellationToken cancellationToken = default!);

    /// <summary>
    /// Searches for files in the repository
    /// </summary>
    /// <param name="query">Search query</param>
    /// <returns>Search results</returns>
    Task<GitHubSearchResult?> SearchFilesAsync(string query, CancellationToken cancellationToken = default!);

    /// <summary>
    /// Sets the owner and repository for subsequent requests
    /// </summary>
    /// <param name="owner">Repository owner</param>
    /// <param name="repository">Repository name</param>
    void SetRepository(string owner, string repository);

    /// <summary>
    /// Sets the branch for subsequent requests
    /// </summary>
    /// <param name="branch">Branch name</param>
    void SetBranch(string branch);

    /// <summary>
    /// Sets the access token for authentication
    /// </summary>
    /// <param name="token">GitHub access token</param>
    void SetAccessToken(string token);
}