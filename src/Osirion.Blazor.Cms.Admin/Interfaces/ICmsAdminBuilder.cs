using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osirion.Blazor.Cms.Admin.Interfaces;

/// <summary>
/// Builder interface for configuring CMS admin services
/// </summary>
public interface ICmsAdminBuilder
{
    /// <summary>
    /// Gets the service collection being configured
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Configures GitHub authentication for the CMS admin
    /// </summary>
    /// <param name="clientId">GitHub OAuth client ID</param>
    /// <param name="clientSecret">GitHub OAuth client secret</param>
    /// <returns>The builder for chaining</returns>
    ICmsAdminBuilder UseGitHubAuthentication(string clientId, string clientSecret);

    /// <summary>
    /// Configures general options for the CMS admin
    /// </summary>
    /// <param name="configure">Action to configure admin options</param>
    /// <returns>The builder for chaining</returns>
    ICmsAdminBuilder Configure(Action<CmsAdminOptions> configure);
}