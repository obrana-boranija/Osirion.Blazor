using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.Content;

namespace Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for registering content services
/// </summary>
public static class ContentExtensions
{
    /// <summary>
    /// Adds content services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddContentServices(this IServiceCollection services)
    {
        // Register content helpers
        services.TryAddSingleton<IContentQueryFilter, ContentQueryFilter>();
        services.TryAddSingleton<IContentSorter, ContentSorter>();
        services.TryAddSingleton<IContentMetadataProcessor, ContentMetadataProcessor>();

        // Register cache managers for repositories - scoped per provider
        services.TryAddScoped<IContentCacheManager>(sp =>
        {
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<ContentCacheManager>>();
            var options = sp.GetRequiredService<IOptions<FileSystemOptions>>();
            var cacheDuration = TimeSpan.FromMinutes(options.Value.CacheDurationMinutes);
            return new ContentCacheManager(logger, cacheDuration);
        });

        return services;
    }
}