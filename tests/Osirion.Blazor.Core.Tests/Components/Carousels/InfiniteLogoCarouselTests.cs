using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Carousels;

public class InfiniteLogoCarouselTests : OsirionComponentTestBase
{
    public InfiniteLogoCarouselTests()
    {
        Services.AddJSComponents();
    }

    [Fact]
    public void LogoCarousel_Defaults_RendersDefaultLogosWithoutTitleOrClone()
    {
        var cut = RenderStatic<InfiniteLogoCarousel>(parameters => { });

        var section = cut.Find("section");
        section.GetAttribute("aria-label").ShouldBe("Our Clients");
        section.ClassList.ShouldContain("osirion-infinite-carousel-pausable");
        section.ClassList.ShouldContain("osirion-carousel-grayscale-enabled");
        section.ClassList.ShouldNotContain("osirion-carousel-autoscroll");
        section.ClassList.ShouldNotContain("osirion-carousel-reverse");
        cut.FindAll("h2").ShouldBeEmpty();
        cut.FindAll("ul[aria-hidden]").ShouldBeEmpty();
        cut.FindAll("li.osirion-logo-item").Count.ShouldBe(6);
        cut.Find(".osirion-carousel-wrapper").GetAttribute("data-autoscroll").ShouldBe("false");
    }

    [Fact]
    public void LogoCarousel_TitleDirectionAndFlags_AreReflectedInMarkup()
    {
        var cut = RenderStatic<InfiniteLogoCarousel>(parameters => parameters
            .Add(p => p.Title, "Trusted by")
            .Add(p => p.SectionTitle, "Partners")
            .Add(p => p.Direction, AnimationDirection.Left)
            .Add(p => p.PauseOnHover, false)
            .Add(p => p.EnableGrayscale, false)
            .Add(p => p.EnableAutoScroll, true));

        cut.Find("h2.osirion-carousel-title").TextContent.ShouldBe("Trusted by");
        var section = cut.Find("section");
        section.GetAttribute("aria-label").ShouldBe("Partners");
        section.ClassList.ShouldContain("osirion-carousel-reverse");
        section.ClassList.ShouldContain("osirion-carousel-autoscroll");
        section.ClassList.ShouldNotContain("osirion-infinite-carousel-pausable");
        section.ClassList.ShouldNotContain("osirion-carousel-grayscale-enabled");
        var wrapper = cut.Find(".osirion-carousel-wrapper");
        wrapper.GetAttribute("data-autoscroll").ShouldBe("true");
        wrapper.GetAttribute("data-autoscroll-direction").ShouldBe("left");
    }

    [Fact]
    public void LogoCarousel_Dimensions_AreWrittenAsCustomProperties()
    {
        var cut = RenderStatic<InfiniteLogoCarousel>(parameters => parameters
            .Add(p => p.AnimationDuration, 30)
            .Add(p => p.LogoWidth, 120)
            .Add(p => p.LogoHeight, 40)
            .Add(p => p.LogoGap, 8));

        cut.Find(".osirion-carousel-container").GetAttribute("style").ShouldBe(
            "--osirion-carousel-duration: 30s; --osirion-logo-width: 120px; --osirion-logo-height: 40px; --osirion-logo-gap: 8px;");
    }

    [Fact]
    public void LogoCarousel_MaxVisibleLogos_LimitsBothLists()
    {
        var cut = RenderStatic<InfiniteLogoCarousel>(parameters => parameters
            .Add(p => p.MaxVisibleLogos, 2)
            .Add(p => p.EnableAutoScroll, true));

        cut.FindAll("ul:not([aria-hidden]) li.osirion-logo-item").Count.ShouldBe(2);
        cut.FindAll("ul[aria-hidden='true'] li.osirion-logo-item").Count.ShouldBe(2);
    }

    [Fact]
    public void LogoCarousel_ZeroMaxVisibleLogos_IsIgnored()
    {
        var cut = RenderStatic<InfiniteLogoCarousel>(parameters => parameters
            .Add(p => p.MaxVisibleLogos, 0));

        cut.FindAll("li.osirion-logo-item").Count.ShouldBe(6);
    }

    [Fact]
    public void LogoCarousel_LogoWithoutUrl_RendersImageWithoutLink()
    {
        var cut = RenderStatic<InfiniteLogoCarousel>(parameters => parameters
            .Add(p => p.CustomLogos, [new LogoItem("/acme.png", "Acme")]));

        cut.FindAll("a").ShouldBeEmpty();
        var image = cut.Find("li.osirion-logo-item > img");
        image.GetAttribute("src").ShouldBe("/acme.png");
        image.GetAttribute("alt").ShouldBe("Acme");
        image.GetAttribute("width").ShouldBe("200");
        image.GetAttribute("height").ShouldBe("80");
    }

    [Fact]
    public void LogoCarousel_DualLogoWithoutUrl_RendersLightAndDarkImagesWithoutLink()
    {
        var cut = RenderStatic<InfiniteLogoCarousel>(parameters => parameters
            .Add(p => p.CustomLogos, [new LogoItem("/acme.png", "Acme", LightImageUrl: "/light.png", DarkImageUrl: "/dark.png")]));

        cut.FindAll("a").ShouldBeEmpty();
        cut.Find("li.osirion-logo-item").ClassList.ShouldContain("osirion-logo-dual");
        cut.Find("li > img.osirion-logo-light").GetAttribute("src").ShouldBe("/light.png");
        cut.Find("li > img.osirion-logo-dark").GetAttribute("src").ShouldBe("/dark.png");
    }

    [Fact]
    public void LogoCarousel_DualLogoWithUrl_RendersBothImagesInsideTheLink()
    {
        var cut = RenderStatic<InfiniteLogoCarousel>(parameters => parameters
            .Add(p => p.CustomLogos, [new LogoItem("/acme.png", "Acme", "/light.png", "/dark.png", Url: "https://acme.example")]));

        var link = cut.Find("li a");
        link.QuerySelector("img.osirion-logo-light")!.GetAttribute("src").ShouldBe("/light.png");
        link.QuerySelector("img.osirion-logo-dark")!.GetAttribute("src").ShouldBe("/dark.png");
    }

    [Fact]
    public void LogoCarousel_LinkDefaults_OpenInNewTabWithNofollowNoopenerAndTitle()
    {
        var cut = RenderStatic<InfiniteLogoCarousel>(parameters => parameters
            .Add(p => p.CustomLogos, [new LogoItem("/acme.png", "Acme", Url: "https://acme.example")]));

        var link = cut.Find("li a");
        link.GetAttribute("href").ShouldBe("https://acme.example");
        link.GetAttribute("target").ShouldBe("_blank");
        link.GetAttribute("rel").ShouldBe("nofollow noopener");
        link.GetAttribute("title").ShouldBe("Acme");
    }

    [Fact]
    public void LogoCarousel_LinkWithFollowAndOpenerAllowed_HasNoRelAndKeepsTarget()
    {
        var cut = RenderStatic<InfiniteLogoCarousel>(parameters => parameters
            .Add(p => p.CustomLogos, [new LogoItem("/acme.png", "Acme", Url: "/acme", Target: "_self", NoFollow: false, NoOpener: false)]));

        var link = cut.Find("li a");
        link.GetAttribute("target").ShouldBe("_self");
        link.HasAttribute("rel").ShouldBeFalse();
    }

    [Theory]
    [InlineData(false, true, "noopener")]
    [InlineData(true, false, "nofollow")]
    public void LogoCarousel_LinkRel_ListsOnlyTheEnabledValues(bool noFollow, bool noOpener, string expectedRel)
    {
        var cut = RenderStatic<InfiniteLogoCarousel>(parameters => parameters
            .Add(p => p.CustomLogos, [new LogoItem("/acme.png", "Acme", Url: "/acme", NoFollow: noFollow, NoOpener: noOpener)]));

        cut.Find("li a").GetAttribute("rel").ShouldBe(expectedRel);
    }

    [Theory]
    [InlineData(true, null, true)]
    [InlineData(true, false, false)]
    [InlineData(false, null, false)]
    [InlineData(false, true, true)]
    public void LogoCarousel_LogoGrayscale_OverridesTheCarouselSetting(bool carouselGrayscale, bool? logoGrayscale, bool expected)
    {
        var cut = RenderStatic<InfiniteLogoCarousel>(parameters => parameters
            .Add(p => p.EnableGrayscale, carouselGrayscale)
            .Add(p => p.CustomLogos, [new LogoItem("/acme.png", "Acme", EnableGrayscale: logoGrayscale)]));

        cut.Find("li.osirion-logo-item").ClassList.Contains("osirion-logo-grayscale").ShouldBe(expected);
    }

    [Fact]
    public void LogoCarousel_AutoScrollClone_HasDecorativeImagesForSingleAndDualLogos()
    {
        var cut = RenderStatic<InfiniteLogoCarousel>(parameters => parameters
            .Add(p => p.EnableAutoScroll, true)
            .Add(p => p.CustomLogos,
            [
                new LogoItem("/acme.png", "Acme"),
                new LogoItem("/dual.png", "Dual", "/light.png", "/dark.png"),
            ]));

        var clone = cut.Find("ul[aria-hidden='true']");
        clone.QuerySelectorAll("img").Select(i => i.GetAttribute("src")).ShouldBe(["/acme.png", "/light.png", "/dark.png"]);
        clone.QuerySelectorAll("img").ShouldAllBe(i => i.GetAttribute("alt") == string.Empty);
    }
}
