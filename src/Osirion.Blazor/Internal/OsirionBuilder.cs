using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Analytics;
using Osirion.Blazor.Analytics.Extensions;
using Osirion.Blazor.Cms;
using Osirion.Blazor.Cms.Admin.Extensions;
using Osirion.Blazor.Cms.Admin.Interfaces;
using Osirion.Blazor.Cms.Core.Extensions;
using Osirion.Blazor.Navigation;
using Osirion.Blazor.Navigation.Extensions;
using Osirion.Blazor.Theming;
using Osirion.Blazor.Theming.Extensions;

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
        Services.AddOsirionCmsAdmin(configure);
        return this;
    }

    public IOsirionBuilder UseAnalytics(Action<IAnalyticsBuilder> configure)
    {
        Services.AddOsirionAnalytics(configure);
        return this;
    }

    public IOsirionBuilder UseNavigation(Action<INavigationBuilder> configure)
    {
        Services.AddOsirionNavigation(configure);
        return this;
    }

    public IOsirionBuilder UseTheming(Action<IThemingBuilder> configure)
    {
        Services.AddOsirionTheming(configure);
        return this;
    }
}