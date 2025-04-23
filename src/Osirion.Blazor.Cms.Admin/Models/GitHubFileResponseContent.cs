namespace Osirion.Blazor.Cms.Admin.Models;

/// <summary>
/// File content information in a GitHub commit response
/// </summary>
public class GitHubFileResponseContent
{
    /// <summary>
    /// Gets or sets the file name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file path
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the SHA hash of the file
    /// </summary>
    public string Sha { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the size of the file in bytes
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    /// Gets or sets the URL of the file
    /// </summary>
    public string Url { get; set; } = string.Empty;
}
