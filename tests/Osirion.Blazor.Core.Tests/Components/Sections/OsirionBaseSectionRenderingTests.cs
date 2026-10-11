using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Sections;

public class OsirionBaseSectionRenderingTests : OsirionComponentTestBase
{
    [Fact]
    public void BaseSection_Defaults_RendersIdClassesAndNoStyleOrHeader()
    {
        var cut = RenderStatic<OsirionBaseSection>(parameters => parameters
            .Add(p => p.Id, "features"));

        var section = cut.Find("section");
        section.Id.ShouldBe("features");
        section.ClassList.ShouldBe(["osirion-section", "osirion-section-align-center", "osirion-section-padding-medium"]);
        section.GetAttribute("style").ShouldBeNullOrEmpty();
        cut.FindAll(".osirion-section-header").ShouldBeEmpty();
        cut.FindAll(".osirion-section-content").ShouldBeEmpty();
        cut.Find("section > div").ClassList.ShouldBe(["osirion-container"]);
    }

    [Fact]
    public void BaseSection_TitleAndDescription_RendersHeaderWithAlignment()
    {
        var cut = RenderStatic<OsirionBaseSection>(parameters => parameters
            .Add(p => p.Title, "Features")
            .Add(p => p.Description, "Everything you need")
            .Add(p => p.TextAlignment, Alignment.Left));

        cut.Find(".osirion-section-header").ClassList.ShouldBe(["osirion-section-header", "osirion-section-header-left"]);
        cut.Find("h2.osirion-section-title").TextContent.ShouldBe("Features");
        cut.Find("p.osirion-section-description").TextContent.ShouldBe("Everything you need");
    }

    [Fact]
    public void BaseSection_TitleContentAndDescriptionContent_ReplaceTheTextParameters()
    {
        var cut = RenderStatic<OsirionBaseSection>(parameters => parameters
            .Add(p => p.Title, "Ignored title")
            .Add(p => p.Description, "Ignored description")
            .Add(p => p.TitleContent, Markup("<h3 class=\"custom-title\">Custom</h3>"))
            .Add(p => p.DescriptionContent, Markup("<p class=\"custom-description\">Details</p>")));

        cut.Find(".custom-title").TextContent.ShouldBe("Custom");
        cut.Find(".custom-description").TextContent.ShouldBe("Details");
        cut.FindAll(".osirion-section-title").ShouldBeEmpty();
        cut.FindAll(".osirion-section-description").ShouldBeEmpty();
    }

    [Fact]
    public void BaseSection_TitleWithoutDescription_RendersNoDescription()
    {
        var cut = RenderStatic<OsirionBaseSection>(parameters => parameters
            .Add(p => p.Title, "Features"));

        cut.FindAll(".osirion-section-description").ShouldBeEmpty();
    }

    [Fact]
    public void BaseSection_ChildContent_RendersContentWrapper()
    {
        var cut = RenderStatic<OsirionBaseSection>(parameters => parameters
            .AddChildContent("<p class=\"body\">Body</p>"));

        cut.Find(".osirion-section-content .body").TextContent.ShouldBe("Body");
    }

    [Fact]
    public void BaseSection_BackgroundImageWithOverlay_RendersOverlayAndBackgroundClass()
    {
        var cut = RenderStatic<OsirionBaseSection>(parameters => parameters
            .Add(p => p.BackgroundImageUrl, "/img/bg.webp"));

        cut.Find("section").ClassList.ShouldContain("osirion-section-with-background");
        cut.FindAll(".osirion-section-overlay").Count.ShouldBe(1);
    }

    [Fact]
    public void BaseSection_OverlayDisabled_RendersNoOverlay()
    {
        var cut = RenderStatic<OsirionBaseSection>(parameters => parameters
            .Add(p => p.BackgroundImageUrl, "/img/bg.webp")
            .Add(p => p.ShowOverlay, false));

        cut.FindAll(".osirion-section-overlay").ShouldBeEmpty();
    }

    [Fact]
    public void BaseSection_PatternShown_RendersPatternAndPatternClass()
    {
        var cut = RenderStatic<OsirionBaseSection>(parameters => parameters
            .Add(p => p.ShowPattern, true)
            .Add(p => p.BackgroundPattern, BackgroundPatternType.Honeycomb));

        cut.Find("section").ClassList.ShouldContain("osirion-section-with-pattern");
        cut.Find(".osirion-bg-wrapper").ClassList.ShouldContain("osirion-bg-honeycomb");
    }

    [Fact]
    public void BaseSection_PatternShownWithoutType_AddsNoPatternClass()
    {
        var cut = RenderStatic<OsirionBaseSection>(parameters => parameters
            .Add(p => p.ShowPattern, true));

        cut.Find("section").ClassList.ShouldNotContain("osirion-section-with-pattern");
        cut.FindAll(".osirion-bg-wrapper").ShouldBeEmpty();
    }

    [Fact]
    public void BaseSection_DividerPaddingAndClass_RendersMatchingClasses()
    {
        var cut = RenderStatic<OsirionBaseSection>(parameters => parameters
            .Add(p => p.HasDivider, true)
            .Add(p => p.Padding, SectionPadding.Large)
            .Add(p => p.TextAlignment, Alignment.Right)
            .Add(p => p.Class, "pricing"));

        cut.Find("section").ClassList.ShouldBe(
        [
            "osirion-section",
            "osirion-section-align-right",
            "osirion-section-padding-large",
            "osirion-section-divider",
            "pricing"
        ]);
    }

    [Fact]
    public void BaseSection_ColorsAndStyle_AreWrittenToTheStyleAttribute()
    {
        var cut = RenderStatic<OsirionBaseSection>(parameters => parameters
            .Add(p => p.BackgroundColor, "#000")
            .Add(p => p.TextColor, "#fff")
            .Add(p => p.Style, "margin-top: 2rem"));

        cut.Find("section").GetAttribute("style").ShouldBe("background-color: #000; color: #fff; margin-top: 2rem;");
    }

    [Fact]
    public void BaseSection_ContainerClass_ReplacesTheFrameworkContainer()
    {
        var cut = RenderStatic<OsirionBaseSection>(parameters => parameters
            .Add(p => p.ContainerClass, "my-container"));

        cut.Find("section > div").ClassList.ShouldBe(["my-container"]);
    }

    [Theory]
    [InlineData(CssFramework.Bootstrap, "container-xxl")]
    [InlineData(CssFramework.FluentUI, "osirion-container")]
    [InlineData(CssFramework.MudBlazor, "mud-container")]
    [InlineData(CssFramework.Radzen, "rz-container")]
    public void BaseSection_Framework_UsesTheFrameworkContainerClass(CssFramework framework, string expected)
    {
        Services.AddSingleton(Options.Create(new ThemingOptions { Framework = framework }));

        var cut = RenderStatic<OsirionBaseSection>(parameters => parameters
            .Add(p => p.Id, "s"));

        cut.Find("section > div").ClassList.ShouldBe([expected]);
    }

    private static RenderFragment Markup(string markup) => builder => builder.AddMarkupContent(0, markup);
}
