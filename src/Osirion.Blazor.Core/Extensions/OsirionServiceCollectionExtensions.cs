using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Internal;

namespace Osirion.Blazor.Extensions;

/// <summary>
/// Extension methods for adding Osirion.Blazor services to the dependency injection container
/// </summary>
public static class OsirionServiceCollectionExtensions
{
    /// <summary>
    /// Adds Osirion.Blazor services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">An action to configure the Osirion builder</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirion(
        this IServiceCollection services,
        Action<IOsirionBuilder>? configure = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        var builder = new OsirionBuilder(services);

        // Apply configuration if provided
        configure?.Invoke(builder);

        return services;
    }
}