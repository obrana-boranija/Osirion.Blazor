using Bunit;
using NSubstitute;
using Osirion.Blazor.Cms.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class LandingPageSectionTests : CmsPageComponentTestBase
{
    private static readonly IReadOnlyList<FeatureItem> Features =
    [
        new FeatureItem { Title = "Fast", Description = "Renders on the server.", Icon = "F", LinkUrl = "/fast" },
        new FeatureItem { Title = "Small", Description = "No script needed.", IconUrl = "/img/small.svg" },
    ];

    private static readonly IReadOnlyList<TestimonialItem> Testimonials =
    [
        new TestimonialItem { Name = "Ada", Role = "Engineer", Company = "Acme", Quote = "It just works.", ImageUrl = "/img/ada.png" },
        new TestimonialItem { Name = "Grace", Quote = "Clear docs." },
        new TestimonialItem { Name = "No quote" },
    ];

    // Fails before the fix: ShowFeatures and FeatureItems were declared and never rendered.
    [Fact]
    public void LandingPage_Features_RenderUnderTheirTitle()
    {
        var cut = RenderStatic<LandingPage>(p => p
            .Add(c => c.ShowFeatures, true)
            .Add(c => c.FeaturesSectionTitle, "Why it works")
            .Add(c => c.FeaturesSectionDescription, "Three reasons.")
            .Add(c => c.FeaturesColumns, 2)
            .Add(c => c.FeatureItems, Features));

        var section = cut.Find("section.osirion-landing-features");
        section.QuerySelector(".osirion-content-section-title")!.TextContent.ShouldBe("Why it works");
        section.QuerySelector(".osirion-content-section-subtitle")!.TextContent.ShouldBe("Three reasons.");
        var items = cut.FindAll(".osirion-landing-feature");
        items.Count.ShouldBe(2);
        items[0].QuerySelector("a")!.GetAttribute("href").ShouldBe("/fast");
        items[0].QuerySelector("a")!.TextContent.ShouldBe("Fast");
        items[0].QuerySelector(".osirion-landing-feature-icon")!.TextContent.ShouldBe("F");
        items[0].QuerySelector("p")!.TextContent.ShouldBe("Renders on the server.");
        items[1].QuerySelector("img")!.GetAttribute("src").ShouldBe("/img/small.svg");
        items[1].QuerySelector("img")!.GetAttribute("alt").ShouldBe(string.Empty);
        items[1].QuerySelectorAll("a").ShouldBeEmpty();
        cut.Find(".osirion-landing-feature-list").GetAttribute("style")!.ShouldContain("--osirion-landing-columns: 2");
    }

    [Fact]
    public void LandingPage_FeaturesOffOrEmpty_RenderNoSection()
    {
        var off = RenderStatic<LandingPage>(p => p.Add(c => c.FeatureItems, Features));
        var empty = RenderStatic<LandingPage>(p => p.Add(c => c.ShowFeatures, true).Add(c => c.FeatureItems, []));

        off.FindAll(".osirion-landing-features").ShouldBeEmpty();
        empty.FindAll(".osirion-landing-features").ShouldBeEmpty();
    }

    // Fails before the fix: ShowTestimonials and TestimonialItems were declared and never rendered.
    [Fact]
    public void LandingPage_Testimonials_RenderQuoteAndAuthor()
    {
        var cut = RenderStatic<LandingPage>(p => p
            .Add(c => c.ShowTestimonials, true)
            .Add(c => c.TestimonialsSectionTitle, "Kind words")
            .Add(c => c.TestimonialItems, Testimonials));

        cut.Find("section.osirion-landing-testimonials .osirion-content-section-title").TextContent.ShouldBe("Kind words");
        var items = cut.FindAll(".osirion-landing-testimonial");
        items.Count.ShouldBe(2);
        items[0].QuerySelector("blockquote")!.TextContent.Trim().ShouldBe("It just works.");
        items[0].QuerySelector(".osirion-landing-testimonial-name")!.TextContent.ShouldBe("Ada");
        items[0].QuerySelector(".osirion-landing-testimonial-role")!.TextContent.ShouldBe("Engineer, Acme");
        items[0].QuerySelector("img")!.GetAttribute("src").ShouldBe("/img/ada.png");
        items[1].QuerySelector(".osirion-landing-testimonial-role").ShouldBeNull();
    }

    // Fails before the fix: ShowTestimonialImages and TestimonialsLayout did nothing.
    [Fact]
    public void LandingPage_TestimonialImagesOffAndListLayout_AreHonored()
    {
        var cut = RenderStatic<LandingPage>(p => p
            .Add(c => c.ShowTestimonials, true)
            .Add(c => c.ShowTestimonialImages, false)
            .Add(c => c.TestimonialsLayout, "list")
            .Add(c => c.TestimonialItems, Testimonials));

        cut.FindAll(".osirion-landing-testimonial img").ShouldBeEmpty();
        cut.Find(".osirion-landing-testimonial-list").ClassList.ShouldContain("osirion-landing-layout-list");
    }

    [Fact]
    public void LandingPage_TestimonialsOffOrEmpty_RenderNoSection()
    {
        var off = RenderStatic<LandingPage>(p => p.Add(c => c.TestimonialItems, Testimonials));
        var empty = RenderStatic<LandingPage>(p => p.Add(c => c.ShowTestimonials, true));

        off.FindAll(".osirion-landing-testimonials").ShouldBeEmpty();
        empty.FindAll(".osirion-landing-testimonials").ShouldBeEmpty();
    }

    // Fails before the fix: the Cta parameters were declared and never rendered.
    [Fact]
    public void LandingPage_CallToAction_RendersTitleDescriptionAndButtons()
    {
        var cut = RenderStatic<LandingPage>(p => p
            .Add(c => c.CtaTitle, "Ready?")
            .Add(c => c.CtaDescription, "Start in a minute.")
            .Add(c => c.CtaPrimaryButtonText, "Get started")
            .Add(c => c.CtaPrimaryButtonUrl, "/docs")
            .Add(c => c.CtaSecondaryButtonText, "Talk to us")
            .Add(c => c.CtaSecondaryButtonUrl, "/contact"));

        var section = cut.Find("section.osirion-landing-cta");
        section.ClassList.ShouldContain("osirion-landing-cta-primary");
        section.QuerySelector("h2")!.TextContent.ShouldBe("Ready?");
        section.QuerySelector("p")!.TextContent.ShouldBe("Start in a minute.");
        var buttons = section.QuerySelectorAll("a");
        buttons.Select(a => a.GetAttribute("href")).ShouldBe(["/docs", "/contact"]);
        buttons.Select(a => a.TextContent.Trim()).ShouldBe(["Get started", "Talk to us"]);
    }

    [Fact]
    public void LandingPage_CallToActionWithoutContent_RendersNoSection()
    {
        var empty = RenderStatic<LandingPage>(p => p.Add(c => c.HeroTitle, "Hello"));
        var off = RenderStatic<LandingPage>(p => p
            .Add(c => c.ShowCallToAction, false)
            .Add(c => c.CtaTitle, "Ready?"));
        var buttonWithoutAddress = RenderStatic<LandingPage>(p => p.Add(c => c.CtaPrimaryButtonText, "Go"));

        empty.FindAll(".osirion-landing-cta").ShouldBeEmpty();
        off.FindAll(".osirion-landing-cta").ShouldBeEmpty();
        buttonWithoutAddress.FindAll(".osirion-landing-cta").ShouldBeEmpty();
    }

    [Theory]
    [InlineData("secondary", "osirion-landing-cta-secondary")]
    [InlineData("primary", "osirion-landing-cta-primary")]
    [InlineData("unknown", "osirion-landing-cta-primary")]
    public void LandingPage_CtaVariant_SelectsTheLookAndFallsBackToPrimary(string variant, string expectedClass)
    {
        var cut = RenderStatic<LandingPage>(p => p
            .Add(c => c.CtaTitle, "Ready?")
            .Add(c => c.CtaVariant, variant));

        cut.Find("section.osirion-landing-cta").ClassList.ShouldContain(expectedClass);
    }

    [Theory]
    [InlineData("#0b5fff", "background-color: #0b5fff")]
    [InlineData("rgb(11, 95, 255)", "background-color: rgb(11, 95, 255)")]
    [InlineData("var(--brand)", "background-color: var(--brand)")]
    [InlineData("red; position: fixed", null)]
    [InlineData("url(https://example.test/x)", null)]
    public void LandingPage_CtaBackgroundColor_IsAppliedOnlyWhenItIsAColor(string value, string? expectedStyle)
    {
        var cut = RenderStatic<LandingPage>(p => p
            .Add(c => c.CtaTitle, "Ready?")
            .Add(c => c.CtaBackgroundColor, value));

        cut.Find("section.osirion-landing-cta").GetAttribute("style").ShouldBe(expectedStyle);
    }

    // Fails before the fix: HeroAlignment was not given to the hero, which stayed left aligned.
    [Theory]
    [InlineData("center", "osirion-hero-align-center")]
    [InlineData("right", "osirion-hero-align-right")]
    [InlineData("LEFT", "osirion-hero-align-left")]
    [InlineData("sideways", "osirion-hero-align-center")]
    public void LandingPage_HeroAlignment_AlignsTheHero(string alignment, string expectedClass)
    {
        var cut = RenderStatic<LandingPage>(p => p
            .Add(c => c.HeroTitle, "Hello")
            .Add(c => c.HeroAlignment, alignment));

        cut.Find("section.osirion-hero-section").ClassList.ShouldContain(expectedClass);
    }

    [Fact]
    public void LandingPage_DefaultHeroAlignment_IsCenter()
    {
        var cut = RenderStatic<LandingPage>(p => p.Add(c => c.HeroTitle, "Hello"));

        cut.Find("section.osirion-hero-section").ClassList.ShouldContain("osirion-hero-align-center");
    }

    // Fails before the fix: ShowHeroButtons was not given to the hero, so its buttons always showed.
    [Fact]
    public void LandingPage_ShowHeroButtonsOff_HidesTheHeroButtons()
    {
        var cut = RenderStatic<LandingPage>(p => p
            .Add(c => c.HeroTitle, "Hello")
            .Add(c => c.HeroPrimaryButtonText, "Go")
            .Add(c => c.HeroPrimaryButtonUrl, "/go")
            .Add(c => c.ShowHeroButtons, false));

        cut.FindAll(".osirion-hero-buttons").ShouldBeEmpty();
    }

    [Fact]
    public void LandingPage_ShowHeroButtonsOn_ShowsTheHeroButtons()
    {
        var cut = RenderStatic<LandingPage>(p => p
            .Add(c => c.HeroTitle, "Hello")
            .Add(c => c.HeroPrimaryButtonText, "Go")
            .Add(c => c.HeroPrimaryButtonUrl, "/go"));

        cut.Find(".osirion-hero-buttons a").GetAttribute("href").ShouldBe("/go");
    }

    // Fails before the fix: the root element had no @attributes and no style, so unmatched attributes, Style and
    // Theme were dropped.
    [Fact]
    public void LandingPage_RootElement_ReceivesAttributesStyleAndTheme()
    {
        var cut = RenderStatic<LandingPage>(p => p
            .Add(c => c.HeroTitle, "Hello")
            .Add(c => c.Style, "min-height: 10rem")
            .Add(c => c.Theme, Osirion.Blazor.Components.ThemeMode.Dark)
            .AddUnmatched("data-test", "landing")
            .AddUnmatched("id", "start"));

        var root = cut.Find("div.osirion-landing-page");
        root.GetAttribute("style").ShouldBe("min-height: 10rem");
        root.GetAttribute("data-test").ShouldBe("landing");
        root.GetAttribute("id").ShouldBe("start");
        root.GetAttribute("data-theme").ShouldBe("dark");
    }

    [Fact]
    public void LandingPage_Sections_FollowTheBodyInOrderAndEndWithTheCallToAction()
    {
        var cut = RenderStatic<LandingPage>(p => p
            .Add(c => c.Content, Doc("launch", "<p>Body</p>"))
            .Add(c => c.ShowFeatures, true)
            .Add(c => c.FeatureItems, Features)
            .Add(c => c.ShowTestimonials, true)
            .Add(c => c.TestimonialItems, Testimonials)
            .Add(c => c.CtaTitle, "Ready?")
            .Add(c => c.CustomContent, "<p>Custom</p>"));

        var markup = cut.Markup;
        markup.IndexOf("Body", StringComparison.Ordinal).ShouldBeLessThan(markup.IndexOf("osirion-landing-features", StringComparison.Ordinal));
        markup.IndexOf("osirion-landing-features", StringComparison.Ordinal).ShouldBeLessThan(markup.IndexOf("osirion-landing-testimonials", StringComparison.Ordinal));
        markup.IndexOf("osirion-landing-testimonials", StringComparison.Ordinal).ShouldBeLessThan(markup.IndexOf("Custom", StringComparison.Ordinal));
        markup.IndexOf("Custom", StringComparison.Ordinal).ShouldBeLessThan(markup.IndexOf("osirion-landing-cta", StringComparison.Ordinal));
    }
}
