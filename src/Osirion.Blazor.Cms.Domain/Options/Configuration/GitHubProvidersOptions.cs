namespace Osirion.Blazor.Cms.Domain.Options.Configuration;

/// <summary>
/// Configuration options for multiple GitHub providers
/// </summary>
public class GitHubProvidersOptions
{
    /// <summary>
    /// Gets or sets the collection of GitHub providers
    /// Key is the provider name, Value is the provider configuration
    /// </summary>
    public Dictionary<string, GitHubProviderOptions> Providers { get; set; } = new();

    /// <summary>
    /// Gets or sets the default provider name
    /// </summary>
    public string? DefaultProvider { get; set; }
}

/// <summary>
/// Configuration options for a single GitHub provider
/// </summary>
public class GitHubProviderOptions : GitHubOptions
{
    /// <summary>
    /// Gets or sets the provider name (used as key in configuration)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this is the default provider
    /// </summary>
    public new bool IsDefault { get; set; }
}