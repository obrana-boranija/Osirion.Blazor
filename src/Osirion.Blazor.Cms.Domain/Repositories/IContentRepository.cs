using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Domain.Repositories;

/// <summary>
/// Repository interface for content items
/// </summary>
public interface IContentRepository : IRepository<ContentItem, string>
{
    /// <summary>
    /// Gets a content item by its path
    /// </summary>
    /// <param name="path">Content path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Found content item or null if not found</returns>
    Task<ContentItem?> GetByPathAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a content item by its URL
    /// </summary>
    /// <param name="url">Content URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Found content item or null if not found</returns>
    Task<ContentItem?> GetByUrlAsync(string url, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds content items by query
    /// </summary>
    /// <param name="query">Content query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching content items</returns>
    Task<IReadOnlyList<ContentItem>> FindByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets content items in a directory
    /// </summary>
    /// <param name="directoryId">Directory ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of content items in the directory</returns>
    Task<IReadOnlyList<ContentItem>> GetByDirectoryAsync(string directoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all content translations for a content ID
    /// </summary>
    /// <param name="contentId">Content ID shared across translations</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of content translations</returns>
    Task<IReadOnlyList<ContentItem>> GetTranslationsAsync(string contentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all content tags
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of content tags</returns>
    Task<IReadOnlyList<ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all content categories
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of content categories</returns>
    //Task<IReadOnlyList<ContentCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves a content item with a commit message (for versioned repositories)
    /// </summary>
    /// <param name="entity">Content item to save</param>
    /// <param name="commitMessage">Commit message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Saved content item</returns>
    Task<ContentItem> SaveWithCommitMessageAsync(ContentItem entity, string commitMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a content item with a commit message (for versioned repositories)
    /// </summary>
    /// <param name="id">Content ID</param>
    /// <param name="commitMessage">Commit message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteWithCommitMessageAsync(string id, string commitMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the repository cache
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RefreshCacheAsync(CancellationToken cancellationToken = default);
}