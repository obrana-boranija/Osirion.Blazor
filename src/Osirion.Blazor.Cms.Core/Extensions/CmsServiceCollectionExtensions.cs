using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Osirion.Blazor.Cms.Caching;
using Osirion.Blazor.Cms.Configuration;
using Osirion.Blazor.Cms.Core.Caching;
using Osirion.Blazor.Cms.Core.Interfaces;
using Osirion.Blazor.Cms.Core.Services;
using Osirion.Blazor.Cms.Interfaces;
using Osirion.Blazor.Cms.Internal;
using Osirion.Blazor.Cms.Services;

namespace Osirion.Blazor.Cms.Core.Extensions;

/// <summary>
/// Extension methods for configuring CMS services in dependency injection
/// </summary>
public static class CmsServiceCollectionExtensions
{
    /// <summary>
    /// Adds core CMS services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Action to configure content providers</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionContent(
        this IServiceCollection services,
        Action<IContentBuilder> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        // Register common services
        services.AddMemoryCache();
        services.TryAddSingleton<IContentCacheService, ContentCacheService>();
        services.TryAddSingleton<IContentParser, ContentParser>();
        services.TryAddScoped<IContentProviderManager, ContentProviderManager>();
        services.TryAddSingleton<IContentProviderFactory, ContentProviderFactory>();

        // Create builder and apply configuration
        var builder = new ContentBuilder(services);
        configure(builder);

        return services;
    }

    /// <summary>
    /// Adds core CMS services from configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionContent(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        // Register caching options
        services.Configure<ContentCacheOptions>(configuration.GetSection("Osirion:Cms:Caching"));

        return services.AddOsirionContent(builder =>
        {
            // Configure GitHub provider if in config
            var githubSection = configuration.GetSection("Osirion:Cms:GitHub");
            if (githubSection.Exists())
            {
                builder.AddGitHub(github =>
                {
                    githubSection.Bind(github);
                });
            }

            // Configure file system provider if in config
            var fileSystemSection = configuration.GetSection("Osirion:Cms:FileSystem");
            if (fileSystemSection.Exists())
            {
                builder.AddFileSystem(fileSystem =>
                {
                    fileSystemSection.Bind(fileSystem);
                });
            }
        });
    }
}