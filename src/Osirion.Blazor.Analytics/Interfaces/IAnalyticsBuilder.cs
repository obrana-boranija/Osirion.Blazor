using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Analytics.Options;

namespace Osirion.Blazor.Analytics;

/// <summary>
/// Builder interface for configuring analytics providers
/// </summary>
public interface IAnalyticsBuilder
{
    /// <summary>
    /// Gets the service collection being configured
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Adds Microsoft Clarity analytics
    /// </summary>
    /// <param name="configure">Action to configure Clarity options</param>
    /// <returns>The builder for chaining</returns>
    IAnalyticsBuilder AddClarity(Action<ClarityOptions>? configure = null);

    /// <summary>
    /// Adds Matomo analytics
    /// </summary>
    /// <param name="configure">Action to configure Matomo options</param>
    /// <returns>The builder for chaining</returns>
    IAnalyticsBuilder AddMatomo(Action<MatomoOptions>? configure = null);

    /// <summary>
    /// Adds a custom analytics provider
    /// </summary>
    /// <typeparam name="TProvider">The provider type</typeparam>
    /// <param name="configure">Action to configure the provider</param>
    /// <returns>The builder for chaining</returns>
    IAnalyticsBuilder AddProvider<TProvider>(Action<TProvider>? configure = null)
        where TProvider : class, IAnalyticsProvider;
}