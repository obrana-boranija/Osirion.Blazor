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

    /// <summary>
    /// Adds analytics services to the service collection using configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionAnalytics(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        var analyticsSection = configuration.GetSection(AnalyticsOptions.Section);
        var claritySection = configuration.GetSection(ClarityOptions.Section);
        var matomoSection = configuration.GetSection(MatomoOptions.Section);

        return services.AddOsirionAnalytics(builder =>
        {
            //// Get the analytics section from configuration
            //var analyticsSection = configuration.GetSection(AnalyticsOptions.Section);

            // Check for Clarity configuration
            var claritySection = configuration.GetSection(ClarityOptions.Section);
            if (claritySection.Exists())
            {
                builder.AddClarity(options => claritySection.Bind(options));
            }

            // Check for Matomo configuration
            var matomoSection = configuration.GetSection(MatomoOptions.Section);
            if (matomoSection.Exists())
            {
                builder.AddMatomo(options => matomoSection.Bind(options));
            }

            // Custom providers could be added here based on configuration
        });
    }
}
