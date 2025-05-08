namespace Osirion.Blazor.Cms.Admin.Configuration;

/// <summary>
/// Configuration options for content rules and governance
/// </summary>
public class ContentRulesOptions
{
    /// <summary>
    /// Gets or sets whether content approval is required before publishing
    /// </summary>
    public bool RequireApproval { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum age (in days) for drafts before they're automatically deleted
    /// </summary>
    public int MaximumDraftAge { get; set; } = 30;

    /// <summary>
    /// Gets or sets whether to enforce front matter validation
    /// </summary>
    public bool EnforceFrontMatterValidation { get; set; } = true;

    /// <summary>
    /// Gets or sets required front matter fields
    /// </summary>
    public List<string> RequiredFrontMatterFields { get; set; } = new() { "title" };

    /// <summary>
    /// Gets or sets whether to automatically generate slugs from titles
    /// </summary>
    public bool AutoGenerateSlugs { get; set; } = true;

    /// <summary>
    /// Gets or sets allowed file extensions
    /// </summary>
    public List<string> AllowedFileExtensions { get; set; } = new() { ".md", ".markdown" };

    /// <summary>
    /// Gets or sets whether file deletions are allowed
    /// </summary>
    public bool AllowFileDeletion { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum file size in bytes
    /// </summary>
    public long MaximumFileSize { get; set; } = 1024 * 1024 * 5; // 5MB default
}