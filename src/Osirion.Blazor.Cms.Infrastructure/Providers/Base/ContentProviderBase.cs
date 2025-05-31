using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Infrastructure.Providers;

/// <summary>
/// Base class for content providers (simplified - no redundant caching)
/// </summary>
public abstract class ContentProviderBase : IContentProvider, IDisposable
{
    protected readonly ILogger Logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentProviderBase"/> class.
    /// </summary>
    /// <param name="memoryCache">The memory cache (kept for compatibility but not used for main data)</param>
    /// <param name="options">Provider options</param>
    /// <param name="logger">The logger</param>
    protected ContentProviderBase(
        IMemoryCache memoryCache,
        IOptions<ContentProviderOptions> options,
        ILogger logger)
    {
        // Keep parameters for compatibility but don't use them for main caching
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public abstract string ProviderId { get; }

    /// <inheritdoc/>
    public abstract string DisplayName { get; }

    /// <inheritdoc/>
    public abstract bool IsReadOnly { get; }

    /// <inheritdoc/>
    public abstract Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<ContentItem?> GetItemByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<ContentItem?> GetItemByUrlAsync(string url, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<IReadOnlyList<ContentItem>> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<IReadOnlyList<ContentCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<IReadOnlyList<ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<IReadOnlyList<DirectoryItem>> GetDirectoriesAsync(string? locale = null, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<DirectoryItem?> GetDirectoryByPathAsync(string path, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<DirectoryItem?> GetDirectoryByIdAsync(string id, string? locale = null, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<DirectoryItem?> GetDirectoryByUrlAsync(string url, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyList<ContentItem>?> GetContentTranslationsAsync(string localizationId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(localizationId))
            return Array.Empty<ContentItem>();

        try
        {
            var query = new ContentQuery { LocalizationId = localizationId };
            return await GetItemsByQueryAsync(query, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving translations for localization ID {LocalizationId}", localizationId);
            return Array.Empty<ContentItem>();
        }
    }

    /// <inheritdoc/>
    public virtual async Task<Dictionary<string, ContentItem>> GetContentTranslationsAsync(string localizationId)
    {
        if (string.IsNullOrWhiteSpace(localizationId))
        {
            return new Dictionary<string, ContentItem>();
        }

        try
        {
            var translations = await GetContentTranslationsAsync(localizationId, CancellationToken.None);
            if (translations is null)
                return new Dictionary<string, ContentItem>();

            return translations.ToDictionary(item => item.Locale, item => item);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving translations dictionary for localization ID {LocalizationId}", localizationId);
            return new Dictionary<string, ContentItem>();
        }
    }

    /// <inheritdoc/>
    public virtual async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        // Base implementation does nothing - derived classes handle their own cache refresh
        Logger.LogDebug("RefreshCacheAsync called on base provider: {ProviderId}", ProviderId);
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Initializing content provider: {ProviderId}", ProviderId);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Executes an operation with exception handling
    /// </summary>
    protected async Task<T?> ExecuteWithExceptionHandlingAsync<T>(Func<Task<T?>> operation, string operationName)
    {
        try
        {
            return await operation();
        }
        catch (ContentProviderException)
        {
            // Re-throw content provider exceptions
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in {OperationName} for provider {ProviderId}: {Message}",
                operationName, ProviderId, ex.Message);

            throw new ContentProviderException(
                $"Error in {operationName} for provider {DisplayName}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Disposes resources used by the provider
    /// </summary>
    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes resources used by the provider
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Dispose managed resources in derived classes
        }

        _disposed = true;
    }
}