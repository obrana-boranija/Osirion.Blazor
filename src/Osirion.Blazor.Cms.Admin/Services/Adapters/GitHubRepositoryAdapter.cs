using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using System.Runtime.CompilerServices;

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
        try
        {
            LogMethodCall();
            return await _gitHubService.GetRepositoriesAsync();
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw;
        }
    }

    public async Task<List<GitHubBranch>> GetBranchesAsync(string repositoryName)
    {
        try
        {
            LogMethodCall(repositoryName);
            return await _gitHubService.GetBranchesAsync(repositoryName);
        }
        catch (Exception ex)
        {
            LogError(ex, $"repository: {repositoryName}");
            throw;
        }
    }

    public async Task<List<GitHubItem>> GetContentsAsync(string path)
    {
        try
        {
            LogMethodCall(path);
            return await _gitHubService.GetRepositoryContentsAsync(path);
        }
        catch (Exception ex)
        {
            LogError(ex, $"path: {path}");
            throw;
        }
    }

    public async Task<List<GitHubItem>> SearchFilesAsync(string query)
    {
        try
        {
            LogMethodCall(query);
            return await _gitHubService.SearchFilesAsync(query);
        }
        catch (Exception ex)
        {
            LogError(ex, $"query: {query}");
            throw;
        }
    }

    public async Task<ContentItem> GetBlogPostAsync(string path)
    {
        try
        {
            LogMethodCall(path);
            return await _gitHubService.GetBlogPostAsync(path);
        }
        catch (Exception ex)
        {
            LogError(ex, $"path: {path}");
            throw;
        }
    }

    public async Task<GitHubFileCommitResponse> SaveContentAsync(
        string path, string content, string message, string? sha = null)
    {
        try
        {
            LogMethodCall(path);
            return await _gitHubService.CreateOrUpdateFileAsync(path, content, message, sha);
        }
        catch (Exception ex)
        {
            LogError(ex, $"path: {path}");
            throw;
        }
    }

    public async Task<GitHubFileCommitResponse> DeleteFileAsync(string path, string message, string sha)
    {
        try
        {
            LogMethodCall(path);
            return await _gitHubService.DeleteFileAsync(path, message, sha);
        }
        catch (Exception ex)
        {
            LogError(ex, $"path: {path}");
            throw;
        }
    }

    public async Task<GitHubBranch> CreateBranchAsync(string name, string baseBranch)
    {
        try
        {
            LogMethodCall($"{name} from {baseBranch}");
            return await _gitHubService.CreateBranchAsync(name, baseBranch);
        }
        catch (Exception ex)
        {
            LogError(ex, $"name: {name}, base: {baseBranch}");
            throw;
        }
    }

    public async Task<GitHubPullRequest> CreatePullRequestAsync(
        string title, string body, string head, string baseBranch)
    {
        try
        {
            LogMethodCall($"{head} -> {baseBranch}");
            return await _gitHubService.CreatePullRequestAsync(title, body, head, baseBranch);
        }
        catch (Exception ex)
        {
            LogError(ex, $"head: {head}, base: {baseBranch}");
            throw;
        }
    }

    public void SetRepository(string repositoryName)
    {
        try
        {
            LogMethodCall(repositoryName);
            _gitHubService.SetRepository(repositoryName);
        }
        catch (Exception ex)
        {
            LogError(ex, $"repository: {repositoryName}");
            throw;
        }
    }

    public void SetBranch(string branchName)
    {
        try
        {
            LogMethodCall(branchName);
            _gitHubService.SetBranch(branchName);
        }
        catch (Exception ex)
        {
            LogError(ex, $"branch: {branchName}");
            throw;
        }
    }

    public async Task SetAccessTokenAsync(string token)
    {
        try
        {
            LogMethodCall();
            await _gitHubService.SetAuthTokenAsync(token);
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw;
        }
    }

    private void LogMethodCall([CallerMemberName] string? methodName = null, string? details = null)
    {
        var message = $"GitHub repository adapter: {methodName}";
        if (!string.IsNullOrEmpty(details))
        {
            message += $" - {details}";
        }

        _logger.LogDebug(message);
    }

    private void LogError(Exception ex, string? context = null, [CallerMemberName] string? methodName = null)
    {
        var message = $"Error in GitHub repository adapter";

        if (methodName != null)
        {
            message += $": {methodName}";
        }

        if (context != null)
        {
            message += $" ({context})";
        }

        _logger.LogError(ex, message);
    }
}