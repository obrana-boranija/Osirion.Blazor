using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osirion.Blazor.Extensions;

/// <summary>
/// Extension methods for the ScrollToTop component
/// </summary>
public static class ScrollToTopCollectionExtensions
{
    /// <summary>
    /// This extension method registers ScrollToTopOptions with configuration from appsettings.json
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceCollection AddScrollToTop(this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        // Configure options from the "ScrollToTop" section of appsettings.json
        services.Configure<ScrollToTopOptions>(
            configuration.GetSection(ScrollToTopOptions.Section));

        return services;
    }

    /// <summary>
    /// This extension method registers ScrollToTopOptions with programmatic configuration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceCollection AddScrollToTop(this IServiceCollection services,
        Action<ScrollToTopOptions> configureOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));

        // Configure options using the provided action
        services.Configure(configureOptions);

        return services;
    }
}
