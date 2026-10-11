using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.Directory;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;

namespace Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for registering directory services
/// </summary>
public static class DirectoryExtensions
{
    /// <summary>
    /// Service key of the directory cache that FileSystem directory repositories share. It is kept apart from the
    /// GitHub cache so that one provider never answers with another provider's directories.
    /// </summary>
    internal const string FileSystemDirectoryCacheKey = "Osirion.Cms.FileSystem.DirectoryCache";

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
            return new DirectoryCacheManager(logger);
        });

        return services;
    }

    /// <summary>
    /// Registers the FileSystem directory cache once, as a keyed singleton separate from other providers' caches.
    /// </summary>
    internal static IServiceCollection TryAddFileSystemDirectoryCache(this IServiceCollection services)
    {
        services.TryAddKeyedSingleton<IDirectoryCacheManager>(
            FileSystemDirectoryCacheKey,
            (sp, _) => new DirectoryCacheManager(sp.GetRequiredService<ILogger<DirectoryCacheManager>>()));

        return services;
    }

    /// <summary>
    /// Creates a <see cref="FileSystemDirectoryRepository"/> that uses the FileSystem directory cache instead of the
    /// default one, which GitHub repositories use.
    /// </summary>
    internal static FileSystemDirectoryRepository CreateFileSystemDirectoryRepository(IServiceProvider serviceProvider)
    {
        var cacheManager = serviceProvider.GetRequiredKeyedService<IDirectoryCacheManager>(FileSystemDirectoryCacheKey);

        return ActivatorUtilities.CreateInstance<FileSystemDirectoryRepository>(serviceProvider, cacheManager);
    }
}
