using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Core.Handlers;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Extensions;

public class CookieConsentPipelineTests
{
    [Fact]
    public async Task UseOsirionCookieConsent_ConsentCookie_ExposesTheConsentToLaterMiddleware()
    {
        object? seenConsent = null;
        var app = new ApplicationBuilder(new ServiceCollection().BuildServiceProvider());
        app.UseOsirionCookieConsent();
        app.Run(context =>
        {
            seenConsent = context.Items["CookieConsent"];
            return Task.CompletedTask;
        });
        var context = new DefaultHttpContext();
        context.Request.Headers.Cookie = "osirion_cookie_consent=" + Uri.EscapeDataString("{\"ConsentType\":\"declined\"}");

        await app.Build()(context);

        seenConsent.ShouldBeOfType<CookieConsentData>().ConsentType.ShouldBe("declined");
    }

    [Fact]
    public async Task UseOsirionCookieConsent_WithoutConsentCookie_StoresNoConsent()
    {
        var app = new ApplicationBuilder(new ServiceCollection().BuildServiceProvider());
        app.UseOsirionCookieConsent();
        var context = new DefaultHttpContext();

        await app.Build()(context);

        context.Items.ShouldContainKey("CookieConsent");
        context.Items["CookieConsent"].ShouldBeNull();
    }
}
