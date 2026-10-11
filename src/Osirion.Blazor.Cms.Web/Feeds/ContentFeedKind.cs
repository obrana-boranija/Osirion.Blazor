namespace Osirion.Blazor.Cms.Web.Feeds;

/// <summary>The documents <see cref="ContentFeedEndpoints"/> serves.</summary>
internal enum ContentFeedKind
{
    /// <summary>The sitemaps.org XML sitemap.</summary>
    Sitemap,

    /// <summary>An RSS 2.0 feed.</summary>
    Rss,

    /// <summary>An Atom 1.0 feed.</summary>
    Atom,
}
