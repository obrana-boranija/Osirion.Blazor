using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Configuration;
using Osirion.Blazor.Cms.Admin.Core.Configuration;
using Osirion.Blazor.Cms.Admin.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.Builders;

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
        Action<ICmsAdminBuilder> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        // Add core services
        services.AddOsirionCmsAdmin();

        // Configure options
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<CmsAdminBuilder>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        var builder = new CmsAdminBuilder(services, configuration, logger);
        configure(builder);

        return services;
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
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        // Add core services
        services.AddOsirionCmsAdmin();

        // Configure from appsettings
        services.Configure<CmsAdminOptions>(configuration.GetSection("Osirion:Cms:Admin"));

        // Configure GitHub provider if present
        if (configuration.GetSection("Osirion:Cms:Admin:GitHub").Exists())
        {
            services.AddGitHubServices(configuration);
        }

        // Configure FileSystem provider if present
        if (configuration.GetSection("Osirion:Cms:Admin:FileSystem").Exists())
        {
            services.AddFileSystemServices(configuration);
        }

        return services;
    }

    // Helper methods for configuring specific providers

    /// <summary>
    /// Adds GitHub services to the service collection
    /// </summary>
    private static IServiceCollection AddGitHubServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<GitHubOptions>(configuration.GetSection("Osirion:Cms:Admin:GitHub"));

        // Add GitHub-specific services here

        return services;
    }

    /// <summary>
    /// Adds FileSystem services to the service collection
    /// </summary>
    private static IServiceCollection AddFileSystemServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<FileSystemAdminOptions>(configuration.GetSection("Osirion:Cms:Admin:FileSystem"));

        // Add FileSystem-specific services here

        return services;
    }
}