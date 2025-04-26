using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Caching;
using Osirion.Blazor.Cms.Interfaces;
using Osirion.Blazor.Cms.Models;

namespace Osirion.Blazor.Cms.Providers.Base;

/// <summary>
/// Base class for writable content providers that implement IContentWriter
/// </summary>
public abstract class WritableContentProviderBase : ContentProviderBase, IContentWriter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WritableContentProviderBase"/> class.
    /// </summary>
    protected WritableContentProviderBase(
        IMemoryCache cacheService,
        ILogger logger)
        : base(cacheService, logger)
    {
    }

    /// <inheritdoc/>
    public override bool IsReadOnly => false;

    /// <summary>
    /// Creates or updates content based on whether it already exists
    /// </summary>
    protected abstract Task<ContentItem> CreateOrUpdateContentInternalAsync(
        ContentItem item,
        string? commitMessage,
        CancellationToken cancellationToken);

    /// <inheritdoc/>
    public virtual async Task<ContentItem> CreateContentAsync(
        ContentItem item,
        string? commitMessage = null,
        CancellationToken cancellationToken = default)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        // Set provider ID
        item.ProviderId = ProviderId;

        // Validate there's no existing item with the same path
        var existingItem = await GetItemByPathAsync(item.Path, cancellationToken);
        if (existingItem != null)
        {
            throw new InvalidOperationException($"Content with path '{item.Path}' already exists. Use UpdateContentAsync instead.");
        }

        // Create the content
        var result = await CreateOrUpdateContentInternalAsync(item, commitMessage, cancellationToken);

        // Refresh cache
        await RefreshCacheAsync(cancellationToken);

        return result;
    }

    /// <inheritdoc/>
    public virtual async Task<ContentItem> UpdateContentAsync(
        ContentItem item,
        string? commitMessage = null,
        CancellationToken cancellationToken = default)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        // Set provider ID
        item.ProviderId = ProviderId;

        // Validate that the item exists
        var existingItem = item.Id != null
            ? await GetItemByIdAsync(item.Id, cancellationToken)
            : await GetItemByPathAsync(item.Path, cancellationToken);

        if (existingItem == null)
        {
            throw new InvalidOperationException($"Content not found. Cannot update non-existent content.");
        }

        // Update the content
        var result = await CreateOrUpdateContentInternalAsync(item, commitMessage, cancellationToken);

        // Refresh cache
        await RefreshCacheAsync(cancellationToken);

        return result;
    }

    /// <inheritdoc/>
    public abstract Task DeleteContentAsync(
        string id,
        string? commitMessage = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<DirectoryItem> CreateDirectoryAsync(
        DirectoryItem directory,
        CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<DirectoryItem> UpdateDirectoryAsync(
        DirectoryItem directory,
        CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task DeleteDirectoryAsync(
        string id,
        bool recursive = false,
        string? commitMessage = null,
        CancellationToken cancellationToken = default);
}