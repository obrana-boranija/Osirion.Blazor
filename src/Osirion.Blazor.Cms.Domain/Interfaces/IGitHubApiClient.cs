using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>
/// Client for interacting with the GitHub API
/// </summary>
public interface IGitHubApiClient
{
    /// <summary>
    /// Gets the content of a repository at the specified path
    /// </summary>
    Task<List<GitHubItem>> GetRepositoryContentsAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the content of a file
    /// </summary>
    Task<GitHubFileContent> GetFileContentAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets information about a file from the commit history
    /// </summary>
    Task<(DateTime Created, DateTime? Modified)> GetFileHistoryAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all branches in the repository
    /// </summary>
    Task<List<GitHubBranch>> GetBranchesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates a file in the repository
    /// </summary>
    Task<GitHubFileCommitResponse> CreateOrUpdateFileAsync(
        string path,
        string content,
        string message,
        string? sha = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from the repository
    /// </summary>
    Task<GitHubFileCommitResponse> DeleteFileAsync(
        string path,
        string message,
        string sha,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for files in the repository
    /// </summary>
    Task<GitHubSearchResult> SearchFilesAsync(string query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the access token for authenticated requests
    /// </summary>
    void SetAccessToken(string token);

    /// <summary>
    /// Sets the repository for API operations
    /// </summary>
    void SetRepository(string owner, string repo);

    /// <summary>
    /// Sets the branch for API operations
    /// </summary>
    void SetBranch(string branch);

    /// <summary>
    /// Creates a new branch
    /// </summary>
    Task<GitHubBranch> CreateBranchAsync(string name, string fromBranch, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a pull request
    /// </summary>
    Task<GitHubPullRequest> CreatePullRequestAsync(
        string title,
        string body,
        string head,
        string baseBranch,
        CancellationToken cancellationToken = default);
}