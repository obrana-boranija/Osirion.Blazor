using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osirion.Blazor.Cms.Core.Options;

/// <summary>
/// Configuration options for the content cache
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
    /// Gets or sets the maximum size of individual cached items in bytes (default 2MB)
    /// </summary>
    public long MaxItemSize { get; set; } = 2 * 1024 * 1024;

    /// <summary>
    /// Gets or sets the memory cache size limit in bytes (default 100MB)
    /// </summary>
    public long MemoryCacheSizeLimit { get; set; } = 100 * 1024 * 1024;
}