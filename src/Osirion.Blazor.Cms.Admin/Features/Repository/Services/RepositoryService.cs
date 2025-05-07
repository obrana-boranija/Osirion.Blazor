using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Services.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.Repository.Services;

public class RepositoryService
{
    private readonly IContentRepositoryAdapter _repositoryAdapter;
    private readonly CmsEventMediator _eventMediator;
    private readonly ILogger<RepositoryService> _logger;

    public RepositoryService(
        IContentRepositoryAdapter repositoryAdapter,
        CmsEventMediator eventMediator,
        ILogger<RepositoryService> logger)
    {
        _repositoryAdapter = repositoryAdapter;
        _eventMediator = eventMediator;
        _logger = logger;
    }

    public async Task<List<GitHubRepository>> GetRepositoriesAsync()
    {
        try
        {
            _logger.LogInformation("Fetching repositories");
            var repositories = await _repositoryAdapter.GetRepositoriesAsync();
            _logger.LogInformation("Retrieved {Count} repositories", repositories.Count);

            // Publish event with all repositories
            _eventMediator.Publish(new RepositoryChangedEvent(repositories));

            return repositories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving repositories");
            _eventMediator.Publish(new ErrorOccurredEvent("Failed to load repositories", ex));
            throw;
        }
    }

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
            _eventMediator.Publish(new ErrorOccurredEvent($"Failed to load branches for {repositoryName}", ex));
            throw;
        }
    }

    public async Task<GitHubBranch> CreateBranchAsync(string branchName, string baseBranch)
    {
        try
        {
            _logger.LogInformation("Creating branch {Branch} from {BaseBranch}", branchName, baseBranch);
            var newBranch = await _repositoryAdapter.CreateBranchAsync(branchName, baseBranch);
            _logger.LogInformation("Branch {Branch} created successfully", branchName);

            _eventMediator.Publish(new StatusNotificationEvent(
                $"Branch {branchName} created successfully", StatusType.Success));

            return newBranch;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating branch {Branch} from {BaseBranch}", branchName, baseBranch);
            _eventMediator.Publish(new ErrorOccurredEvent($"Failed to create branch {branchName}", ex));
            throw;
        }
    }

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
            _eventMediator.Publish(new ErrorOccurredEvent($"Failed to set repository {repositoryName}", ex));
            throw;
        }
    }

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
            _eventMediator.Publish(new ErrorOccurredEvent($"Failed to set branch {branchName}", ex));
            throw;
        }
    }

    public async Task<GitHubPullRequest> CreatePullRequestAsync(
        string title,
        string body,
        string headBranch,
        string baseBranch)
    {
        try
        {
            _logger.LogInformation("Creating pull request from {Head} to {Base}: {Title}",
                headBranch, baseBranch, title);

            var pullRequest = await _repositoryAdapter.CreatePullRequestAsync(
                title, body, headBranch, baseBranch);

            _logger.LogInformation("Pull request created successfully: {Url}",
                pullRequest.HtmlUrl);

            _eventMediator.Publish(new StatusNotificationEvent(
                "Pull request created successfully", StatusType.Success));

            return pullRequest;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating pull request from {Head} to {Base}",
                headBranch, baseBranch);

            _eventMediator.Publish(new ErrorOccurredEvent(
                $"Failed to create pull request from {headBranch} to {baseBranch}", ex));

            throw;
        }
    }
}