using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Core.Services;

namespace Osirion.Blazor.Example.Bootstrap.Components.Demos;

/// <summary>
/// The live examples the site's Markdown embeds with a fenced <c>demo</c> block. Each demo is a component in this
/// folder; its file is embedded (see the project file), so the page's Code tab shows exactly what compiled.
/// </summary>
internal static class SiteDemos
{
    /// <summary>Registers every demo.</summary>
    /// <param name="demos">The registry from <c>AddOsirionContentDemos</c>.</param>
    public static void Register(ContentDemoRegistry demos)
    {
        Add<InstallTabsDemo>(demos, "install-tabs", "Install command tabs");
        Add<FeatureCardDemo>(demos, "feature-card", "A feature card");
        Add<TestimonialCardDemo>(demos, "testimonial-card", "A testimonial card");
        Add<SubscriptionCardDemo>(demos, "subscription-card", "A subscription card");
        Add<AuthorCardDemo>(demos, "author-card", "An author card");
        Add<MetricCardDemo>(demos, "metric-card", "Metric cards");
        Add<PageLayoutDemo>(demos, "page-layout", "A page layout in a frame");
        Add<PageLoadingDemo>(demos, "page-loading", "Page loading spinners");
        Add<FooterDemo>(demos, "footer", "A footer");
        Add<StickySidebarDemo>(demos, "sticky-sidebar", "A sticky sidebar");
        Add<BackgroundPatternDemo>(demos, "background-pattern", "Background patterns");
    }

    private static void Add<TDemo>(ContentDemoRegistry demos, string key, string title)
        where TDemo : IComponent =>
        demos.Add<TDemo>(key, title: title, source: ContentDemoRegistry.ReadEmbeddedSource(
            typeof(TDemo).Assembly, $"{typeof(TDemo).Namespace}.{typeof(TDemo).Name}.razor"));
}
