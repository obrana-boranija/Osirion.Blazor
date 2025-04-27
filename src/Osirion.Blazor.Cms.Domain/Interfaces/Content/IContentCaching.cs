namespace Osirion.Blazor.Cms.Domain.Interfaces.Content;

/// <summary>
/// Interface for cache management operations
/// </summary>
public interface IContentCaching
{
    /// <summary>
    /// Refreshes the provider cache
    /// </summary>
    Task RefreshCacheAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Initializes the provider
    /// </summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);
}