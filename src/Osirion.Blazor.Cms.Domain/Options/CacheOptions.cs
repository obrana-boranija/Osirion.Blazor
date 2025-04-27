namespace Osirion.Blazor.Cms.Domain.Options;

/// <summary>
/// Configuration options for caching
/// </summary>
public class CacheOptions
{
    /// <summary>
    /// The section name in the configuration file
    /// </summary>
    public const string Section = "Osirion:Cms:Cache";

    /// <summary>
    /// Gets or sets whether caching is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the duration in minutes after which an entry is considered stale
    /// </summary>
    public int StaleTimeMinutes { get; set; } = 5;

    /// <summary>
    /// Gets or sets the maximum age in minutes for cache entries
    /// </summary>
    public int MaxAgeMinutes { get; set; } = 60;

    /// <summary>
    /// Gets or sets whether to use the stale-while-revalidate pattern
    /// </summary>
    public bool UseStaleWhileRevalidate { get; set; } = true;

    /// <summary>
    /// Gets or sets the size limit in MB for the memory cache
    /// </summary>
    public int SizeLimitMB { get; set; } = 64;

    /// <summary>
    /// Gets or sets the compaction percentage when the size limit is reached
    /// </summary>
    public double CompactionPercentage { get; set; } = 0.25;
}