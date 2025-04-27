// Create src/Osirion.Blazor.Cms.Infrastructure/Options/FileSystemOptions.cs

namespace Osirion.Blazor.Cms.Infrastructure.Options;

/// <summary>
/// Configuration options for file system content provider
/// </summary>
public class FileSystemOptions
{
    /// <summary>
    /// Gets or sets the base path for content files
    /// </summary>
    public string BasePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to watch for file changes and automatically update cache
    /// </summary>
    public bool WatchForChanges { get; set; } = true;

    /// <summary>
    /// Gets or sets the polling interval in milliseconds when file system watchers are not available
    /// </summary>
    public int PollingIntervalMs { get; set; } = 30000;

    /// <summary>
    /// Gets or sets whether to include subdirectories when searching for content
    /// </summary>
    public bool IncludeSubdirectories { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to create directories if they don't exist
    /// </summary>
    public bool CreateDirectoriesIfNotExist { get; set; } = false;

    /// <summary>
    /// Gets or sets the file glob patterns to include
    /// </summary>
    public List<string> IncludePatterns { get; set; } = new() { "**/*.md", "**/*.markdown" };

    /// <summary>
    /// Gets or sets the file glob patterns to exclude
    /// </summary>
    public List<string> ExcludePatterns { get; set; } = new() { "**/node_modules/**", "**/bin/**", "**/obj/**" };

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

    /// <summary>
    /// Gets or sets whether the provider is the default provider
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Gets or sets whether localization is enabled
    /// </summary>
    public bool EnableLocalization { get; set; } = false;

    /// <summary>
    /// Gets or sets the default locale to use when localization is disabled
    /// </summary>
    public string DefaultLocale { get; set; } = "en";

    /// <summary>
    /// Gets or sets the supported locales
    /// </summary>
    public List<string> SupportedLocales { get; set; } = new() { "en" };

    /// <summary>
    /// Gets or sets the content root path (for URL generation)
    /// </summary>
    public string? ContentRoot { get; set; }

    /// <summary>
    /// Gets or sets whether to validate content on write operations
    /// </summary>
    public bool ValidateContent { get; set; } = true;
}