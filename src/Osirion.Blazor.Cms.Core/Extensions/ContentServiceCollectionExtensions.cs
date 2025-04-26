using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Caching;
using Osirion.Blazor.Cms.Configuration;
using Osirion.Blazor.Cms.Core.Caching;
using Osirion.Blazor.Cms.Core.Interfaces;
using Osirion.Blazor.Cms.Core.Providers.FileSystem;
using Osirion.Blazor.Cms.Core.Providers.GitHub;
using Osirion.Blazor.Cms.Core.Providers.Interfaces;
using Osirion.Blazor.Cms.Core.Services;
using Osirion.Blazor.Cms.Interfaces;
using Osirion.Blazor.Cms.Internal;
using Osirion.Blazor.Cms.Options;
using Osirion.Blazor.Cms.Services;

namespace Osirion.Blazor.Cms.Extensions;

/// <summary>
/// Extension methods for adding Osirion.Blazor.Cms services to the dependency injection container
/// </summary>
public static class ContentServiceCollectionExtensions
{
    /// <summary>
    /// Adds Osirion.Blazor.Cms services to the service collection
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

        // Register core services
        AddCoreServices(services);

        // Create builder and apply configuration
        var builder = new ContentBuilder(services);
        configure(builder);

        // Register ContentProviderManager - scoped to match typical Blazor component lifetime
        services.TryAddScoped<IContentProviderManager, ContentProviderManager>();

        return services;
    }

    /// <summary>
    /// Adds Osirion.Blazor.Cms services from configuration
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

    /// <summary>
    /// Adds GitHub content provider to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Action to configure the GitHub provider</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddGitHubContentProvider(
        this IServiceCollection services,
        Action<GitHubContentOptions> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        // Configure options
        services.Configure(configure);

        // Add HttpClient for GitHub API
        services.AddHttpClient<IGitHubApiClient, GitHubApiClient>();

        // Register GitHub provider
        services.TryAddScoped<GitHubContentProvider>();
        services.TryAddScoped<IContentProvider>(sp => sp.GetRequiredService<GitHubContentProvider>());

        return services;
    }

    /// <summary>
    /// Adds file system content provider to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Action to configure the file system provider</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddFileSystemContentProvider(
        this IServiceCollection services,
        Action<FileSystemContentOptions> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        // Configure options
        services.Configure(configure);

        // Register file system provider
        services.TryAddScoped<FileSystemContentProvider>();
        services.TryAddScoped<IContentProvider>(sp => sp.GetRequiredService<FileSystemContentProvider>());

        return services;
    }

    private static void AddCoreServices(IServiceCollection services)
    {
        // Register content cache service
        services.TryAddSingleton<IContentCacheService, ContentCacheService>();

        // Add memory cache if not already registered
        services.TryAddSingleton<Microsoft.Extensions.Caching.Memory.IMemoryCache, Microsoft.Extensions.Caching.Memory.MemoryCache>();

        // Register content parser
        services.TryAddSingleton<IContentParser, ContentParser>();

        // Register content provider factory
        services.TryAddSingleton<IContentProviderFactory, ContentProviderFactory>();

        // Register default options
        services.TryAddSingleton<IOptions<ContentCacheOptions>>(
            new OptionsWrapper<ContentCacheOptions>(new ContentCacheOptions()));
    }
}