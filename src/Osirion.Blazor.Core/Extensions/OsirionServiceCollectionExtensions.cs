using Microsoft.Extensions.DependencyInjection;

namespace Osirion.Blazor.Core.Extensions;

/// <summary>
/// Extension methods for adding Osirion.Blazor Core services to the dependency injection container
/// </summary>
public static class OsirionServiceCollectionExtensions
{
    /// <summary>
    /// Adds Osirion.Blazor Core services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionCore(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        // Add any core services if needed
        // Currently, core doesn't need any specific services

        return services;
    }
}