using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Navigation.Options;
using Osirion.Blazor.Navigation.Services;

namespace Osirion.Blazor.Navigation.Internal;

/// <summary>
/// Implementation of the navigation builder
/// </summary>
internal class NavigationBuilder : INavigationBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationBuilder"/> class.
    /// </summary>
    public NavigationBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <inheritdoc/>
    public INavigationBuilder UseEnhancedNavigation(Action<EnhancedNavigationOptions>? configure = null)
    {
        var options = new EnhancedNavigationOptions();
        configure?.Invoke(options);

        Services.Configure<EnhancedNavigationOptions>(opt =>
        {
            opt.Behavior = options.Behavior;
            opt.ResetScrollOnNavigation = options.ResetScrollOnNavigation;
            opt.PreserveScrollForSamePageNavigation = options.PreserveScrollForSamePageNavigation;
        });

        AddManagerService();

        return this;
    }

    /// <inheritdoc/>
    public INavigationBuilder UseEnhancedNavigation(IConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var options = new EnhancedNavigationOptions();
        configuration.Bind(options);

        Services.Configure<EnhancedNavigationOptions>(opt =>
        {
            opt.Behavior = options.Behavior;
            opt.ResetScrollOnNavigation = options.ResetScrollOnNavigation;
            opt.PreserveScrollForSamePageNavigation = options.PreserveScrollForSamePageNavigation;
        });

        AddManagerService();

        return this;
    }

    /// <inheritdoc/>
    public INavigationBuilder AddScrollToTop(Action<ScrollToTopOptions>? configure = null)
    {
        var options = new ScrollToTopOptions();
        configure?.Invoke(options);

        Services.Configure<ScrollToTopOptions>(opt =>
        {
            opt.Position = options.Position;
            opt.Behavior = options.Behavior;
            opt.VisibilityThreshold = options.VisibilityThreshold;
            opt.Text = options.Text;
            opt.Title = options.Title;
            opt.CssClass = options.CssClass;
            opt.CustomIcon = options.CustomIcon;
        });

        // Create and register the ScrollToTopManager
        AddManagerService(options);

        return this;
    }

    /// <inheritdoc/>
    public INavigationBuilder AddScrollToTop(IConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var options = new ScrollToTopOptions();
        configuration.Bind(options);

        Services.Configure<ScrollToTopOptions>(opt =>
        {
            opt.Position = options.Position;
            opt.Behavior = options.Behavior;
            opt.VisibilityThreshold = options.VisibilityThreshold;
            opt.Text = options.Text;
            opt.Title = options.Title;
            opt.CssClass = options.CssClass;
            opt.CustomIcon = options.CustomIcon;
        });

        AddManagerService(options);

        return this;
    }

    private void AddManagerService(ScrollToTopOptions? options = null)
    {
        var manager = new ScrollToTopManager
        {
            IsEnabled = true,
            Position = options?.Position ?? Position.BottomRight,
            Behavior = options?.Behavior ?? ScrollBehavior.Smooth,
            VisibilityThreshold = options?.VisibilityThreshold ?? 300,
            Text = options?.Text ?? null,
            Title = options?.Title ?? "Scroll to top",
            CssClass = options?.CssClass ?? null,
            CustomIcon = options?.CustomIcon ?? null
        };

        Services.AddSingleton(manager);
    }
}