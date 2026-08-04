using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using System.Runtime.CompilerServices;

namespace Osirion.Blazor.Cms.Admin.Services.Adapters;

/// <summary>Adapts GitHub content operations to the CMS repository contract.</summary>
public class GitHubRepositoryAdapter : IContentRepositoryAdapter
{
    private readonly IGitHubAdminService _gitHubService;
    private readonly ILogger<GitHubRepositoryAdapter> _logger;

    /// <summary>Initializes a new GitHub repository adapter.</summary>
    public GitHubRepositoryAdapter(
        IGitHubAdminService gitHubService,
        ILogger<GitHubRepositoryAdapter> logger)
    {
        _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
        _logger = logger;
    }

    /// <summary>Gets repositories available to the current account.</summary>
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

    /// <summary>Gets branches for a repository.</summary>
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

    /// <summary>Gets repository contents at a path.</summary>
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

    /// <summary>Searches repository files.</summary>
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

    /// <summary>Gets a blog post by path.</summary>
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

    /// <summary>Saves content to the repository.</summary>
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

    /// <summary>Deletes a file from the repository.</summary>
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

    /// <summary>Creates a branch from a base branch.</summary>
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

    /// <summary>Creates a pull request.</summary>
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

    /// <summary>Sets the repository used by the adapter.</summary>
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

    /// <summary>Sets the branch used by the adapter.</summary>
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

    /// <summary>Sets the access token used by the adapter.</summary>
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
        if (!string.IsNullOrWhiteSpace(details))
        {
            message += $" - {details}";
        }

        _logger.LogDebug(message);
    }

    private void LogError(Exception ex, string? context = null, [CallerMemberName] string? methodName = null)
    {
        var message = $"Error in GitHub repository adapter";

        if (methodName is not null)
        {
            message += $": {methodName}";
        }

        if (context is not null)
        {
            message += $" ({context})";
        }

        _logger.LogError(ex, message);
    }
}
