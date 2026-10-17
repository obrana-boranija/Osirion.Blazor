using Bunit;
using NSubstitute;
using Osirion.Blazor.Cms.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

/// <summary>
/// The views and pages that render a not-found block answer 404 with it, as <c>ArticlePage</c> and
/// <c>DocumentPage</c> do, so a missing page is not a soft 404 for crawlers.
/// </summary>
public class ViewNotFoundStatusTests : CmsPageComponentTestBase
{
    // Fails before the fix: ContentPage rendered its not-found block and left the response at 200.
    [Fact]
    public void ContentPage_PathFindsNothing_SignalsNotFoundOnce()
    {
        var cut = RenderStatic<ContentPage>(p => p.Add(c => c.Path, "blog/missing"));
        cut.Render(p => p.Add(c => c.Path, "blog/missing"));

        Navigation.NotFoundCalls.ShouldBe(1);
        cut.FindAll(".osirion-content-not-found").ShouldHaveSingleItem();
    }

    // Fails before the fix: the not-found block without a path or an item answered 200 as well.
    [Fact]
    public void ContentPage_NoPathAndNoItem_SignalsNotFound()
    {
        RenderStatic<ContentPage>(_ => { });

        Navigation.NotFoundCalls.ShouldBe(1);
    }

    [Fact]
    public void ContentPage_ItemFound_DoesNotSignalNotFound()
    {
        Provider.GetItemByUrlAsync("blog/post").Returns(Doc("post"));

        RenderStatic<ContentPage>(p => p.Add(c => c.Path, "blog/post"));

        Navigation.NotFoundCalls.ShouldBe(0);
    }

    // Fails before the fix: ContentView rendered "Content Not Found" with status 200.
    [Fact]
    public void ContentView_PathFindsNothing_SignalsNotFoundOnce()
    {
        var cut = RenderStatic<ContentView>(p => p.Add(c => c.Path, "blog/missing.md"));
        cut.Render(p => p.Add(c => c.Path, "blog/missing.md"));

        Navigation.NotFoundCalls.ShouldBe(1);
        cut.FindAll(".osirion-content-not-found").ShouldHaveSingleItem();
    }

    // Fails before the fix: the same soft 404 without a path and without an item.
    [Fact]
    public void ContentView_NoPathAndNoItem_SignalsNotFound()
    {
        RenderStatic<ContentView>(_ => { });

        Navigation.NotFoundCalls.ShouldBe(1);
    }

    [Fact]
    public void ContentView_ItemSupplied_DoesNotSignalNotFound()
    {
        RenderStatic<ContentView>(p => p.Add(c => c.Item, Doc("post")));

        Navigation.NotFoundCalls.ShouldBe(0);
    }

    // Fails before the fix: LocalizedContentView rendered its not-found text with status 200.
    [Fact]
    public void LocalizedContentView_PathFindsNothing_SignalsNotFoundOnce()
    {
        var cut = RenderStatic<LocalizedContentView>(p => p.Add(c => c.Path, "blog/missing.md"));
        cut.Render(p => p.Add(c => c.Path, "blog/missing.md"));

        Navigation.NotFoundCalls.ShouldBe(1);
        cut.FindAll(".osirion-not-found").ShouldHaveSingleItem();
    }

    // Fails before the fix: the same soft 404 without a path and without an item.
    [Fact]
    public void LocalizedContentView_NoPathAndNoItem_SignalsNotFound()
    {
        RenderStatic<LocalizedContentView>(_ => { });

        Navigation.NotFoundCalls.ShouldBe(1);
    }

    [Fact]
    public void LocalizedContentView_ItemSupplied_DoesNotSignalNotFound()
    {
        RenderStatic<LocalizedContentView>(p => p.Add(c => c.Item, Doc("post")));

        Navigation.NotFoundCalls.ShouldBe(0);
    }

    // Fails before the fix: LandingPage rendered "Page content not found." with status 200.
    [Fact]
    public void LandingPage_ContentPathFindsNothing_SignalsNotFoundOnce()
    {
        var cut = RenderStatic<LandingPage>(p => p.Add(c => c.ContentPath, "pages/missing.md"));
        cut.Render(p => p.Add(c => c.ContentPath, "pages/missing.md"));

        Navigation.NotFoundCalls.ShouldBe(1);
        cut.FindAll(".osirion-not-found").ShouldHaveSingleItem();
    }

    [Fact]
    public void LandingPage_ContentPathFindsTheItem_DoesNotSignalNotFound()
    {
        Provider.GetItemByPathAsync("pages/launch.md").Returns(Doc("launch"));

        RenderStatic<LandingPage>(p => p.Add(c => c.ContentPath, "pages/launch.md"));

        Navigation.NotFoundCalls.ShouldBe(0);
    }

    // A landing page built from parameters alone is a page, not a missing one.
    [Fact]
    public void LandingPage_NoContentPath_DoesNotSignalNotFound()
    {
        RenderStatic<LandingPage>(p => p.Add(c => c.HeroTitle, "Hello"));

        Navigation.NotFoundCalls.ShouldBe(0);
    }
}
