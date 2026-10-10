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
