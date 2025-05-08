using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Admin.Interfaces;

/// <summary>
/// Interface for managing content in the admin interface
/// </summary>
public interface IAdminContentService
{
    /// <summary>
    /// Gets content by ID
    /// </summary>
    /// <param name="id">The content ID</param>
    /// <param name="providerId">Optional provider ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The content item or null if not found</returns>
    Task<ContentItem?> GetContentByIdAsync(string id, string? providerId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for content based on query parameters
    /// </summary>
    /// <param name="searchQuery">The search query</param>
    /// <param name="providerId">Optional provider ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of matching content items</returns>
    Task<IReadOnlyList<ContentItem>> SearchContentAsync(ContentQuery searchQuery, string? providerId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates new content
    /// </summary>
    /// <param name="title">The content title</param>
    /// <param name="content">The content body</param>
    /// <param name="path">The content path</param>
    /// <param name="author">Optional author</param>
    /// <param name="description">Optional description</param>
    /// <param name="slug">Optional slug</param>
    /// <param name="tags">Optional tags</param>
    /// <param name="categories">Optional categories</param>
    /// <param name="isFeatured">Whether the content is featured</param>
    /// <param name="providerId">Optional provider ID</param>
    /// <param name="commitMessage">Optional commit message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created content item or null if creation failed</returns>
    Task<ContentItem?> CreateContentAsync(
        string title,
        string content,
        string path,
        string? author = null,
        string? description = null,
        string? slug = null,
        List<string>? tags = null,
        List<string>? categories = null,
        bool isFeatured = false,
        string? providerId = null,
        string? commitMessage = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates existing content
    /// </summary>
    /// <param name="id">The content ID</param>
    /// <param name="title">The content title</param>
    /// <param name="content">The content body</param>
    /// <param name="path">The content path</param>
    /// <param name="author">Optional author</param>
    /// <param name="description">Optional description</param>
    /// <param name="slug">Optional slug</param>
    /// <param name="tags">Optional tags</param>
    /// <param name="categories">Optional categories</param>
    /// <param name="isFeatured">Whether the content is featured</param>
    /// <param name="providerId">Optional provider ID</param>
    /// <param name="commitMessage">Optional commit message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated content item or null if update failed</returns>
    Task<ContentItem?> UpdateContentAsync(
        string id,
        string title,
        string content,
        string path,
        string? author = null,
        string? description = null,
        string? slug = null,
        List<string>? tags = null,
        List<string>? categories = null,
        bool isFeatured = false,
        string? providerId = null,
        string? commitMessage = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes content by ID
    /// </summary>
    /// <param name="id">The content ID</param>
    /// <param name="providerId">Optional provider ID</param>
    /// <param name="commitMessage">Optional commit message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteContentAsync(
        string id,
        string? providerId = null,
        string? commitMessage = null,
        CancellationToken cancellationToken = default);
}