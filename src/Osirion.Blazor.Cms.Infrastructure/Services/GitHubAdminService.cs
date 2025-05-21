using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Options.Configuration;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

/// <summary>
/// Implementation of IGitHubAdminService for managing GitHub repositories
/// </summary>
public class GitHubAdminService : IGitHubAdminService
{
    private readonly IGitHubApiClient _apiClient;
    private readonly ILogger<GitHubAdminService> _logger;
    private readonly GitHubOptions _options;

    public GitHubAdminService(
        IGitHubApiClient apiClient,
        IOptions<CmsAdminOptions> options,
        ILogger<GitHubAdminService> logger)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options.Value.GitHub;

        // Initialize API client with configuration
        if (!string.IsNullOrEmpty(_options.Owner) && !string.IsNullOrEmpty(_options.Repository))
        {
            _apiClient.SetRepository(_options.Owner, _options.Repository);
        }

        if (!string.IsNullOrEmpty(_options.Branch))
        {
            _apiClient.SetBranch(_options.Branch);
        }
    }

    public string CurrentBranch { get; private set; } = "main";
    public string CurrentRepository { get; private set; } = string.Empty;

    public async Task<GitHubBranch> CreateBranchAsync(string branchName, string fromBranch)
    {
        try
        {
            return await _apiClient.CreateBranchAsync(branchName, fromBranch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating branch {BranchName} from {FromBranch}", branchName, fromBranch);
            throw;
        }
    }

    public async Task<GitHubFileCommitResponse> CreateOrUpdateFileAsync(string path, string content, string commitMessage, string? existingSha = null)
    {
        try
        {
            return await _apiClient.CreateOrUpdateFileAsync(path, content, commitMessage, existingSha);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating/updating file {Path}", path);
            throw;
        }
    }

    public async Task<GitHubPullRequest> CreatePullRequestAsync(string title, string body, string head, string baseBranch)
    {
        try
        {
            return await _apiClient.CreatePullRequestAsync(title, body, head, baseBranch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating pull request {Title}", title);
            throw;
        }
    }

    public async Task<GitHubFileCommitResponse> DeleteFileAsync(string path, string commitMessage, string sha)
    {
        try
        {
            return await _apiClient.DeleteFileAsync(path, commitMessage, sha);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {Path}", path);
            throw;
        }
    }

    public async Task<BlogPost> GetBlogPostAsync(string path)
    {
        try
        {
            var fileContent = await _apiClient.GetFileContentAsync(path);
            return BlogPost.FromGitHubFile(fileContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting blog post {Path}", path);
            throw;
        }
    }

    public async Task<List<GitHubBranch>> GetBranchesAsync(string repository)
    {
        try
        {
            return await _apiClient.GetBranchesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting branches for {Repository}", repository);
            throw;
        }
    }

    public async Task<GitHubFileContent> GetFileContentAsync(string path)
    {
        try
        {
            return await _apiClient.GetFileContentAsync(path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file content {Path}", path);
            throw;
        }
    }

    public async Task<List<GitHubRepository>> GetRepositoriesAsync()
    {
        try
        {
            return await _apiClient.GetRepositoriesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting repositories");
            throw;
        }
    }

    public async Task<List<GitHubItem>> GetRepositoryContentsAsync(string path = "")
    {
        try
        {
            return await _apiClient.GetRepositoryContentsAsync(path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting repository contents {Path}", path);
            throw;
        }
    }

    public async Task<List<GitHubItem>> SearchFilesAsync(string query)
    {
        try
        {
            var result = await _apiClient.SearchFilesAsync(query);
            return result.Items;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching files with query {Query}", query);
            throw;
        }
    }

    public async Task SetAuthTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Attempted to set empty auth token");
            return;
        }

        _logger.LogInformation("Setting auth token in GitHubAdminService. Token length: {length}", token.Length);
        _apiClient.SetAccessToken(token);

        // Test the token works
        try
        {
            await _apiClient.GetRepositoriesAsync();
            _logger.LogInformation("Token successfully verified");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token validation failed");
        }
    }

    public void SetBranch(string branch)
    {
        CurrentBranch = branch;
        _apiClient.SetBranch(branch);
    }

    public void SetRepository(string repository)
    {
        CurrentRepository = repository;

        // Parse owner/repo format
        var parts = repository.Split('/');
        if (parts.Length == 2)
        {
            _apiClient.SetRepository(parts[0], parts[1]);
        }
        else
        {
            _logger.LogWarning("Invalid repository format: {Repository}. Expected format: owner/repo", repository);
        }
    }
}