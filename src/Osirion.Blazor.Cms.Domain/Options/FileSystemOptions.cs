namespace Osirion.Blazor.Cms.Domain.Options;

/// <summary>
/// Configuration options for file system content provider
/// </summary>
public class FileSystemOptions : ContentProviderOptions
{
    /// <summary>
    /// The configuration section read when <see cref="WebSection"/> is absent. Kept so existing configuration keeps
    /// working; new configuration should use <see cref="WebSection"/>.
    /// </summary>
    public const string Section = "Osirion:Cms:FileSystem:Web";

    /// <summary>
    /// The configuration section for the FileSystem provider, next to the GitHub providers under
    /// <c>Osirion:Cms:Web</c>. It wins over <see cref="Section"/> when both exist.
    /// </summary>
    public const string WebSection = "Osirion:Cms:Web:FileSystem";

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
    [Obsolete("The file system provider does not poll. Use WatchForChanges to refresh content when files change. Removed in the next major version.")]
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
    [Obsolete("The file system provider does not read include patterns. Use SupportedExtensions to choose the file types to load, and ExcludePatterns to skip paths. Removed in the next major version.")]
    public List<string> IncludePatterns { get; set; } = new() { "**/*.md", "**/*.markdown" };

    /// <summary>
    /// Gets or sets the file glob patterns to exclude
    /// </summary>
    public List<string> ExcludePatterns { get; set; } = new() { "**/node_modules/**", "**/bin/**", "**/obj/**" };

    /// <summary>
    /// Gets or sets the content root path (for URL generation)
    /// </summary>
    public string? ContentRoot { get; set; }

    /// <summary>
    /// Gets or sets whether a file's write time stands in for the content's last-modified date when its front matter
    /// declares no <c>last_modified</c>. Off by default: a fresh checkout gives every file the time of the checkout,
    /// which then reaches <c>dateModified</c>, <c>article:modified_time</c> and the sitemap's <c>lastmod</c> as a date
    /// nobody edited anything. Turn it on only where files are edited in place and their times are kept.
    /// </summary>
    public bool UseFileWriteTimeAsLastModified { get; set; }
}
