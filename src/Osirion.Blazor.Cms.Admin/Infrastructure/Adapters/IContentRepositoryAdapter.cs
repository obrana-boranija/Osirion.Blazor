using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;

/// <summary>
/// Adapter for GitHub admin operations that bridges with domain repositories
/// </summary>
public interface IContentRepositoryAdapter
{
    Task<List<GitHubRepository>> GetRepositoriesAsync();
    Task<List<GitHubBranch>> GetBranchesAsync(string repositoryName);
    Task<List<GitHubItem>> GetContentsAsync(string path);
    Task<BlogPost> GetBlogPostAsync(string path);
    Task<GitHubFileCommitResponse> SaveContentAsync(string path, string content, string message, string? sha = null);
    Task<GitHubFileCommitResponse> DeleteFileAsync(string path, string message, string sha);
    Task<GitHubBranch> CreateBranchAsync(string name, string baseBranch);
    Task<GitHubPullRequest> CreatePullRequestAsync(string title, string body, string head, string baseBranch);
    Task<List<GitHubItem>> SearchFilesAsync(string query);
    void SetRepository(string repositoryName);
    void SetBranch(string branchName);
    Task SetAccessTokenAsync(string token);
}