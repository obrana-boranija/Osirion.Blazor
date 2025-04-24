using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Interfaces;
using Osirion.Blazor.Cms.Internal;
using Osirion.Blazor.Cms.Services;

namespace Osirion.Blazor.Cms;

/// <summary>
/// Extension methods for configuring content services
/// </summary>
public static class ContentServiceCollectionExtensions
{
    /// <summary>
    /// Adds content services to the service collection
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

        // Add required services
        services.AddMemoryCache();

        // Create builder and apply configuration
        var builder = new ContentBuilder(services);
        configure(builder);

        // Add ContentProviderManager
        services.AddScoped<IContentProviderManager, ContentProviderManager>();

        return services;
    }
}