using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Configuration;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Admin.Builders;

/// <summary>
/// Builder interface for configuring Osirion CMS Admin
/// </summary>
public interface IOsirionCmsAdminBuilder
{
    /// <summary>
    /// Gets the service collection for configuration
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Configures GitHub provider for CMS admin
    /// </summary>
    IOsirionCmsAdminBuilder UseGitHubProvider(Action<GitHubAdminOptions>? configure = null);

    /// <summary>
    /// Configures FileSystem provider for CMS admin
    /// </summary>
    IOsirionCmsAdminBuilder UseFileSystemProvider(Action<FileSystemAdminOptions>? configure = null);

    /// <summary>
    /// Configures authentication for CMS admin
    /// </summary>
    IOsirionCmsAdminBuilder ConfigureAuthentication(Action<AuthenticationOptions> configure);

    /// <summary>
    /// Configures UI theme for CMS admin
    /// </summary>
    IOsirionCmsAdminBuilder ConfigureTheme(Action<ThemeOptions> configure);

    /// <summary>
    /// Configures content rules for CMS admin
    /// </summary>
    IOsirionCmsAdminBuilder ConfigureContentRules(Action<ContentRulesOptions> configure);

    /// <summary>
    /// Adds a custom content provider to CMS admin
    /// </summary>
    IOsirionCmsAdminBuilder AddProvider<TProvider>() where TProvider : class, IContentProvider;
}