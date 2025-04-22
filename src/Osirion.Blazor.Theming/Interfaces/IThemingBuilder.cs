using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Theming.Options;

namespace Osirion.Blazor.Theming;

/// <summary>
/// Builder interface for configuring theming services
/// </summary>
public interface IThemingBuilder
{
    /// <summary>
    /// Gets the service collection being configured
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Configures theming options
    /// </summary>
    /// <param name="configure">Action to configure theming options</param>
    /// <returns>The builder for chaining</returns>
    IThemingBuilder Configure(Action<ThemingOptions> configure);

    /// <summary>
    /// Sets the CSS framework to integrate with
    /// </summary>
    /// <param name="framework">The CSS framework</param>
    /// <returns>The builder for chaining</returns>
    IThemingBuilder UseFramework(CssFramework framework);

    /// <summary>
    /// Enables dark mode support
    /// </summary>
    /// <param name="enabled">Whether dark mode is enabled</param>
    /// <returns>The builder for chaining</returns>
    IThemingBuilder EnableDarkMode(bool enabled = true);

    /// <summary>
    /// Enables following system theme preference
    /// </summary>
    /// <param name="useSystem">Whether to use system preference</param>
    /// <returns>The builder for chaining</returns>
    IThemingBuilder UseSystemPreference(bool useSystem = true);

    /// <summary>
    /// Sets custom CSS variables
    /// </summary>
    /// <param name="variables">The custom CSS variables</param>
    /// <returns>The builder for chaining</returns>
    IThemingBuilder WithCustomVariables(string variables);
}