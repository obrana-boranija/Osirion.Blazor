using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Factories;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Providers;
using Osirion.Blazor.Cms.Infrastructure.Services;

namespace Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for configuring Osirion.Blazor.Cms services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds core Osirion.Blazor.Cms services to the service collection
    /// </summary>
    public static IServiceCollection AddCms(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<OsirionCmsBuilder>? configureCms = null)
    {
        // Register options
        services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.Section));

        services.AddMemoryCache();

        // Register core services
        services.AddSingleton<IMarkdownProcessor, MarkdownProcessor>();
        services.AddScoped<IMarkdownRendererService, MarkdownRendererService>();
        services.AddScoped<IStateStorageService, LocalStorageService>();
        services.AddScoped<IContentCacheService, InMemoryContentCacheService>();

        // Register provider registry and manager
        services.AddSingleton<IContentProviderRegistry, ContentProviderRegistry>();
        services.AddScoped<IContentProviderManager, ContentProviderManager>();
        services.AddSingleton<IContentProviderInitializer, ContentProviderInitializer>();

        // Register CQRS and domain events
        services.AddOsirionCqrs();
        services.AddOsirionDomainEvents();

        // Register repositories and unit of work
        services.AddScoped<IRepositoryFactory, RepositoryFactory>();
        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

        // Apply builder configuration if provided
        if (configureCms != null)
        {
            var builder = new OsirionCmsBuilder(services, configuration);
            configureCms(builder);
        }

        return services;
    }

    /// <summary>
    /// Adds GitHub content provider
    /// </summary>
    public static IServiceCollection AddGitHubContentProvider(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<GitHubOptions>? configureOptions = null)
    {
        // Configure options
        if (configureOptions != null)
        {
            services.Configure<GitHubOptions>(options => {
                // First apply configuration from settings
                configuration.GetSection(GitHubOptions.Section).Bind(options);
                // Then apply custom configuration
                configureOptions(options);
            });
        }
        else
        {
            services.Configure<GitHubOptions>(configuration.GetSection(GitHubOptions.Section));
        }

        // Register HTTP clients and services
        services.AddHttpClient<IGitHubApiClient, GitHubApiClient>();
        services.AddHttpClient<IGitHubTokenProvider, GitHubTokenProvider>();
        services.AddHttpClient<IAuthenticationService, AuthenticationService>();
        services.AddHttpClient<IGitHubAdminService, GitHubAdminService>();

        // Register GitHub repositories and provider
        services.AddScoped<GitHubContentRepository>();
        services.AddScoped<GitHubDirectoryRepository>();
        services.AddScoped<GitHubContentProvider>();

        // Register as IContentProvider
        services.AddScoped<IContentProvider>(sp =>
            sp.GetRequiredService<GitHubContentProvider>());

        return services;
    }

    /// <summary>
    /// Adds FileSystem content provider
    /// </summary>
    public static IServiceCollection AddFileSystemContentProvider(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<FileSystemOptions>? configureOptions = null)
    {
        // Configure options
        if (configureOptions != null)
        {
            services.Configure<FileSystemOptions>(options => {
                // First apply configuration from settings
                configuration.GetSection(FileSystemOptions.Section).Bind(options);
                // Then apply custom configuration
                configureOptions(options);
            });
        }
        else
        {
            services.Configure<FileSystemOptions>(configuration.GetSection(FileSystemOptions.Section));
        }

        // Register FileSystem repositories and provider
        services.AddScoped<FileSystemContentRepository>();
        services.AddScoped<FileSystemDirectoryRepository>();
        services.AddScoped<FileSystemContentProvider>();

        // Register as IContentProvider
        services.AddScoped<IContentProvider>(sp =>
            sp.GetRequiredService<FileSystemContentProvider>());

        return services;
    }
}