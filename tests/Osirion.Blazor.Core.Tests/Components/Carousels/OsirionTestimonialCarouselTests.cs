using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Carousels;

public class OsirionTestimonialCarouselTests : OsirionComponentTestBase
{
    public OsirionTestimonialCarouselTests()
    {
        Services.AddJSComponents();
    }

    [Fact]
    public void TestimonialCarousel_Defaults_RendersSampleTestimonialsWithDefaultClassesAndNoTitle()
    {
        var cut = RenderStatic<OsirionTestimonialCarousel>(parameters => { });

        var section = cut.Find("section");
        section.GetAttribute("aria-label").ShouldBe("Customer Testimonials");
        section.ClassList.ShouldContain("osirion-testimonial-carousel-pausable");
        section.ClassList.ShouldContain("osirion-testimonial-carousel-normal");
        section.ClassList.ShouldNotContain("osirion-testimonial-carousel-reverse");
        cut.FindAll("h2").ShouldBeEmpty();
        cut.FindAll(".osirion-testimonial-carousel-slide:not([aria-hidden])").Count.ShouldBe(8);
        cut.FindAll(".osirion-testimonial-card.osirion-testimonial-theme-system").Count.ShouldBe(16);
    }

    [Fact]
    public void TestimonialCarousel_TitleDirectionAndNoPause_AreReflectedInMarkup()
    {
        var cut = RenderStatic<OsirionTestimonialCarousel>(parameters => parameters
            .Add(p => p.Title, "What people say")
            .Add(p => p.SectionTitle, "Reviews")
            .Add(p => p.Direction, AnimationDirection.Left)
            .Add(p => p.PauseOnHover, false));

        cut.Find("h2.osirion-testimonial-carousel-title").TextContent.ShouldBe("What people say");
        var section = cut.Find("section");
        section.GetAttribute("aria-label").ShouldBe("Reviews");
        section.ClassList.ShouldContain("osirion-testimonial-carousel-reverse");
        section.ClassList.ShouldNotContain("osirion-testimonial-carousel-pausable");
    }

    [Theory]
    [InlineData(CarouselSpeed.Slow, 60, "slow", 90)]
    [InlineData(CarouselSpeed.Normal, 60, "normal", 60)]
    [InlineData(CarouselSpeed.Fast, 60, "fast", 30)]
    [InlineData(CarouselSpeed.Fast, 20, "fast", 10)]
    public void TestimonialCarousel_Speed_SetsClassAndEffectiveDuration(CarouselSpeed speed, int duration, string speedClass, int expectedSeconds)
    {
        var cut = RenderStatic<OsirionTestimonialCarousel>(parameters => parameters
            .Add(p => p.Speed, speed)
            .Add(p => p.AnimationDuration, duration));

        cut.Find("section").ClassList.ShouldContain($"osirion-testimonial-carousel-{speedClass}");
        cut.Find(".osirion-testimonial-carousel-track").GetAttribute("style")!
            .ShouldContain($"--osirion-testimonial-carousel-duration: {expectedSeconds}s");
    }

    [Fact]
    public void TestimonialCarousel_CardDimensions_AreWrittenAsCustomProperties()
    {
        var cut = RenderStatic<OsirionTestimonialCarousel>(parameters => parameters
            .Add(p => p.CardWidth, 320)
            .Add(p => p.CardHeight, 250)
            .Add(p => p.CardGap, 12));

        cut.Find(".osirion-testimonial-carousel-track").GetAttribute("style").ShouldBe(
            "--osirion-testimonial-carousel-duration: 60s; --osirion-testimonial-card-width: 320px; --osirion-testimonial-card-height: 250px; --osirion-testimonial-card-gap: 12px;");
    }

    [Fact]
    public void TestimonialCarousel_MaxVisibleTestimonials_LimitsTheRenderedSet()
    {
        var cut = RenderStatic<OsirionTestimonialCarousel>(parameters => parameters
            .Add(p => p.MaxVisibleTestimonials, 3));

        var names = cut.FindAll(".osirion-testimonial-carousel-slide:not([aria-hidden]) cite").Select(c => c.TextContent);
        names.ShouldBe(["Sarah Johnson", "Michael Chen", "Lisa Rodriguez"]);
        cut.FindAll(".osirion-testimonial-carousel-slide[aria-hidden='true']").Count.ShouldBe(3);
    }

    [Fact]
    public void TestimonialCarousel_ZeroMaxVisibleTestimonials_IsIgnored()
    {
        var cut = RenderStatic<OsirionTestimonialCarousel>(parameters => parameters
            .Add(p => p.MaxVisibleTestimonials, 0));

        cut.FindAll(".osirion-testimonial-carousel-slide:not([aria-hidden])").Count.ShouldBe(8);
    }

    [Fact]
    public void TestimonialCarousel_FewerThanThreeCustomTestimonials_RepeatsThemToFillThree()
    {
        var testimonials = new List<TestimonialItem> { new("Ann"), new("Bob") };

        var cut = RenderStatic<OsirionTestimonialCarousel>(parameters => parameters
            .Add(p => p.CustomTestimonials, testimonials));

        cut.FindAll(".osirion-testimonial-carousel-slide:not([aria-hidden]) cite")
            .Select(c => c.TextContent)
            .ShouldBe(["Ann", "Bob", "Ann"]);
    }

    [Fact]
    public void TestimonialCarousel_FirstTwoCards_LoadImagesEagerlyAndClonesLazily()
    {
        var testimonials = new List<TestimonialItem>
        {
            new("Ann", ProfileImageUrl: "/a.jpg"),
            new("Bob", ProfileImageUrl: "/b.jpg"),
            new("Cid", ProfileImageUrl: "/c.jpg"),
        };

        var cut = RenderStatic<OsirionTestimonialCarousel>(parameters => parameters
            .Add(p => p.CustomTestimonials, testimonials));

        cut.FindAll(".osirion-testimonial-carousel-slide:not([aria-hidden]) img")
            .Select(i => i.GetAttribute("loading"))
            .ShouldBe(["eager", "eager", "lazy"]);
        cut.FindAll(".osirion-testimonial-carousel-slide[aria-hidden='true'] img")
            .ShouldAllBe(i => i.GetAttribute("loading") == "lazy");
    }

    [Fact]
    public void TestimonialCarousel_ItemSettings_FlowIntoTheCards()
    {
        var testimonials = new List<TestimonialItem>
        {
            new("Ann", ShowRating: true, Rating: 4, Elevated: false, AdditionalCssClass: "featured"),
            new("Bob", ShowRating: true, Rating: 5),
            new("Cid"),
        };

        var cut = RenderStatic<OsirionTestimonialCarousel>(parameters => parameters
            .Add(p => p.CustomTestimonials, testimonials)
            .Add(p => p.ShowRating, true)
            .Add(p => p.CardElevated, true)
            .Add(p => p.CardBorderless, true)
            .Add(p => p.CardVariant, TestimonialVariant.Minimal)
            .Add(p => p.CardSize, TestimonialSize.Small));

        var cards = cut.FindAll(".osirion-testimonial-carousel-slide:not([aria-hidden]) .osirion-testimonial-card");
        cards[0].ClassList.ShouldContain("featured");
        cards[0].ClassList.ShouldNotContain("osirion-testimonial-elevated");
        cards[1].ClassList.ShouldContain("osirion-testimonial-elevated");
        cards.ShouldAllBe(c => c.ClassList.Contains("osirion-testimonial-borderless")
            && c.ClassList.Contains("osirion-testimonial-minimal")
            && c.ClassList.Contains("osirion-testimonial-small"));
        cards[0].QuerySelector(".osirion-testimonial-rating")!.GetAttribute("aria-label").ShouldBe("Rating: 4 out of 5 stars");
        cards[2].QuerySelector(".osirion-testimonial-rating").ShouldBeNull();
    }

    [Fact]
    public void TestimonialCarousel_ShowRatingOff_HidesItemRatings()
    {
        var cut = RenderStatic<OsirionTestimonialCarousel>(parameters => parameters
            .Add(p => p.ShowRating, false));

        cut.FindAll(".osirion-testimonial-rating").ShouldBeEmpty();
    }

    [Theory]
    [InlineData(true, "osirion-testimonial-theme-dark")]
    [InlineData(false, "osirion-testimonial-theme-system")]
    public void TestimonialCarousel_InheritTheme_DecidesTheCardTheme(bool inherit, string expectedClass)
    {
        var cut = RenderStatic<OsirionTestimonialCarousel>(parameters => parameters
            .Add(p => p.Theme, ThemeMode.Dark)
            .Add(p => p.InheritTheme, inherit));

        cut.FindAll(".osirion-testimonial-card").ShouldAllBe(c => c.ClassList.Contains(expectedClass));
    }

    [Fact]
    public void TestimonialCarousel_GetFirstVisibleImageUrl_ReturnsTheFirstTestimonialImage()
    {
        var cut = RenderStatic<OsirionTestimonialCarousel>(parameters => parameters
            .Add(p => p.CustomTestimonials, [new("Ann", ProfileImageUrl: "/a.jpg"), new("Bob"), new("Cid")]));

        cut.Instance.GetFirstVisibleImageUrl().ShouldBe("/a.jpg");
    }

    [Fact]
    public void TestimonialCarousel_GetFirstVisibleImageUrl_WithoutImage_ReturnsNull()
    {
        var cut = RenderStatic<OsirionTestimonialCarousel>(parameters => parameters
            .Add(p => p.CustomTestimonials, [new("Ann"), new("Bob"), new("Cid")]));

        cut.Instance.GetFirstVisibleImageUrl().ShouldBeNull();
    }
}
