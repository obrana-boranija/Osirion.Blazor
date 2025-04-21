using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Options;

namespace Osirion.Blazor.Extensions;

/// <summary>
/// Extension methods for configuring Osirion styling in the DI container
/// </summary>
public static class OsirionStyleServiceCollectionExtensions
{
    /// <summary>
    /// Adds Osirion style configuration to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration instance</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionStyle(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration == null)
        {
            return services;
        }

        services.Configure<OsirionStyleOptions>(configuration.GetSection(OsirionStyleOptions.Section));
        return services;
    }

    /// <summary>
    /// Adds Osirion style configuration to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">The configuration action</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionStyle(this IServiceCollection services, Action<OsirionStyleOptions> configureOptions)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configureOptions == null)
        {
            return services;
        }

        services.Configure(configureOptions);
        return services;
    }

    /// <summary>
    /// Adds Osirion style configuration with specific framework integration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="framework">The CSS framework to integrate with</param>
    /// <param name="useStyles">Whether to use default styles</param>
    /// <param name="customVariables">Optional custom CSS variables</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionStyle(
        this IServiceCollection services,
        CssFramework framework,
        bool useStyles = true,
        string? customVariables = null)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.Configure<OsirionStyleOptions>(options =>
        {
            options.FrameworkIntegration = framework;
            options.UseStyles = useStyles;
            options.CustomVariables = customVariables;
        });

        return services;
    }
}