using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Core.Components.Sections;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;
using ViewportMode = Osirion.Blazor.Core.Components.Sections.OsirionResponsiveShowcaseSection.ViewportMode;

namespace Osirion.Blazor.Core.Tests.Components.Sections;

public class OsirionResponsiveShowcaseSectionTests : OsirionComponentTestBase
{
    [Fact]
    public void Showcase_Defaults_RendersDesktopViewportWithPlaceholder()
    {
        var cut = RenderStatic<OsirionResponsiveShowcaseSection>(_ => { });

        cut.Find("h3.showcase-title").TextContent.ShouldBe("Component Preview");
        var container = cut.Find(".viewport-container");
        container.ClassList.ShouldContain("viewport-desktop");
        container.GetAttribute("style").ShouldBe("width: 1200px; max-width: 100%; min-height: 600px");
        cut.Find(".chrome-address").TextContent.ShouldBe("example.com - Desktop View");
        cut.Find(".placeholder p").TextContent.ShouldBe("No component provided");
        cut.Find(".dimensions-info small").TextContent.ShouldStartWith("Desktop: 1200px");
        cut.FindAll(".code-section").ShouldBeEmpty();
    }

    // Fails before the fix: the root element had a fixed class, so Class and unmatched attributes were dropped.
    [Fact]
    public void Showcase_ClassAndAdditionalAttributes_AreWrittenOnTheRoot()
    {
        var cut = RenderStatic<OsirionResponsiveShowcaseSection>(parameters => parameters
            .Add(p => p.Class, "docs-preview")
            .AddUnmatched("data-testid", "showcase"));

        var root = cut.Find(".osirion-responsive-showcase");
        root.ClassList.ShouldBe(["osirion-responsive-showcase", "with-chrome", "docs-preview"]);
        root.GetAttribute("data-testid").ShouldBe("showcase");
    }

    [Fact]
    public void Showcase_BrowserChromeDisabled_LeavesOutTheChromeClass()
    {
        var cut = RenderStatic<OsirionResponsiveShowcaseSection>(parameters => parameters
            .Add(p => p.ShowBrowserChrome, false));

        cut.Find(".osirion-responsive-showcase").ClassList.ShouldBe(["osirion-responsive-showcase"]);
    }

    [Fact]
    public void Showcase_Defaults_MarksOnlyTheDesktopButtonActive()
    {
        var cut = RenderStatic<OsirionResponsiveShowcaseSection>(_ => { });

        cut.FindAll(".viewport-btn").Select(b => b.ClassList.Contains("active")).ShouldBe([true, false, false]);
    }

    [Theory]
    [InlineData(ViewportMode.Tablet, "viewport-tablet", "width: 768px; max-width: 100%; min-height: 500px", "example.com - Tablet View", "Tablet: 768px")]
    [InlineData(ViewportMode.Mobile, "viewport-mobile", "width: 375px; max-width: 100%; min-height: 600px", "example.com - Mobile View", "Mobile: 375px")]
    public void Showcase_InitialViewport_RendersThatViewport(
        ViewportMode viewport, string expectedClass, string expectedStyle, string expectedTitle, string expectedDimensions)
    {
        var cut = RenderStatic<OsirionResponsiveShowcaseSection>(parameters => parameters
            .Add(p => p.InitialViewport, viewport));

        var container = cut.Find(".viewport-container");
        container.ClassList.ShouldContain(expectedClass);
        container.GetAttribute("style").ShouldBe(expectedStyle);
        cut.Find(".chrome-address").TextContent.ShouldBe(expectedTitle);
        cut.Find(".dimensions-info small").TextContent.ShouldStartWith(expectedDimensions);
        cut.Instance.CurrentViewport.ShouldBe(viewport);
    }

    // Fails before the fix: the source held U+FFFD where a multiplication sign was meant, so every viewport showed a
    // replacement character between width and height.
    [Theory]
    [InlineData(ViewportMode.Desktop, "Desktop: 1200px × 600px+ (Large screens, laptops)")]
    [InlineData(ViewportMode.Tablet, "Tablet: 768px × 500px+ (iPad, tablets)")]
    [InlineData(ViewportMode.Mobile, "Mobile: 375px × 600px+ (iPhone, Android phones)")]
    public void Showcase_Dimensions_SeparateWidthAndHeightWithAMultiplicationSign(ViewportMode viewport, string expected)
    {
        var cut = RenderStatic<OsirionResponsiveShowcaseSection>(parameters => parameters
            .Add(p => p.InitialViewport, viewport));

        cut.Find(".dimensions-info small").TextContent.ShouldBe(expected);
    }

    [Fact]
    public void Showcase_UndefinedViewport_FallsBackToDesktopClassAndGenericTitle()
    {
        var cut = RenderStatic<OsirionResponsiveShowcaseSection>(parameters => parameters
            .Add(p => p.InitialViewport, (ViewportMode)99)
            .Add(p => p.Style, "border: 0"));

        cut.Find(".viewport-container").ClassList.ShouldContain("viewport-desktop");
        cut.Find(".viewport-container").GetAttribute("style").ShouldBe("border: 0");
        cut.Find(".chrome-address").TextContent.ShouldBe("example.com");
        cut.Find(".dimensions-info small").TextContent.ShouldBeEmpty();
    }

    [Fact]
    public void Showcase_Style_IsAppendedToTheViewportStyle()
    {
        var cut = RenderStatic<OsirionResponsiveShowcaseSection>(parameters => parameters
            .Add(p => p.InitialViewport, ViewportMode.Mobile)
            .Add(p => p.Style, "background: white"));

        cut.Find(".viewport-container").GetAttribute("style")
            .ShouldBe("width: 375px; max-width: 100%; min-height: 600px; background: white");
    }

    [Fact]
    public void Showcase_TitleAndChildContent_RendersThemInsteadOfPlaceholder()
    {
        var cut = RenderStatic<OsirionResponsiveShowcaseSection>(parameters => parameters
            .Add(p => p.Title, "Card preview")
            .AddChildContent("<div class=\"demo\">Demo</div>"));

        cut.Find("h3.showcase-title").TextContent.ShouldBe("Card preview");
        cut.Find(".component-wrapper .demo").TextContent.ShouldBe("Demo");
        cut.FindAll(".placeholder").ShouldBeEmpty();
    }

    [Fact]
    public void Showcase_DimensionsHidden_RendersNoDimensions()
    {
        var cut = RenderStatic<OsirionResponsiveShowcaseSection>(parameters => parameters
            .Add(p => p.ShowDimensions, false));

        cut.FindAll(".dimensions-info").ShouldBeEmpty();
    }

    [Fact]
    public void Showcase_CodeSnippet_RendersCollapsedToggle()
    {
        var cut = RenderStatic<OsirionResponsiveShowcaseSection>(parameters => parameters
            .Add(p => p.CodeSnippet, "<OsirionCard />"));

        cut.Find(".code-toggle").TextContent.Trim().ShouldBe("Show Code");
        cut.FindAll(".code-content").ShouldBeEmpty();
    }

    [Fact]
    public void Showcase_CodeHidden_RendersNoCodeSectionEvenWithSnippet()
    {
        var cut = RenderStatic<OsirionResponsiveShowcaseSection>(parameters => parameters
            .Add(p => p.ShowCode, false)
            .Add(p => p.CodeSnippet, "<OsirionCard />"));

        cut.FindAll(".code-section").ShouldBeEmpty();
    }

    [Fact]
    public void Showcase_ToggleCodeClicked_ShowsThenHidesTheSnippet()
    {
        SetRendererInfo(new RendererInfo("Server", true));
        var cut = Render<OsirionResponsiveShowcaseSection>(parameters => parameters
            .Add(p => p.CodeSnippet, "<OsirionCard />"));

        cut.Find(".code-toggle").Click();

        cut.Find(".code-content code").TextContent.ShouldBe("<OsirionCard />");
        cut.Find(".code-toggle").TextContent.Trim().ShouldBe("Hide Code");

        cut.Find(".code-toggle").Click();

        cut.FindAll(".code-content").ShouldBeEmpty();
    }

    [Theory]
    [InlineData(1, ViewportMode.Tablet, "viewport-tablet")]
    [InlineData(2, ViewportMode.Mobile, "viewport-mobile")]
    public void Showcase_ViewportButtonClicked_SwitchesTheViewport(int buttonIndex, ViewportMode expected, string expectedClass)
    {
        SetRendererInfo(new RendererInfo("Server", true));
        var cut = Render<OsirionResponsiveShowcaseSection>();

        cut.FindAll(".viewport-btn")[buttonIndex].Click();

        cut.Instance.CurrentViewport.ShouldBe(expected);
        cut.Find(".viewport-container").ClassList.ShouldContain(expectedClass);
        cut.FindAll(".viewport-btn")[buttonIndex].ClassList.ShouldContain("active");
    }

    [Fact]
    public void Showcase_DesktopButtonClickedFromMobile_SwitchesBackToDesktop()
    {
        SetRendererInfo(new RendererInfo("Server", true));
        var cut = Render<OsirionResponsiveShowcaseSection>(parameters => parameters
            .Add(p => p.InitialViewport, ViewportMode.Mobile));

        cut.FindAll(".viewport-btn")[0].Click();

        cut.Instance.CurrentViewport.ShouldBe(ViewportMode.Desktop);
    }

    [Fact]
    public void Showcase_CurrentViewportButtonClicked_KeepsTheViewport()
    {
        SetRendererInfo(new RendererInfo("Server", true));
        var cut = Render<OsirionResponsiveShowcaseSection>();

        cut.FindAll(".viewport-btn")[0].Click();

        cut.Instance.CurrentViewport.ShouldBe(ViewportMode.Desktop);
        cut.Find(".viewport-container").ClassList.ShouldContain("viewport-desktop");
    }
}
