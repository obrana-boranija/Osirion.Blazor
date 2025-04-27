using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.GitHub;

namespace Osirion.Blazor.Cms.Infrastructure.Extensions;

/// <summary>
/// Extension methods for repository registration
/// </summary>
public static class RepositoryRegistrationExtensions
{
    /// <summary>
    /// Registers all repository implementations
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Register GitHub repositories
        services.AddScoped<GitHubContentRepository>();
        services.AddScoped<GitHubDirectoryRepository>();

        // Register FileSystem repositories
        services.AddScoped<FileSystemContentRepository>();
        services.AddScoped<FileSystemDirectoryRepository>();

        return services;
    }

    /// <summary>
    /// Updates the RepositoryFactory to correctly create repositories
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection UpdateRepositoryFactory(this IServiceCollection services)
    {
        // Replace the existing RepositoryFactory implementation with an updated one that knows about all repository types
        services.AddScoped<IRepositoryFactory, Factories.RepositoryFactory>(serviceProvider =>
        {
            var defaultProviderId = serviceProvider.GetService<string>() ?? "github";
            var logger = serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Factories.RepositoryFactory>>();

            return new Factories.RepositoryFactory(serviceProvider, logger, defaultProviderId);
        });

        return services;
    }
}