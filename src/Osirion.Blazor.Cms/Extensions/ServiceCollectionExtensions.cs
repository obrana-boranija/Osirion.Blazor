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
        //// Required services
        //services.AddMemoryCache();
        //services.AddScoped<IMarkdownProcessor, MarkdownProcessor>();
        //services.AddScoped<IMarkdownRendererService, MarkdownRendererService>();
        //services.AddScoped<IStateStorageService, LocalStorageService>();

        //// Configuration
        //services.Configure<GitHubOptions>(configuration.GetSection(GitHubOptions.Section));
        //services.Configure<FileSystemOptions>(configuration.GetSection(FileSystemOptions.Section));
        //services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.Section));

        //// Register HTTP client
        //services.AddHttpClient<IGitHubApiClient, GitHubApiClient>();

        //// Register base repositories
        //services.AddScoped<GitHubContentRepository>();
        //services.AddScoped<GitHubDirectoryRepository>();
        //services.AddScoped<FileSystemContentRepository>();
        //services.AddScoped<FileSystemDirectoryRepository>();

        //// Register factory services
        //services.AddSingleton<CacheDecoratorFactory>();
        //services.AddScoped<IRepositoryFactory, RepositoryFactory>();
        //services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

        //// Register CQRS services
        //services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        //services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        //// Provider services
        //services.AddScoped<IContentProviderManager, ContentProviderManager>();

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