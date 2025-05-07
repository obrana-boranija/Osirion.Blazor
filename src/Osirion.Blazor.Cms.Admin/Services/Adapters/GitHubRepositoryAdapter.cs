using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using System.Runtime.CompilerServices;
using System.Text;

namespace Osirion.Blazor.Cms.Admin.Services.Adapters;

public class GitHubRepositoryAdapter : IContentRepositoryAdapter
{
    private readonly IGitHubAdminService _gitHubService;
    private readonly ILogger<GitHubRepositoryAdapter> _logger;

    public GitHubRepositoryAdapter(
        IGitHubAdminService gitHubService,
        ILogger<GitHubRepositoryAdapter> logger)
    {
        _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
        _logger = logger;
    }

    public async Task<List<GitHubRepository>> GetRepositoriesAsync()
    {
        LogMethodCall();
        return await _gitHubService.GetRepositoriesAsync();
    }

    public async Task<List<GitHubBranch>> GetBranchesAsync(string repositoryName)
    {
        LogMethodCall(repositoryName);
        return await _gitHubService.GetBranchesAsync(repositoryName);
    }

    public async Task<List<GitHubItem>> GetContentsAsync(string path)
    {
        LogMethodCall(path);
        return await _gitHubService.GetRepositoryContentsAsync(path);
    }

    public async Task<BlogPost> GetBlogPostAsync(string path)
    {
        LogMethodCall(path);
        return await _gitHubService.GetBlogPostAsync(path);
    }

    public async Task<GitHubFileCommitResponse> SaveContentAsync(
        string path, string content, string message, string? sha = null)
    {
        LogMethodCall(path);
        return await _gitHubService.CreateOrUpdateFileAsync(path, content, message, sha);
    }

    public async Task<GitHubFileCommitResponse> DeleteFileAsync(string path, string message, string sha)
    {
        LogMethodCall(path);
        return await _gitHubService.DeleteFileAsync(path, message, sha);
    }

    public async Task<GitHubBranch> CreateBranchAsync(string name, string baseBranch)
    {
        LogMethodCall($"{name} from {baseBranch}");
        return await _gitHubService.CreateBranchAsync(name, baseBranch);
    }

    public void SetRepository(string repositoryName)
    {
        LogMethodCall(repositoryName);
        _gitHubService.SetRepository(repositoryName);
    }

    public void SetBranch(string branchName)
    {
        LogMethodCall(branchName);
        _gitHubService.SetBranch(branchName);
    }

    public async Task SetAccessTokenAsync(string token)
    {
        LogMethodCall();
        await _gitHubService.SetAuthTokenAsync(token);
    }

    public async Task<GitHubPullRequest> CreatePullRequestAsync(
        string title, string body, string head, string baseBranch)
    {
        LogMethodCall($"{head} -> {baseBranch}");
        return await _gitHubService.CreatePullRequestAsync(title, body, head, baseBranch);
    }

    public async Task<List<GitHubItem>> SearchFilesAsync(string query)
    {
        LogMethodCall(query);
        return await _gitHubService.SearchFilesAsync(query);
    }

    private void LogMethodCall([CallerMemberName] string? methodName = null, string? details = null)
    {
        var logMessage = new StringBuilder($"GitHub repository adapter: {methodName}");

        if (!string.IsNullOrEmpty(details))
        {
            logMessage.Append($" - {details}");
        }

        _logger.LogDebug(logMessage.ToString());
    }
}