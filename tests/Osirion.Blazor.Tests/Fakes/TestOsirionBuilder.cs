using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Analytics;
using Osirion.Blazor.Analytics.Extensions;
using Osirion.Blazor.Cms;
using Osirion.Blazor.Cms.Admin.Interfaces;
using Osirion.Blazor.Cms.Core.Extensions;
using Osirion.Blazor.Navigation;
using Osirion.Blazor.Navigation.Extensions;
using Osirion.Blazor.Theming;
using Osirion.Blazor.Theming.Extensions;

namespace Osirion.Blazor.Tests.Fakes;

/// <summary>
/// Test implementation of IOsirionBuilder for unit testing
/// </summary>
public class TestOsirionBuilder : IOsirionBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestOsirionBuilder"/> class.
    /// </summary>
    public TestOsirionBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Tracks whether UseContent was called
    /// </summary>
    public bool UseContentCalled { get; private set; }

    /// <summary>
    /// Tracks whether UseAnalytics was called
    /// </summary>
    public bool UseAnalyticsCalled { get; private set; }

    /// <summary>
    /// Tracks whether UseNavigation was called
    /// </summary>
    public bool UseNavigationCalled { get; private set; }

    /// <summary>
    /// Tracks whether UseTheming was called
    /// </summary>
    public bool UseThemingCalled { get; private set; }

    /// <inheritdoc/>
    public IOsirionBuilder UseContent(Action<IContentBuilder> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        UseContentCalled = true;
        Services.AddOsirionContent(configure);
        return this;
    }

    /// <inheritdoc/>
    public IOsirionBuilder UseAnalytics(Action<IAnalyticsBuilder> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        UseAnalyticsCalled = true;
        Services.AddOsirionAnalytics(configure);
        return this;
    }

    /// <inheritdoc/>
    public IOsirionBuilder UseNavigation(Action<INavigationBuilder> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        UseNavigationCalled = true;
        Services.AddOsirionNavigation(configure);
        return this;
    }

    /// <inheritdoc/>
    public IOsirionBuilder UseTheming(Action<IThemingBuilder> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        UseThemingCalled = true;
        Services.AddOsirionTheming(configure);
        return this;
    }

    public IOsirionBuilder UseCmsAdmin(Action<ICmsAdminBuilder> configure)
    {
        throw new NotImplementedException();
    }
}