using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Infrastructure.UnitOfWork
{
    /// <summary>
    /// GitHub implementation of the Unit of Work pattern
    /// </summary>
    public class GitHubUnitOfWork : BaseUnitOfWork
    {
        private readonly IGitHubApiClient _apiClient;
        private string? _temporaryBranch = null;
        private string _originalBranch;

        public GitHubUnitOfWork(
            IGitHubApiClient apiClient,
            IContentRepository contentRepository,
            IDirectoryRepository directoryRepository,
            IDomainEventDispatcher eventDispatcher,
            ILogger<GitHubUnitOfWork> logger)
            : base(contentRepository, directoryRepository, "github", logger, eventDispatcher)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

            // Get the current branch from the API client
            _originalBranch = "main"; // Default value, should be retrieved from the API client
        }

        protected override async Task OnBeginTransactionAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Create a temporary branch for our transaction
                _temporaryBranch = $"temp-{Guid.NewGuid():N}";
                await _apiClient.CreateBranchAsync(
                    _temporaryBranch,
                    _originalBranch,
                    cancellationToken);

                // Switch to the temporary branch
                _apiClient.SetBranch(_temporaryBranch);
            }
            catch (Exception ex)
            {
                _transactionStarted = false;
                throw new InvalidOperationException("Failed to start GitHub transaction", ex);
            }
        }

        protected override async Task OnCommitTransactionAsync(CancellationToken cancellationToken)
        {
            if (_temporaryBranch == null)
                throw new InvalidOperationException("No temporary branch created");

            // Create a pull request to merge changes
            await _apiClient.CreatePullRequestAsync(
                "Automatic commit from CMS",
                "This pull request was created automatically from a transaction.",
                _temporaryBranch,
                _originalBranch,
                cancellationToken);

            // Switch back to the original branch
            _apiClient.SetBranch(_originalBranch);
            _temporaryBranch = null;
        }

        protected override Task OnRollbackTransactionAsync(CancellationToken cancellationToken)
        {
            // Simply discard changes by switching back to original branch
            _apiClient.SetBranch(_originalBranch);
            _temporaryBranch = null;

            return Task.CompletedTask;
        }

        // GitHub doesn't natively support savepoints
        protected override Task OnCreateSavePointAsync(string savePointName, CancellationToken cancellationToken)
        {
            // Not implemented for GitHub
            return Task.CompletedTask;
        }

        protected override Task OnRollbackToSavePointAsync(string savePointName, CancellationToken cancellationToken)
        {
            // Not implemented for GitHub
            return Task.CompletedTask;
        }
    }
}