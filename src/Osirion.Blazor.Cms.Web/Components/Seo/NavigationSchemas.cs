using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Web.Components.Seo;

/// <summary>
/// Builds the schema.org objects that describe where a page sits in the site and what it lists: the breadcrumb
/// trail, a collection page and the posts it lists, and the generated FAQ page.
/// </summary>
internal static class NavigationSchemas
{
    private const string SchemaContext = "https://schema.org";

    /// <summary>
    /// The breadcrumb trail: Home, then the custom breadcrumbs when given, otherwise one item per segment of the
    /// content URL (the last one named with the content title), otherwise the content alone.
    /// </summary>
    internal static object Breadcrumbs(SeoSchemaContext context)
    {
        List<Dictionary<string, object?>> items = [ListItem(1, context.BreadcrumbHomeText, context.BaseUrl)];

        if (context.CustomBreadcrumbs?.Any() == true)
        {
            items.AddRange(context.CustomBreadcrumbs.Select((crumb, index) => ListItem(index + 2, crumb.Name, crumb.Url)));
        }
        else if (!string.IsNullOrWhiteSpace(context.Content?.Url))
        {
            items.AddRange(PathBreadcrumbs(context, context.Content));
        }
        else if (context.Content != null)
        {
            items.Add(ListItem(2, context.Content.Title, context.CurrentUrl));
        }

        return new Dictionary<string, object?>
        {
            ["@context"] = SchemaContext,
            ["@type"] = "BreadcrumbList",
            ["itemListElement"] = items.ToArray()
        };
    }

    /// <summary>
    /// A collection page with its items as an item list, a two-level breadcrumb trail and the site as publisher.
    /// </summary>
    internal static object CollectionPage(SeoSchemaContext context)
    {
        var itemListElements = (context.ContentItems ?? [])
            .Select((item, index) => new Dictionary<string, object?>
            {
                ["@type"] = "ListItem",
                ["position"] = index + 1,
                ["url"] = $"{context.BaseUrl}{item.Url}",
                ["name"] = item.Title,
                ["description"] = item.Description,
                ["datePublished"] = ContentUrls.FormatUtc(item.PublishDate),
                ["image"] = context.NormalizeImageUrl(item.FeaturedImageUrl)
            })
            .ToList();

        return new Dictionary<string, object?>
        {
            ["@context"] = SchemaContext,
            ["@type"] = "CollectionPage",
            ["name"] = context.CollectionTitle ?? context.MetaTitle,
            ["description"] = context.CollectionDescription ?? context.MetaDescription,
            ["url"] = context.CurrentUrl,
            ["inLanguage"] = context.LanguageCode,
            ["mainEntity"] = new Dictionary<string, object?>
            {
                ["@type"] = "ItemList",
                ["numberOfItems"] = context.ContentItems?.Count ?? 0,
                ["itemListElement"] = itemListElements
            },
            ["breadcrumb"] = new Dictionary<string, object?>
            {
                ["@type"] = "BreadcrumbList",
                ["itemListElement"] = new[]
                {
                    ListItem(1, context.BreadcrumbHomeText, context.BaseUrl),
                    ListItem(2, context.CollectionTitle ?? "Collection", context.CurrentUrl)
                }
            },
            ["publisher"] = ContentSchemas.PublisherWithLogo(context)
        };
    }

    /// <summary>A blog posting for one item of a collection page.</summary>
    internal static object CollectionItemPosting(ContentItem item, SeoSchemaContext context)
    {
        var itemUrl = $"{context.BaseUrl.TrimEnd('/')}{item.Url}";
        return new Dictionary<string, object?>
        {
            ["@context"] = SchemaContext,
            ["@type"] = "BlogPosting",
            ["headline"] = item.Title,
            ["description"] = item.Description,
            ["image"] = context.NormalizeImageUrl(item.FeaturedImageUrl) ?? context.OgImage,
            ["datePublished"] = ContentUrls.FormatUtc(item.PublishDate),
            ["dateModified"] = ContentUrls.FormatUtc(item.LastModified ?? item.PublishDate),
            ["author"] = new Dictionary<string, object?>
            {
                ["@type"] = "Person",
                ["name"] = PostingAuthor(item, context)
            },
            ["publisher"] = new Dictionary<string, object?>
            {
                ["@type"] = "Organization",
                ["name"] = context.SiteName,
                ["logo"] = new Dictionary<string, object?>
                {
                    ["@type"] = "ImageObject",
                    ["url"] = context.PublisherLogoUrl
                }
            },
            ["url"] = itemUrl,
            ["mainEntityOfPage"] = new Dictionary<string, object?>
            {
                ["@type"] = "WebPage",
                ["@id"] = itemUrl
            },
            ["keywords"] = JoinTags(item.Tags),
            ["articleSection"] = item.Categories?.FirstOrDefault(),
            ["inLanguage"] = item.Locale ?? context.LanguageCode,
            ["timeRequired"] = ReadingDuration(item.ReadTimeMinutes)
        };
    }

    /// <summary>A FAQ page with one question per entry of <paramref name="answers"/> (question to answer text).</summary>
    internal static object FaqPage(IReadOnlyDictionary<string, string> answers) => new Dictionary<string, object?>
    {
        ["@context"] = SchemaContext,
        ["@type"] = "FAQPage",
        ["mainEntity"] = answers.Select(answer => new Dictionary<string, object?>
        {
            ["@type"] = "Question",
            ["name"] = answer.Key,
            ["acceptedAnswer"] = new Dictionary<string, object?>
            {
                ["@type"] = "Answer",
                ["text"] = answer.Value
            }
        }).ToArray()
    };

    private static string PostingAuthor(ContentItem item, SeoSchemaContext context) =>
        item.Author ?? context.Author ?? "Anonymous";

    private static string? JoinTags(IReadOnlyList<string>? tags) => tags?.Any() == true ? string.Join(", ", tags) : null;

    private static string? ReadingDuration(int minutes) => minutes > 0 ? $"PT{minutes}M" : null;

    private static IEnumerable<Dictionary<string, object?>> PathBreadcrumbs(SeoSchemaContext context, ContentItem content)
    {
        var position = 2;
        foreach (var segment in new Osirion.Blazor.Components.BreadcrumbPath(content.Url).Segments)
        {
            // The last item is this page: its title keeps the punctuation and casing a slug loses. Earlier segments
            // are directories, named from the slug.
            yield return segment.IsLast
                ? ListItem(position++, content.Title, context.CurrentUrl)
                : ListItem(position++, SeoText.FormatBreadcrumbSegment(segment.Name), $"{context.BaseUrl}/{segment.Path}");
        }
    }

    private static Dictionary<string, object?> ListItem(int position, string? name, string? item) => new()
    {
        ["@type"] = "ListItem",
        ["position"] = position,
        ["name"] = name,
        ["item"] = item
    };
}
