using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Components;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Popups;

/// <summary>
/// The banner text, its optional buttons and the customization panel shown after the visitor chose to customize.
/// </summary>
public class OsirionCookieConsentContentTests : BunitContext
{
    private readonly DefaultHttpContext _httpContext = new();
    private readonly IHttpContextAccessor _httpContextAccessor = Substitute.For<IHttpContextAccessor>();

    public OsirionCookieConsentContentTests()
    {
        _httpContextAccessor.HttpContext.Returns(_httpContext);

        Services.AddSingleton(Options.Create(new ThemingOptions()));
        Services.AddSingleton(_httpContextAccessor);
    }

    [Fact]
    public void OsirionCookieConsent_Defaults_RendersTheDefaultTextAndAllButtons()
    {
        var cut = RenderOnPage(parameters => { });

        cut.Find("form").GetAttribute("action").ShouldBe("/api/cookie-consent");
        cut.Find(".osirion-cookie-consent-title").TextContent.ShouldBe("Cookie Consent");
        cut.Find(".osirion-cookie-consent-description").TextContent.ShouldContain("We use cookies");
        cut.FindAll("button[name=consent]").Select(button => button.GetAttribute("value"))
            .ShouldBe(["declined", "customize", "accepted"]);
        cut.FindAll(".osirion-cookie-consent-link").ShouldBeEmpty();
        cut.FindAll(".osirion-cookie-consent-icon").ShouldBeEmpty();
        cut.FindAll(".osirion-cookie-consent-customize").ShouldBeEmpty();
        cut.Find(".osirion-cookie-consent").ClassName.ShouldBe("osirion-cookie-consent osirion-cookie-consent-bottom");
    }

    [Fact]
    public void OsirionCookieConsent_CustomText_RendersItOnTheBannerAndButtons()
    {
        var cut = RenderOnPage(parameters => parameters
            .Add(p => p.Title, "Cookies")
            .Add(p => p.Message, "We bake cookies.")
            .Add(p => p.AcceptButtonText, "Yes")
            .Add(p => p.DeclineButtonText, "No")
            .Add(p => p.CustomizeButtonText, "Choose")
            .Add(p => p.ConsentEndpoint, "/consent"));

        cut.Find("form").GetAttribute("action").ShouldBe("/consent");
        cut.Find(".osirion-cookie-consent-title").TextContent.ShouldBe("Cookies");
        cut.Find(".osirion-cookie-consent-description").TextContent.Trim().ShouldBe("We bake cookies.");
        cut.Find("button[value=accepted]").TextContent.Trim().ShouldBe("Yes");
        cut.Find("button[value=declined]").TextContent.Trim().ShouldBe("No");
        cut.Find("button[value=customize]").TextContent.Trim().ShouldBe("Choose");
    }

    [Fact]
    public void OsirionCookieConsent_PolicyLink_RendersTheLinkWithItsText()
    {
        var cut = RenderOnPage(parameters => parameters
            .Add(p => p.PolicyLink, "/privacy")
            .Add(p => p.PolicyLinkText, "Privacy policy"));

        var link = cut.Find("a.osirion-cookie-consent-link");
        link.GetAttribute("href").ShouldBe("/privacy");
        link.TextContent.ShouldBe("Privacy policy");
    }

    [Fact]
    public void OsirionCookieConsent_Icon_RendersInsideTheIconContainer()
    {
        var cut = RenderOnPage(parameters => parameters
            .Add(p => p.Icon, (RenderFragment)(builder => builder.AddMarkupContent(0, "<svg class=\"cookie-icon\"></svg>"))));

        cut.Find(".osirion-cookie-consent-icon svg").ClassName.ShouldBe("cookie-icon");
    }

    [Fact]
    public void OsirionCookieConsent_OptionalButtonsHidden_RendersTheAcceptButtonOnly()
    {
        var cut = RenderOnPage(parameters => parameters
            .Add(p => p.ShowDeclineButton, false)
            .Add(p => p.ShowCustomizeButton, false));

        cut.FindAll("button[name=consent]").Select(button => button.GetAttribute("value")).ShouldBe(["accepted"]);
    }

    [Fact]
    public void OsirionCookieConsent_PositionAndClass_AreAddedToTheBannerClasses()
    {
        var cut = RenderOnPage(parameters => parameters
            .Add(p => p.Position, "top")
            .Add(p => p.Class, "my-banner"));

        cut.Find(".osirion-cookie-consent").ClassName.ShouldBe("osirion-cookie-consent osirion-cookie-consent-top my-banner");
    }

    [Fact]
    public void OsirionCookieConsent_CustomizeQuery_RendersTheDefaultCategories()
    {
        _httpContext.Request.QueryString = new QueryString("?customize-cookies=true");

        var cut = RenderOnPage(parameters => { });

        cut.Find(".osirion-cookie-consent").ClassList.ShouldContain("osirion-cookie-consent-customizing");
        cut.Find(".osirion-cookie-consent-customize-title").TextContent.ShouldBe("Cookie Preferences");
        cut.FindAll(".osirion-cookie-consent-category input").Select(input => input.GetAttribute("name"))
            .ShouldBe(["category_necessary", "category_analytics", "category_marketing", "category_preferences"]);
        cut.Find("button[value=save-preferences]").TextContent.Trim().ShouldBe("Save Preferences");
    }

    [Fact]
    public void OsirionCookieConsent_CustomizeQuery_RequiredCategoryIsCheckedDisabledAndMarked()
    {
        _httpContext.Request.QueryString = new QueryString("?customize-cookies=true");

        var cut = RenderOnPage(parameters => { });

        var necessary = cut.Find("input[name=category_necessary]");
        necessary.HasAttribute("checked").ShouldBeTrue();
        necessary.HasAttribute("disabled").ShouldBeTrue();
        var analytics = cut.Find("input[name=category_analytics]");
        analytics.HasAttribute("checked").ShouldBeFalse();
        analytics.HasAttribute("disabled").ShouldBeFalse();
        cut.FindAll(".osirion-cookie-consent-required").Count.ShouldBe(1);
    }

    [Fact]
    public void OsirionCookieConsent_CustomCategories_RendersThemWithTheirState()
    {
        _httpContext.Request.QueryString = new QueryString("?customize-cookies=true");
        CookieCategory[] categories =
        [
            new() { Id = "stats", Name = "Statistics", Description = "Counts visits.", IsEnabled = true }
        ];

        var cut = RenderOnPage(parameters => parameters
            .Add(p => p.Categories, categories)
            .Add(p => p.CustomizePanelTitle, "Choose")
            .Add(p => p.SavePreferencesButtonText, "Save"));

        cut.Find(".osirion-cookie-consent-customize-title").TextContent.ShouldBe("Choose");
        var input = cut.Find(".osirion-cookie-consent-category input");
        input.GetAttribute("name").ShouldBe("category_stats");
        input.HasAttribute("checked").ShouldBeTrue();
        cut.Find(".osirion-cookie-consent-category-name").TextContent.Trim().ShouldBe("Statistics");
        cut.Find(".osirion-cookie-consent-category-description").TextContent.ShouldBe("Counts visits.");
        cut.Find("button[value=save-preferences]").TextContent.Trim().ShouldBe("Save");
    }

    [Fact]
    public void OsirionCookieConsent_CustomizationPanelDisabled_IgnoresTheCustomizeQuery()
    {
        _httpContext.Request.QueryString = new QueryString("?customize-cookies=true");

        var cut = RenderOnPage(parameters => parameters.Add(p => p.ShowCustomizationPanel, false));

        cut.FindAll(".osirion-cookie-consent-customize").ShouldBeEmpty();
    }

    [Fact]
    public void OsirionCookieConsent_NoHttpContext_RendersNothing()
    {
        _httpContextAccessor.HttpContext.Returns((HttpContext?)null);

        var cut = RenderOnPage(parameters => { });

        cut.Markup.Trim().ShouldBeEmpty();
    }

    private IRenderedComponent<OsirionCookieConsent> RenderOnPage(
        Action<ComponentParameterCollectionBuilder<OsirionCookieConsent>> parameterBuilder)
    {
        SetRendererInfo(new RendererInfo("Static", false));
        Services.GetRequiredService<BunitNavigationManager>().NavigateTo("http://localhost/page");
        return Render(parameterBuilder);
    }
}
