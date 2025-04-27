using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Analytics.Internal;
using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Services;

namespace Osirion.Blazor.Analytics.Extensions;

/// <summary>
/// Extension methods for configuring analytics services
/// </summary>
public static class AnalyticsServiceCollectionExtensions
{
    /// <summary>
    /// Adds analytics services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Action to configure analytics providers</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionAnalytics(
        this IServiceCollection services,
        Action<IAnalyticsBuilder> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        // Create builder and apply configuration
        var builder = new AnalyticsBuilder(services);
        configure(builder);

        // Register analytics service
        services.AddSingleton<IAnalyticsService, AnalyticsService>();

        return services;
    }

    public static IServiceCollection AddOsirionAnalytics(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        // Example: Retrieve configuration section and register services
        var analyticsConfigSection = configuration.GetSection(AnalyticsOptions.Section);
        services.Configure<AnalyticsOptions>(analyticsConfigSection);

        // Register analytics service
        services.AddSingleton<IAnalyticsService, AnalyticsService>();

        return services;
    }
}