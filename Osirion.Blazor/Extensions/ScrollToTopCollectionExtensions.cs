using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components.Navigation;
using Osirion.Blazor.Options;
using Osirion.Blazor.Services;

namespace Osirion.Blazor.Extensions;

/// <summary>
/// Extension methods for the ScrollToTop component
/// </summary>
public static class ScrollToTopCollectionExtensions
{
    /// <summary>
    /// Adds and configures the global ScrollToTop functionality with options from configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration containing ScrollToTop settings</param>
    /// <returns>The service collection for chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or configuration is null</exception>
    public static IServiceCollection AddScrollToTop(this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (configuration == null)
            return services;

        // Get options from configuration
        var options = new ScrollToTopOptions();
        configuration.GetSection(ScrollToTopOptions.Section).Bind(options);

        // Create and configure manager from options
        var manager = new ScrollToTopManager
        {
            IsEnabled = true,
            Position = options.Position,
            Behavior = options.Behavior,
            VisibilityThreshold = options.VisibilityThreshold,
            Text = options.Text,
            Title = options.Title,
            CssClass = options.CssClass,
            CustomIcon = options.CustomIcon
        };

        // Register the manager as singleton
        services.AddSingleton(manager);

        // Also register the options for components that need it
        services.Configure<ScrollToTopOptions>(
            configuration.GetSection(ScrollToTopOptions.Section));

        return services;
    }

    /// <summary>
    /// Adds and configures the global ScrollToTop functionality with customized options
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="position">Position of the ScrollToTop button</param>
    /// <param name="behavior">Scroll behavior</param>
    /// <param name="visibilityThreshold">Visibility threshold in pixels</param>
    /// <param name="text">Optional text to display</param>
    /// <returns>The service collection for chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null</exception>
    public static IServiceCollection AddScrollToTop(
        this IServiceCollection services,
        ButtonPosition position = ButtonPosition.BottomRight,
        ScrollBehavior behavior = ScrollBehavior.Smooth,
        int visibilityThreshold = 300,
        string? text = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        // Register the ScrollToTopManager as a singleton
        services.AddSingleton(new ScrollToTopManager
        {
            IsEnabled = true,
            Position = position,
            Behavior = behavior,
            VisibilityThreshold = visibilityThreshold,
            Text = text
        });

        return services;
    }

    /// <summary>
    /// Adds and configures the global ScrollToTop functionality with detailed customization
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureAction">Action to configure the ScrollToTop manager</param>
    /// <returns>The service collection for chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or configureAction is null</exception>
    public static IServiceCollection AddScrollToTop(
        this IServiceCollection services,
        Action<ScrollToTopManager> configureAction)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (configureAction == null)
            return services;

        // Create a new manager instance with default settings
        var manager = new ScrollToTopManager { IsEnabled = true };

        // Apply custom configuration
        configureAction.Invoke(manager);

        // Register as singleton
        services.AddSingleton(manager);

        return services;
    }

    /// <summary>
    /// Registers ScrollToTopOptions from configuration without enabling the global ScrollToTop
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration containing ScrollToTop settings</param>
    /// <returns>The service collection for chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or configuration is null</exception>
    public static IServiceCollection AddScrollToTopOptions(this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (configuration == null)
            return services;

        // Configure options from the "ScrollToTop" section of appsettings.json
        services.Configure<ScrollToTopOptions>(
            configuration.GetSection(ScrollToTopOptions.Section));

        return services;
    }

    /// <summary>
    /// Registers ScrollToTopOptions with programmatic configuration without enabling the global ScrollToTop
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Action to configure the ScrollToTop options</param>
    /// <returns>The service collection for chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or configureOptions is null</exception>
    public static IServiceCollection AddScrollToTopOptions(this IServiceCollection services,
        Action<ScrollToTopOptions> configureOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (configureOptions == null)
            return services;

        // Configure options using the provided action
        services.Configure(configureOptions);

        return services;
    }
}