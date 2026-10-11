using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Features.Navigation.Components;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Navigation;

public class DefaultNavigationTests : AdminComponentTestContext
{
    public DefaultNavigationTests()
    {
        SetRendererInfo(new RendererInfo("Static", false));
    }

    private static HashSet<string> RoutedPages() =>
        typeof(DefaultNavigation).Assembly.GetTypes()
            .SelectMany(type => type.GetCustomAttributes<RouteAttribute>())
            .Select(route => route.Template)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

    private IRenderedComponent<DefaultNavigation> RenderAt(string relativeUri)
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo(relativeUri);
        return Render<DefaultNavigation>();
    }

    private static List<string> InternalHrefs(IRenderedComponent<DefaultNavigation> cut) =>
        cut.FindAll("a[href]")
            .Select(link => link.GetAttribute("href")!)
            .Where(href => href.StartsWith('/'))
            .ToList();

    [Fact]
    public void DefaultNavigation_EveryInternalLink_PointsAtARoutedPage()
    {
        var routes = RoutedPages();

        var hrefs = InternalHrefs(Render<DefaultNavigation>());

        hrefs.ShouldNotBeEmpty();
        hrefs.ShouldAllBe(href => routes.Contains(href));
    }

    [Theory]
    [InlineData("/osirion", "/osirion")]
    [InlineData("/osirion/", "/osirion")]
    [InlineData("/osirion/dashboard", "/osirion")]
    [InlineData("/osirion/content", "/osirion/content")]
    [InlineData("/osirion/content/edit", "/osirion/content/edit")]
    [InlineData("/osirion/content/edit?Path=blog%2Fpost.md", "/osirion/content/edit")]
    [InlineData("/OSIRION/Settings", "/osirion/settings")]
    public void DefaultNavigation_OnARoutedPage_MarksExactlyItsLinkActive(string relativeUri, string expectedHref)
    {
        var cut = RenderAt(relativeUri);

        cut.FindAll("a.active").ShouldHaveSingleItem().GetAttribute("href").ShouldBe(expectedHref);
    }

    [Fact]
    public void DefaultNavigation_EveryLink_IsActiveOnItsOwnRoute()
    {
        var hrefs = InternalHrefs(Render<DefaultNavigation>());

        foreach (var href in hrefs)
        {
            var cut = RenderAt(href);

            cut.FindAll("a.active").ShouldHaveSingleItem().GetAttribute("href").ShouldBe(href);
        }
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/osirion/login")]
    [InlineData("/osirion-blog")]
    public void DefaultNavigation_OffTheLinkedRoutes_MarksNothingActive(string relativeUri)
    {
        var cut = RenderAt(relativeUri);

        cut.FindAll("a.active").ShouldBeEmpty();
    }
}
