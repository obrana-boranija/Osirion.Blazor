using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Osirion.Blazor.Cms.Admin.Interfaces;
using Osirion.Blazor.Cms.Admin.Internal;
using Osirion.Blazor.Cms.Admin.Options;
using Osirion.Blazor.Cms.Admin.Services;

namespace Osirion.Blazor.Cms.Admin.Extensions;

/// <summary>
/// Extension methods for adding Osirion.Blazor.Cms.Admin services
/// </summary>
public static class CmsAdminServiceCollectionExtensions
{
    /// <summary>
    /// Adds Osirion.Blazor.Cms.Admin services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Action to configure CMS admin services</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionCmsAdmin(
        this IServiceCollection services,
        Action<ICmsAdminBuilder> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        // Create builder and apply configuration
        var builder = new CmsAdminBuilder(services);
        configure(builder);

        services.TryAddScoped<IStateStorageService, SSRFriendlyStorageService>();
        services.TryAddScoped<CmsAdminState>();
        services.TryAddScoped<IGitHubAdminService, GitHubAdminService>();
        services.TryAddScoped<IAuthenticationService, AuthenticationService>();
        services.TryAddScoped<IStateStorageService, LocalStorageService>();
        services.TryAddScoped<CmsAdminState, CmsAdminStatePersistent>();

        return services;
    }
}