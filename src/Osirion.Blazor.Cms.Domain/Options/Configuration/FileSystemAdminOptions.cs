namespace Osirion.Blazor.Cms.Domain.Options.Configuration;

/// <summary>
/// Configuration options for FileSystem integration
/// </summary>
public class FileSystemAdminOptions
{
    /// <summary>
    /// Gets or sets the root path for content files
    /// </summary>
    public string RootPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content directory name
    /// </summary>
    public string ContentDirectory { get; set; } = "content";

    /// <summary>
    /// Gets or sets whether to create directories if they don't exist
    /// </summary>
    public bool CreateDirectoriesIfNotExist { get; set; } = true;
}