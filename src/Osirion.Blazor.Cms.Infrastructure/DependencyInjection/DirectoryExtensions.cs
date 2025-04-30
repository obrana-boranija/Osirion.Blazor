using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.Directory;

namespace Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for registering directory services
/// </summary>
public static class DirectoryExtensions
{
    /// <summary>
    /// Adds directory services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDirectoryServices(this IServiceCollection services)
    {
        // Register directory helpers and managers
        services.TryAddSingleton<IDirectoryMetadataProcessor, DirectoryMetadataProcessor>();
        services.TryAddSingleton<IPathUtilities>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<FileSystemOptions>>();
            return new PathUtilities(options);
        });

        // Register cache managers for repositories - scoped per provider
        services.TryAddSingleton<IDirectoryCacheManager>(sp =>
        {
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<DirectoryCacheManager>>();
            var options = sp.GetRequiredService<IOptions<FileSystemOptions>>();
            var cacheDuration = TimeSpan.FromMinutes(options.Value.CacheDurationMinutes);
            return new DirectoryCacheManager(logger, cacheDuration);
        });

        return services;
    }
}