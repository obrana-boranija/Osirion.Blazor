using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Providers;

public class ProviderContractTests
{
    private static readonly AnalyticsScriptContext ConsentContext = new() { ConsentGateActive = true };

    private static ClarityProvider Clarity(ClarityOptions options) => new(Microsoft.Extensions.Options.Options.Create(options));

    private static GA4Provider GA4(GA4Options options) => new(Microsoft.Extensions.Options.Options.Create(options));

    private static MatomoProvider Matomo(MatomoOptions options) => new(Microsoft.Extensions.Options.Options.Create(options));

    private static YandexMetricaProvider Yandex(YandexMetricaOptions options) => new(Microsoft.Extensions.Options.Options.Create(options));

    [Fact]
    public void Providers_ProviderIds_AreStable()
    {
        Clarity(new ClarityOptions()).ProviderId.ShouldBe("clarity");
        GA4(new GA4Options()).ProviderId.ShouldBe("ga4");
        Matomo(new MatomoOptions()).ProviderId.ShouldBe("matomo");
        Yandex(new YandexMetricaOptions()).ProviderId.ShouldBe("yandex-metrica");
    }

    [Fact]
    public void Providers_NullOptions_Throw()
    {
        Should.Throw<ArgumentNullException>(() => new ClarityProvider(null!)).ParamName.ShouldBe("options");
        Should.Throw<ArgumentNullException>(() => new GA4Provider(null!)).ParamName.ShouldBe("options");
        Should.Throw<ArgumentNullException>(() => new MatomoProvider(null!)).ParamName.ShouldBe("options");
        Should.Throw<ArgumentNullException>(() => new YandexMetricaProvider(null!)).ParamName.ShouldBe("options");
    }

    [Fact]
    public async Task Providers_TrackCalls_CompleteWithoutWork()
    {
        IAnalyticsProvider[] providers =
        [
            Clarity(new ClarityOptions { SiteId = "abc" }),
            GA4(new GA4Options { MeasurementId = "G-TEST1234" }),
            Matomo(new MatomoOptions { SiteId = "1", TrackerUrl = "https://m.example.com/" }),
            Yandex(new YandexMetricaOptions { CounterId = "123" })
        ];

        foreach (var provider in providers)
        {
            var pageView = provider.TrackPageViewAsync("/page");
            var trackedEvent = provider.TrackEventAsync("category", "action", "label", 1);

            pageView.IsCompletedSuccessfully.ShouldBeTrue();
            trackedEvent.IsCompletedSuccessfully.ShouldBeTrue();
            await pageView;
            await trackedEvent;
        }
    }

    [Theory]
    [InlineData(true, "abc", "https://www.clarity.ms/tag/", true)]
    [InlineData(false, "abc", "https://www.clarity.ms/tag/", false)]
    [InlineData(true, " ", "https://www.clarity.ms/tag/", false)]
    [InlineData(true, "abc", "javascript:alert(1)", false)]
    public void ClarityProvider_ShouldRender_FollowsEnabledSiteIdAndTrackerUrl(bool enabled, string siteId, string trackerUrl, bool expected)
    {
        var provider = Clarity(new ClarityOptions { Enabled = enabled, SiteId = siteId, TrackerUrl = trackerUrl });

        provider.IsEnabled.ShouldBe(expected);
        provider.ShouldRender.ShouldBe(expected);
    }

    [Fact]
    public void ClarityProvider_GetScriptWithContext_WritesSiteIdAndTrackerUrl()
    {
        var provider = Clarity(new ClarityOptions { SiteId = "site-42", TrackerUrl = "https://clarity.example.com/tag/" });

        var script = provider.GetScript(ConsentContext);

        script.ShouldContain("\"site-42\"");
        script.ShouldContain("\"https://clarity.example.com/tag/\" + i");
        script.ShouldContain("\"clarity\", \"script\"");
    }

    [Fact]
    public void ClarityProvider_CreateOptionsCopy_IsIndependentOfConfiguredOptions()
    {
        var configured = new ClarityOptions { SiteId = "configured" };
        var provider = Clarity(configured);

        var copy = provider.CreateOptionsCopy();
        copy.SiteId = "changed";

        copy.ShouldNotBeSameAs(configured);
        configured.SiteId.ShouldBe("configured");
    }

    [Fact]
    public void ClarityProvider_NullArguments_Throw()
    {
        var provider = Clarity(new ClarityOptions { SiteId = "abc" });

        Should.Throw<ArgumentNullException>(() => provider.CanRender(null!));
        Should.Throw<ArgumentNullException>(() => provider.GetScript(null!, AnalyticsScriptContext.Default));
        Should.Throw<ArgumentNullException>(() => provider.GetScript(new ClarityOptions(), null!));
    }

    [Fact]
    public void GA4Provider_CookieAndPrivacyOptions_AreWrittenToConfig()
    {
        var provider = GA4(new GA4Options
        {
            MeasurementId = "G-TEST1234",
            CookieFlags = "secure",
            CookieDomain = "example.com",
            CookieExpires = 3600,
            RestrictDataProcessing = true
        });

        var script = provider.GetScript();

        script.ShouldContain("\"cookie_flags\":\"secure\"");
        script.ShouldContain("\"cookie_domain\":\"example.com\"");
        script.ShouldContain("\"cookie_expires\":3600");
        script.ShouldContain("\"restricted_data_processing\":true");
    }

    [Fact]
    public void GA4Provider_DefaultOptions_OmitOptionalConfigKeys()
    {
        var provider = GA4(new GA4Options { MeasurementId = "G-TEST1234", AnonymizeIp = false, CookieFlags = " ", CookieDomain = "" });

        var script = provider.GetScript();

        script.ShouldNotContain("anonymize_ip");
        script.ShouldNotContain("cookie_flags");
        script.ShouldNotContain("cookie_domain");
        script.ShouldNotContain("cookie_expires");
        script.ShouldNotContain("restricted_data_processing");
        script.ShouldContain("\"transport_type\":\"beacon\"");
    }

    [Fact]
    public void GA4Provider_ConfigParameters_OverrideBuiltInKeys()
    {
        var provider = GA4(new GA4Options
        {
            MeasurementId = "G-TEST1234",
            ConfigParameters = new Dictionary<string, object> { ["transport_type"] = "xhr", ["custom_key"] = 7 }
        });

        var script = provider.GetScript();

        script.ShouldContain("\"transport_type\":\"xhr\"");
        script.ShouldNotContain("\"transport_type\":\"beacon\"");
        script.ShouldContain("\"custom_key\":7");
    }

    [Fact]
    public void GA4Provider_DebugModeAndNoLinkAttribution_ShapeTheScript()
    {
        var provider = GA4(new GA4Options { MeasurementId = "G-TEST1234", DebugMode = true, LinkAttribution = false });

        var script = provider.GetScript(AnalyticsScriptContext.Default);

        script.ShouldContain("window.ga_debug = {debug_mode: true};");
        script.ShouldNotContain("gtag('require', 'linkid');");
    }

    [Theory]
    [InlineData(true, "G-TEST1234", true)]
    [InlineData(false, "G-TEST1234", false)]
    [InlineData(true, "UA-1234", false)]
    public void GA4Provider_ShouldRender_FollowsEnabledAndMeasurementId(bool enabled, string measurementId, bool expected)
    {
        var provider = GA4(new GA4Options { Enabled = enabled, MeasurementId = measurementId });

        provider.ShouldRender.ShouldBe(expected);
    }

    [Fact]
    public void GA4Provider_NullArguments_Throw()
    {
        var provider = GA4(new GA4Options { MeasurementId = "G-TEST1234" });

        Should.Throw<ArgumentNullException>(() => provider.CanRender(null!));
        Should.Throw<ArgumentNullException>(() => provider.GetScript(null!, AnalyticsScriptContext.Default));
        Should.Throw<ArgumentNullException>(() => provider.GetScript(new GA4Options(), null!));
    }

    [Fact]
    public void MatomoProvider_GetScriptWithContext_UsesConfiguredOptions()
    {
        var provider = Matomo(new MatomoOptions { SiteId = "17", TrackerUrl = "https://m.example.com/" });

        var script = provider.GetScript(ConsentContext);

        script.ShouldContain("_paq.push(['setSiteId', \"17\"]);");
        script.ShouldContain("var u = \"https://m.example.com/\";");
    }

    [Fact]
    public void YandexProvider_GetScriptWithContext_UsesConfiguredCounter()
    {
        var provider = Yandex(new YandexMetricaOptions { CounterId = "98765" });

        var script = provider.GetScript(ConsentContext);

        script.ShouldContain("98765");
    }

    [Fact]
    public void YandexProvider_UserParams_AreWrittenToInitOptions()
    {
        var provider = Yandex(new YandexMetricaOptions
        {
            CounterId = "98765",
            UserParams = new Dictionary<string, object> { ["UserID"] = 42 }
        });

        var script = provider.GetScript();

        script.ShouldContain("\"userParams\":{\"UserID\":42}");
    }

    [Fact]
    public void YandexProvider_EmptyParamDictionaries_AreOmitted()
    {
        var provider = Yandex(new YandexMetricaOptions
        {
            CounterId = "98765",
            Params = [],
            UserParams = []
        });

        var script = provider.GetScript();

        script.ShouldNotContain("\"params\"");
        script.ShouldNotContain("\"userParams\"");
    }
}
