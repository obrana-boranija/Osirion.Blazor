using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Infrastructure.Repositories;

/// <summary>
/// Base class for all repositories
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
/// <typeparam name="TId">Entity ID type</typeparam>
public abstract class RepositoryBase<T, TId> : IRepository<T, TId> where T : class
{
    protected readonly ILogger Logger;
    protected readonly string ProviderId;

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
}