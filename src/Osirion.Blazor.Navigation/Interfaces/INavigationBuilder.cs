using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Navigation.Options;

namespace Osirion.Blazor.Navigation;

/// <summary>
/// Builder interface for configuring navigation services
/// </summary>
public interface INavigationBuilder
{
    /// <summary>
    /// Gets the service collection being configured
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Configures enhanced navigation
    /// </summary>
    /// <param name="configure">Action to configure enhanced navigation options</param>
    /// <returns>The builder for chaining</returns>
    INavigationBuilder UseEnhancedNavigation(Action<EnhancedNavigationOptions>? configure = null);

    /// <summary>
    /// Configures enhanced navigation using a configuration section
    /// </summary>
    /// <param name="configuration">The configuration</param>
    /// <returns>The builder for chaining</returns>
    INavigationBuilder UseEnhancedNavigation(IConfiguration configuration);

    /// <summary>
    /// Adds scroll to top functionality
    /// </summary>
    /// <param name="configure">Action to configure scroll to top options</param>
    /// <returns>The builder for chaining</returns>
    INavigationBuilder AddScrollToTop(Action<ScrollToTopOptions>? configure = null);

    /// <summary>
    /// Adds scroll to top functionality using a configuration section
    /// </summary>
    /// <param name="configuration">The configuration</param>
    /// <returns>The builder for chaining</returns>
    INavigationBuilder AddScrollToTop(IConfiguration configuration);
}