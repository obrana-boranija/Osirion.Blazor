using Osirion.Blazor.Cms.Components;
using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Examples.Shared.Docs;

/// <summary>A group with the pages that joined it.</summary>
/// <param name="Group">The group.</param>
/// <param name="Page">The group's own page, when it has one and it is published.</param>
/// <param name="Members">The group's other pages, in order.</param>
public sealed record DocsSection(DocsGroup Group, ContentItem? Page, IReadOnlyList<ContentItem> Members)
{
    /// <summary>Where the group's heading links: its page, or its section of the component gallery.</summary>
    public string HeadingUrl => Page is not null ? DocsOutline.UrlOf(Page) : DocsOutline.GalleryUrl + "#" + Group.Key;
}

/// <summary>
/// The documentation's structure: which group each page belongs to, the sidebar, the order that previous and next
/// links follow, and the component gallery's sections. Built from the published content on every request, so a new
/// page appears once its front matter names a group.
/// </summary>
public sealed class DocsOutline
{
    /// <summary>The address of the component gallery.</summary>
    public const string GalleryUrl = "/components";

    /// <summary>The address of the theme page, the theming group's own page.</summary>
    public const string ThemesUrl = "/themes";

    // Documentation pages outside /docs and /components.
    private static readonly string[] RootPages = ["/docs", GalleryUrl, ThemesUrl, "/templates"];

    private const string GroupField = "group";

    private readonly List<ContentItem> _sequence;

    /// <summary>The groups, in the order the sidebar and the gallery show them.</summary>
    public static IReadOnlyList<DocsGroup> Groups { get; } =
    [
        new("getting-started", "Getting started", DocsGroupKind.Start),
        new("packages", "Packages", DocsGroupKind.Section, PageUrl: "/docs/packages"),
        new("layout", "Layout", DocsGroupKind.Section, "/components/layout", InGallery: true),
        new("navigation", "Navigation", DocsGroupKind.Section, "/components/navigation", InGallery: true,
            SeeAlso: new("The menu, the scroll to top button and enhanced navigation", "/docs/packages/navigation")),
        new("content", "Content", DocsGroupKind.Section, "/components/content", InGallery: true),
        new("sections", "Sections", DocsGroupKind.Section, "/components/sections", InGallery: true),
        new("cards", "Cards", DocsGroupKind.Section, "/components/cards", InGallery: true),
        new("forms-and-feedback", "Forms and feedback", DocsGroupKind.Section, "/components/forms-and-feedback", InGallery: true),
        new("cms-pages", "CMS pages", DocsGroupKind.Section, "/components/cms-pages", InGallery: true),
        new("cms-navigation", "CMS navigation", DocsGroupKind.Section, "/components/cms-navigation", InGallery: true),
        new("seo-and-feeds", "SEO and feeds", DocsGroupKind.Section, "/components/seo-and-feeds", InGallery: true),
        new("analytics", "Analytics", DocsGroupKind.Section, "/components/analytics", InGallery: true,
            SeeAlso: new("Install, registration, configuration and the consent gate for all six trackers", "/docs/packages/analytics")),
        new("theming", "Theming", DocsGroupKind.Section, ThemesUrl, InGallery: true,
            SeeAlso: new("Setup, options and the CSS framework adapters", "/docs/packages/theming")),
    ];

    private DocsOutline(IReadOnlyList<DocsSection> sections, IReadOnlyList<ContentItem> pages, IReadOnlyList<ContentItem> unplaced)
    {
        Sections = sections;
        Pages = pages;
        Unplaced = unplaced;
        _sequence = [.. sections.SelectMany(section => section.Page is null ? section.Members : [section.Page, .. section.Members])];
    }

    /// <summary>Every group with its pages, including groups that have none yet.</summary>
    public IReadOnlyList<DocsSection> Sections { get; }

    /// <summary>The published pages under <c>/docs</c> and <c>/components</c>, the theme page and the templates page.</summary>
    public IReadOnlyList<ContentItem> Pages { get; }

    /// <summary>Documentation pages that name no known group, so no link leads to them.</summary>
    public IReadOnlyList<ContentItem> Unplaced { get; }

    /// <summary>
    /// Builds the outline from content items; items outside <c>/docs</c>, <c>/components</c>, <c>/themes</c> and
    /// <c>/templates</c> are ignored.
    /// </summary>
    /// <param name="items">The published content.</param>
    /// <returns>The outline.</returns>
    public static DocsOutline Build(IEnumerable<ContentItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        var pages = items.Where(item => item.IsPublished && IsDocsUrl(UrlOf(item))).ToList();
        var byGroup = pages.ToLookup(GroupOf, StringComparer.Ordinal);
        var known = Groups.Select(group => group.Key).ToHashSet(StringComparer.Ordinal);

        var sections = Groups.Select(group =>
        {
            var inGroup = byGroup[group.Key].ToList();
            var page = group.PageUrl is null ? null : inGroup.FirstOrDefault(item => UrlOf(item) == group.PageUrl);
            var members = inGroup
                .Where(item => item != page)
                .OrderBy(item => item.OrderIndex)
                .ThenBy(item => item.Title, StringComparer.OrdinalIgnoreCase)
                .ToList();
            return new DocsSection(group, page, members);
        }).ToList();

        var unplaced = pages.Where(item => GroupOf(item) is not { } key || !known.Contains(key)).ToList();
        return new DocsOutline(sections, pages, unplaced);
    }

    /// <summary>The site address of a content item, such as <c>/components/tabs</c>.</summary>
    /// <param name="item">The item.</param>
    /// <returns>The address, starting with a slash.</returns>
    public static string UrlOf(ContentItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return "/" + item.Url.Trim('/');
    }

    /// <summary>Finds the page at an address. The match is exact, so an address in other letter case is not found.</summary>
    /// <param name="url">The address, with or without its leading slash.</param>
    /// <returns>The page, or <see langword="null"/>.</returns>
    public ContentItem? Find(string url)
    {
        var wanted = "/" + (url ?? string.Empty).Trim('/');
        return Pages.FirstOrDefault(item => string.Equals(UrlOf(item), wanted, StringComparison.Ordinal));
    }

    /// <summary>The pages before and after a page, in the order of the sidebar.</summary>
    /// <param name="page">The current page.</param>
    /// <returns>The neighbours; either is <see langword="null"/> at an end or when the page is not in a group.</returns>
    public (ContentItem? Previous, ContentItem? Next) NeighboursOf(ContentItem page)
    {
        var index = _sequence.IndexOf(page);
        if (index < 0)
        {
            return (null, null);
        }

        return (index > 0 ? _sequence[index - 1] : null, index < _sequence.Count - 1 ? _sequence[index + 1] : null);
    }

    /// <summary>
    /// The sidebar for a page: the start pages, then a heading for every group that has a page, with the group's pages
    /// under it. Only the current page's group is expanded.
    /// </summary>
    /// <param name="current">The page the sidebar is shown on.</param>
    /// <returns>The items for <see cref="DocumentPage.NavigationItems"/>.</returns>
    public IReadOnlyList<DocumentNavigationItem> SidebarFor(ContentItem? current)
    {
        var items = new List<DocumentNavigationItem>();
        foreach (var section in Sections)
        {
            if (section.Group.Kind == DocsGroupKind.Start)
            {
                items.AddRange(section.Members.Select(ToNavigationItem));
                continue;
            }

            if (section.Page is null && section.Members.Count == 0)
            {
                continue;
            }

            items.Add(new DocumentNavigationItem
            {
                Title = section.Group.Title,
                Url = section.HeadingUrl,
                Path = section.Page?.Path,
                IsExpanded = current is not null && (current == section.Page || section.Members.Contains(current)),
                Children = [.. section.Members.Select(ToNavigationItem)],
            });
        }

        return items;
    }

    private static DocumentNavigationItem ToNavigationItem(ContentItem item) => new()
    {
        Title = item.Title,
        Url = UrlOf(item),
        Path = item.Path,
        Order = item.OrderIndex,
    };

    private static string? GroupOf(ContentItem item) =>
        item.Metadata?.CustomFields is { } fields && fields.TryGetValue(GroupField, out var value)
            ? value?.ToString()
            : null;

    private static bool IsDocsUrl(string url) =>
        RootPages.Contains(url, StringComparer.Ordinal)
        || url.StartsWith("/docs/", StringComparison.Ordinal)
        || url.StartsWith(GalleryUrl + "/", StringComparison.Ordinal);
}
