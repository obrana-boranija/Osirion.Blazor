using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Navigation.Internal;
using Osirion.Blazor.Navigation.Options;
using Osirion.Blazor.Navigation.Services;

namespace Osirion.Blazor.Navigation.Extensions;

/// <summary>
/// Extension methods for configuring navigation services
/// </summary>
public static class NavigationServiceCollectionExtensions
{
    /// <summary>
    /// Adds enhanced navigation services to the service collection
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

    /// <summary>
    /// Adds enhanced navigation services to the service collection using configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionNavigation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        return services.AddOsirionNavigation(builder =>
        {
            // Check for EnhancedNavigation configuration
            var enhancedSection = configuration.GetSection(EnhancedNavigationOptions.Section);
            if (enhancedSection.Exists())
            {
                builder.AddEnhancedNavigation(options => enhancedSection.Bind(options));
            }

            // Check for ScrollToTop configuration
            var scrollToTopSection = configuration.GetSection(ScrollToTopOptions.Section);
            if (scrollToTopSection.Exists())
            {
                builder.AddScrollToTop(options => scrollToTopSection.Bind(options));
            }
        });
    }
}