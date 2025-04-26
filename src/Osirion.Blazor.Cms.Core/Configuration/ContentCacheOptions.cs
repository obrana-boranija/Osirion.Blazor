namespace Osirion.Blazor.Cms.Configuration;

/// <summary>
/// Configuration options for content cache
/// </summary>
public class ContentCacheOptions
{
    /// <summary>
    /// Gets or sets whether caching is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the default cache duration in minutes
    /// </summary>
    public int DefaultDurationMinutes { get; set; } = 30;

    /// <summary>
    /// Gets or sets whether to set size limits for cached items
    /// </summary>
    public bool SetSizeLimit { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum cache size in bytes (if supported by the cache implementation)
    /// </summary>
    public long MaximumSizeBytes { get; set; } = 104857600; // 100 MB

    /// <summary>
    /// Gets or sets whether to use a distributed cache if available
    /// </summary>
    public bool UseDistributedCache { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to compress cached data (for distributed cache)
    /// </summary>
    public bool CompressData { get; set; } = false;
}