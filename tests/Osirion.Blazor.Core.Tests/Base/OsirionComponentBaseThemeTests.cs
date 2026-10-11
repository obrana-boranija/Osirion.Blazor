using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Base;

public class OsirionComponentBaseThemeTests : OsirionComponentTestBase
{
    public OsirionComponentBaseThemeTests()
    {
        SetRendererInfo(new RendererInfo("Static", false));
    }

    [Fact]
    public void Theme_Dark_WritesThemeAttributeToMarkup()
    {
        var cut = Render<ThemeProbe>(parameters => parameters.Add(p => p.Theme, ThemeMode.Dark));

        cut.Find("div").GetAttribute("data-theme").ShouldBe("dark");
    }

    // Fails before the fix: the attribute was written once in OnInitialized, so a later change of Theme never
    // reached the markup.
    [Fact]
    public void Theme_ChangedAfterFirstRender_UpdatesThemeAttribute()
    {
        var cut = Render<ThemeProbe>(parameters => parameters.Add(p => p.Theme, ThemeMode.Dark));

        cut.Render(parameters => parameters.Add(p => p.Theme, ThemeMode.Light));
        cut.Find("div").GetAttribute("data-theme").ShouldBe("light");

        cut.Render(parameters => parameters.Add(p => p.Theme, ThemeMode.System));
        cut.Find("div").HasAttribute("data-theme").ShouldBeFalse();
    }

    // Fails before the fix: the parent's dictionary was written to directly.
    [Fact]
    public void Theme_AttributesSuppliedByTheCaller_AreCopiedNotModified()
    {
        IDictionary<string, object> supplied = new Dictionary<string, object> { ["data-x"] = "1" };

        var cut = Render<ThemeProbe>(parameters => parameters
            .Add(p => p.Attributes, supplied)
            .Add(p => p.Theme, ThemeMode.Dark));

        supplied.ShouldNotContainKey("data-theme");
        cut.Find("div").GetAttribute("data-x").ShouldBe("1");
        cut.Find("div").GetAttribute("data-theme").ShouldBe("dark");
    }

    [Fact]
    public void Theme_SystemWithCallerThemeAttribute_KeepsCallerValue()
    {
        var cut = Render<ThemeProbe>(parameters => parameters
            .AddUnmatched("data-theme", "custom"));

        cut.Find("div").GetAttribute("data-theme").ShouldBe("custom");
    }

    private sealed class ThemeProbe : OsirionComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddMultipleAttributes(1, Attributes);
            builder.CloseElement();
        }
    }
}
