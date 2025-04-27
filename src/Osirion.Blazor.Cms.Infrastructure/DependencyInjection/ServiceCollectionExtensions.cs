using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Builders;
using Osirion.Blazor.Cms.Infrastructure.Factories;
using Osirion.Blazor.Cms.Infrastructure.Markdown;
using Osirion.Blazor.Cms.Infrastructure.Services;

namespace Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for configuring Osirion.Blazor.Cms services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Osirion.Blazor.Cms services to the service collection
    /// </summary>
    public static IServiceCollection AddOsirionCms(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IContentBuilder>? configureContent = null)
    {
        // Register options
        services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.Section));

        // Register core services
        services.TryAddSingleton<IMarkdownProcessor, MarkdownProcessor>();
        services.TryAddScoped<IMarkdownRendererService, MarkdownRendererService>();
        services.TryAddScoped<IStateStorageService, LocalStorageService>();

        // Register content cache service
        services.TryAddScoped<IContentCacheService, InMemoryContentCacheService>();

        // Register factories
        services.TryAddSingleton<IContentProviderFactory, ContentProviderFactory>();
        services.TryAddScoped<IRepositoryFactory, RepositoryFactory>();
        services.TryAddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

        // Register provider manager
        services.TryAddScoped<IContentProviderManager, ContentProviderManager>();

        // Add CQRS components
        services.AddOsirionCqrs();

        // Add domain events
        services.AddOsirionDomainEvents();

        // Add repositories
        services.AddOsirionRepositories();

        // Configure content providers if specified
        services.AddOsirionContent(configuration, configureContent);

        // Register content provider initializer
        services.TryAddSingleton<IContentProviderInitializer, ContentProviderInitializer>();

        return services;
    }

    /// <summary>
    /// Adds content provider configuration to the service collection
    /// </summary>
    public static IServiceCollection AddOsirionContent(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IContentBuilder>? configure = null)
    {
        // Create logger for the builder
        var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<ContentBuilder>();

        // Create the content builder
        var contentBuilder = new ContentBuilder(services, configuration, logger);

        // Apply configuration or use defaults
        if (configure != null)
        {
            configure(contentBuilder);
        }
        else
        {
            contentBuilder.AddGitHub();
            contentBuilder.AddFileSystem();
        }

        return services;
    }

    /// <summary>
    /// Adds Osirion.Blazor.Cms.Admin services to the service collection
    /// </summary>
    public static IServiceCollection AddOsirionCmsAdmin(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ICmsAdminBuilder> configureAdmin)
    {
        if (configureAdmin == null)
            throw new ArgumentNullException(nameof(configureAdmin));

        // Create logger for the builder
        var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<CmsAdminBuilder>();

        // Create the admin builder
        var adminBuilder = new CmsAdminBuilder(services, configuration, logger);

        // Apply configuration
        configureAdmin(adminBuilder);

        return services;
    }
}