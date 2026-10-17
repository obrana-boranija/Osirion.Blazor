using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Web.Feeds;
using Osirion.Blazor.Cms.Web.Options;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Feeds;

public class ContentFeedEndpointsTests
{
    private static readonly XNamespace Sitemap = "http://www.sitemaps.org/schemas/sitemap/0.9";
    private static readonly XNamespace Atom = "http://www.w3.org/2005/Atom";

    private readonly IContentProviderManager _manager = Substitute.For<IContentProviderManager>();
    private ContentQuery? _lastQuery;

    public ContentFeedEndpointsTests()
    {
        _manager.GetContentByQueryAsync(Arg.Do<ContentQuery>(q => _lastQuery = q), Arg.Any<CancellationToken>())
            .Returns(new List<ContentItem>
            {
                Item("blog/second-post", "Second post", new DateTime(2026, 9, 2, 8, 30, 0, DateTimeKind.Utc)),
                Item("blog/first-post", "First post", new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc)),
            });
    }

    [Fact]
    public async Task Sitemap_ListsEachItemOnceWithAnAbsoluteUrlAndLastModified()
    {
        var hidden = Item("blog/draft-like", "Hidden", new DateTime(2026, 9, 3, 0, 0, 0, DateTimeKind.Utc));
        hidden.Metadata = new() { SeoProperties = new() { Robots = "noindex, follow" } };
        _manager.GetContentByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>()).Returns(new List<ContentItem>
        {
            Item("blog/second-post", "Second post", new DateTime(2026, 9, 2, 8, 30, 0, DateTimeKind.Utc)),
            Item("blog/second-post", "Second post (sr)", new DateTime(2026, 9, 2, 8, 30, 0, DateTimeKind.Utc)),
            hidden,
        });
        var options = new OsirionFeedOptions { SiteUrl = "https://example.com", AdditionalSitemapUrls = ["/", "/blog"] };

        var (context, body) = await InvokeAsync(ContentFeedKind.Sitemap, options);

        context.Response.ContentType.ShouldBe("application/xml; charset=utf-8");
        var urls = XDocument.Parse(body).Root!.Elements(Sitemap + "url").ToList();
        urls.Select(u => u.Element(Sitemap + "loc")!.Value).ShouldBe(["https://example.com/", "https://example.com/blog", "https://example.com/blog/second-post"]);
        urls[2].Element(Sitemap + "lastmod")!.Value.ShouldBe("2026-09-02T08:30:00Z");
        urls[0].Element(Sitemap + "lastmod").ShouldBeNull();
    }

    [Fact]
    public async Task Rss_WritesAChannelWithItemsNewestFirst()
    {
        var options = new OsirionFeedOptions { SiteUrl = "https://example.com/", Title = "Osirion blog", Description = "News" };

        var (context, body) = await InvokeAsync(ContentFeedKind.Rss, options);

        context.Response.ContentType.ShouldBe("application/rss+xml; charset=utf-8");
        context.Response.Headers.CacheControl.ToString().ShouldBe("public, max-age=600");
        var channel = XDocument.Parse(body).Root!.Element("channel")!;
        channel.Element("title")!.Value.ShouldBe("Osirion blog");
        channel.Element("link")!.Value.ShouldBe("https://example.com/");
        channel.Element(Atom + "link")!.Attribute("href")!.Value.ShouldBe("https://example.com/feed.xml");
        var items = channel.Elements("item").ToList();
        items.Count.ShouldBe(2);
        items[0].Element("title")!.Value.ShouldBe("Second post");
        items[0].Element("link")!.Value.ShouldBe("https://example.com/blog/second-post");
        items[0].Element("guid")!.Value.ShouldBe("https://example.com/blog/second-post");
        items[0].Element("pubDate")!.Value.ShouldBe("Wed, 02 Sep 2026 08:30:00 GMT");
        items[0].Element("description")!.Value.ShouldBe("About Second post");
    }

    [Fact]
    public async Task Atom_WritesEntriesWithIdsDatesAndAnAuthor()
    {
        var options = new OsirionFeedOptions { SiteUrl = "https://example.com", Title = "Osirion blog" };

        var (context, body) = await InvokeAsync(ContentFeedKind.Atom, options);

        context.Response.ContentType.ShouldBe("application/atom+xml; charset=utf-8");
        var feed = XDocument.Parse(body).Root!;
        feed.Name.ShouldBe(Atom + "feed");
        feed.Element(Atom + "updated")!.Value.ShouldBe("2026-09-02T08:30:00Z");
        feed.Element(Atom + "author")!.Element(Atom + "name")!.Value.ShouldBe("Osirion blog");
        var entry = feed.Elements(Atom + "entry").First();
        entry.Element(Atom + "id")!.Value.ShouldBe("https://example.com/blog/second-post");
        entry.Element(Atom + "link")!.Attribute("href")!.Value.ShouldBe("https://example.com/blog/second-post");
        entry.Element(Atom + "updated")!.Value.ShouldBe("2026-09-02T08:30:00Z");
        entry.Element(Atom + "author")!.Element(Atom + "name")!.Value.ShouldBe("Dejan");
    }

    [Fact]
    public async Task Feed_QueriesTheConfiguredDirectoryLocaleAndCount()
    {
        var options = new OsirionFeedOptions { SiteUrl = "https://example.com", FeedDirectory = "blog", FeedLocale = "en", FeedItemCount = 5 };

        await InvokeAsync(ContentFeedKind.Rss, options);

        _lastQuery.ShouldNotBeNull();
        _lastQuery.Directory.ShouldBe("blog");
        _lastQuery.Locale.ShouldBe("en");
        _lastQuery.Take.ShouldBe(5);
        _lastQuery.IncludeUnpublished.ShouldBeFalse();
    }

    [Fact]
    public async Task Feed_NoSiteUrl_UsesTheRequestSchemeHostAndPathBase()
    {
        var (_, body) = await InvokeAsync(ContentFeedKind.Rss, new OsirionFeedOptions(), request =>
        {
            request.Scheme = "http";
            request.Host = new HostString("localhost", 5296);
            request.PathBase = "/site";
        });

        XDocument.Parse(body).Root!.Element("channel")!.Element("item")!.Element("link")!.Value
            .ShouldBe("http://localhost:5296/site/blog/second-post");
    }

    [Fact]
    public async Task Sitemap_NoFeedSiteUrl_UsesTheSeoSiteUrl()
    {
        // Fails before the fix: only OsirionFeedOptions.SiteUrl was read, so the sitemap listed the request's
        // address (http://request.example behind a proxy) although Osirion:Seo:SiteUrl named the site.
        var (_, body) = await InvokeAsync(ContentFeedKind.Sitemap, new OsirionFeedOptions(), seoSiteUrl: "https://example.com/");

        XDocument.Parse(body).Root!.Elements(Sitemap + "url").Select(u => u.Element(Sitemap + "loc")!.Value)
            .ShouldBe(["https://example.com/blog/second-post", "https://example.com/blog/first-post"]);
    }

    [Fact]
    public async Task Rss_NoFeedSiteUrl_UsesTheSeoSiteUrlForTheChannelAndItems()
    {
        var (_, body) = await InvokeAsync(ContentFeedKind.Rss, new OsirionFeedOptions(), seoSiteUrl: "https://example.com");

        var channel = XDocument.Parse(body).Root!.Element("channel")!;
        channel.Element("link")!.Value.ShouldBe("https://example.com/");
        channel.Element("item")!.Element("link")!.Value.ShouldBe("https://example.com/blog/second-post");
    }

    [Fact]
    public async Task Feed_FeedSiteUrlAndSeoSiteUrl_FeedSiteUrlWins()
    {
        var (_, body) = await InvokeAsync(
            ContentFeedKind.Rss,
            new OsirionFeedOptions { SiteUrl = "https://feeds.example.com" },
            seoSiteUrl: "https://example.com");

        XDocument.Parse(body).Root!.Element("channel")!.Element("item")!.Element("link")!.Value
            .ShouldBe("https://feeds.example.com/blog/second-post");
    }

    [Theory]
    [InlineData("not a url")]
    [InlineData("/relative")]
    [InlineData("ftp://example.com")]
    public async Task Feed_SeoSiteUrlNotAnAbsoluteHttpUrl_FallsBackToTheRequest(string seoSiteUrl)
    {
        var (_, body) = await InvokeAsync(ContentFeedKind.Rss, new OsirionFeedOptions(), seoSiteUrl: seoSiteUrl);

        XDocument.Parse(body).Root!.Element("channel")!.Element("item")!.Element("link")!.Value
            .ShouldBe("https://request.example/blog/second-post");
    }

    [Fact]
    public async Task Feed_TitleWithMarkup_IsEscapedAndRoundTrips()
    {
        _manager.GetContentByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>()).Returns(new List<ContentItem>
        {
            Item("blog/x", "</title><script>alert(1)</script> & more", new DateTime(2026, 9, 2, 0, 0, 0, DateTimeKind.Utc)),
        });

        var (_, body) = await InvokeAsync(ContentFeedKind.Rss, new OsirionFeedOptions { SiteUrl = "https://example.com" });

        body.ShouldNotContain("<script>");
        XDocument.Parse(body).Root!.Element("channel")!.Element("item")!.Element("title")!.Value
            .ShouldBe("</title><script>alert(1)</script> & more");
    }

    [Fact]
    public async Task Feed_TitleWithACharacterXmlCannotHold_DropsOnlyThatCharacter()
    {
        _manager.GetContentByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>()).Returns(new List<ContentItem>
        {
            Item("blog/x", "Bell\u0007 and emoji \U0001F600", new DateTime(2026, 9, 2, 0, 0, 0, DateTimeKind.Utc)),
        });

        var (_, body) = await InvokeAsync(ContentFeedKind.Atom, new OsirionFeedOptions { SiteUrl = "https://example.com" });

        XDocument.Parse(body).Root!.Element(Atom + "entry")!.Element(Atom + "title")!.Value
            .ShouldBe("Bell and emoji \U0001F600");
    }

    [Fact]
    public async Task Feed_NoTitleConfigured_FallsBackToTheSeoSiteName()
    {
        var (_, body) = await InvokeAsync(ContentFeedKind.Rss, new OsirionFeedOptions { SiteUrl = "https://example.com" });

        var channel = XDocument.Parse(body).Root!.Element("channel")!;
        channel.Element("title")!.Value.ShouldBe("Osirion");
        channel.Element("description")!.Value.ShouldBe("Docs");
    }

    [Fact]
    public async Task Feed_ContentUrlFormatter_BuildsTheItemPaths()
    {
        var options = new OsirionFeedOptions { SiteUrl = "https://example.com", ContentUrlFormatter = item => $"/posts/{item.Url}" };

        var (_, body) = await InvokeAsync(ContentFeedKind.Rss, options);

        XDocument.Parse(body).Root!.Element("channel")!.Element("item")!.Element("link")!.Value
            .ShouldBe("https://example.com/posts/blog/second-post");
    }

    [Fact]
    public async Task Feed_ProviderIgnoresTake_ListsOnlyTheNewestConfiguredCount()
    {
        _manager.GetContentByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>()).Returns(new List<ContentItem>
        {
            Item("blog/old", "Old", new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
            Item("blog/new", "New", new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc)),
            Item("blog/middle", "Middle", new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc)),
        });

        var (_, body) = await InvokeAsync(ContentFeedKind.Rss, new OsirionFeedOptions { SiteUrl = "https://example.com", FeedItemCount = 2 });

        XDocument.Parse(body).Root!.Element("channel")!.Elements("item").Select(i => i.Element("title")!.Value)
            .ShouldBe(["New", "Middle"]);
    }

    [Fact]
    public async Task Atom_NoItems_IsStillAValidFeedWithAnUpdatedDate()
    {
        _manager.GetContentByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>()).Returns(new List<ContentItem>());

        var (_, body) = await InvokeAsync(ContentFeedKind.Atom, new OsirionFeedOptions { SiteUrl = "https://example.com" });

        var feed = XDocument.Parse(body).Root!;
        feed.Elements(Atom + "entry").ShouldBeEmpty();
        DateTimeOffset.TryParse(feed.Element(Atom + "updated")!.Value, out _).ShouldBeTrue();
    }

    [Fact]
    public void Validate_FeedItemCountBelowOne_Throws()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new OsirionFeedOptions { FeedItemCount = 0 }.Validate());
    }

    [Theory]
    [InlineData("example.com")]
    [InlineData("ftp://example.com")]
    [InlineData("/relative")]
    public void Validate_SiteUrlNotAnAbsoluteHttpUrl_Throws(string siteUrl)
    {
        Should.Throw<ArgumentException>(() => new OsirionFeedOptions { SiteUrl = siteUrl }.Validate());
    }

    private async Task<(HttpContext Context, string Body)> InvokeAsync(
        ContentFeedKind kind, OsirionFeedOptions options, Action<HttpRequest>? configureRequest = null, string? seoSiteUrl = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton(_manager);
        services.AddSingleton(Microsoft.Extensions.Options.Options.Create(new SeoMetadataOptions { SiteName = "Osirion", SiteDescription = "Docs", SiteUrl = seoSiteUrl }));
        var context = new DefaultHttpContext { RequestServices = services.BuildServiceProvider() };
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("request.example");
        configureRequest?.Invoke(context.Request);
        var body = new MemoryStream();
        context.Response.Body = body;

        await ContentFeedEndpoints.HandleAsync(context, kind, options);

        return (context, System.Text.Encoding.UTF8.GetString(body.ToArray()));
    }

    private static ContentItem Item(string url, string title, DateTime date)
    {
        var item = ContentItem.Create(url, title, "<p>Body</p>", $"{url}.md", "test");
        item.Url = url;
        item.DateCreated = date;
        item.Description = $"About {title}";
        item.Author = "Dejan";
        return item;
    }
}
