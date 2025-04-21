using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components.Analytics.Options;
using Osirion.Blazor.Components.Navigation;
using Osirion.Blazor.Options;
using Osirion.Blazor.Services;

namespace Osirion.Blazor.Extensions;

/// <summary>
/// Extension methods for configuring Osirion.Blazor services in the DI container
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all Osirion.Blazor services with the default configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddOsirionBlazor(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // Add base services that don't require specific configuration
        return services;
    }

    /// <summary>
    /// Adds all Osirion.Blazor services with configuration from appsettings sections.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration to bind from.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or configuration is null.</exception>
    public static IServiceCollection AddOsirionBlazor(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        // Add services with configuration from conventionally named sections
        if (configuration.GetSection(ScrollToTopOptions.Section).Exists())
        {
            services.AddScrollToTop(configuration);
        }

        if (configuration.GetSection(ClarityOptions.Section).Exists())
        {
            services.AddClarityTracker(configuration);
        }

        if (configuration.GetSection(MatomoOptions.Section).Exists())
        {
            services.AddMatomoTracker(configuration);
        }

        if (configuration.GetSection(GitHubCmsOptions.Section).Exists())
        {
            services.AddGitHubCms(configuration);
        }

        if (configuration.GetSection(OsirionStyleOptions.Section).Exists())
        {
            services.AddOsirionStyle(configuration);
        }

        return services;
    }

    /// <summary>
    /// Adds all Osirion.Blazor services with a fluent API for granular configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure services using the builder.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or configure is null.</exception>
    public static IServiceCollection AddOsirionBlazor(this IServiceCollection services, Action<IOsirionBlazorBuilder> configure)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var builder = new OsirionBlazorBuilder(services);
        configure(builder);

        return services;
    }
}