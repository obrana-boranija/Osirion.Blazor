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

        Services.AddSingleton(manager);

        return this;
    }
}