namespace Osirion.Blazor.Cms.Domain.Repositories;

/// <summary>
/// Interface for the Unit of Work pattern
/// </summary>
public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the content repository
    /// </summary>
    IContentRepository ContentRepository { get; }

    /// <summary>
    /// Gets the directory repository
    /// </summary>
    IDirectoryRepository DirectoryRepository { get; }

    /// <summary>
    /// Gets the provider ID associated with this unit of work
    /// </summary>
    string ProviderId { get; }

    /// <summary>
    /// Begins a transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a savepoint within the current transaction
    /// </summary>
    /// <param name="savePointName">Name of the savepoint</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SavePointAsync(string savePointName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back to a savepoint within the current transaction
    /// </summary>
    /// <param name="savePointName">Name of the savepoint</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RollbackToSavePointAsync(string savePointName, CancellationToken cancellationToken = default);
}