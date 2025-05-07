using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Services.Adapters;

public interface IContentRepositoryAdapter
{
    // Repository operations
    Task<List<GitHubRepository>> GetRepositoriesAsync();
    Task<List<GitHubBranch>> GetBranchesAsync(string repositoryName);

    // Content operations
    Task<List<GitHubItem>> GetContentsAsync(string path);
    Task<BlogPost> GetBlogPostAsync(string path);

    // Mutation operations
    Task<GitHubFileCommitResponse> SaveContentAsync(string path, string content, string message, string? sha = null);
    Task<GitHubFileCommitResponse> DeleteFileAsync(string path, string message, string sha);
    Task<GitHubBranch> CreateBranchAsync(string name, string baseBranch);

    // Configuration
    void SetRepository(string repositoryName);
    void SetBranch(string branchName);
    Task SetAccessTokenAsync(string token);
}