using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class LocalizedNavigationTreeTests : CmsWebComponentTestBase
{
    private readonly IContentProvider _provider = Substitute.For<IContentProvider>();
    private readonly IContentProviderManager _manager = Substitute.For<IContentProviderManager>();

    public LocalizedNavigationTreeTests()
    {
        Services.AddSingleton(_manager);
        _manager.GetDefaultProvider().Returns(_provider);
    }

    // Fails before the fix: a locale's directory list is flat at every depth, so a nested directory was rendered at the top level and again under its parent.
    [Fact]
    public void LocalizedNavigation_FlatLocaleList_RendersNestedDirectoriesOnlyUnderTheirParent()
    {
        var (docs, guides) = ReturnLocaleTree();

        var cut = RenderStatic<LocalizedNavigation>(p => p
            .Add(c => c.CurrentLocale, "en")
            .Add(c => c.ExpandAllDirectories, true));

        cut.FindAll("ul.osirion-nav-list > li > a.osirion-nav-link").Select(a => a.TextContent.Trim()).ShouldBe(["Docs"]);
        cut.FindAll("ul.osirion-nav-sublist a.osirion-nav-link").Select(a => a.TextContent.Trim()).ShouldBe(["Guides"]);
        docs.Children.ShouldContain(guides);
    }

    // Fails before the fix: a directory's own _index.md was listed as a page under the directory's own title.
    [Fact]
    public void LocalizedNavigation_DirectoryWithIndexItem_LeavesTheIndexOutOfTheItemList()
    {
        var (docs, _) = ReturnLocaleTree();
        docs.AddItem(Page("docs/_index.md", "Docs", "/docs"));
        docs.AddItem(Page("docs/intro.md", "Intro", "/docs/intro"));

        var cut = RenderStatic<LocalizedNavigation>(p => p
            .Add(c => c.CurrentLocale, "en")
            .Add(c => c.ExpandAllDirectories, true));

        cut.FindAll("a.osirion-nav-content-link").Select(a => a.TextContent.Trim()).ShouldBe(["Intro"]);
    }

    // Fails before the fix: the default links were "/docs/intro.md" and "/en/docs", neither of which is a page of the site.
    [Fact]
    public void LocalizedNavigation_DefaultLinks_UseTheItemUrlAndTheDirectoryUrl()
    {
        var (docs, _) = ReturnLocaleTree();
        docs.AddItem(Page("docs/intro.md", "Intro", "/docs/intro"));

        var cut = RenderStatic<LocalizedNavigation>(p => p
            .Add(c => c.CurrentLocale, "en")
            .Add(c => c.ExpandAllDirectories, true));

        cut.Find("a.osirion-nav-content-link").GetAttribute("href").ShouldBe("/docs/intro");
        cut.FindAll("a.osirion-nav-link").Select(a => a.GetAttribute("href")).ShouldBe(["/docs", "/docs/guides"]);
    }

    // Fails before the fix: onclick:preventDefault was always set, so the attribute reached the static HTML and, interactively, no link navigated.
    [Fact]
    public void LocalizedNavigation_NoCallbacks_WritesNoHandlerAttributes()
    {
        var (docs, _) = ReturnLocaleTree();
        docs.AddItem(Page("docs/intro.md", "Intro", "/docs/intro"));

        var cut = RenderStatic<LocalizedNavigation>(p => p
            .Add(c => c.CurrentLocale, "en")
            .Add(c => c.ExpandAllDirectories, true));

        cut.Markup.ShouldNotContain("preventDefault");
        cut.Markup.ShouldNotContain("onclick");
    }

    [Fact]
    public void LocalizedNavigation_OnlyTheDirectoryCallback_PreventsDefaultOnDirectoryLinksOnly()
    {
        var (docs, _) = ReturnLocaleTree();
        docs.AddItem(Page("docs/intro.md", "Intro", "/docs/intro"));

        var cut = RenderStatic<LocalizedNavigation>(p => p
            .Add(c => c.CurrentLocale, "en")
            .Add(c => c.ExpandAllDirectories, true)
            .Add(c => c.OnDirectorySelected, (DirectoryItem _) => { }));

        cut.Find("a.osirion-nav-link").OuterHtml.ShouldContain("preventDefault");
        cut.Find("a.osirion-nav-content-link").OuterHtml.ShouldNotContain("preventDefault");
    }

    private (DirectoryItem Docs, DirectoryItem Guides) ReturnLocaleTree()
    {
        var locale = Directory("en", "en", "en");
        var docs = Directory("en/docs", "Docs", "en");
        var guides = Directory("en/docs/guides", "Guides", "en");
        locale.AddChild(docs);
        docs.AddChild(guides);
        _provider.GetDirectoriesAsync("en", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<DirectoryItem>>([docs, guides]));
        return (docs, guides);
    }

    private static DirectoryItem Directory(string path, string name, string locale)
    {
        var directory = DirectoryItem.Create($"id-{path}", path, name, "test");
        directory.SetLocale(locale);
        return directory;
    }

    private static ContentItem Page(string path, string title, string url)
    {
        var item = ContentItem.Create($"id-{path}", title, "<p>x</p>", path, "test");
        item.SetUrl(url);
        return item;
    }
}
