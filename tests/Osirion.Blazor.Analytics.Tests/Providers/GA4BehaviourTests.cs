using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Providers;

public class GA4BehaviourTests
{
    private static GA4Provider Create(GA4Options options) =>
        new(Microsoft.Extensions.Options.Options.Create(options));

    [Fact]
    public void GetScript_SendPageViewFalse_WritesFalse()
    {
        var script = Create(new GA4Options { MeasurementId = "G-TEST1234", SendPageView = false }).GetScript();

        script.ShouldContain("\"send_page_view\":false");
        script.ShouldNotContain("\"send_page_view\":true");
    }

    [Fact]
    public void GetScript_OutboundLinkHandler_DoesNotBlockNavigation()
    {
        var script = Create(new GA4Options { MeasurementId = "G-TEST1234", TrackOutboundLinks = true }).GetScript();

        script.ShouldNotContain("preventDefault");
        script.ShouldNotContain("document.location");
        script.ShouldNotContain("event_callback");
        script.ShouldContain("'transport_type': 'beacon'");
        script.ShouldContain("e.button !== 0");
        script.ShouldContain("e.metaKey || e.ctrlKey || e.shiftKey || e.altKey");
        script.ShouldContain("target !== '_self'");
        script.ShouldContain("el.protocol !== 'http:' && el.protocol !== 'https:'");
    }

    [Fact]
    public void GetScript_OutboundLinksOff_OmitsHandler()
    {
        var script = Create(new GA4Options { MeasurementId = "G-TEST1234", TrackOutboundLinks = false }).GetScript();

        script.ShouldNotContain("addEventListener('click'");
    }

    [Fact]
    public void GetScript_ConsentGateActive_DeclaresDeniedDefaultsBeforeConfig()
    {
        var script = Create(new GA4Options { MeasurementId = "G-TEST1234" })
            .GetScript(new AnalyticsScriptContext { ConsentGateActive = true });

        var defaults = script.IndexOf("gtag('consent', 'default'", StringComparison.Ordinal);
        var config = script.IndexOf("gtag('config'", StringComparison.Ordinal);
        defaults.ShouldBeGreaterThan(-1);
        defaults.ShouldBeLessThan(config);
        script.ShouldContain("'ad_storage': 'denied'");
        script.ShouldContain("'analytics_storage': 'denied'");
        script.ShouldContain("gtag('consent', 'update', {'analytics_storage': 'granted'})");
    }

    [Fact]
    public void GetScript_NoConsentGate_OmitsConsentMode()
    {
        Create(new GA4Options { MeasurementId = "G-TEST1234" }).GetScript().ShouldNotContain("'consent'");
    }
}
