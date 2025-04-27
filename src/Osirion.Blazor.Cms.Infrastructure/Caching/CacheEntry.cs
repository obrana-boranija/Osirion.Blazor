namespace Osirion.Blazor.Cms.Infrastructure.Caching;

/// <summary>
/// Represents a cache entry with metadata about when it was last updated
/// </summary>
/// <typeparam name="T">Type of cached value</typeparam>
public class CacheEntry<T>
{
    /// <summary>
    /// Gets or sets the cached value
    /// </summary>
    public T Value { get; set; } = default!;

    /// <summary>
    /// Gets or sets when the entry was last updated
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Gets or sets whether the entry is being refreshed
    /// </summary>
    public bool IsRefreshing { get; set; }
}