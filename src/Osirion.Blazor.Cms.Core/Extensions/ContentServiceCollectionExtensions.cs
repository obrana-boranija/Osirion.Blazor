using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Caching;
using Osirion.Blazor.Cms.Configuration;
using Osirion.Blazor.Cms.Core.Caching;
using Osirion.Blazor.Cms.Core.Interfaces;
using Osirion.Blazor.Cms.Core.Providers.FileSystem;
using Osirion.Blazor.Cms.Core.Services;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Markdown;
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

        AddCoreServices(services);

        var builder = new ContentBuilder(services);
        configure(builder);

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
            var githubSection = configuration.GetSection("Osirion:Cms:GitHub");
            if (githubSection.Exists())
            {
                builder.AddGitHub(github =>
                {
                    githubSection.Bind(github);
                });
            }

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

        services.Configure(configure);

        services.AddHttpClient<IGitHubApiClient, GitHubApiClient>();

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

        services.Configure(configure);

        services.TryAddScoped<FileSystemContentProvider>();
        services.TryAddScoped<IContentProvider>(sp => sp.GetRequiredService<FileSystemContentProvider>());

        return services;
    }

    private static void AddCoreServices(IServiceCollection services)
    {
        services.TryAddSingleton<IContentCacheService, ContentCacheService>();

        services.TryAddSingleton<CachedContentProviderFactory>();

        services.TryAddSingleton<IMemoryCache, MemoryCache>();

        services.TryAddSingleton<IMarkdownProcessor, MarkdownProcessor>();

        services.TryAddSingleton<IContentParser, ContentParser>();

        services.TryAddSingleton<IContentProviderFactory, ContentProviderFactory>();

        services.TryAddSingleton<IOptions<ContentCacheOptions>>(
            new OptionsWrapper<ContentCacheOptions>(new ContentCacheOptions()));
    }
}