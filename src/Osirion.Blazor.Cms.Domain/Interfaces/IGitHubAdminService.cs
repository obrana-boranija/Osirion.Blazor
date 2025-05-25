using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Domain.Interfaces;

public interface IGitHubAdminService
{
    string CurrentBranch { get; }
    string CurrentRepository { get; }
    string CurrentProvider { get; }

    /// <summary>
    /// Sets the current provider to use
    /// </summary>
    /// <param name="providerName">Name of the provider</param>
    void SetProvider(string providerName);

    Task<GitHubBranch> CreateBranchAsync(string branchName, string fromBranch);
    Task<GitHubFileCommitResponse> CreateOrUpdateFileAsync(string path, string content, string commitMessage, string? existingSha = null);
    Task<GitHubPullRequest> CreatePullRequestAsync(string title, string body, string head, string baseBranch);
    Task<GitHubFileCommitResponse> DeleteFileAsync(string path, string commitMessage, string sha);
    Task<BlogPost> GetBlogPostAsync(string path);
    Task<List<GitHubBranch>> GetBranchesAsync(string repository);
    Task<GitHubFileContent> GetFileContentAsync(string path);
    Task<List<GitHubRepository>> GetRepositoriesAsync();
    Task<List<GitHubItem>> GetRepositoryContentsAsync(string path = "");
    Task<List<GitHubItem>> SearchFilesAsync(string query);
    Task SetAuthTokenAsync(string token);
    void SetBranch(string branch);
    void SetRepository(string repository);
}