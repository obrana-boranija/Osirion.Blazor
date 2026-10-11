using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Web.Feeds;

/// <summary>
/// Options for the sitemap, RSS and Atom endpoints that <see cref="ContentFeedEndpoints.MapOsirionFeeds"/> maps.
/// </summary>
public sealed class OsirionFeedOptions
{
    /// <summary>
    /// Gets or sets the site's public address, for example <c>https://getosirion.com</c>, used for every absolute URL
    /// in the sitemap and feeds. When empty, the request's scheme, host and path base are used; set it in production
    /// so a forged <c>Host</c> header cannot change the URLs a cache or crawler stores.
    /// </summary>
    public string? SiteUrl { get; set; }

    /// <summary>
    /// Gets or sets the feed title. Defaults to the SEO <c>SiteName</c>, then the host name.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the feed description. Defaults to the SEO <c>SiteDescription</c>, then the title.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the content directory the feeds cover, for example <c>blog</c>. Defaults to all content.
    /// </summary>
    public string? FeedDirectory { get; set; }

    /// <summary>
    /// Gets or sets the locale the feeds cover, for example <c>en</c>. Defaults to every locale.
    /// </summary>
    public string? FeedLocale { get; set; }

    /// <summary>
    /// Gets or sets how many of the newest items the feeds list. Defaults to 20.
    /// </summary>
    public int FeedItemCount { get; set; } = 20;

    /// <summary>
    /// Gets or sets paths that are not content items but belong in the sitemap, such as <c>/</c> or <c>/blog</c>.
    /// </summary>
    public IList<string> AdditionalSitemapUrls { get; set; } = [];

    /// <summary>
    /// Gets or sets how a content item's path is built. Defaults to <c>/{item.Url}</c>, the same path the content
    /// lists link to; set it when the site routes content elsewhere.
    /// </summary>
    public Func<ContentItem, string>? ContentUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets how long browsers and caches may keep a response. Defaults to 10 minutes.
    /// </summary>
    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(10);

    /// <summary>Gets or sets whether <c>/sitemap.xml</c> is mapped. Defaults to <see langword="true"/>.</summary>
    public bool EnableSitemap { get; set; } = true;

    /// <summary>Gets or sets whether the RSS 2.0 feed at <c>/feed.xml</c> is mapped. Defaults to <see langword="true"/>.</summary>
    public bool EnableRss { get; set; } = true;

    /// <summary>Gets or sets whether the Atom feed at <c>/atom.xml</c> is mapped. Defaults to <see langword="true"/>.</summary>
    public bool EnableAtom { get; set; } = true;

    /// <summary>
    /// Checks the options and throws when they cannot produce valid URLs or feeds.
    /// </summary>
    /// <exception cref="ArgumentException"><see cref="SiteUrl"/> is not an absolute http or https URL.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><see cref="FeedItemCount"/> is less than 1.</exception>
    public void Validate()
    {
        if (!string.IsNullOrWhiteSpace(SiteUrl)
            && (!Uri.TryCreate(SiteUrl, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)))
        {
            throw new ArgumentException($"SiteUrl must be an absolute http or https URL, but was '{SiteUrl}'.", nameof(SiteUrl));
        }

        if (FeedItemCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(FeedItemCount), FeedItemCount, "FeedItemCount must be at least 1.");
        }
    }
}
