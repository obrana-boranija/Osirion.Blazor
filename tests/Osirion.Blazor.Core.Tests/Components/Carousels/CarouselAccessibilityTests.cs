using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Core.Extensions;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Carousels;

public class CarouselAccessibilityTests : OsirionComponentTestBase
{
    public CarouselAccessibilityTests()
    {
        Services.AddJSComponents();
    }

    [Fact]
    public void InfiniteLogoCarousel_LoopingCopy_IsInertSoItsLinksLeaveTheTabOrder()
    {
        var cut = RenderStatic<InfiniteLogoCarousel>(parameters => parameters
            .Add(p => p.EnableAutoScroll, true)
            .Add(p => p.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenElement(0, "a");
                builder.AddAttribute(1, "href", "/more");
                builder.AddContent(2, "More clients");
                builder.CloseElement();
            })));

        var clone = cut.Find("ul[aria-hidden='true']");

        clone.HasAttribute("inert").ShouldBeTrue();
        clone.QuerySelector("a")!.TextContent.ShouldBe("More clients");
    }

    [Fact]
    public void InfiniteLogoCarousel_PrimaryList_IsNotInert()
    {
        var cut = RenderStatic<InfiniteLogoCarousel>(parameters => parameters.Add(p => p.EnableAutoScroll, true));

        cut.FindAll("ul.osirion-carousel-list:not([aria-hidden])").ShouldNotBeEmpty();
        cut.FindAll("ul.osirion-carousel-list:not([aria-hidden])").ShouldAllBe(list => !list.HasAttribute("inert"));
    }

    [Fact]
    public void OsirionTestimonialCarousel_LoopingCopies_AreInertAndOriginalsAreNot()
    {
        var cut = RenderStatic<OsirionTestimonialCarousel>(parameters => { });

        var clones = cut.FindAll(".osirion-testimonial-carousel-slide[aria-hidden='true']");
        var originals = cut.FindAll(".osirion-testimonial-carousel-slide:not([aria-hidden])");

        clones.ShouldNotBeEmpty();
        clones.Count.ShouldBe(originals.Count);
        clones.ShouldAllBe(slide => slide.HasAttribute("inert"));
        originals.ShouldAllBe(slide => !slide.HasAttribute("inert"));
    }

    [Theory]
    [InlineData("OsirionTestimonialCarousel")]
    [InlineData("InfiniteLogoCarousel")]
    public void CarouselScript_NeverCancelsVerticalWheelAndGatesTheFrameLoop(string component)
    {
        var script = ReadCarouselAsset(component + ".razor.js");

        // Vertical wheel must reach the page: the only preventDefault on wheel input is behind a horizontal-dominant check
        script.ShouldContain("Math.abs(event.deltaX) <= Math.abs(event.deltaY)");
        // The frame loop is started only by syncFocusTracking, which requires visibility, motion and a running animation
        script.ShouldContain("IntersectionObserver");
        script.ShouldContain("prefers-reduced-motion");
        script.ShouldNotContain("this.focusAnimationFrame = window.requestAnimationFrame(update)");
    }

    [Theory]
    [InlineData("OsirionTestimonialCarousel")]
    [InlineData("InfiniteLogoCarousel")]
    public void CarouselStyles_PauseWhileKeyboardFocusIsInside(string component)
    {
        var css = ReadCarouselAsset(component + ".razor.css");

        css.ShouldContain(":focus-within");
        css.ShouldContain(".osirion-carousel-pause");
    }

    [Fact]
    public void TestimonialCarouselStyles_ReducedMotion_DoesNotHideRealTestimonials()
    {
        var css = ReadCarouselAsset("OsirionTestimonialCarousel.razor.css");
        var reducedMotion = css[css.IndexOf("@media (prefers-reduced-motion: reduce)", StringComparison.Ordinal)..];
        var reducedMotionBlock = reducedMotion[..reducedMotion.IndexOf("/* High contrast", StringComparison.Ordinal)];

        reducedMotionBlock.ShouldNotContain("nth-child");
        reducedMotionBlock.ShouldContain("[aria-hidden=\"true\"]");
    }

    private static string ReadCarouselAsset(string fileName)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "src", "Osirion.Blazor.Core", "Components", "Carousels", fileName);
            if (File.Exists(candidate))
            {
                return File.ReadAllText(candidate);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Could not locate {fileName} above {AppContext.BaseDirectory}");
    }
}
