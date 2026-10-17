using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Examples.Shared.Docs;
using Shouldly;

namespace Osirion.Blazor.Examples.Tests.Docs;

public class DocsOutlineTests
{
    [Fact]
    public void Build_PagesWithAGroup_AreListedInOrderThenTitle()
    {
        // Arrange
        var items = new[]
        {
            Page("components/tabs", "Tabs", "content", order: 2),
            Page("components/callout", "Callouts", "content", order: 1),
            Page("components/accordion", "Accordion", "content", order: 2),
        };

        // Act
        var outline = DocsOutline.Build(items);

        // Assert
        Section(outline, "content").Members.Select(item => item.Title).ShouldBe(["Callouts", "Accordion", "Tabs"]);
    }

    [Fact]
    public void Build_GroupPage_IsTheHeadingNotAMember()
    {
        // Arrange
        var groupPage = Page("components/cards", "Cards", "cards");
        var card = Page("components/feature-card", "OsirionFeatureCard", "cards");

        // Act
        var section = Section(DocsOutline.Build([groupPage, card]), "cards");

        // Assert
        section.Page.ShouldBe(groupPage);
        section.Members.ShouldBe([card]);
        section.HeadingUrl.ShouldBe("/components/cards");
    }

    [Fact]
    public void HeadingUrl_GroupWithoutAPage_IsItsGallerySection()
    {
        // Act
        var section = Section(DocsOutline.Build([Page("components/seo-metadata-renderer", "SeoMetadataRenderer", "seo-and-feeds")]), "seo-and-feeds");

        // Assert
        section.HeadingUrl.ShouldBe("/components#seo-and-feeds");
    }

    [Fact]
    public void Build_PageWithoutAKnownGroup_IsUnplaced()
    {
        // Arrange
        var noGroup = Page("components/orphan", "Orphan", group: null);
        var unknownGroup = Page("docs/typo", "Typo", "guidez");

        // Act
        var outline = DocsOutline.Build([noGroup, unknownGroup, Page("docs/getting-started", "Getting started", "getting-started")]);

        // Assert
        outline.Unplaced.ShouldBe([noGroup, unknownGroup], ignoreOrder: true);
    }

    [Fact]
    public void Build_PagesOutsideTheDocumentationOrUnpublished_AreLeftOut()
    {
        // Arrange
        var post = Page("blog/news", "News", "content");
        var draft = Page("components/draft", "Draft", "content");
        draft.IsPublished = false;
        var lookalike = Page("docsearch/page", "Lookalike", "content");

        // Act
        var outline = DocsOutline.Build([post, draft, lookalike]);

        // Assert
        outline.Pages.ShouldBeEmpty();
        outline.Unplaced.ShouldBeEmpty();
    }

    [Fact]
    public void Find_MatchesTheExactAddress()
    {
        // Arrange
        var tabs = Page("components/tabs", "Tabs", "content");
        var outline = DocsOutline.Build([tabs]);

        // Act and assert
        outline.Find("/components/tabs").ShouldBe(tabs);
        outline.Find("components/tabs/").ShouldBe(tabs);
        outline.Find("/components/Tabs").ShouldBeNull();
        outline.Find("/components/tab").ShouldBeNull();
    }

    [Fact]
    public void NeighboursOf_FollowsTheSidebarAcrossGroups()
    {
        // Arrange
        var start = Page("docs/getting-started", "Getting started", "getting-started");
        var layoutPage = Page("components/layout", "Layout", "layout");
        var footer = Page("components/footer", "OsirionFooter", "layout");
        var breadcrumbs = Page("components/breadcrumbs", "OsirionBreadcrumbs", "navigation");
        var outline = DocsOutline.Build([breadcrumbs, footer, layoutPage, start]);

        // Act and assert
        outline.NeighboursOf(start).ShouldBe((null, layoutPage));
        outline.NeighboursOf(layoutPage).ShouldBe((start, footer));
        outline.NeighboursOf(footer).ShouldBe((layoutPage, breadcrumbs));
        outline.NeighboursOf(breadcrumbs).ShouldBe((footer, null));
    }

    [Fact]
    public void NeighboursOf_UnplacedPage_HasNone()
    {
        // Arrange
        var orphan = Page("components/orphan", "Orphan", group: null);
        var outline = DocsOutline.Build([orphan, Page("components/tabs", "Tabs", "content")]);

        // Act and assert
        outline.NeighboursOf(orphan).ShouldBe((null, null));
    }

    [Fact]
    public void SidebarFor_ListsStartPagesFlatAndExpandsOnlyTheCurrentGroup()
    {
        // Arrange
        var docs = Page("docs", "Documentation", "getting-started", order: 1);
        var start = Page("docs/getting-started", "Getting started", "getting-started", order: 2);
        var layoutPage = Page("components/layout", "Layout", "layout");
        var footer = Page("components/footer", "OsirionFooter", "layout");
        var tabs = Page("components/tabs", "Tabs", "content");
        var outline = DocsOutline.Build([docs, start, layoutPage, footer, tabs]);

        // Act
        var sidebar = outline.SidebarFor(footer);

        // Assert: the start pages, then one heading per group that has pages
        sidebar.Select(item => item.Title).ShouldBe(["Documentation", "Getting started", "Layout", "Content"]);
        sidebar[0].Children.ShouldBeNull();
        var layout = sidebar[2];
        layout.Url.ShouldBe("/components/layout");
        layout.Path.ShouldBe(layoutPage.Path);
        layout.IsExpanded.ShouldBeTrue();
        layout.Children!.Select(child => child.Url).ShouldBe(["/components/footer"]);
        layout.Children![0].Path.ShouldBe(footer.Path);
        sidebar[3].IsExpanded.ShouldBeFalse();
        sidebar[3].Url.ShouldBe("/components#content");
    }

    [Fact]
    public void SidebarFor_GroupPage_ExpandsItsGroup()
    {
        // Arrange
        var layoutPage = Page("components/layout", "Layout", "layout");
        var outline = DocsOutline.Build([layoutPage, Page("components/footer", "OsirionFooter", "layout")]);

        // Act and assert
        outline.SidebarFor(layoutPage).Single().IsExpanded.ShouldBeTrue();
    }

    [Fact]
    public void Groups_HaveUniqueKeys()
    {
        DocsOutline.Groups.Select(group => group.Key).ShouldBeUnique();
    }

    private static DocsSection Section(DocsOutline outline, string key) =>
        outline.Sections.Single(section => section.Group.Key == key);

    private static ContentItem Page(string url, string title, string? group, int order = 0)
    {
        var item = ContentItem.Create("id-" + url, title, "<p>Body</p>", "en/" + url + ".md", "test");
        item.SetUrl(url);
        item.SetOrderIndex(order);
        var frontMatter = FrontMatter.Create(title);
        item.Metadata = group is null
            ? frontMatter
            : frontMatter.WithCustomFields(new Dictionary<string, object> { ["group"] = group });
        return item;
    }
}
