using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Analytics;
using Osirion.Blazor.Analytics.Extensions;
using Osirion.Blazor.Cms.Admin.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Front.DependencyInjection;
using Osirion.Blazor.Navigation;
using Osirion.Blazor.Theming;
using Osirion.Blazor.Theming.Extensions;
using Osirion.Blazor.Navigation.Extensions;

namespace Osirion.Blazor.Internal;

internal class OsirionBuilder : IOsirionBuilder
{
    public OsirionBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public IServiceCollection Services { get; }

    public IOsirionBuilder UseContent(Action<IContentBuilder> configure)
    {
        Services.AddOsirionContent(configure);
        return this;
    }

    public IOsirionBuilder UseCmsAdmin(Action<ICmsAdminBuilder> configure)
    {
        //Services.AddOsirionCmsAdmin(configure);
        return this;
    }

    //public IOsirionBuilder UseTheming(Action<IThemingBuilder> configure)
    //{
    //    Services.AddOsirionTheming(configure);
    //    return this;
    //}

    public IOsirionBuilder UseContent(IConfiguration configuration)
    {
        Services.AddOsirionContent(configuration);
        return this;
    }

    public IOsirionBuilder UseCmsAdmin(IConfiguration configuration)
    {
        Services.AddOsirionCmsAdmin(configuration);
        return this;
    }

    /// <inheritdoc/>
    public IOsirionBuilder UseAnalytics(Action<IAnalyticsBuilder> configure)
    {
        Services.AddOsirionAnalytics(configure);
        return this;
    }

    /// <inheritdoc/>
    public IOsirionBuilder UseAnalytics(IConfiguration configuration)
    {
        Services.AddOsirionAnalytics(configuration);
        return this;
    }

    /// <inheritdoc/>
    public IOsirionBuilder UseNavigation(Action<INavigationBuilder> configure)
    {
        Services.AddOsirionNavigation(configure);
        return this;
    }

    /// <inheritdoc/>
    public IOsirionBuilder UseNavigation(IConfiguration configuration)
    {
        Services.AddOsirionNavigation(configuration);
        return this;
    }

    /// <inheritdoc/>
    public IOsirionBuilder UseTheming(Action<IThemingBuilder> configure)
    {
        Services.AddOsirionTheming(configure);
        return this;
    }

    /// <inheritdoc/>
    public IOsirionBuilder UseTheming(IConfiguration configuration)
    {
        Services.AddOsirionTheming(configuration);
        return this;
    }

    //public IOsirionBuilder UseTheming(IConfiguration configuration)
    //{
    //    Services.AddOsirionTheming(configuration);
    //    return this;
    //}
}