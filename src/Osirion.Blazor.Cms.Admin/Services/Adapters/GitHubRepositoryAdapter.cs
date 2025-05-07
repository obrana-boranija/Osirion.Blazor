using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Services.Adapters;

public class GitHubRepositoryAdapter : IContentRepositoryAdapter
{
    private readonly IGitHubAdminService _gitHubService;

    public GitHubRepositoryAdapter(IGitHubAdminService gitHubService)
    {
        _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
    }

    public Task<List<GitHubRepository>> GetRepositoriesAsync() =>
        _gitHubService.GetRepositoriesAsync();

    public Task<List<GitHubBranch>> GetBranchesAsync(string repositoryName) =>
        _gitHubService.GetBranchesAsync(repositoryName);

    public Task<List<GitHubItem>> GetContentsAsync(string path) =>
        _gitHubService.GetRepositoryContentsAsync(path);

    public Task<BlogPost> GetBlogPostAsync(string path) =>
        _gitHubService.GetBlogPostAsync(path);

    public Task<GitHubFileCommitResponse> SaveContentAsync(string path, string content, string message, string? sha = null) =>
        _gitHubService.CreateOrUpdateFileAsync(path, content, message, sha);

    public Task<GitHubFileCommitResponse> DeleteFileAsync(string path, string message, string sha) =>
        _gitHubService.DeleteFileAsync(path, message, sha);

    public Task<GitHubBranch> CreateBranchAsync(string name, string baseBranch) =>
        _gitHubService.CreateBranchAsync(name, baseBranch);

    public void SetRepository(string repositoryName) =>
        _gitHubService.SetRepository(repositoryName);

    public void SetBranch(string branchName) =>
        _gitHubService.SetBranch(branchName);

    public async Task SetAccessTokenAsync(string token) =>
        await _gitHubService.SetAuthTokenAsync(token);
}