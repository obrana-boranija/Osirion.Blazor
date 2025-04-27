using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Domain.Repositories;

/// <summary>
/// Repository interface for directory items
/// </summary>
public interface IDirectoryRepository : IRepository<DirectoryItem, string>
{
    /// <summary>
    /// Gets a directory by its path
    /// </summary>
    /// <param name="path">Directory path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Found directory or null if not found</returns>
    Task<DirectoryItem?> GetByPathAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all directories for a specific locale
    /// </summary>
    /// <param name="locale">Locale code, null for all locales</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of directories</returns>
    Task<IReadOnlyList<DirectoryItem>> GetByLocaleAsync(string? locale = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets child directories for a parent directory
    /// </summary>
    /// <param name="parentId">Parent directory ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of child directories</returns>
    Task<IReadOnlyList<DirectoryItem>> GetChildrenAsync(string parentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a directory recursively with all content
    /// </summary>
    /// <param name="id">Directory ID</param>
    /// <param name="commitMessage">Optional commit message for versioned repositories</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task DeleteRecursiveAsync(string id, string? commitMessage = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a directory by its URL
    /// </summary>
    /// <param name="url">URL of the directory</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Found directory or null if not found</returns>
    Task<DirectoryItem?> GetByUrlAsync(string url, CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves a directory to a new parent
    /// </summary>
    /// <param name="id">Directory ID to move</param>
    /// <param name="newParentId">New parent directory ID, null for root</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated directory</returns>
    Task<DirectoryItem> MoveAsync(string id, string? newParentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the full directory tree
    /// </summary>
    /// <param name="locale">Optional locale filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of root directories with populated child hierarchies</returns>
    Task<IReadOnlyList<DirectoryItem>> GetTreeAsync(string? locale = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the directory cache
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RefreshCacheAsync(CancellationToken cancellationToken = default);
}