using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.Repository.Services;

/// <summary>Defines the RepositoryService API contract.</summary>
public class RepositoryService
{
    private readonly IContentRepositoryAdapter _repositoryAdapter;
    private readonly ILogger<RepositoryService> _logger;

    /// <summary>Performs the RepositoryService operation.</summary>
    public RepositoryService(
        IContentRepositoryAdapter repositoryAdapter,
        ILogger<RepositoryService> logger)
    {
        _repositoryAdapter = repositoryAdapter;
        _logger = logger;
    }

    /// <summary>Performs the GetRepositories operation asynchronously.</summary>
    public async Task<List<GitHubRepository>> GetRepositoriesAsync()
    {
        try
        {
            _logger.LogInformation("Fetching repositories");
            var repositories = await _repositoryAdapter.GetRepositoriesAsync();
            _logger.LogInformation("Retrieved {Count} repositories", repositories.Count);
            return repositories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving repositories");
            throw;
        }
    }

    /// <summary>Performs the GetBranches operation asynchronously.</summary>
    public async Task<List<GitHubBranch>> GetBranchesAsync(string repositoryName)
    {
        try
        {
            _logger.LogInformation("Fetching branches for repository: {Repository}", repositoryName);
            var branches = await _repositoryAdapter.GetBranchesAsync(repositoryName);
            _logger.LogInformation("Retrieved {Count} branches for repository: {Repository}",
                branches.Count, repositoryName);
            return branches;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving branches for repository: {Repository}", repositoryName);
            throw;
        }
    }

    /// <summary>Performs the CreateBranch operation asynchronously.</summary>
    public async Task<GitHubBranch> CreateBranchAsync(string branchName, string baseBranch)
    {
        try
        {
            _logger.LogInformation("Creating branch {Branch} from {BaseBranch}", branchName, baseBranch);
            var newBranch = await _repositoryAdapter.CreateBranchAsync(branchName, baseBranch);
            _logger.LogInformation("Branch {Branch} created successfully", branchName);
            return newBranch;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating branch {Branch} from {BaseBranch}", branchName, baseBranch);
            throw;
        }
    }

    /// <summary>Gets or sets the SetRepository value.</summary>
    public void SetRepository(string repositoryName)
    {
        try
        {
            _logger.LogInformation("Setting current repository to: {Repository}", repositoryName);
            _repositoryAdapter.SetRepository(repositoryName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting repository: {Repository}", repositoryName);
            throw;
        }
    }

    /// <summary>Gets or sets the SetBranch value.</summary>
    public void SetBranch(string branchName)
    {
        try
        {
            _logger.LogInformation("Setting current branch to: {Branch}", branchName);
            _repositoryAdapter.SetBranch(branchName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting branch: {Branch}", branchName);
            throw;
        }
    }
}
