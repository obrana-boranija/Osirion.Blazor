using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Analytics;
using Osirion.Blazor.Navigation;
using Osirion.Blazor.Theming;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Admin.Interfaces;

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

    /// <summary>
    /// Configures content services
    /// </summary>
    /// <param name="configure">Action to configure content providers</param>
    /// <returns>The builder for chaining</returns>
    IOsirionBuilder UseContent(Action<IContentBuilder> configure);

    /// <summary>
    /// Configures content services using an IConfiguration instance
    /// </summary>
    /// <param name="configuration">The configuration</param>
    /// <returns>The builder for chaining</returns>
    IOsirionBuilder UseContent(IConfiguration configuration);

    /// <summary>
    /// Configures CMS admin services
    /// </summary>
    /// <param name="configure">Action to configure CMS admin services</param>
    /// <returns>The builder for chaining</returns>
    IOsirionBuilder UseCmsAdmin(Action<ICmsAdminBuilder> configure);

    /// <summary>
    /// Configures CMS admin services using an IConfiguration instance
    /// </summary>
    /// <param name="configuration">The configuration</param>
    /// <returns>The builder for chaining</returns>
    IOsirionBuilder UseCmsAdmin(IConfiguration configuration);

    /// <summary>
    /// Configures analytics services
    /// </summary>
    /// <param name="configure">Action to configure analytics providers</param>
    /// <returns>The builder for chaining</returns>
    IOsirionBuilder UseAnalytics(Action<IAnalyticsBuilder> configure);

    /// <summary>
    /// Configures analytics services using an IConfiguration instance
    /// </summary>
    /// <param name="configuration">The configuration</param>
    /// <returns>The builder for chaining</returns>
    IOsirionBuilder UseAnalytics(IConfiguration configuration);

    /// <summary>
    /// Configures navigation services
    /// </summary>
    /// <param name="configure">Action to configure navigation services</param>
    /// <returns>The builder for chaining</returns>
    IOsirionBuilder UseNavigation(Action<INavigationBuilder> configure);

    /// <summary>
    /// Configures navigation services using an IConfiguration instance
    /// </summary>
    /// <param name="configuration">The configuration</param>
    /// <returns>The builder for chaining</returns>
    IOsirionBuilder UseNavigation(IConfiguration configuration);

    /// <summary>
    /// Configures theming services
    /// </summary>
    /// <param name="configure">Action to configure theming services</param>
    /// <returns>The builder for chaining</returns>
    IOsirionBuilder UseTheming(Action<IThemingBuilder> configure);

    /// <summary>
    /// Configures theming services using an IConfiguration instance
    /// </summary>
    /// <param name="configuration">The configuration</param>
    /// <returns>The builder for chaining</returns>
    IOsirionBuilder UseTheming(IConfiguration configuration);

    /// <summary>
    /// Configures email services with specified configuration action
    /// </summary>
    /// <param name="configure">Action to configure email options</param>
    /// <returns>The builder for chaining</returns>
    IOsirionBuilder UseEmailServices(Action<Osirion.Blazor.Core.Configuration.EmailOptions> configure);

    /// <summary>
    /// Configures email services using an IConfiguration instance
    /// </summary>
    /// <param name="configuration">The configuration</param>
    /// <returns>The builder for chaining</returns>
    IOsirionBuilder UseEmailServices(IConfiguration configuration);
}