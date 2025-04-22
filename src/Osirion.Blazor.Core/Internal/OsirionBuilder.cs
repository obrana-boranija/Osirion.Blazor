using Microsoft.Extensions.DependencyInjection;

namespace Osirion.Blazor.Internal;

internal class OsirionBuilder : IOsirionBuilder
{
    public OsirionBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public IServiceCollection Services { get; }

    //public IOsirionBuilder UseContent(Action<IContentBuilder> configure)
    //{
    //    Services.AddOsirionContent(configure);
    //    return this;
    //}

    //public IOsirionBuilder UseAnalytics(Action<IAnalyticsBuilder> configure)
    //{
    //    Services.AddOsirionAnalytics(configure);
    //    return this;
    //}

    //public IOsirionBuilder UseNavigation(Action<INavigationBuilder> configure)
    //{
    //    Services.AddOsirionNavigation(configure);
    //    return this;
    //}

    //public IOsirionBuilder UseTheming(Action<IThemingBuilder> configure)
    //{
    //    Services.AddOsirionTheming(configure);
    //    return this;
    //}
}