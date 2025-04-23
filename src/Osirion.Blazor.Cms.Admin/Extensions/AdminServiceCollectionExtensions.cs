using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Services;

namespace Osirion.Blazor.Cms.Admin.Extensions;

/// <summary>
/// Extension methods for adding CMS Admin services to the dependency injection container
/// </summary>
public static class AdminServiceCollectionExtensions
{
    /// <summary>
    /// Adds Osirion.Blazor.Cms.Admin services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionCmsAdmin(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        // Register GitHub admin service
        services.AddHttpClient<IGitHubAdminService, GitHubAdminService>();

        // Register state services
        services.AddScoped<CmsAdminState>();

        return services;
    }

    /// <summary>
    /// Adds Osirion.Blazor.Cms.Admin services to the service collection with options
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Action to configure CMS admin options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionCmsAdmin(
        this IServiceCollection services,
        Action<CmsAdminOptions> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        // Configure options
        services.Configure(configure);

        // Add core services
        services.AddOsirionCmsAdmin();

        return services;
    }
}