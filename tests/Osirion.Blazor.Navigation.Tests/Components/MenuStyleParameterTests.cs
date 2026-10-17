using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Components;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class MenuStyleParameterTests : BunitContext
{
    private const string Written = "color: red;";

    public MenuStyleParameterTests()
    {
        SetRendererInfo(new RendererInfo("Server", false));
    }

    // Fails before the fix: Style is declared on the base component, but none of these components wrote it, so a
    // style="color: red;" in the markup bound to the parameter and vanished from the output.
    [Fact]
    public void MenuItem_Style_IsWrittenOnTheRootElement() =>
        Render<MenuItem>(p => p.Add(c => c.Text, "A").Add(c => c.Style, Written))
            .Find(".osirion-menu-item-wrapper").GetAttribute("style").ShouldBe(Written);

    // Fails before the fix: see MenuItem_Style_IsWrittenOnTheRootElement.
    [Fact]
    public void MenuGroup_Style_IsWrittenOnTheRootElement() =>
        Render<MenuGroup>(p => p.Add(c => c.Style, Written))
            .Find(".osirion-menu-group").GetAttribute("style").ShouldBe(Written);

    // Fails before the fix: see MenuItem_Style_IsWrittenOnTheRootElement.
    [Fact]
    public void MenuDivider_Style_IsWrittenOnTheRootElement() =>
        Render<MenuDivider>(p => p.Add(c => c.Style, Written))
            .Find(".osirion-menu-divider").GetAttribute("style").ShouldBe(Written);

    // Fails before the fix: see MenuItem_Style_IsWrittenOnTheRootElement.
    [Fact]
    public void MenuCallToAction_Style_IsWrittenOnTheRootElement() =>
        Render<MenuCallToAction>(p => p.Add(c => c.Style, Written))
            .Find(".osirion-menu-call-to-action").GetAttribute("style").ShouldBe(Written);

    // Fails before the fix: see MenuItem_Style_IsWrittenOnTheRootElement.
    [Fact]
    public void MenuImageCard_Style_IsWrittenOnTheRootElement() =>
        Render<MenuImageCard>(p => p.Add(c => c.Style, Written))
            .Find(".osirion-menu-image-card").GetAttribute("style").ShouldBe(Written);

    // Fails before the fix: see MenuItem_Style_IsWrittenOnTheRootElement.
    [Fact]
    public void MenuPanel_Style_IsWrittenOnTheRootElement() =>
        Render<MenuPanel>(p => p.Add(c => c.Style, Written))
            .Find(".osirion-menu-panel").GetAttribute("style").ShouldBe(Written);

    // Fails before the fix: see MenuItem_Style_IsWrittenOnTheRootElement.
    [Fact]
    public void MenuPanelLink_Style_IsWrittenOnTheRootElement() =>
        Render<MenuPanelLink>(p => p.Add(c => c.Style, Written))
            .Find(".osirion-menu-panel-link").GetAttribute("style").ShouldBe(Written);

    // Fails before the fix: see MenuItem_Style_IsWrittenOnTheRootElement.
    [Fact]
    public void Menu_Style_IsWrittenBeforeTheStickyZIndex() =>
        Render<Menu>(p => p.Add(c => c.Style, "color: red").Add(c => c.StickyZIndex, 7))
            .Find(".osirion-menu-container").GetAttribute("style").ShouldBe("color: red; z-index: 7;");

    [Fact]
    public void Menu_StyleEndingInASemicolon_GetsNoSecondOne() =>
        Render<Menu>(p => p.Add(c => c.Style, "color: red; ").Add(c => c.StickyZIndex, 7))
            .Find(".osirion-menu-container").GetAttribute("style").ShouldBe("color: red; z-index: 7;");

    [Fact]
    public void Menu_StyleWithoutSticky_IsTheOnlyStyle() =>
        Render<Menu>(p => p.Add(c => c.Style, Written).Add(c => c.Sticky, false))
            .Find(".osirion-menu-container").GetAttribute("style").ShouldBe(Written);
}
