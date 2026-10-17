using Bunit;
using NSubstitute;
using Osirion.Blazor.Cms.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class DocumentPageReloadTests : CmsPageComponentTestBase
{
    // Fails before the fix: the page loaded only while Content was null, so an interactive change of ContentPath kept
    // showing the first document.
    [Fact]
    public void DocumentPage_ContentPathChanges_LoadsTheNewDocument()
    {
        Provider.GetItemByPathAsync("docs/one.md").Returns(Doc("one"));
        Provider.GetItemByPathAsync("docs/two.md").Returns(Doc("two"));

        var cut = RenderStatic<DocumentPage>(p => p.Add(c => c.ContentPath, "docs/one.md"));
        cut.Find(".osirion-document-title").TextContent.ShouldBe("one");

        cut.Render(p => p.Add(c => c.ContentPath, "docs/two.md"));

        cut.Find(".osirion-document-title").TextContent.ShouldBe("two");
    }

    // Fails before the fix: the new path was never looked up, so a missing document kept the old one on screen and
    // never reached the not-found state.
    [Fact]
    public void DocumentPage_ContentPathChangesToAMissingDocument_ShowsNotFound()
    {
        Provider.GetItemByPathAsync("docs/one.md").Returns(Doc("one"));

        var cut = RenderStatic<DocumentPage>(p => p.Add(c => c.ContentPath, "docs/one.md"));

        cut.Render(p => p.Add(c => c.ContentPath, "docs/gone.md"));

        cut.FindAll("article").ShouldBeEmpty();
        cut.FindAll(".osirion-content-not-found").ShouldHaveSingleItem();
    }

    // Fails before the fix: the previous and next documents of the first path stayed under the second document.
    [Fact]
    public void DocumentPage_ContentPathChanges_ReplacesTheNeighborsItLoaded()
    {
        var intro = Doc("intro");
        var install = Doc("install");
        var usage = Doc("usage");
        var advanced = Doc("advanced");
        Provider.GetItemByPathAsync("docs/install.md").Returns(install);
        Provider.GetItemByPathAsync("docs/advanced.md").Returns(advanced);
        Provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>())
            .Returns(Task.FromResult<IReadOnlyList<ContentItem>?>([intro, install, usage, advanced]));

        var cut = RenderStatic<DocumentPage>(p => p.Add(c => c.ContentPath, "docs/install.md"));
        cut.Find("a.osirion-doc-nav-next .osirion-doc-nav-title").TextContent.ShouldBe("usage");

        cut.Render(p => p.Add(c => c.ContentPath, "docs/advanced.md"));

        cut.Find("a.osirion-doc-nav-previous .osirion-doc-nav-title").TextContent.ShouldBe("usage");
        cut.FindAll("a.osirion-doc-nav-next").ShouldBeEmpty();
    }

    // Fails before the fix: the page marked the first document as current and kept it so, because it filled CurrentPath
    // only while it was blank.
    [Fact]
    public void DocumentPage_ContentPathChanges_MovesTheCurrentMarkerOfTheSideNavigation()
    {
        var one = Doc("one");
        var two = Doc("two");
        Provider.GetItemByPathAsync("docs/one.md").Returns(one);
        Provider.GetItemByPathAsync("docs/two.md").Returns(two);
        Provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>())
            .Returns(Task.FromResult<IReadOnlyList<ContentItem>?>([one, two]));

        var cut = RenderStatic<DocumentPage>(p => p.Add(c => c.ContentPath, "docs/one.md"));
        cut.Render(p => p.Add(c => c.ContentPath, "docs/two.md"));

        cut.Find("a[aria-current=page]").TextContent.Trim().ShouldBe("two");
    }

    // Fails before the fix: a supplied PreviousItem was replaced by the neighbor the page looked up.
    [Fact]
    public void DocumentPage_SuppliedPreviousItem_IsNotOverwritten()
    {
        var chosen = Doc("chosen");
        var intro = Doc("intro");
        var install = Doc("install");
        var usage = Doc("usage");
        Provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>())
            .Returns(Task.FromResult<IReadOnlyList<ContentItem>?>([intro, install, usage]));

        var cut = RenderStatic<DocumentPage>(p => p
            .Add(c => c.Content, install)
            .Add(c => c.PreviousItem, chosen));

        cut.Find("a.osirion-doc-nav-previous .osirion-doc-nav-title").TextContent.ShouldBe("chosen");
        cut.Find("a.osirion-doc-nav-next .osirion-doc-nav-title").TextContent.ShouldBe("usage");
    }

    // Fails before the fix: a supplied NextItem was replaced by the neighbor the page looked up.
    [Fact]
    public void DocumentPage_SuppliedNextItem_IsNotOverwritten()
    {
        var chosen = Doc("chosen");
        var intro = Doc("intro");
        var install = Doc("install");
        var usage = Doc("usage");
        Provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>())
            .Returns(Task.FromResult<IReadOnlyList<ContentItem>?>([intro, install, usage]));

        var cut = RenderStatic<DocumentPage>(p => p
            .Add(c => c.Content, install)
            .Add(c => c.NextItem, chosen));

        cut.Find("a.osirion-doc-nav-next .osirion-doc-nav-title").TextContent.ShouldBe("chosen");
        cut.Find("a.osirion-doc-nav-previous .osirion-doc-nav-title").TextContent.ShouldBe("intro");
    }

    // Fails before the fix: the page queried the folder again on every parameter update.
    [Fact]
    public void DocumentPage_SameContentRendersAgain_DoesNotQueryTheFolderAgain()
    {
        var install = Doc("install");
        Provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>())
            .Returns(Task.FromResult<IReadOnlyList<ContentItem>?>([install]));

        var cut = RenderStatic<DocumentPage>(p => p.Add(c => c.Content, install));
        cut.Render(p => p.Add(c => c.Content, install).Add(c => c.Class, "again"));

        Provider.Received(1).GetItemsByQueryAsync(Arg.Any<ContentQuery>());
    }

    // The page's own item lookup (ItemPath, ItemSlug, Locale and the rest of the base class) has no meaning here:
    // the document is Content. Guards the hook that keeps calling the base from running an unfiltered query.
    [Fact]
    public void DocumentPage_WithContent_RunsNoItemLookup()
    {
        RenderStatic<DocumentPage>(p => p.Add(c => c.Content, Doc("install")));

        Manager.DidNotReceive().GetContentByQueryAsync(Arg.Any<ContentQuery>(), Arg.Any<CancellationToken>());
    }
}
