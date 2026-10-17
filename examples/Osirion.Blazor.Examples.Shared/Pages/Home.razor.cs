using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Examples.Shared.Pages;

public partial class Home(IContentProviderManager contentProviderManager)
{
    private static readonly FeaturedComponent[] Cards =
    [
        new("ContentList", "/components/content-list", "A grid of content cards, filtered by directory, category, tag, author or search, with paging."),
        new("HeroSection", "/components/hero-section", "A page header with a title, subtitle, buttons and a side image, background image or pattern."),
        new("TableOfContents", "/components/table-of-contents", "Links to the headings of a rendered article, as a nested list."),
        new("OsirionTabs", "/components/tabs", "Tabs that switch without script and work with the keyboard."),
        new("OsirionCookieConsent", "/components/cookie-consent", "A consent banner that works without script and can hold analytics back until the visitor agrees."),
        new("SeoMetadataRenderer", "/components/seo-metadata-renderer", "Title, description, canonical URL, Open Graph, Twitter Card and JSON-LD tags for a page."),
    ];

    [Parameter]
    public string? Locale { get; set; } = "en";

    private ContentItem? Content { get; set; }

    private IReadOnlyList<ContentItem> LatestPosts { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        await LoadContentAsync();
        await LoadLatestPostsAsync();
    }

    private async Task LoadContentAsync()
    {
        var result = await contentProviderManager.GetContentByQueryAsync(new Cms.Domain.Repositories.ContentQuery { Locale = Locale, Slug = "homepage" });
        Content = result?.FirstOrDefault();
    }

    private async Task LoadLatestPostsAsync()
    {
        var provider = contentProviderManager.GetDefaultProvider();
        var items = provider is null ? [] : await provider.GetAllItemsAsync();

        // The blog folder's own page, /blog, is not a post.
        LatestPosts = [.. items
            .Where(item => item.IsPublished && item.Url.Trim('/').StartsWith("blog/", StringComparison.Ordinal))
            .OrderByDescending(item => item.DateCreated)
            .Take(3)];
    }

    private sealed record FeaturedComponent(string Title, string Href, string Text);
}
