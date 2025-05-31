using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Infrastructure.UnitOfWork
{
    /// <summary>
    /// Base implementation of IUnitOfWork
    /// </summary>
    public abstract class BaseUnitOfWork : IUnitOfWork
    {
        private readonly ILogger _logger;
        private readonly IDomainEventDispatcher? _eventDispatcher;
        protected bool _transactionStarted = false;
        protected readonly Dictionary<string, string> _savePoints = new();

        public BaseUnitOfWork(
            IContentRepository contentRepository,
            IDirectoryRepository directoryRepository,
            string providerId,
            ILogger logger,
            IDomainEventDispatcher? eventDispatcher = null)
        {
            ContentRepository = contentRepository ?? throw new ArgumentNullException(nameof(contentRepository));
            DirectoryRepository = directoryRepository ?? throw new ArgumentNullException(nameof(directoryRepository));
            ProviderId = providerId ?? throw new ArgumentNullException(nameof(providerId));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventDispatcher = eventDispatcher;
        }

        public IContentRepository ContentRepository { get; }

        public IDirectoryRepository DirectoryRepository { get; }

        public string ProviderId { get; }

        /// <summary>
        /// Begins a new transaction
        /// </summary>
        public virtual async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transactionStarted)
                throw new InvalidOperationException("Transaction already started");

            _transactionStarted = true;
            _savePoints.Clear();

            _logger.LogInformation("Started transaction for provider {ProviderId}", ProviderId);

            await OnBeginTransactionAsync(cancellationToken);
        }

        /// <summary>
        /// Commits the current transaction
        /// </summary>
        public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (!_transactionStarted)
                throw new InvalidOperationException("No transaction in progress");

            try
            {
                var domainEvents = GetDomainEventsFromTrackedEntities();

                await OnCommitTransactionAsync(cancellationToken);

                _transactionStarted = false;
                _savePoints.Clear();

                _logger.LogInformation("Committed transaction for provider {ProviderId}", ProviderId);

                if (_eventDispatcher is not null)
                {
                    await DispatchDomainEventsAsync(domainEvents);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to commit transaction for provider {ProviderId}", ProviderId);
                throw;
            }
        }

        /// <summary>
        /// Rolls back the current transaction
        /// </summary>
        public virtual async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (!_transactionStarted)
                throw new InvalidOperationException("No transaction in progress");

            try
            {
                await OnRollbackTransactionAsync(cancellationToken);

                _transactionStarted = false;
                _savePoints.Clear();

                _logger.LogInformation("Rolled back transaction for provider {ProviderId}", ProviderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to roll back transaction for provider {ProviderId}", ProviderId);
                throw;
            }
        }

        /// <summary>
        /// Creates a savepoint within the current transaction
        /// </summary>
        public virtual async Task SavePointAsync(string savePointName, CancellationToken cancellationToken = default)
        {
            if (!_transactionStarted)
                throw new InvalidOperationException("No transaction in progress");

            if (string.IsNullOrWhiteSpace(savePointName))
                throw new ArgumentException("Savepoint name cannot be empty", nameof(savePointName));

            if (_savePoints.ContainsKey(savePointName))
                throw new ArgumentException($"Savepoint '{savePointName}' already exists");

            await OnCreateSavePointAsync(savePointName, cancellationToken);

            // Store current savepoint state
            _savePoints[savePointName] = DateTime.UtcNow.Ticks.ToString();

            _logger.LogInformation("Created savepoint {SavePoint} for provider {ProviderId}",
                savePointName, ProviderId);
        }

        /// <summary>
        /// Rolls back to a savepoint within the current transaction
        /// </summary>
        public virtual async Task RollbackToSavePointAsync(string savePointName, CancellationToken cancellationToken = default)
        {
            if (!_transactionStarted)
                throw new InvalidOperationException("No transaction in progress");

            if (!_savePoints.TryGetValue(savePointName, out var _))
                throw new ArgumentException($"Savepoint '{savePointName}' does not exist");

            try
            {
                await OnRollbackToSavePointAsync(savePointName, cancellationToken);

                // Remove savepoints that came after this one
                var savePointsToRemove = new List<string>();
                bool foundCurrentSavepoint = false;

                foreach (var kvp in _savePoints)
                {
                    if (kvp.Key == savePointName)
                    {
                        foundCurrentSavepoint = true;
                        continue;
                    }

                    if (foundCurrentSavepoint)
                    {
                        savePointsToRemove.Add(kvp.Key);
                    }
                }

                foreach (var sp in savePointsToRemove)
                {
                    _savePoints.Remove(sp);
                }

                _logger.LogInformation("Rolled back to savepoint {SavePoint} for provider {ProviderId}",
                    savePointName, ProviderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rolling back to savepoint {SavePoint} for provider {ProviderId}",
                    savePointName, ProviderId);
                throw;
            }
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

            Dispose(true);
            GC.SuppressFinalize(this);
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

            await DisposeAsyncCore();
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// When overridden in a derived class, performs provider-specific transaction begin logic
        /// </summary>
        protected virtual Task OnBeginTransactionAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// When overridden in a derived class, performs provider-specific transaction commit logic
        /// </summary>
        protected virtual Task OnCommitTransactionAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// When overridden in a derived class, performs provider-specific transaction rollback logic
        /// </summary>
        protected virtual Task OnRollbackTransactionAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// When overridden in a derived class, performs provider-specific savepoint creation logic
        /// </summary>
        protected virtual Task OnCreateSavePointAsync(string savePointName, CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// When overridden in a derived class, performs provider-specific savepoint rollback logic
        /// </summary>
        protected virtual Task OnRollbackToSavePointAsync(string savePointName, CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// Gets domain events from tracked entities
        /// </summary>
        protected virtual List<IDomainEvent> GetDomainEventsFromTrackedEntities()
        {
            // Derived classes should override this to collect domain events
            return new List<IDomainEvent>();
        }

        /// <summary>
        /// Dispatches domain events
        /// </summary>
        protected async Task DispatchDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents)
        {
            if (_eventDispatcher is null)
                return;

            foreach (var domainEvent in domainEvents)
            {
                await _eventDispatcher.DispatchAsync(domainEvent);
            }
        }

        /// <summary>
        /// Disposes the unit of work
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Derived classes can override to dispose resources
        }

        /// <summary>
        /// Asynchronously disposes the unit of work
        /// </summary>
        protected virtual ValueTask DisposeAsyncCore()
        {
            // Derived classes can override to dispose resources asynchronously
            return ValueTask.CompletedTask;
        }
    }
}