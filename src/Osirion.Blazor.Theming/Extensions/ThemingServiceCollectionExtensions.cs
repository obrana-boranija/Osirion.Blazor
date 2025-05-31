using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Internal;
using Osirion.Blazor.Theming.Services;

namespace Osirion.Blazor.Theming.Extensions;

/// <summary>
/// Extension methods for configuring theming services
/// </summary>
public static class ThemingServiceCollectionExtensions
{
    /// <summary>
    /// Adds theming services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Action to configure theming services</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionTheming(
        this IServiceCollection services,
        Action<IThemingBuilder> configure)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configure is null) throw new ArgumentNullException(nameof(configure));

        services.AddSingleton<IThemeService, ThemeService>();

        // Create builder and apply configuration
        var builder = new ThemingBuilder(services);
        configure(builder);

        return services;
    }

    /// <summary>
    /// Adds theming services to the service collection using configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionTheming(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        return services.AddOsirionTheming(builder =>
        {
            // Get the theming section from configuration
            var themingSection = configuration.GetSection(ThemingOptions.Section);

            if (themingSection.Exists())
            {
                // Configure theme options
                builder.Configure(options => themingSection.Bind(options));

                var followSystemPreference = themingSection.GetValue<bool>("FollowSystemPreference");
                if (followSystemPreference)
                {
                    builder.UseSystemPreference(followSystemPreference);
                }

                var framework = themingSection.GetValue<CssFramework>("Framework");
                if (framework != CssFramework.None)
                {
                    builder.UseFramework(framework);
                }

                //var defaultMode = themingSection.GetValue<bool>("DefaultMode");
                //if (defaultMode)
                //{
                //    builder.EnableDarkMode(defaultMode);
                //}

                var darkMode = themingSection.GetValue<bool>("EnableDarkMode");
                if (darkMode)
                {
                    builder.EnableDarkMode(darkMode);
                }

                var customVariables = themingSection.GetValue<string>("CustomVariables");
                if (!string.IsNullOrWhiteSpace(customVariables))
                {
                    builder.WithCustomVariables(customVariables);
                }
            }
        });

        //// Bind configuration to ThemingOptions
        //services.Configure<ThemingOptions>(configuration.GetSection(ThemingOptions.Section));

        //// Register theme service
        //services.AddSingleton<IThemeService, ThemeService>();

        //return services;
    }

    /// <summary>
    /// Adds minimal theming services with default options
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionTheming(this IServiceCollection services)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        // Register default options
        services.Configure<ThemingOptions>(options =>
        {
            options.UseDefaultStyles = true;
            options.Framework = CssFramework.None;
            options.DefaultMode = ThemeMode.Light;
            options.EnableDarkMode = true;
            options.FollowSystemPreference = false;
        });

        // Register theme service
        services.AddSingleton<IThemeService, ThemeService>();

        return services;
    }
}