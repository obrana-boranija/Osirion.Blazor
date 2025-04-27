namespace Osirion.Blazor.Cms.Domain.Repositories;

/// <summary>
/// Base interface for all repositories
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
/// <typeparam name="TId">Entity ID type</typeparam>
public interface IRepository<T, TId> where T : class
{
    /// <summary>
    /// Gets all entities
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all entities</returns>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an entity by its ID
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Found entity or null if not found</returns>
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds or updates an entity
    /// </summary>
    /// <param name="entity">Entity to save</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Saved entity</returns>
    Task<T> SaveAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity by its ID
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteAsync(TId id, CancellationToken cancellationToken = default);
}