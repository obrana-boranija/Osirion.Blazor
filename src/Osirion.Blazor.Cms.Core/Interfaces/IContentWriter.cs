using Osirion.Blazor.Cms.Models;

namespace Osirion.Blazor.Cms.Interfaces;

/// <summary>
/// Interface for content providers that support write operations
/// </summary>
public interface IContentWriter
{
    /// <summary>
    /// Creates a new content item
    /// </summary>
    /// <param name="item">The content item to create</param>
    /// <param name="commitMessage">Optional commit message for versioned providers</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created content item with updated metadata</returns>
    Task<ContentItem> CreateContentAsync(ContentItem item, string? commitMessage = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing content item
    /// </summary>
    /// <param name="item">The content item to update</param>
    /// <param name="commitMessage">Optional commit message for versioned providers</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated content item with updated metadata</returns>
    Task<ContentItem> UpdateContentAsync(ContentItem item, string? commitMessage = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a content item
    /// </summary>
    /// <param name="id">The ID of the content item to delete</param>
    /// <param name="commitMessage">Optional commit message for versioned providers</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteContentAsync(string id, string? commitMessage = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new directory
    /// </summary>
    /// <param name="directory">The directory to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created directory with updated metadata</returns>
    Task<DirectoryItem> CreateDirectoryAsync(DirectoryItem directory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing directory
    /// </summary>
    /// <param name="directory">The directory to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated directory with updated metadata</returns>
    Task<DirectoryItem> UpdateDirectoryAsync(DirectoryItem directory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a directory
    /// </summary>
    /// <param name="id">The ID of the directory to delete</param>
    /// <param name="recursive">Whether to recursively delete all contents</param>
    /// <param name="commitMessage">Optional commit message for versioned providers</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteDirectoryAsync(string id, bool recursive = false, string? commitMessage = null, CancellationToken cancellationToken = default);
}