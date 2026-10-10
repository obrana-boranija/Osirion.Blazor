using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Components;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Popups;

public class OsirionCookieConsentTests : BunitContext
{
    private const string TokenFieldName = "__RequestVerificationToken";

    private readonly DefaultHttpContext _httpContext = new();

    public OsirionCookieConsentTests()
    {
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(_httpContext);

        Services.AddSingleton(Options.Create(new ThemingOptions()));
        Services.AddSingleton(httpContextAccessor);
    }

    [Fact]
    public void OsirionCookieConsent_ReturnUrl_IsTheLocalPathAndQuery()
    {
        AddAntiforgeryStateProvider();

        var component = RenderOnPage("http://localhost/products/access?a=1");

        component.Find("input[name=returnUrl]").GetAttribute("value").ShouldBe("/products/access?a=1");
    }

    [Fact]
    public void OsirionCookieConsent_Form_ContainsTheAntiforgeryToken()
    {
        AddAntiforgeryStateProvider();

        var component = RenderOnPage("http://localhost/products/access?a=1");

        component.Find($"form input[type=hidden][name={TokenFieldName}]").GetAttribute("value").ShouldBe("test-token");
    }

    [Fact]
    public void OsirionCookieConsent_WithoutAntiforgeryStateProvider_StillRendersTheForm()
    {
        var component = RenderOnPage("http://localhost/products/access?a=1");

        component.FindAll("form").Count.ShouldBe(1);
        component.FindAll($"input[name={TokenFieldName}]").ShouldBeEmpty();
    }

    [Fact]
    public void OsirionCookieConsent_ConsentCookiePresent_RendersNothing()
    {
        _httpContext.Request.Headers.Cookie = "osirion_cookie_consent=accepted";

        var component = RenderOnPage("http://localhost/products/access?a=1");

        component.Markup.Trim().ShouldBeEmpty();
    }

    // Fails before the fix: the endpoint hard-coded 365 days and the banner never sent its ConsentExpiryDays.
    [Fact]
    public void OsirionCookieConsent_ConsentExpiryDays_IsPostedWithTheForm()
    {
        AddAntiforgeryStateProvider();
        SetRendererInfo(new RendererInfo("Static", false));
        Services.GetRequiredService<BunitNavigationManager>().NavigateTo("http://localhost/");

        var component = Render<OsirionCookieConsent>(parameters => parameters.Add(p => p.ConsentExpiryDays, 90));

        component.Find("input[name=expiryDays]").GetAttribute("value").ShouldBe("90");
    }

    // Fails before the fix: the unmatched attributes were applied to the form and to the inner banner, so an id repeated.
    [Fact]
    public void OsirionCookieConsent_UnmatchedAttributes_AreAppliedOnce()
    {
        AddAntiforgeryStateProvider();
        SetRendererInfo(new RendererInfo("Static", false));
        Services.GetRequiredService<BunitNavigationManager>().NavigateTo("http://localhost/");

        var component = Render<OsirionCookieConsent>(parameters => parameters.AddUnmatched("id", "consent-banner"));

        component.FindAll("#consent-banner").Count.ShouldBe(1);
    }

    // Fails before the fix: the form carried the actions class and the buttons sat beside the message, so the
    // action layout and the mobile rules never reached them and wide framework buttons wrapped under the text.
    [Fact]
    public void OsirionCookieConsent_ConsentButtons_AreInsideTheActionsContainer()
    {
        var component = RenderOnPage("http://localhost/");

        component.FindAll(".osirion-cookie-consent-actions > button[name=consent]").Count.ShouldBe(3);
        component.Find("form").ClassList.ShouldNotContain("osirion-cookie-consent-actions");
    }

    // Fails before the fix: resolving the component threw because no IHttpContextAccessor is registered under WebAssembly.
    [Fact]
    public void OsirionCookieConsent_WithoutHttpContextAccessor_RendersNothingAndDoesNotThrow()
    {
        using var context = new BunitContext();
        context.Services.AddSingleton(Options.Create(new ThemingOptions()));
        context.SetRendererInfo(new RendererInfo("WebAssembly", true));

        var component = context.Render<OsirionCookieConsent>();

        component.Markup.Trim().ShouldBeEmpty();
    }

    private IRenderedComponent<OsirionCookieConsent> RenderOnPage(string uri)
    {
        // The banner is rendered with static server-side rendering
        SetRendererInfo(new RendererInfo("Static", false));
        Services.GetRequiredService<BunitNavigationManager>().NavigateTo(uri);
        return Render<OsirionCookieConsent>();
    }

    private void AddAntiforgeryStateProvider()
    {
        var antiforgeryStateProvider = Substitute.For<AntiforgeryStateProvider>();
        antiforgeryStateProvider.GetAntiforgeryToken().Returns(new AntiforgeryRequestToken("test-token", TokenFieldName));
        Services.AddSingleton(antiforgeryStateProvider);
    }
}
