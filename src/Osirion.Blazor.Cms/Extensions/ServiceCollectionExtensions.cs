using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

namespace Osirion.Blazor.Cms;

/// <summary>
/// Extension methods for setting up essential Osirion.Blazor.Cms services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Osirion CMS services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionCms(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddCms(configuration);

        return services;
    }

    /// <summary>
    /// Adds configuration for GitHub content provider
    /// </summary>
    public static IServiceCollection AddOsirionGitHubContentProvider(
        this IServiceCollection services,
        Action<GitHubOptions>? configure = null)
    {
        //services.AddGitHubContentProvider(configure);
        //// Allow additional configuration
        //if (configure != null)
        //{
        //    services.Configure(configure);
        //}

        return services;
    }

    /// <summary>
    /// Adds configuration for file system content provider
    /// </summary>
    public static IServiceCollection AddOsirionFileSystemContentProvider(
        this IServiceCollection services,
        Action<FileSystemOptions>? configure = null)
    {
        //services.AddFileSystemContentProvider(configure);
        // Allow additional configuration
        if (configure != null)
        {
            services.Configure(configure);
        }

        return services;
    }
}