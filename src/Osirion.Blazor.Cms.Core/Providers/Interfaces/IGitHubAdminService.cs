using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Infrastructure.GitHub.Models;

namespace Osirion.Blazor.Cms.Core.Providers.Interfaces;

/// <summary>
/// Service for interacting with GitHub APIs to manage content
/// </summary>
public interface IGitHubAdminService
{
    // Repository and branch operations
    Task<List<GitHubRepository>> GetRepositoriesAsync();
    Task<List<GitHubBranch>> GetBranchesAsync(string repository);
    Task<GitHubBranch> CreateBranchAsync(string branchName, string fromBranch);

    // File operations
    Task<List<GitHubItem>> GetRepositoryContentsAsync(string path = "");
    Task<GitHubFileContent> GetFileContentAsync(string path);
    Task<GitHubFileCommitResponse> CreateOrUpdateFileAsync(string path, string content, string commitMessage, string? existingSha = null);
    Task<GitHubFileCommitResponse> DeleteFileAsync(string path, string commitMessage, string sha);

    // Content operations
    Task<BlogPost> GetBlogPostAsync(string path);
    Task<List<GitHubItem>> SearchFilesAsync(string query);

    // Pull request operations
    Task<GitHubPullRequest> CreatePullRequestAsync(string title, string body, string head, string baseBranch);

    // Configuration
    string CurrentRepository { get; }
    string CurrentBranch { get; }
    void SetRepository(string repository);
    void SetBranch(string branch);
    Task SetAuthTokenAsync(string token);
}