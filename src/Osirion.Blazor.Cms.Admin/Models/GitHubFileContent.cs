using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Admin.Models;

/// <summary>
/// Represents a file content object from GitHub
/// </summary>
public class GitHubFileContent
{
    /// <summary>
    /// Gets or sets the type of the content (file, dir, etc.)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file encoding (usually base64 for files)
    /// </summary>
    public string Encoding { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file size in bytes
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    /// Gets or sets the file name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file path relative to the repository root
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content of the file (usually base64 encoded)
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the SHA hash of the file
    /// </summary>
    public string Sha { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL of the file in the GitHub API
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the download URL of the file
    /// </summary>
    public string DownloadUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Git URL of the file
    /// </summary>
    public string GitUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the HTML URL of the file
    /// </summary>
    public string HtmlUrl { get; set; } = string.Empty;

    /// <summary>
    /// Checks if the file is a markdown file
    /// </summary>
    /// <returns>True if the file is a markdown file</returns>
    public bool IsMarkdownFile()
    {
        return Path.EndsWith(".md") || Path.EndsWith(".markdown");
    }

    /// <summary>
    /// Gets the decoded content as a string
    /// </summary>
    /// <returns>The decoded content</returns>
    public string GetDecodedContent()
    {
        if (string.IsNullOrWhiteSpace(Content))
        {
            return string.Empty;
        }

        if (Encoding.Equals("base64", StringComparison.OrdinalIgnoreCase))
        {
            // Replace any whitespace that might be in the base64 string
            var base64 = Content.Replace("\n", "").Replace("\r", "");
            var bytes = Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        return Content;
    }
}