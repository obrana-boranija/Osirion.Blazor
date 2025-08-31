using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Analytics;
using Osirion.Blazor.Analytics.Extensions;
using Osirion.Blazor.Cms.Admin.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Web.DependencyInjection;
using Osirion.Blazor.Core.Configuration;
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
    public TestOsirionBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public IServiceCollection Services { get; }

    public bool UseContentCalled { get; private set; }
    public bool UseAnalyticsCalled { get; private set; }
    public bool UseNavigationCalled { get; private set; }
    public bool UseThemingCalled { get; private set; }

    public IOsirionBuilder UseContent(Action<IContentBuilder> configure)
    {
        if (configure is null) throw new ArgumentNullException(nameof(configure));

        UseContentCalled = true;
        Services.AddOsirionContent(configure);
        return this;
    }

    public IOsirionBuilder UseContent(IConfiguration configuration)
    {
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        UseContentCalled = true;
        Services.AddOsirionContent(builder =>
        {
            builder.AddGitHub(options => configuration.GetSection("Osirion:Content:GitHub").Bind(options));
            builder.AddFileSystem(options => configuration.GetSection("Osirion:Content:FileSystem").Bind(options));
        });
        return this;
    }

    public IOsirionBuilder UseAnalytics(Action<IAnalyticsBuilder> configure)
    {
        if (configure is null) throw new ArgumentNullException(nameof(configure));

        UseAnalyticsCalled = true;
        Services.AddOsirionAnalytics(configure);
        return this;
    }

    public IOsirionBuilder UseAnalytics(IConfiguration configuration)
    {
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        UseAnalyticsCalled = true;
        Services.AddOsirionAnalytics(builder =>
        {
            builder.AddClarity(options => configuration.GetSection("Osirion:Analytics:Clarity").Bind(options));
            builder.AddMatomo(options => configuration.GetSection("Osirion:Analytics:Matomo").Bind(options));
        });
        return this;
    }

    public IOsirionBuilder UseNavigation(Action<INavigationBuilder> configure)
    {
        if (configure is null) throw new ArgumentNullException(nameof(configure));

        UseNavigationCalled = true;
        Services.AddOsirionNavigation(configure);
        return this;
    }

    public IOsirionBuilder UseNavigation(IConfiguration configuration)
    {
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        UseNavigationCalled = true;
        Services.AddOsirionNavigation(builder =>
        {
            builder.AddEnhancedNavigation(configuration.GetSection("Osirion:Navigation:Enhanced"));
            builder.AddScrollToTop(configuration.GetSection("Osirion:Navigation:ScrollToTop"));
        });
        return this;
    }

    public IOsirionBuilder UseTheming(Action<IThemingBuilder> configure)
    {
        if (configure is null) throw new ArgumentNullException(nameof(configure));

        UseThemingCalled = true;
        Services.AddOsirionTheming(configure);
        return this;
    }

    public IOsirionBuilder UseTheming(IConfiguration configuration)
    {
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        UseThemingCalled = true;
        Services.AddOsirionTheming(builder =>
        {
            builder.Configure(options => configuration.GetSection("Osirion:Theming").Bind(options));
        });
        return this;
    }

    public IOsirionBuilder UseCmsAdmin(Action<ICmsAdminBuilder> configure)
    {
        throw new NotImplementedException();
    }

    public IOsirionBuilder UseCmsAdmin(IConfiguration configuration)
    {
        throw new NotImplementedException();
    }

    public IOsirionBuilder UseEmailServices(Action<EmailOptions> configure)
    {
        throw new NotImplementedException();
    }

    public IOsirionBuilder UseEmailServices(IConfiguration configuration)
    {
        throw new NotImplementedException();
    }
}