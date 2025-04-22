namespace Osirion.Blazor.Cms.Options;

/// <summary>
/// Configuration options for file system content provider
/// </summary>
public class FileSystemContentOptions : ContentProviderOptions
{
    /// <summary>
    /// Gets or sets the base path for content files
    /// </summary>
    public string BasePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to watch for file changes
    /// </summary>
    public bool WatchForChanges { get; set; } = false;
}