using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Components;
using Osirion.Blazor.Theming.Services;
using Shouldly;

namespace Osirion.Blazor.Theming.Tests.Components;

public class ThemePickerTests : BunitContext
{
    private readonly IThemeService _themeService = Substitute.For<IThemeService>();

    public ThemePickerTests()
    {
        _themeService.CurrentPalette.Returns(ThemePalette.Framework);
        Services.AddSingleton(_themeService);
        SetRendererInfo(new RendererInfo("Static", false));
        ComponentFactories.AddStub<BlazorJSComponents.JS>();
    }

    [Fact]
    public void ThemePicker_Default_OffersEveryPaletteByIdAndName()
    {
        // Act
        var cut = Render<ThemePicker>();

        // Assert
        var options = cut.FindAll("select option").Select(o => (o.GetAttribute("value"), o.TextContent)).ToList();
        options.ShouldBe(
        [
            ("framework", "Default"), ("osirion", "Osirion"), ("commit", "Commit"), ("editor", "Editor"),
            ("studio", "Studio"), ("studio-blue", "Studio Blue"), ("contrast", "Contrast"),
        ]);
    }

    // Passes before the fix as well: the picker already keeps its script in a file, and this holds it there.
    [Fact]
    public void ThemePicker_Markup_HasNoInlineHandlerAndNoInlineScript()
    {
        var cut = Render<ThemePicker>();

        cut.FindAll("script").ShouldBeEmpty();
        cut.FindAll("*").SelectMany(element => element.Attributes)
            .Where(attribute => attribute.Name.StartsWith("on", StringComparison.OrdinalIgnoreCase))
            .ShouldBeEmpty();
    }

    [Fact]
    public void ThemePicker_Markup_IsALabelledSelectTheScriptCanFind()
    {
        // Act
        var cut = Render<ThemePicker>(p => p.Add(c => c.Label, "Colours"));

        // Assert: the select sits inside its label, so it is named without an id
        var label = cut.Find("label.osirion-theme-picker");
        label.QuerySelector(".osirion-theme-picker-label")!.TextContent.ShouldBe("Colours");
        var select = label.QuerySelector("select")!;
        select.HasAttribute("data-osirion-palette-picker").ShouldBeTrue();
        select.GetAttribute("data-osirion-palettes-href").ShouldBe("_content/Osirion.Blazor.Theming/css/palettes.min.css");
        cut.FindAll("[id]").ShouldBeEmpty();
    }

    [Theory]
    [InlineData(ThemePalette.Framework, "framework")]
    [InlineData(ThemePalette.StudioBlue, "studio-blue")]
    public void ThemePicker_CurrentPalette_IsTheOnlySelectedOption(ThemePalette current, string id)
    {
        // Arrange
        _themeService.CurrentPalette.Returns(current);

        // Act
        var cut = Render<ThemePicker>();

        // Assert
        var selected = cut.FindAll("option[selected]");
        selected.Count.ShouldBe(1);
        selected[0].GetAttribute("value").ShouldBe(id);
    }

    [Fact]
    public void ThemePicker_PalettesGiven_OffersThemInThatOrderWithoutDuplicates()
    {
        // Act
        var cut = Render<ThemePicker>(p => p.Add(c => c.Palettes,
            [ThemePalette.Contrast, ThemePalette.Framework, ThemePalette.Contrast, (ThemePalette)42]));

        // Assert
        cut.FindAll("option").Select(o => o.GetAttribute("value")).ShouldBe(["contrast", "framework"]);
    }

    [Fact]
    public void ThemePicker_CurrentPaletteNotListed_IsOfferedFirstSoTheListShowsIt()
    {
        // Arrange
        _themeService.CurrentPalette.Returns(ThemePalette.Editor);

        // Act
        var cut = Render<ThemePicker>(p => p.Add(c => c.Palettes, [ThemePalette.Commit]));

        // Assert
        cut.FindAll("option").Select(o => o.GetAttribute("value")).ShouldBe(["editor", "commit"]);
        cut.Find("option[selected]").GetAttribute("value").ShouldBe("editor");
    }

    [Fact]
    public void ThemePicker_FrameworkLabel_NamesTheNoPaletteOption()
    {
        // Act
        var cut = Render<ThemePicker>(p => p.Add(c => c.FrameworkLabel, "Site colours"));

        // Assert
        cut.Find("option[value=framework]").TextContent.ShouldBe("Site colours");
    }

    [Fact]
    public void ThemePicker_ClassAndAttributes_ReachTheLabel()
    {
        // Act
        var cut = Render<ThemePicker>(p => p
            .Add(c => c.Class, "header-picker")
            .AddUnmatched("data-test", "picker"));

        // Assert
        var label = cut.Find("label");
        label.ClassList.ShouldContain("osirion-theme-picker");
        label.ClassList.ShouldContain("header-picker");
        label.GetAttribute("data-test").ShouldBe("picker");
    }

    [Fact]
    public void ThemePicker_Script_IsLoaded()
    {
        // Act
        var cut = Render<ThemePicker>();

        // Assert
        var stub = cut.FindComponent<Stub<BlazorJSComponents.JS>>();
        stub.Instance.Parameters.Get(x => x.Src).ShouldBe("./_content/Osirion.Blazor.Theming/js/theme-picker.js");
    }

    [Fact]
    public void ThemePicker_PaletteChangedOnTheServer_RendersTheNewSelection()
    {
        // Arrange
        var cut = Render<ThemePicker>();
        _themeService.CurrentPalette.Returns(ThemePalette.Commit);

        // Act
        _themeService.ThemeChanged += Raise.EventWith(_themeService, new ThemeChangedEventArgs(ThemeMode.Light, ThemeMode.Light));

        // Assert
        cut.WaitForAssertion(() => cut.Find("option[selected]").GetAttribute("value").ShouldBe("commit"));
    }
}
