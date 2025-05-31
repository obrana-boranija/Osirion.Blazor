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
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configure is null) throw new ArgumentNullException(nameof(configure));

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
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        return services.AddOsirionAnalytics(builder =>
        {
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

            // Check for Google Analytics 4 configuration
            var ga4Section = configuration.GetSection(GA4Options.Section);
            if (ga4Section.Exists())
            {
                builder.AddGA4(options => ga4Section.Bind(options));
            }

            // Check for Yandex Metrica configuration
            var yandexSection = configuration.GetSection(YandexMetricaOptions.Section);
            if (yandexSection.Exists())
            {
                builder.AddYandexMetrica(options => yandexSection.Bind(options));
            }

            // Custom providers could be added here based on configuration
        });
    }
}
