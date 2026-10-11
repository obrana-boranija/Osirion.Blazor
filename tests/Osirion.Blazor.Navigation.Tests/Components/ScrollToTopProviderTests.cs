using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components;
using Osirion.Blazor.Navigation.Components;
using Osirion.Blazor.Navigation.Services;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class ScrollToTopProviderTests : BunitContext
{
    private readonly ScrollToTopManager _manager = new();

    public ScrollToTopProviderTests()
    {
        Services.AddSingleton(_manager);
        SetRendererInfo(new RendererInfo("Static", false));
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void ScrollToTopProvider_ManagerDisabled_RendersNothing()
    {
        var cut = Render<ScrollToTopProvider>();

        cut.FindAll("button").ShouldBeEmpty();
        cut.FindComponents<ScrollToTop>().ShouldBeEmpty();
    }

    [Fact]
    public void ScrollToTopProvider_ManagerEnabled_RendersButtonFromManagerSettings()
    {
        _manager.IsEnabled = true;
        _manager.Position = Position.TopLeft;
        _manager.Text = "Top";
        _manager.Title = "Back up";
        _manager.CssClass = "site-scroll";
        _manager.IconCssClass = "icon-up";

        var cut = Render<ScrollToTopProvider>();

        var button = cut.Find("button.osirion-scroll-to-top");
        button.ClassList.ShouldContain("top-left");
        button.ClassList.ShouldContain("site-scroll");
        button.GetAttribute("aria-label").ShouldBe("Back up");
        button.QuerySelector("span.icon-up").ShouldNotBeNull();
        button.QuerySelector("span.scroll-text")!.TextContent.ShouldBe("Top");
    }

    [Fact]
    public void ScrollToTopProvider_ManagerEnabledLater_RendersTheButton()
    {
        var cut = Render<ScrollToTopProvider>();

        _manager.IsEnabled = true;

        cut.WaitForAssertion(() => cut.FindAll("button.osirion-scroll-to-top").Count.ShouldBe(1));
    }

    [Fact]
    public void ScrollToTopProvider_ManagerSettingChanged_RendersTheNewSetting()
    {
        _manager.IsEnabled = true;
        var cut = Render<ScrollToTopProvider>();

        _manager.Position = Position.BottomLeft;

        cut.WaitForAssertion(() => cut.Find("button.osirion-scroll-to-top").ClassList.ShouldContain("bottom-left"));
    }

    [Fact]
    public void ScrollToTopProvider_Disposed_StopsFollowingTheManager()
    {
        var cut = Render<ScrollToTopProvider>();
        var renders = cut.RenderCount;

        cut.Instance.Dispose();
        _manager.IsEnabled = true;

        cut.RenderCount.ShouldBe(renders);
        cut.FindAll("button").ShouldBeEmpty();
    }

    [Theory]
    [InlineData(Position.TopRight, "top-right")]
    [InlineData(Position.BottomLeft, "bottom-left")]
    [InlineData(Position.BottomRight, "bottom-right")]
    [InlineData((Position)99, "bottom-right")]
    public void ScrollToTop_Position_MapsToPositionClass(Position position, string expectedClass)
    {
        var cut = Render<ScrollToTop>(p => p.Add(c => c.Position, position));

        cut.Find("button.osirion-scroll-to-top").ClassList.ShouldContain(expectedClass);
    }
}
