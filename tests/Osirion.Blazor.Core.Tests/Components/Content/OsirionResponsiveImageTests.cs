using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Content;

public class OsirionResponsiveImageTests : OsirionComponentTestBase
{
    public OsirionResponsiveImageTests() => SetRendererInfo(new RendererInfo("Static", false));

    [Fact]
    public void OsirionResponsiveImage_Defaults_LazyAsyncWithDimensionsAndNoSrcSet()
    {
        var cut = Render<OsirionResponsiveImage>(p => p
            .Add(i => i.Src, "/img/hero.jpg")
            .Add(i => i.Alt, "A mountain at dawn")
            .Add(i => i.Width, 1600)
            .Add(i => i.Height, 900));

        var img = cut.Find("img");
        img.GetAttribute("src").ShouldBe("/img/hero.jpg");
        img.GetAttribute("alt").ShouldBe("A mountain at dawn");
        img.GetAttribute("width").ShouldBe("1600");
        img.GetAttribute("height").ShouldBe("900");
        img.GetAttribute("loading").ShouldBe("lazy");
        img.GetAttribute("decoding").ShouldBe("async");
        img.HasAttribute("fetchpriority").ShouldBeFalse();
        img.HasAttribute("srcset").ShouldBeFalse();
        img.HasAttribute("sizes").ShouldBeFalse();
        img.ClassList.ShouldContain("osirion-responsive-image");
        cut.FindAll("figure").ShouldBeEmpty();
    }

    [Fact]
    public void OsirionResponsiveImage_Priority_LoadsEagerlyWithHighFetchPriority()
    {
        var cut = Render<OsirionResponsiveImage>(p => p
            .Add(i => i.Src, "/img/hero.jpg")
            .Add(i => i.Alt, "Hero")
            .Add(i => i.Priority, true));

        var img = cut.Find("img");
        img.GetAttribute("loading").ShouldBe("eager");
        img.GetAttribute("fetchpriority").ShouldBe("high");
    }

    [Fact]
    public void OsirionResponsiveImage_WidthsAndFormatter_BuildASortedDistinctSrcSetWithAutoSizes()
    {
        var cut = Render<OsirionResponsiveImage>(p => p
            .Add(i => i.Src, "https://images.example.com/a.jpg")
            .Add(i => i.Alt, "A")
            .Add(i => i.Widths, [1280, 640, 640, 0, 1920])
            .Add(i => i.SrcFormatter, (src, width) => $"{src}?w={width}"));

        var img = cut.Find("img");
        img.GetAttribute("srcset").ShouldBe(
            "https://images.example.com/a.jpg?w=640 640w, https://images.example.com/a.jpg?w=1280 1280w, https://images.example.com/a.jpg?w=1920 1920w");
        img.GetAttribute("sizes").ShouldBe("auto, 100vw");
    }

    [Fact]
    public void OsirionResponsiveImage_ExplicitSrcSetAndSizes_AreUsedAsWritten()
    {
        var cut = Render<OsirionResponsiveImage>(p => p
            .Add(i => i.Src, "/a.jpg")
            .Add(i => i.Alt, "A")
            .Add(i => i.SrcSet, "/a-480.jpg 480w, /a-960.jpg 960w")
            .Add(i => i.Widths, [100])
            .Add(i => i.SrcFormatter, (src, width) => "ignored")
            .Add(i => i.Sizes, "(min-width: 768px) 50vw, 100vw"));

        var img = cut.Find("img");
        img.GetAttribute("srcset").ShouldBe("/a-480.jpg 480w, /a-960.jpg 960w");
        img.GetAttribute("sizes").ShouldBe("(min-width: 768px) 50vw, 100vw");
    }

    [Fact]
    public void OsirionResponsiveImage_PriorityWithSrcSet_DefaultsSizesToTheViewportWidth()
    {
        var cut = Render<OsirionResponsiveImage>(p => p
            .Add(i => i.Src, "/a.jpg")
            .Add(i => i.Alt, "A")
            .Add(i => i.SrcSet, "/a-480.jpg 480w")
            .Add(i => i.Priority, true));

        cut.Find("img").GetAttribute("sizes").ShouldBe("100vw");
    }

    [Fact]
    public void OsirionResponsiveImage_Caption_WrapsTheImageInAFigure()
    {
        var cut = Render<OsirionResponsiveImage>(p => p
            .Add(i => i.Src, "/a.jpg")
            .Add(i => i.Alt, "Chart of monthly visitors")
            .Add(i => i.Caption, "Visitors per month, 2026"));

        cut.Find("figure > img.osirion-responsive-image").ShouldNotBeNull();
        cut.Find("figure > figcaption").TextContent.ShouldBe("Visitors per month, 2026");
    }

    [Fact]
    public void OsirionResponsiveImage_ClassAndAttributes_AreApplied()
    {
        var cut = Render<OsirionResponsiveImage>(p => p
            .Add(i => i.Src, "/a.jpg")
            .Add(i => i.Alt, "")
            .Add(i => i.Class, "rounded")
            .AddUnmatched("data-test", "x"));

        var img = cut.Find("img");
        img.ClassList.ShouldContain("rounded");
        img.GetAttribute("data-test").ShouldBe("x");
        img.GetAttribute("alt").ShouldBe(string.Empty);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(10, -1)]
    public void OsirionResponsiveImage_NonPositiveDimension_Throws(int width, int height)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Render<OsirionResponsiveImage>(p => p
            .Add(i => i.Src, "/a.jpg")
            .Add(i => i.Alt, "A")
            .Add(i => i.Width, width)
            .Add(i => i.Height, height)));
    }
}
