using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Configuration;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Admin.Builders;

/// <summary>
/// Builder interface for configuring CMS admin features
/// </summary>
public interface IAdminBuilder
{
    /// <summary>
    /// Gets the service collection
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Configures GitHub provider for CMS admin
    /// </summary>
    IAdminBuilder UseGitHubProvider(Action<GitHubAdminOptions> configure = null);

    /// <summary>
    /// Configures FileSystem provider for CMS admin
    /// </summary>
    IAdminBuilder UseFileSystemProvider(Action<FileSystemAdminOptions> configure = null);

    /// <summary>
    /// Configures authentication for CMS admin
    /// </summary>
    IAdminBuilder ConfigureAuthentication(Action<AuthenticationOptions> configure);

    /// <summary>
    /// Configures UI theme for CMS admin
    /// </summary>
    IAdminBuilder ConfigureTheme(Action<ThemeOptions> configure);

    /// <summary>
    /// Adds a custom content provider to CMS admin
    /// </summary>
    IAdminBuilder AddProvider<TProvider>() where TProvider : class, IContentProvider;
}