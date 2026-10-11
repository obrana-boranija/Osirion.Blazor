using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

/// <summary>
/// Content with fixed dates, so the SEO meta tags and structured data rendered from it are the same on every run.
/// </summary>
internal static class SeoTestContent
{
    public const string Origin = "http://localhost";

    public const string PageUrl = "http://localhost/blog/guides/my-post";

    public const string Description = "A practical description of testing. It covers the basics.";

    public static ContentItem Article(
        string title = "Guide to testing Blazor",
        string url = "/blog/guides/my-post",
        string locale = "en",
        bool withTaxonomy = true)
    {
        var item = new ContentItem
        {
            Title = title,
            Author = "Jane Doe",
            Content = "one two three four five",
            Description = Description,
            Path = "blog/guides/my-post.md",
            Url = url,
            Locale = locale,
            FeaturedImageUrl = "/images/cover.png",
            DateCreated = new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        };

        if (withTaxonomy)
        {
            item.AddTag("blazor");
            item.AddTag("testing");
            item.AddCategory("Guides");
            item.AddCategory("Dev");
        }

        // Set last: adding a tag or category stamps LastModified with the current time.
        item.LastModified = new DateTime(2020, 2, 3, 4, 5, 6, DateTimeKind.Utc);
        return item;
    }

    /// <summary>A second collection item without author, image, body or modification date.</summary>
    public static ContentItem BarePost()
    {
        var item = Article(title: "Second post", url: "/blog/second");
        item.Author = string.Empty;
        item.FeaturedImageUrl = null;
        item.Content = string.Empty;
        item.LastModified = null;
        return item;
    }
}
