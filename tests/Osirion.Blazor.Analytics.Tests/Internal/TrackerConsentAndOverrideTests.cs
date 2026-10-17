using System.Text.Json;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Analytics.Components;
using Osirion.Blazor.Analytics.Extensions;
using Osirion.Blazor.Core.Handlers;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Internal;

public class TrackerConsentAndOverrideTests : BunitContext
{
    private void Register(Action<IAnalyticsBuilder> configure)
    {
        Services.AddOsirionAnalytics(configure);
        SetRendererInfo(new RendererInfo("Server", false));
    }

    private static DefaultHttpContext HttpContextWithConsent(bool? analytics)
    {
        var context = new DefaultHttpContext();
        if (analytics is not null)
        {
            var data = new CookieConsentData
            {
                Version = "1.0",
                ConsentType = "save-preferences",
                Categories = new Dictionary<string, bool> { ["necessary"] = true, ["analytics"] = analytics.Value }
            };
            context.Request.Headers.Cookie = "osirion_cookie_consent=" + Uri.EscapeDataString(JsonSerializer.Serialize(data));
        }

        return context;
    }

    private static void ConfigureEverything(IAnalyticsBuilder analytics)
    {
        analytics.AddGA4(o => o.MeasurementId = "G-TEST1234");
        analytics.AddMatomo(o => { o.SiteId = "7"; o.TrackerUrl = "https://m.example/"; });
        analytics.AddClarity(o => o.SiteId = "clarity1");
        analytics.AddYandexMetrica(o => o.CounterId = "99");
    }

    [Fact]
    public void ConsentGateOff_RendersWithoutAnyHttpContext()
    {
        Register(ConfigureEverything);

        Render<GA4Tracker>().Markup.ShouldContain("G-TEST1234");
    }

    [Fact]
    public void ConsentGateOn_WithoutCookie_RendersNothing()
    {
        Register(a => { ConfigureEverything(a); a.RequireConsent(); });
        var context = HttpContextWithConsent(null);

        Render<GA4Tracker>(p => p.AddCascadingValue((HttpContext)context)).Markup.ShouldNotContain("<script");
        Render<MatomoTracker>(p => p.AddCascadingValue((HttpContext)context)).Markup.ShouldNotContain("<script");
        Render<ClarityTracker>(p => p.AddCascadingValue((HttpContext)context)).Markup.ShouldNotContain("<script");
        Render<YandexMetricaTracker>(p => p.AddCascadingValue((HttpContext)context)).Markup.ShouldNotContain("<script");
    }

    [Fact]
    public void ConsentGateOn_AnalyticsDeclined_RendersNothing()
    {
        Register(a => { ConfigureEverything(a); a.RequireConsent(); });

        Render<MatomoTracker>(p => p.AddCascadingValue((HttpContext)HttpContextWithConsent(false)))
            .Markup.ShouldNotContain("<script");
    }

    [Fact]
    public void ConsentGateOn_WithoutHttpContext_FailsClosed()
    {
        Register(a => { ConfigureEverything(a); a.RequireConsent(); });

        Render<GA4Tracker>().Markup.ShouldNotContain("<script");
    }

    [Fact]
    public void ConsentGateOn_AnalyticsAccepted_RendersAndDeclaresConsentMode()
    {
        Register(a => { ConfigureEverything(a); a.RequireConsent(); });
        var context = HttpContextWithConsent(true);

        var ga4 = Render<GA4Tracker>(p => p.AddCascadingValue((HttpContext)context)).Markup;
        ga4.ShouldContain("gtag('consent', 'default'");
        ga4.ShouldContain("G-TEST1234");
        Render<MatomoTracker>(p => p.AddCascadingValue((HttpContext)context)).Markup.ShouldContain("matomo.js");
    }

    [Fact]
    public void ConsentGateOn_MatomoRequireConsentIsNotEmittedOnceVisitorConsented()
    {
        Register(a =>
        {
            a.AddMatomo(o => { o.SiteId = "7"; o.TrackerUrl = "https://m.example/"; o.RequireConsent = true; });
            a.RequireConsent();
        });

        Render<MatomoTracker>(p => p.AddCascadingValue((HttpContext)HttpContextWithConsent(true)))
            .Markup.ShouldNotContain("requireConsent");
    }

    [Fact]
    public void ConsentGateOff_MatomoRequireConsentIsStillEmitted()
    {
        Register(a => a.AddMatomo(o => { o.SiteId = "7"; o.TrackerUrl = "https://m.example/"; o.RequireConsent = true; }));

        Render<MatomoTracker>().Markup.ShouldContain("requireConsent");
    }

    [Fact]
    public void ConsentGateOn_ViaConfiguration_IsRead()
    {
        var configuration = new Microsoft.Extensions.Configuration.ConfigurationManager();
        configuration["Osirion:Analytics:Consent:RequireConsent"] = "true";
        configuration["Osirion:Analytics:GA4:MeasurementId"] = "G-TEST1234";
        Services.AddOsirionAnalytics(configuration);
        SetRendererInfo(new RendererInfo("Server", false));

        Render<GA4Tracker>().Markup.ShouldNotContain("<script");
    }

    [Fact]
    public void Ga4_MeasurementIdAndDebugMode_OverrideOptionsForThatRenderOnly()
    {
        Register(a => a.AddGA4(o => { o.MeasurementId = "G-ORIGINAL"; o.DebugMode = false; }));

        var overridden = Render<GA4Tracker>(p => p.Add(x => x.MeasurementId, "G-CUSTOM").Add(x => x.DebugMode, true)).Markup;
        var plain = Render<GA4Tracker>().Markup;

        overridden.ShouldContain("G-CUSTOM");
        overridden.ShouldContain("\"debug_mode\":true");
        plain.ShouldContain("G-ORIGINAL");
        plain.ShouldNotContain("debug_mode");
    }

    [Fact]
    public void EnabledParameter_CanEnableAndDisable()
    {
        Register(a =>
        {
            a.AddClarity(o => { o.SiteId = "off"; o.Enabled = false; });
            a.AddMatomo(o => { o.SiteId = "7"; o.TrackerUrl = "https://m.example/"; });
        });

        Render<ClarityTracker>(p => p.Add(x => x.Enabled, true)).Markup.ShouldContain("\"off\"");
        Render<MatomoTracker>(p => p.Add(x => x.Enabled, false)).Markup.ShouldNotContain("<script");
    }

    [Fact]
    public void Yandex_Parameters_OverrideOptions()
    {
        Register(a => a.AddYandexMetrica(o =>
        {
            o.CounterId = "1";
            o.WebVisor = false;
            o.ClickMap = true;
            o.TrackLinks = true;
            o.AccurateTrackBounce = true;
        }));

        var markup = Render<YandexMetricaTracker>(p => p
            .Add(x => x.CounterId, "2")
            .Add(x => x.WebVisor, true)
            .Add(x => x.ClickMap, false)
            .Add(x => x.TrackLinks, false)
            .Add(x => x.AccurateTrackBounce, false)).Markup;

        markup.ShouldContain("ym(2,");
        markup.ShouldContain("\"webvisor\":true");
        markup.ShouldNotContain("clickmap");
        markup.ShouldNotContain("trackLinks");
        markup.ShouldNotContain("accurateTrackBounce");
    }

    [Fact]
    public void MatomoAndYandex_EmitEnhancedNavigationModuleTag()
    {
        Register(ConfigureEverything);

        var matomo = Render<MatomoTracker>().Markup;
        var yandex = Render<YandexMetricaTracker>().Markup;

        matomo.ShouldContain("src=\"/_content/Osirion.Blazor.Analytics/js/osirion-analytics.js\"");
        matomo.ShouldContain("data-osirion-analytics=\"matomo\"");
        yandex.ShouldContain("data-osirion-analytics=\"yandex\"");
        yandex.ShouldContain("data-counter=\"99\"");
    }

    [Fact]
    public void AutoTrackPageViewsOff_OmitsEnhancedNavigationModuleTag()
    {
        Register(a => a.AddMatomo(o => { o.SiteId = "7"; o.TrackerUrl = "https://m.example/"; o.AutoTrackPageViews = false; }));

        Render<MatomoTracker>().Markup.ShouldNotContain("osirion-analytics.js");
    }
}
