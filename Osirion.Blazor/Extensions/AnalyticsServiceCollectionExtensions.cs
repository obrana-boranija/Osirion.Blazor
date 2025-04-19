using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components.Analytics.Options;

namespace Osirion.Blazor.Extensions;

/// <summary>
/// Extension methods for configuring analytics services in the DI container
/// </summary>
public static class AnalyticsServiceCollectionExtensions
{
    /// <summary>
    /// Adds Microsoft Clarity analytics configuration to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration instance</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddClarityTracker(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        services.Configure<ClarityOptions>(configuration.GetSection(ClarityOptions.Section));
        return services;
    }

    /// <summary>
    /// Adds Microsoft Clarity analytics configuration to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">The configuration action</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddClarityTracker(this IServiceCollection services, Action<ClarityOptions> configureOptions)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configureOptions == null)
        {
            throw new ArgumentNullException(nameof(configureOptions));
        }

        services.Configure(configureOptions);
        return services;
    }

    /// <summary>
    /// Adds Matomo analytics configuration to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration instance</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddMatomoTracker(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        services.Configure<MatomoOptions>(configuration.GetSection(MatomoOptions.Section));
        return services;
    }

    /// <summary>
    /// Adds Matomo analytics configuration to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">The configuration action</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddMatomoTracker(this IServiceCollection services, Action<MatomoOptions> configureOptions)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configureOptions == null)
        {
            throw new ArgumentNullException(nameof(configureOptions));
        }

        services.Configure(configureOptions);
        return services;
    }
}