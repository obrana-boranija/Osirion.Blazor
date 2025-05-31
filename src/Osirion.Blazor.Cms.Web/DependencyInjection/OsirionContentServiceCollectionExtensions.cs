using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.Builders;
using Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

namespace Osirion.Blazor.Cms.Web.DependencyInjection;

/// <summary>
/// Extension methods for adding Osirion CMS content services
/// </summary>
public static class OsirionContentServiceCollectionExtensions
{
    /// <summary>
    /// Adds Osirion CMS content services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Action to configure content services</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionContent(
        this IServiceCollection services,
        Action<IContentBuilder> configure)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configure is null) throw new ArgumentNullException(nameof(configure));

        // Add core CMS services
        services.AddCms(services.BuildServiceProvider().GetRequiredService<IConfiguration>());

        // Create builder and apply configuration
        var serviceProvider = services.BuildServiceProvider();
        var builder = new ContentBuilder(
            services,
            serviceProvider.GetRequiredService<IConfiguration>(),
            serviceProvider.GetRequiredService<ILogger<ContentBuilder>>());

        configure(builder);

        return services;
    }

    /// <summary>
    /// Adds Osirion CMS content services to the service collection using configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionContent(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        // Add core CMS services
        services.AddCms(configuration);

        // Configure providers based on configuration
        var cmsSection = configuration.GetSection("Osirion:Cms");

        // Configure GitHub provider if present
        var githubSection = cmsSection.GetSection("Web:GitHub");
        if (githubSection.Exists())
        {
            //services.AddGitHubContentProvider(configuration);
            services.AddGitHubProvidersFromConfiguration(configuration);
        }

        // Configure FileSystem provider if present
        var fileSystemSection = cmsSection.GetSection("Web:FileSystem");
        if (fileSystemSection.Exists())
        {
            services.AddFileSystemContentProvider(configuration);
        }

        return services;
    }
}