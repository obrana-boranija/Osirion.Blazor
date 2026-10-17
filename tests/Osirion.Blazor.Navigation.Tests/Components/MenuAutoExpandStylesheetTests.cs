using System.Text.RegularExpressions;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Components;

public partial class MenuAutoExpandStylesheetTests
{
    // Fails before the fix: the class osirion-menu-auto-expand was written but no rule used it, so the submenu of
    // the active item in a vertical menu stayed closed until the reader opened it.
    [Fact]
    public void VerticalMenu_AutoExpand_OpensTheSubmenuOfTheActiveItem()
    {
        var css = Squash(File.ReadAllText(MenuBrandLogoTests.StylesheetPath("MenuItem.razor.css")));

        css.ShouldContain(".osirion-menu-vertical.osirion-menu-auto-expand.osirion-menu-item-wrapper-active.osirion-menu-item-has-submenu>.osirion-submenu{display:block;}");
        css.ShouldContain(".osirion-menu-vertical.osirion-menu-auto-expand.osirion-menu-item-wrapper-active.osirion-menu-item-has-submenu>.osirion-submenu-mega{display:flex;");
    }

    private static string Squash(string css) => WhitespaceRun().Replace(CssComment().Replace(css, string.Empty), string.Empty);

    [GeneratedRegex(@"/\*.*?\*/", RegexOptions.Singleline)]
    private static partial Regex CssComment();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRun();
}
