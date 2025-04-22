using Microsoft.Extensions.DependencyInjection;

namespace Osirion.Blazor;

/// <summary>
/// Main builder interface for configuring Osirion.Blazor services
/// </summary>
public interface IOsirionBuilder
{
    /// <summary>
    /// Gets the service collection being configured
    /// </summary>
    IServiceCollection Services { get; }

    ///// <summary>
    ///// Configures content services
    ///// </summary>
    ///// <param name="configure">Action to configure content providers</param>
    ///// <returns>The builder for chaining</returns>
    //IOsirionBuilder UseContent(Action<IContentBuilder> configure);

    ///// <summary>
    ///// Configures analytics services
    ///// </summary>
    ///// <param name="configure">Action to configure analytics providers</param>
    ///// <returns>The builder for chaining</returns>
    //IOsirionBuilder UseAnalytics(Action<IAnalyticsBuilder> configure);

    ///// <summary>
    ///// Configures navigation services
    ///// </summary>
    ///// <param name="configure">Action to configure navigation services</param>
    ///// <returns>The builder for chaining</returns>
    //IOsirionBuilder UseNavigation(Action<INavigationBuilder> configure);

    ///// <summary>
    ///// Configures theming services
    ///// </summary>
    ///// <param name="configure">Action to configure theming services</param>
    ///// <returns>The builder for chaining</returns>
    //IOsirionBuilder UseTheming(Action<IThemingBuilder> configure);
}