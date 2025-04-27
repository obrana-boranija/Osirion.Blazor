using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Infrastructure.UnitOfWork;

/// <summary>
/// GitHub implementation of the Unit of Work pattern
/// </summary>
public class GitHubUnitOfWork : IUnitOfWork
{
    private readonly IGitHubApiClient _apiClient;
    private readonly IContentRepository _contentRepository;
    private readonly IDirectoryRepository _directoryRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<GitHubUnitOfWork> _logger;
    private bool _transactionStarted = false;
    private string? _temporaryBranch = null;
    private string _originalBranch;

    public GitHubUnitOfWork(
        IGitHubApiClient apiClient,
        IContentRepository contentRepository,
        IDirectoryRepository directoryRepository,
        IDomainEventDispatcher eventDispatcher,
        ILogger<GitHubUnitOfWork> logger)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _contentRepository = contentRepository ?? throw new ArgumentNullException(nameof(contentRepository));
        _directoryRepository = directoryRepository ?? throw new ArgumentNullException(nameof(directoryRepository));
        _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Get the current branch from the API client
        _originalBranch = "main"; // Default value, should be retrieved from the API client
    }

    public GitHubUnitOfWork(IGitHubApiClient apiClient, IContentRepository contentRepository, IDirectoryRepository directoryRepository, ILogger<GitHubUnitOfWork> logger)
    {
        _apiClient = apiClient;
        _contentRepository = contentRepository;
        _directoryRepository = directoryRepository;
        _logger = logger;
    }

    public IContentRepository ContentRepository => _contentRepository;

    public IDirectoryRepository DirectoryRepository => _directoryRepository;

    public string ProviderId => "github";

    private async Task DispatchDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await _eventDispatcher.DispatchAsync(domainEvent);
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transactionStarted)
            throw new InvalidOperationException("Transaction already started");

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

            _transactionStarted = true;
            _logger.LogInformation("Started transaction on branch {Branch}", _temporaryBranch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start transaction");
            throw;
        }
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (!_transactionStarted)
            throw new InvalidOperationException("No transaction in progress");

        if (_temporaryBranch == null)
            throw new InvalidOperationException("No temporary branch created");

        try
        {
            var domainEvents = GetDomainEventsFromTrackedEntities();

            await _apiClient.CreatePullRequestAsync(
                "Automatic commit from CMS",
                "This pull request was created automatically from a transaction.",
                _temporaryBranch,
                _originalBranch,
                cancellationToken);

            _apiClient.SetBranch(_originalBranch);

            _transactionStarted = false;
            _logger.LogInformation("Committed transaction from branch {Branch}", _temporaryBranch);
            _temporaryBranch = null;

            await DispatchDomainEventsAsync(domainEvents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to commit transaction from branch {Branch}", _temporaryBranch);
            throw;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (!_transactionStarted)
            throw new InvalidOperationException("No transaction in progress");

        try
        {
            // Simply discard changes by switching back to original branch
            _apiClient.SetBranch(_originalBranch);

            // We could also delete the temporary branch if the API supports it

            _transactionStarted = false;
            _logger.LogInformation("Rolled back transaction on branch {Branch}", _temporaryBranch);
            _temporaryBranch = null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to roll back transaction on branch {Branch}", _temporaryBranch);
            throw;
        }
    }

    public async Task SavePointAsync(string savePointName, CancellationToken cancellationToken = default)
    {
        // GitHub doesn't natively support savepoints, but we could implement
        // by creating commits with specific messages
        if (!_transactionStarted)
            throw new InvalidOperationException("No transaction in progress");

        _logger.LogWarning("Savepoints are not fully implemented for GitHub repositories");

        // Create a commit with a savepoint message
        // This would require modifying the GitHubApiClient to support creating commits directly

        await Task.CompletedTask;
    }

    public async Task RollbackToSavePointAsync(string savePointName, CancellationToken cancellationToken = default)
    {
        if (!_transactionStarted)
            throw new InvalidOperationException("No transaction in progress");

        _logger.LogWarning("Savepoints are not fully implemented for GitHub repositories");

        // This would require modifying the GitHubApiClient to support reverting to specific commits

        await Task.CompletedTask;
    }

    public void Dispose()
    {
        // If transaction is still active, roll it back
        if (_transactionStarted)
        {
            try
            {
                _logger.LogWarning("Transaction still active during disposal, rolling back");
                RollbackAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rolling back transaction during disposal");
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        // If transaction is still active, roll it back
        if (_transactionStarted)
        {
            try
            {
                _logger.LogWarning("Transaction still active during async disposal, rolling back");
                await RollbackAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rolling back transaction during async disposal");
            }
        }
    }

    private List<IDomainEvent> GetDomainEventsFromTrackedEntities()
    {
        // This is a simplified implementation - we need to track entities during the transaction
        var events = new List<IDomainEvent>();

        // Placeholder for collecting events from tracked entities
        // We need to track entities modified during the transaction
        // and collect their domain events here

        return events;
    }
}