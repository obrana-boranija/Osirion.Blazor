using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Infrastructure.Repositories;

/// <summary>
/// Base class for all repositories
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
/// <typeparam name="TId">Entity ID type</typeparam>
public abstract class RepositoryBase<T, TId> : IRepository<T, TId>, IDisposable where T : class
{
    private bool _disposed;
    /// <summary>Performs the Logger operation.</summary>
    protected readonly ILogger Logger;
    /// <summary>Gets or sets the ProviderId value.</summary>
    protected readonly string ProviderId;

    /// <summary>Gets or sets the RepositoryBase value.</summary>
    protected RepositoryBase(string providerId, ILogger logger)
    {
        ProviderId = providerId;
        Logger = logger;
    }

    /// <summary>
    /// Gets all entities
    /// </summary>
    public abstract Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an entity by its ID
    /// </summary>
    public abstract Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds or updates an entity
    /// </summary>
    public abstract Task<T> SaveAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity by its ID
    /// </summary>
    public abstract Task DeleteAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs an operation
    /// </summary>
    protected void LogOperation(string operation, TId? id = default)
    {
        Logger.LogInformation("{Operation} entity of type {EntityType} with ID {Id} in provider {ProviderId}",
            operation, typeof(T).Name, id, ProviderId);
    }

    /// <summary>
    /// Logs an error
    /// </summary>
    protected void LogError(Exception exception, string operation, TId? id = default)
    {
        Logger.LogError(exception, "Error {Operation} entity of type {EntityType} with ID {Id} in provider {ProviderId}: {Message}",
            operation, typeof(T).Name, id, ProviderId, exception.Message);
    }

    /// <summary>Releases resources held by the component or service.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Releases resources held by the component or service.</summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Dispose managed resources
        }

        _disposed = true;
    }
}
