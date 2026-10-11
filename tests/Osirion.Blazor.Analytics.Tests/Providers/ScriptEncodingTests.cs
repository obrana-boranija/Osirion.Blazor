using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Providers;

public class ScriptEncodingTests
{
    private const string Hostile = "x</script><script>alert(1)</script>\"';";

    private static Microsoft.Extensions.Options.IOptions<T> Wrap<T>(T value) where T : class =>
        Microsoft.Extensions.Options.Options.Create(value);

    [Fact]
    public void Clarity_GetScript_EncodesSiteIdAndTrackerUrl()
    {
        var provider = new ClarityProvider(Wrap(new ClarityOptions { SiteId = Hostile, TrackerUrl = "https://t.example/tag/?a=1&b=<2>" }));

        var script = provider.GetScript();

        script.ShouldNotContain("</script><script>");
        script.ShouldNotContain("alert(1)</script>");
        script.ShouldContain("\\u003C/script\\u003E");
        script.ShouldContain("\\u0026b=\\u003C2\\u003E");
    }

    [Fact]
    public void Matomo_GetScript_EncodesSiteIdAndTrackerUrl()
    {
        var provider = new MatomoProvider(Wrap(new MatomoOptions { SiteId = Hostile, TrackerUrl = "https://m.example/'+alert(1)+'/" }));

        var script = provider.GetScript();

        script.ShouldNotContain("</script><script>");
        script.ShouldNotContain("'+alert(1)+'");
        script.ShouldContain("\\u0027\\u002Balert(1)\\u002B\\u0027");
    }

    [Fact]
    public void GA4_InvalidMeasurementId_IsNotRendered()
    {
        var provider = new GA4Provider(Wrap(new GA4Options { MeasurementId = "G-1');alert(1);//" }));

        provider.ShouldRender.ShouldBeFalse();
    }

    [Fact]
    public void GA4_GetScript_EncodesMeasurementIdInAttributeAndScript()
    {
        // The overload takes options as given, so this proves the encoding itself, independent of validation.
        var options = new GA4Options { MeasurementId = "G-AB\"><x'" };
        var provider = new GA4Provider(Wrap(options));

        var script = provider.GetScript(options, AnalyticsScriptContext.Default);

        script.ShouldNotContain("\"><x");
        script.ShouldContain("id=G-AB%22%3E%3Cx%27");
        script.ShouldContain("\"G-AB\\u0022\\u003E\\u003Cx\\u0027\"");
    }

    [Fact]
    public void Yandex_GetScript_EmitsNumericCounterBare()
    {
        var provider = new YandexMetricaProvider(Wrap(new YandexMetricaOptions { CounterId = "12345" }));

        provider.GetScript().ShouldContain("ym(12345, \"init\"");
    }

    [Theory]
    [InlineData("1);alert(1);//")]
    [InlineData("12 34")]
    [InlineData("abc")]
    public void Yandex_NonNumericCounterId_IsNotRendered(string counterId)
    {
        var provider = new YandexMetricaProvider(Wrap(new YandexMetricaOptions { CounterId = counterId }));

        provider.ShouldRender.ShouldBeFalse();
    }

    [Fact]
    public void Yandex_GetScript_EncodesAlternativeCdnAndFallsBackWhenNotHttp()
    {
        var withCdn = new YandexMetricaProvider(Wrap(new YandexMetricaOptions { CounterId = "1", AlternativeCdn = "https://cdn.example/t.js?a=<b>" }));
        var withBadCdn = new YandexMetricaProvider(Wrap(new YandexMetricaOptions { CounterId = "1", AlternativeCdn = "javascript:alert(1)" }));

        withCdn.GetScript().ShouldContain("\"https://cdn.example/t.js?a=\\u003Cb\\u003E\"");
        var fallback = withBadCdn.GetScript();
        fallback.ShouldNotContain("javascript:");
        fallback.ShouldContain("https://mc.yandex.ru/metrika/tag.js");
    }

    [Fact]
    public void Matomo_TrackerUrlThatIsNotHttp_IsNotRendered()
    {
        var provider = new MatomoProvider(Wrap(new MatomoOptions { SiteId = "1", TrackerUrl = "javascript:alert(1)//" }));

        provider.ShouldRender.ShouldBeFalse();
    }

    [Fact]
    public void Matomo_ProtocolRelativeTrackerUrl_IsStillRendered()
    {
        var provider = new MatomoProvider(Wrap(new MatomoOptions { SiteId = "1", TrackerUrl = "//analytics.example.com/" }));

        provider.ShouldRender.ShouldBeTrue();
    }
}
