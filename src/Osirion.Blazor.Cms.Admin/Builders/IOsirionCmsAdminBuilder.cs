using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
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
    /// <param name="configure">Optional configuration action</param>
    /// <returns>The builder instance for method chaining</returns>
    IOsirionCmsAdminBuilder UseGitHubProvider(Action<GitHubAdminOptions>? configure = null);

    /// <summary>
    /// Configures FileSystem provider for CMS admin
    /// </summary>
    /// <param name="configure">Optional configuration action</param>
    /// <returns>The builder instance for method chaining</returns>
    IOsirionCmsAdminBuilder UseFileSystemProvider(Action<FileSystemAdminOptions>? configure = null);

    /// <summary>
    /// Configures authentication for CMS admin
    /// </summary>
    /// <param name="configure">Configuration action</param>
    /// <returns>The builder instance for method chaining</returns>
    IOsirionCmsAdminBuilder ConfigureAuthentication(Action<AuthenticationOptions> configure);

    /// <summary>
    /// Configures UI theme for CMS admin
    /// </summary>
    /// <param name="configure">Configuration action</param>
    /// <returns>The builder instance for method chaining</returns>
    IOsirionCmsAdminBuilder ConfigureTheme(Action<ThemeOptions> configure);

    /// <summary>
    /// Configures content rules for CMS admin
    /// </summary>
    /// <param name="configure">Configuration action</param>
    /// <returns>The builder instance for method chaining</returns>
    IOsirionCmsAdminBuilder ConfigureContentRules(Action<ContentRulesOptions> configure);

    /// <summary>
    /// Configures localization for CMS admin
    /// </summary>
    /// <param name="configure">Configuration action</param>
    /// <returns>The builder instance for method chaining</returns>
    IOsirionCmsAdminBuilder UseLocalization(Action<LocalizationOptions> configure);

    /// <summary>
    /// Adds a custom content provider to CMS admin
    /// </summary>
    /// <typeparam name="TProvider">The provider type</typeparam>
    /// <returns>The builder instance for method chaining</returns>
    IOsirionCmsAdminBuilder AddProvider<TProvider>() where TProvider : class, IContentProvider;
}