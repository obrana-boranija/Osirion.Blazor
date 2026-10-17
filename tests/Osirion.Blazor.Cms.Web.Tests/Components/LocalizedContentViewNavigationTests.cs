using Bunit;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class LocalizedContentViewNavigationTests : CmsPageComponentTestBase
{
    // Fails before the fix: the neighbor query used the directory's display name ("Documentation"), which no provider
    // matches by path, so a folder whose title differs from its path showed no previous and next links.
    [Fact]
    public void LocalizedContentView_Neighbors_AreLookedUpByTheDirectoryPath()
    {
        var directory = DirectoryItem.Create("d", "docs", "Documentation", "test");
        var intro = Doc("intro");
        var install = Doc("install");
        var usage = Doc("usage");
        directory.AddItem(intro);
        directory.AddItem(install);
        directory.AddItem(usage);
        Provider.GetItemsByQueryAsync(Arg.Is<ContentQuery>(q => q.Directory == "docs"))
            .Returns(Task.FromResult<IReadOnlyList<ContentItem>?>([usage, install, intro]));

        var cut = RenderStatic<LocalizedContentView>(p => p
            .Add(c => c.Item, install)
            .Add(c => c.ShowNavigationLinks, true));

        cut.Find("a.osirion-content-previous .osirion-content-navigation-title").TextContent.ShouldBe("usage");
        cut.Find("a.osirion-content-next .osirion-content-navigation-title").TextContent.ShouldBe("intro");
    }

    // Fails before the fix: the neighbor query replaced the PreviousItem and NextItem the parent supplied.
    [Fact]
    public void LocalizedContentView_SuppliedNeighbors_AreNotOverwritten()
    {
        var directory = DirectoryItem.Create("d", "docs", "Documentation", "test");
        var intro = Doc("intro");
        var install = Doc("install");
        var usage = Doc("usage");
        directory.AddItem(intro);
        directory.AddItem(install);
        directory.AddItem(usage);
        Provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>())
            .Returns(Task.FromResult<IReadOnlyList<ContentItem>?>([usage, install, intro]));

        var cut = RenderStatic<LocalizedContentView>(p => p
            .Add(c => c.Item, install)
            .Add(c => c.PreviousItem, Doc("chosen-previous"))
            .Add(c => c.NextItem, Doc("chosen-next"))
            .Add(c => c.ShowNavigationLinks, true));

        cut.Find("a.osirion-content-previous .osirion-content-navigation-title").TextContent.ShouldBe("chosen-previous");
        cut.Find("a.osirion-content-next .osirion-content-navigation-title").TextContent.ShouldBe("chosen-next");
        Provider.DidNotReceive().GetItemsByQueryAsync(Arg.Any<ContentQuery>());
    }

    // Fails before the fix: a supplied PreviousItem was replaced as soon as the query found the item.
    [Fact]
    public void LocalizedContentView_OneSuppliedNeighbor_IsKeptAndNothingIsLookedUp()
    {
        var directory = DirectoryItem.Create("d", "docs", "Documentation", "test");
        var intro = Doc("intro");
        var install = Doc("install");
        var usage = Doc("usage");
        directory.AddItem(intro);
        directory.AddItem(install);
        directory.AddItem(usage);
        Provider.GetItemsByQueryAsync(Arg.Any<ContentQuery>())
            .Returns(Task.FromResult<IReadOnlyList<ContentItem>?>([usage, install, intro]));

        var cut = RenderStatic<LocalizedContentView>(p => p
            .Add(c => c.Item, install)
            .Add(c => c.PreviousItem, Doc("chosen-previous"))
            .Add(c => c.ShowNavigationLinks, true));

        cut.Find("a.osirion-content-previous .osirion-content-navigation-title").TextContent.ShouldBe("chosen-previous");
        cut.FindAll("a.osirion-content-next").ShouldBeEmpty();
        Provider.DidNotReceive().GetItemsByQueryAsync(Arg.Any<ContentQuery>());
    }

    // Fails before the fix: inside ContentPage the view ran a second neighbor query, by directory name, and replaced
    // the previous and next items ContentPage had computed.
    [Fact]
    public void ContentPage_Neighbors_AreNotReplacedByTheInnerView()
    {
        var directory = DirectoryItem.Create("d", "docs", "Documentation", "test");
        var intro = Doc("intro");
        var install = Doc("install");
        var usage = Doc("usage");
        directory.AddItem(intro);
        directory.AddItem(install);
        directory.AddItem(usage);
        Provider.GetItemByUrlAsync("docs/install").Returns(install);
        Provider.GetItemsByQueryAsync(Arg.Is<ContentQuery>(q => q.Directory == "docs"))
            .Returns(Task.FromResult<IReadOnlyList<ContentItem>?>([usage, install, intro]));
        Provider.GetItemsByQueryAsync(Arg.Is<ContentQuery>(q => q.Directory == "Documentation"))
            .Returns(Task.FromResult<IReadOnlyList<ContentItem>?>([Doc("wrong-previous"), install, Doc("wrong-next")]));

        var cut = RenderStatic<ContentPage>(p => p.Add(c => c.Path, "docs/install"));

        cut.Find("a.osirion-content-previous .osirion-content-navigation-title").TextContent.ShouldBe("usage");
        cut.Find("a.osirion-content-next .osirion-content-navigation-title").TextContent.ShouldBe("intro");
    }

    // Fails before the fix: the built-in providers store the same Url for every translation, so each translation link
    // pointed at the address the visitor was already on.
    [Fact]
    public void LocalizedContentView_TranslationsWithTheSameUrl_LinkToTheirOwnLocale()
    {
        var english = Translation("post-en", "en", "blog/post");
        Provider.GetContentTranslationsAsync("post-translations").Returns(new Dictionary<string, ContentItem>
        {
            ["en"] = english,
            ["sr"] = Translation("post-sr", "sr", "blog/post"),
            ["de"] = Translation("post-de", "de", "/blog/post"),
        });

        var cut = RenderStatic<LocalizedContentView>(p => p.Add(c => c.Item, english));

        cut.FindAll(".osirion-translations a").Select(a => a.GetAttribute("href")).ShouldBe(["/sr/blog/post", "/de/blog/post"], ignoreOrder: true);
    }

    // The stored URL already tells the translation apart, so it is the translation's own address.
    [Fact]
    public void LocalizedContentView_TranslationWithADistinctUrl_LinksToThatUrl()
    {
        var english = Translation("post-en", "en", "about");
        Provider.GetContentTranslationsAsync("post-translations").Returns(new Dictionary<string, ContentItem>
        {
            ["en"] = english,
            ["sr"] = Translation("post-sr", "sr", "o-nama"),
        });

        var cut = RenderStatic<LocalizedContentView>(p => p.Add(c => c.Item, english));

        cut.Find(".osirion-translations a").GetAttribute("href").ShouldBe("/o-nama");
    }

    private static ContentItem Translation(string id, string locale, string url)
    {
        var item = ContentItem.Create(id, "Post", "<p>Post</p>", $"{locale}/blog/post.md", "test");
        item.SetLocale(locale);
        item.SetContentId("post-translations");
        item.SetUrl(url);
        return item;
    }
}
