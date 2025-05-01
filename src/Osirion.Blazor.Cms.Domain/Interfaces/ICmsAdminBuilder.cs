using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Admin.Interfaces;

/// <summary>
/// Builder interface for CMS administration features
/// </summary>
public interface ICmsAdminBuilder : ICmsBuilder
{
    /// <summary>
    /// Configures the CMS admin options
    /// </summary>
    /// <param name="configure">The configuration action</param>
    /// <returns>The builder for chaining</returns>
    ICmsAdminBuilder Configure(Action<GitHubOptions> configure);

    /// <summary>
    /// Adds GitHub authentication for the admin panel
    /// </summary>
    /// <param name="configure">The GitHub auth configuration</param>
    /// <returns>The builder for chaining</returns>
    ICmsAdminBuilder AddGitHubAuth(Action<GithubAuthorizationOptions> configure);
}