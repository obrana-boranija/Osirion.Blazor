namespace Osirion.Blazor.Cms.Domain.Options;

/// <summary>
/// Configuration options for file system content provider
/// </summary>
public class FileSystemOptions : ContentProviderOptions
{
    /// <summary>
    /// The section name in the configuration file
    /// </summary>
    public const string Section = "Osirion:Cms:FileSystem:Web";

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
    /// Gets or sets the content root path (for URL generation)
    /// </summary>
    public string? ContentRoot { get; set; }
}