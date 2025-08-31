﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Components;
using Osirion.Blazor.Internal;
using Osirion.Blazor.Navigation.Options;
using Osirion.Blazor.Core.Configuration;

namespace Osirion.Blazor.Extensions;

/// <summary>
/// Extension methods for adding Osirion.Blazor services
/// </summary>
public static class OsirionServiceCollectionExtensions
{
    /// <summary>
    /// Adds Osirion.Blazor services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Action to configure services</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirion(
        this IServiceCollection services,
        Action<IOsirionBuilder>? configure = null)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        // Create the builder
        var builder = new OsirionBuilder(services);

        // Apply defaults
        ApplyDefaults(builder);

        // Apply custom configuration
        configure?.Invoke(builder);

        return services;
    }

    /// <summary>
    /// Adds Osirion.Blazor services from configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirion(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        return services.AddOsirion(builder =>
        {
            // Content configuration
            var contentSection = configuration.GetSection("Osirion:Cms");
            if (contentSection.Exists())
            {
                builder.UseContent(configuration);
            }

            // CMS Admin configuration
            var cmsAdminSection = configuration.GetSection("Osirion:Cms:Admin");
            if (cmsAdminSection.Exists())
            {
                builder.UseCmsAdmin(configuration);
            }

            // Analytics configuration
            if (configuration.GetSection(AnalyticsOptions.Section).Exists())
            {
                builder.UseAnalytics(configuration);
            }

            if (configuration.GetSection(NavigationOptions.Section).Exists())
            {
                builder.UseNavigation(configuration);
            }

            // Theming configuration
            if (configuration.GetSection(ThemingOptions.Section).Exists())
            {
                builder.UseTheming(configuration);
            }

            // Email services configuration
            if (configuration.GetSection(EmailOptions.Section).Exists())
            {
                builder.UseEmailServices(configuration);
            }
        });
    }

    private static void ApplyDefaults(IOsirionBuilder builder)
    {
        // Add basic navigation with enhanced features
        builder.UseNavigation(navigation =>
        {
            navigation.AddEnhancedNavigation();
            navigation.AddScrollToTop();
        });

        // Use default theming (no framework integration)
        builder.UseTheming(theming =>
        {
            theming.UseFramework(CssFramework.None);
        });
    }
}