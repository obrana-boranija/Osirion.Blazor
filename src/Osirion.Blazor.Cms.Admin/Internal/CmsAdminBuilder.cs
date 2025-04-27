using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Interfaces;

namespace Osirion.Blazor.Cms.Admin.Internal;

/// <summary>
/// Implementation of the CMS admin builder
/// </summary>
internal class CmsAdminBuilder : ICmsAdminBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CmsAdminBuilder"/> class.
    /// </summary>
    public CmsAdminBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <inheritdoc/>
    public ICmsAdminBuilder UseGitHubAuthentication(string clientId, string clientSecret)
    {
        //Services.AddGitHubAuthentication(clientId, clientSecret);
        return this;
    }

    /// <inheritdoc/>
    public ICmsAdminBuilder Configure(Action<CmsAdminOptions> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        Services.Configure(configure);
        return this;
    }
}