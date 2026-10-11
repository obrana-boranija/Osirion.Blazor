using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Osirion.Blazor.Cms.Admin.Builders;
using Osirion.Blazor.Cms.Domain.Options.Configuration;

namespace Osirion.Blazor.Cms.Admin.DependencyInjection;

/// <summary>
/// Extension methods for adding Osirion CMS Admin services to the service collection
/// </summary>
public static class OsirionCmsAdminServiceCollectionExtensions
{
    /// <summary>
    /// Adds Osirion CMS Admin services to the service collection with builder pattern
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Action to configure the CMS admin</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddOsirionCmsAdmin(
        this IServiceCollection services,
        Action<IOsirionCmsAdminBuilder> configure)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configure is null) throw new ArgumentNullException(nameof(configure));

        // Add core services
        services.AddOsirionCmsAdminDI();

        // Building a second service provider here would duplicate every singleton registered so far, so the
        // configuration is read from its registration instead. Registration time has no logger pipeline yet.
        var configuration = FindRegisteredConfiguration(services);

        var builder = new OsirionCmsAdminBuilder(
            services,
            configuration,
            NullLogger<OsirionCmsAdminBuilder>.Instance);
        configure(builder);

        return services;
    }

    private static IConfiguration FindRegisteredConfiguration(IServiceCollection services)
    {
        var descriptor = services.LastOrDefault(registered => registered.ServiceType == typeof(IConfiguration));
        if (descriptor is { IsKeyedService: false, ImplementationInstance: IConfiguration instance })
        {
            return instance;
        }

        // The host registered no configuration instance (for example a bare ServiceCollection): use an empty one.
        return new ConfigurationBuilder().Build();
    }

    /// <summary>
    /// Adds Osirion CMS Admin services to the service collection with configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddOsirionCmsAdmin(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        // Add core services
        services.AddOsirionCmsAdminDI();

        // Configure from appsettings
        services.Configure<CmsAdminOptions>(configuration.GetSection("Osirion:Cms:Admin"));

        // Configure GitHub provider if present
        if (configuration.GetSection("Osirion:Cms:Admin:GitHub").Exists())
        {
            services.AddGitHubAdminServices(configuration);
        }

        // Configure FileSystem provider if present
        if (configuration.GetSection("Osirion:Cms:Admin:FileSystem").Exists())
        {
            services.AddFileSystemAdminServices(configuration);
        }

        return services;
    }
}
