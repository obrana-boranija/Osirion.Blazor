using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Caching;
using Osirion.Blazor.Cms.Infrastructure.DependencyInjection;
using Osirion.Blazor.Cms.Infrastructure.Factories;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.GitHub;

namespace Osirion.Blazor.Cms.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring Osirion CMS Infrastructure services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds infrastructure services to the service collection
    /// </summary>
    public static IServiceCollection AddOsirionCmsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register options
        services.Configure<GitHubOptions>(configuration.GetSection(GitHubOptions.Section));
        services.Configure<FileSystemOptions>(configuration.GetSection(FileSystemOptions.Section));

        // Register GitHub services
        services.AddHttpClient<IGitHubApiClient, GitHubApiClient>();
        services.AddScoped<GitHubContentRepository>();
        services.AddScoped<GitHubDirectoryRepository>();

        // Register FileSystem services
        services.AddScoped<FileSystemContentRepository>();
        services.AddScoped<FileSystemDirectoryRepository>();

        // Register factories
        services.AddScoped<IRepositoryFactory, RepositoryFactory>();
        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

        // Register domain events
        services.AddDomainEvents();

        // Resolve required dependencies for ContentBuilder
        var providerFactory = services.BuildServiceProvider().GetRequiredService<IContentProviderFactory>();
        var cacheFactory = services.BuildServiceProvider().GetRequiredService<CacheDecoratorFactory>();

        var contentBuilder = new ContentBuilder(services, configuration, providerFactory, cacheFactory);

        // Add GitHub and FileSystem providers
        contentBuilder.AddGitHub();
        contentBuilder.AddFileSystem();

        return services;
    }
}
