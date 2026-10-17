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
        Add<ArticleMetadataDemo>(demos, "article-metadata", "Article metadata");
        Add<CalloutDemo>(demos, "callout", "Callouts");
        Add<AccordionFaqDemo>(demos, "accordion-and-faq", "An accordion and an FAQ");
        Add<ResponsiveImageDemo>(demos, "responsive-image", "A responsive image");
        Add<VideoEmbedDemo>(demos, "video-embed", "A video embed");
        Add<ReadingProgressDemo>(demos, "reading-progress", "A reading progress bar");
        Add<RevealDemo>(demos, "reveal", "Reveal on scroll");
        Add<HtmlRendererDemo>(demos, "html-renderer", "Sanitized HTML with code");
        Add<ContentDemoDemo>(demos, "content-demo", "A registered demo");
    }

    private static void Add<TDemo>(ContentDemoRegistry demos, string key, string title)
        where TDemo : IComponent =>
        demos.Add<TDemo>(key, title: title, source: ContentDemoRegistry.ReadEmbeddedSource(
            typeof(TDemo).Assembly, $"{typeof(TDemo).Namespace}.{typeof(TDemo).Name}.razor"));
}
