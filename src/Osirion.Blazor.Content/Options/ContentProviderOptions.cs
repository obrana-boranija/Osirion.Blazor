namespace Osirion.Blazor.Content.Options;

/// <summary>
/// Base options for content providers
/// </summary>
public abstract class ContentProviderOptions
{
    /// <summary>
    /// Gets or sets the unique identifier for the provider
    /// </summary>
    public string? ProviderId { get; set; }

    /// <summary>
    /// Gets or sets whether caching is enabled
    /// </summary>
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    /// Gets or sets the cache duration in minutes
    /// </summary>
    public int CacheDurationMinutes { get; set; } = 30;

    /// <summary>
    /// Gets or sets the supported file extensions
    /// </summary>
    public List<string> SupportedExtensions { get; set; } = new() { ".md", ".markdown" };
}