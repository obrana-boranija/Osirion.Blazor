using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Navigation.Internal;
using Osirion.Blazor.Navigation.Services;

namespace Osirion.Blazor.Navigation.Extensions;

/// <summary>
/// Extension methods for configuring navigation services
/// </summary>
public static class NavigationServiceCollectionExtensions
{
    /// <summary>
    /// Adds navigation services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Action to configure navigation services</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionNavigation(
        this IServiceCollection services,
        Action<INavigationBuilder> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        // Create builder and apply configuration
        var builder = new NavigationBuilder(services);
        configure(builder);

        // Register navigation service
        services.AddSingleton<INavigationService, NavigationService>();

        return services;
    }
}