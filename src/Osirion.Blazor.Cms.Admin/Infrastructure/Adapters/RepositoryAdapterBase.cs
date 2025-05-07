using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Services.Adapters;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using System.Runtime.CompilerServices;

namespace Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;

public abstract class RepositoryAdapterBase : IContentRepositoryAdapter
{
    protected readonly ILogger _logger;

    protected RepositoryAdapterBase(ILogger logger)
    {
        _logger = logger;
    }

    public abstract Task<List<GitHubRepository>> GetRepositoriesAsync();

    public abstract Task<List<GitHubBranch>> GetBranchesAsync(string repositoryName);

    public abstract Task<List<GitHubItem>> GetContentsAsync(string path);

    public abstract Task<BlogPost> GetBlogPostAsync(string path);

    public abstract Task<GitHubFileCommitResponse> SaveContentAsync(
        string path, string content, string message, string? sha = null);

    public abstract Task<GitHubFileCommitResponse> DeleteFileAsync(
        string path, string message, string sha);

    public abstract Task<GitHubBranch> CreateBranchAsync(string name, string baseBranch);

    public abstract void SetRepository(string repositoryName);

    public abstract void SetBranch(string branchName);

    public abstract Task SetAccessTokenAsync(string token);

    protected void LogOperation(string operation, string? details = null, [CallerMemberName] string? methodName = null)
    {
        var message = $"{GetType().Name}: {methodName} - {operation}";
        if (!string.IsNullOrEmpty(details))
        {
            message += $" ({details})";
        }
        _logger.LogDebug(message);
    }
}