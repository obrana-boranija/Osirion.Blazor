using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class DirectoryNavigationAccessibilityTests : CmsWebComponentTestBase
{
    private readonly IContentProvider _provider = Substitute.For<IContentProvider>();
    private readonly IContentProviderManager _manager = Substitute.For<IContentProviderManager>();

    public DirectoryNavigationAccessibilityTests()
    {
        Services.AddSingleton(_manager);
        _manager.GetDefaultProvider().Returns(_provider);
        _provider.GetDirectoriesAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<DirectoryItem>>([]));
    }

    // Fails before the fix: GetDirectoryNavClass never added the Class parameter, so the extra classes were dropped.
    [Fact]
    public void DirectoryNavigation_Class_IsAddedToTheNav()
    {
        var cut = RenderStatic<DirectoryNavigation>(p => p.Add(c => c.Class, "side"));

        cut.Find("nav").ClassList.ShouldBe(["osirion-directory-navigation", "side"], ignoreOrder: true);
    }

    // Fails before the fix: the nav had no accessible name.
    [Fact]
    public void DirectoryNavigation_Nav_HasAnAccessibleNameThatCanBeChanged()
    {
        RenderStatic<DirectoryNavigation>(_ => { }).Find("nav").GetAttribute("aria-label").ShouldBe("Directory navigation");

        var named = RenderStatic<DirectoryNavigation>(p => p.Add(c => c.AriaLabel, "Sections"));

        named.Find("nav").GetAttribute("aria-label").ShouldBe("Sections");
    }

    // Fails before the fix: the nav had no accessible name.
    [Fact]
    public void LocalizedNavigation_Nav_HasAnAccessibleNameThatCanBeChanged()
    {
        RenderStatic<LocalizedNavigation>(_ => { }).Find("nav").GetAttribute("aria-label").ShouldBe("Localized navigation");

        var named = RenderStatic<LocalizedNavigation>(p => p.Add(c => c.AriaLabel, "Docs sections"));

        named.Find("nav").GetAttribute("aria-label").ShouldBe("Docs sections");
    }
}
