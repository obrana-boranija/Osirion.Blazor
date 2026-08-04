using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Services.Adapters;

/// <summary>
/// Implementation of the content repository adapter using the GitHub admin service
/// </summary>
public class ContentRepositoryAdapter : IContentRepositoryAdapter
{
    private readonly IGitHubAdminService _gitHubService;
    private readonly IAuthenticationService _authService;
    private readonly ILogger<ContentRepositoryAdapter> _logger;

    /// <summary>Initializes a content repository adapter.</summary>
    public ContentRepositoryAdapter(
        IGitHubAdminService gitHubService,
        IAuthenticationService authService,
        ILogger<ContentRepositoryAdapter> logger)
    {
        _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger;

        // If authentication service already has a token, use it
        if (_authService.IsAuthenticated)
        {
            _ = _gitHubService.SetAuthTokenAsync(_authService.AccessToken!);
        }

        // Subscribe to authentication changes
        _authService.AuthenticationChanged += OnAuthenticationChanged;
    }

    /// <summary>Gets repositories available to the authenticated account.</summary>
    public async Task<List<GitHubRepository>> GetRepositoriesAsync()
    {
        try
        {
            _logger.LogInformation("Getting repositories");
            return await _gitHubService.GetRepositoriesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting repositories");
            throw;
        }
    }

    /// <summary>Gets branches for a repository.</summary>
    public async Task<List<GitHubBranch>> GetBranchesAsync(string repositoryName)
    {
        try
        {
            _logger.LogInformation("Getting branches for repository: {Repository}", repositoryName);
            return await _gitHubService.GetBranchesAsync(repositoryName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting branches for repository: {Repository}", repositoryName);
            throw;
        }
    }

    /// <summary>Gets repository contents at a path.</summary>
    public async Task<List<GitHubItem>> GetContentsAsync(string path)
    {
        try
        {
            _logger.LogInformation("Getting contents for path: {Path}", path);
            return await _gitHubService.GetRepositoryContentsAsync(path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contents for path: {Path}", path);
            throw;
        }
    }

    /// <summary>Gets a blog post by path.</summary>
    public async Task<ContentItem> GetBlogPostAsync(string path)
    {
        try
        {
            _logger.LogInformation("Getting blog post: {Path}", path);
            return await _gitHubService.GetBlogPostAsync(path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting blog post: {Path}", path);
            throw;
        }
    }

    /// <summary>Saves content to the repository.</summary>
    public async Task<GitHubFileCommitResponse> SaveContentAsync(string path, string content, string message, string? sha = null)
    {
        try
        {
            _logger.LogInformation("Saving content at path: {Path}", path);
            return await _gitHubService.CreateOrUpdateFileAsync(path, content, message, sha);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving content at path: {Path}", path);
            throw;
        }
    }

    /// <summary>Deletes a file from the repository.</summary>
    public async Task<GitHubFileCommitResponse> DeleteFileAsync(string path, string message, string sha)
    {
        try
        {
            _logger.LogInformation("Deleting file at path: {Path}", path);
            return await _gitHubService.DeleteFileAsync(path, message, sha);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file at path: {Path}", path);
            throw;
        }
    }

    /// <summary>Creates a branch from a base branch.</summary>
    public async Task<GitHubBranch> CreateBranchAsync(string name, string baseBranch)
    {
        try
        {
            _logger.LogInformation("Creating branch: {Name} from {Base}", name, baseBranch);
            return await _gitHubService.CreateBranchAsync(name, baseBranch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating branch: {Name} from {Base}", name, baseBranch);
            throw;
        }
    }

    /// <summary>Creates a pull request.</summary>
    public async Task<GitHubPullRequest> CreatePullRequestAsync(string title, string body, string head, string baseBranch)
    {
        try
        {
            _logger.LogInformation("Creating pull request: {Title}", title);
            return await _gitHubService.CreatePullRequestAsync(title, body, head, baseBranch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating pull request: {Title}", title);
            throw;
        }
    }

    /// <summary>Searches repository files.</summary>
    public async Task<List<GitHubItem>> SearchFilesAsync(string query)
    {
        try
        {
            _logger.LogInformation("Searching files with query: {Query}", query);
            return await _gitHubService.SearchFilesAsync(query);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching files with query: {Query}", query);
            throw;
        }
    }

    /// <summary>Sets the active repository.</summary>
    public void SetRepository(string repositoryName)
    {
        try
        {
            _logger.LogInformation("Setting repository: {Repository}", repositoryName);
            _gitHubService.SetRepository(repositoryName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting repository: {Repository}", repositoryName);
            throw;
        }
    }

    /// <summary>Sets the active branch.</summary>
    public void SetBranch(string branchName)
    {
        try
        {
            _logger.LogInformation("Setting branch: {Branch}", branchName);
            _gitHubService.SetBranch(branchName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting branch: {Branch}", branchName);
            throw;
        }
    }

    /// <summary>Validates and sets the access token.</summary>
    public async Task SetAccessTokenAsync(string token)
    {
        try
        {
            _logger.LogInformation("Setting access token");

            // Use the authentication service to validate and set the token
            var success = await _authService.SetAccessTokenAsync(token);

            if (!success)
            {
                throw new UnauthorizedAccessException("Invalid access token");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting access token");
            throw;
        }
    }

    private async void OnAuthenticationChanged(bool isAuthenticated)
    {
        if (isAuthenticated && !string.IsNullOrWhiteSpace(_authService.AccessToken))
        {
            await _gitHubService.SetAuthTokenAsync(_authService.AccessToken);
        }
    }
}
