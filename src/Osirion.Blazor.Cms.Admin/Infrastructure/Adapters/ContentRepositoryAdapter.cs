using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;

/// <summary>
/// Implementation of the content repository adapter using the GitHub admin service
/// </summary>
public class ContentRepositoryAdapter : IContentRepositoryAdapter
{
    private readonly IGitHubAdminService _gitHubService;
    private readonly ILogger<ContentRepositoryAdapter> _logger;

    public ContentRepositoryAdapter(
        IGitHubAdminService gitHubService,
        ILogger<ContentRepositoryAdapter> logger)
    {
        _gitHubService = gitHubService;
        _logger = logger;
    }

    public async Task<List<GitHubRepository>> GetRepositoriesAsync()
    {
        _logger.LogInformation("Getting repositories");
        return await _gitHubService.GetRepositoriesAsync();
    }

    public async Task<List<GitHubBranch>> GetBranchesAsync(string repositoryName)
    {
        _logger.LogInformation("Getting branches for repository: {Repository}", repositoryName);
        return await _gitHubService.GetBranchesAsync(repositoryName);
    }

    public async Task<List<GitHubItem>> GetContentsAsync(string path)
    {
        _logger.LogInformation("Getting contents for path: {Path}", path);
        return await _gitHubService.GetRepositoryContentsAsync(path);
    }

    public async Task<BlogPost> GetBlogPostAsync(string path)
    {
        _logger.LogInformation("Getting blog post: {Path}", path);
        return await _gitHubService.GetBlogPostAsync(path);
    }

    public async Task<GitHubFileCommitResponse> SaveContentAsync(string path, string content, string message, string? sha = null)
    {
        _logger.LogInformation("Saving content at path: {Path}", path);
        return await _gitHubService.CreateOrUpdateFileAsync(path, content, message, sha);
    }

    public async Task<GitHubFileCommitResponse> DeleteFileAsync(string path, string message, string sha)
    {
        _logger.LogInformation("Deleting file at path: {Path}", path);
        return await _gitHubService.DeleteFileAsync(path, message, sha);
    }

    public async Task<GitHubBranch> CreateBranchAsync(string name, string baseBranch)
    {
        _logger.LogInformation("Creating branch: {Name} from {Base}", name, baseBranch);
        return await _gitHubService.CreateBranchAsync(name, baseBranch);
    }

    public async Task<GitHubPullRequest> CreatePullRequestAsync(string title, string body, string head, string baseBranch)
    {
        _logger.LogInformation("Creating pull request: {Title}", title);
        return await _gitHubService.CreatePullRequestAsync(title, body, head, baseBranch);
    }

    public async Task<List<GitHubItem>> SearchFilesAsync(string query)
    {
        _logger.LogInformation("Searching files with query: {Query}", query);
        return await _gitHubService.SearchFilesAsync(query);
    }

    public void SetRepository(string repositoryName)
    {
        _logger.LogInformation("Setting repository: {Repository}", repositoryName);
        _gitHubService.SetRepository(repositoryName);
    }

    public void SetBranch(string branchName)
    {
        _logger.LogInformation("Setting branch: {Branch}", branchName);
        _gitHubService.SetBranch(branchName);
    }

    public async Task SetAccessTokenAsync(string token)
    {
        _logger.LogInformation("Setting access token");
        await _gitHubService.SetAuthTokenAsync(token);
    }
}