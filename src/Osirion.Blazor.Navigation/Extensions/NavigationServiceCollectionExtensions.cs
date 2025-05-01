using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Osirion.Blazor.Navigation.Internal;
using Osirion.Blazor.Navigation.Options;
using Osirion.Blazor.Navigation.Services;

namespace Osirion.Blazor.Navigation;

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
    public static IServiceCollection AddEnhancedNavigation(
        this IServiceCollection services,
        Action<INavigationBuilder> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        // Create builder and apply configuration
        var builder = new NavigationBuilder(services);
        configure(builder);

        // Register navigation service
        services.TryAddSingleton<INavigationService, NavigationService>();

        return services;
    }

    /// <summary>
    /// Adds navigation services to the service collection using an IConfiguration instance
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration instance</param>
    /// <returns>The service collection for chaining</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceCollection AddEnhancedNavigation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        // Example: Use configuration to configure navigation services
        var navigationSection = configuration.GetSection(EnhancedNavigationOptions.Section);
        services.Configure<EnhancedNavigationOptions>(navigationSection);

        // Register navigation service
        services.TryAddSingleton<INavigationService, NavigationService>();

        return services;
    }
}