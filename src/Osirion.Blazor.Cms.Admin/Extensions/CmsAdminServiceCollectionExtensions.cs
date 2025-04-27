using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Osirion.Blazor.Cms.Admin.Interfaces;
using Osirion.Blazor.Cms.Admin.Internal;
using Osirion.Blazor.Cms.Admin.Services;
using Osirion.Blazor.Cms.Core.Providers.GitHub;

using Osirion.Blazor.Cms.Domain.Interfaces;

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

        var builder = new CmsAdminBuilder(services);
        configure(builder);

        services.TryAddScoped<IGitHubTokenProvider, GitHubTokenProvider>();
        services.TryAddScoped<CmsAdminState>();
        services.TryAddScoped<IGitHubAdminService, GitHubAdminService>();
        services.TryAddScoped<CmsAdminState, CmsAdminStatePersistent>();

        return services;
    }

    /// <summary>
    /// Adds Osirion.Blazor.Cms.Admin services to the service collection using an IConfiguration instance
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceCollection AddOsirionCmsAdmin(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        // Example: Configure services using the provided configuration
        var section = configuration.GetSection(CmsAdminOptions.Section);
        services.Configure<CmsAdminOptions>(section);

        services.TryAddScoped<IGitHubTokenProvider, GitHubTokenProvider>();
        services.TryAddScoped<CmsAdminState>();
        services.TryAddScoped<IGitHubAdminService, GitHubAdminService>();
        services.TryAddScoped<CmsAdminState, CmsAdminStatePersistent>();

        return services;
    }
}