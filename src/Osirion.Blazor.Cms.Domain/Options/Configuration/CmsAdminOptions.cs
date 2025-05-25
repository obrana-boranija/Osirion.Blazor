namespace Osirion.Blazor.Cms.Domain.Options.Configuration;

/// <summary>
/// Consolidated configuration options for Osirion CMS Admin
/// </summary>
public class CmsAdminOptions
{
    /// <summary>
    /// The section name in the configuration file
    /// </summary>
    public const string Section = "Osirion:Cms:Github:Admin";

    /// <summary>
    /// Gets or sets the default content provider
    /// </summary>
    public string DefaultContentProvider { get; set; } = "GitHub";

    /// <summary>
    /// Gets or sets whether to persist user selections across sessions
    /// </summary>
    public bool PersistUserSelections { get; set; } = true;

    /// <summary>
    /// Gets or sets GitHub provider options
    /// </summary>
    public GitHubOptions GitHub { get; set; } = new();

    /// <summary>
    /// Gets or sets FileSystem provider options
    /// </summary>
    public FileSystemAdminOptions FileSystem { get; set; } = new();

    /// <summary>
    /// Gets or sets authentication options
    /// </summary>
    public AuthenticationOptions Authentication { get; set; } = new();

    /// <summary>
    /// Gets or sets theme options
    /// </summary>
    public ThemeOptions Theme { get; set; } = new();

    /// <summary>
    /// Gets or sets content rules options
    /// </summary>
    public ContentRulesOptions ContentRules { get; set; } = new();

    /// <summary>
    /// Gets or sets localization options
    /// </summary>
    public LocalizationOptions Localization { get; set; } = new();
}