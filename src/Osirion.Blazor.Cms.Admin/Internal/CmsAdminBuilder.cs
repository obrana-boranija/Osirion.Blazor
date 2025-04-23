using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Osirion.Blazor.Cms.Admin.Extensions;
using Osirion.Blazor.Cms.Admin.Interfaces;
using Osirion.Blazor.Cms.Admin.Services;

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
        services.TryAddScoped<CmsAdminState>();
        services.TryAddScoped<IGitHubAdminService, GitHubAdminService>();
        services.TryAddScoped<IAuthenticationService, AuthenticationService>();
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

        Services.Configure<CmsAdminOptions>(configure);
        return this;
    }
}