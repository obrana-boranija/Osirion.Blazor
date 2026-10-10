using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Analytics.Extensions;
using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Internal;

public class AnalyticsRegistrationBehaviourTests
{
    private static ServiceProvider Build(Action<IAnalyticsBuilder> configure)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOsirionAnalytics(configure);
        return services.BuildServiceProvider();
    }

    private static ServiceProvider Build(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOsirionAnalytics(configuration);
        return services.BuildServiceProvider();
    }

    private static IConfiguration Config(params (string Key, string Value)[] values) =>
        BuildConfiguration(values);

    private static IConfiguration BuildConfiguration((string Key, string Value)[] values)
    {
        var configuration = new ConfigurationManager();
        foreach (var (key, value) in values)
        {
            configuration[key] = value;
        }

        return configuration;
    }

    [Fact]
    public void RepeatedAddCalls_RegisterEachProviderOnce()
    {
        using var provider = Build(a =>
        {
            a.AddGA4(o => o.MeasurementId = "G-AAAA1111");
            a.AddGA4(o => o.MeasurementId = "G-BBBB2222");
            a.AddYandexMetrica(o => o.CounterId = "1");
            a.AddYandexMetrica(o => o.CounterId = "2");
            a.AddClarity(o => o.SiteId = "c");
            a.AddMatomo(o => { o.SiteId = "1"; o.TrackerUrl = "https://m.example/"; });
            a.AddMatomo(o => { o.SiteId = "1"; o.TrackerUrl = "https://m.example/"; });
        });

        provider.GetServices<IAnalyticsProvider>().Count().ShouldBe(4);
    }

    [Fact]
    public void InterfaceRegistration_ReturnsTheSameInstanceAsTheConcreteSingleton()
    {
        using var provider = Build(a =>
        {
            a.AddGA4(o => o.MeasurementId = "G-AAAA1111");
            a.AddYandexMetrica(o => o.CounterId = "1");
            a.AddClarity(o => o.SiteId = "c");
            a.AddMatomo(o => { o.SiteId = "1"; o.TrackerUrl = "https://m.example/"; });
        });

        var exposed = provider.GetServices<IAnalyticsProvider>().ToList();

        exposed.Single(p => p is GA4Provider).ShouldBeSameAs(provider.GetRequiredService<GA4Provider>());
        exposed.Single(p => p is YandexMetricaProvider).ShouldBeSameAs(provider.GetRequiredService<YandexMetricaProvider>());
        exposed.Single(p => p is ClarityProvider).ShouldBeSameAs(provider.GetRequiredService<ClarityProvider>());
        exposed.Single(p => p is MatomoProvider).ShouldBeSameAs(provider.GetRequiredService<MatomoProvider>());
    }

    [Fact]
    public void YandexSection_NewKeyIsRead()
    {
        using var provider = Build(Config(("Osirion:Analytics:YandexMetrica:CounterId", "4242")));

        provider.GetRequiredService<IOptions<YandexMetricaOptions>>().Value.CounterId.ShouldBe("4242");
    }

    [Fact]
    public void YandexSection_LegacyKeyIsStillReadAsFallback()
    {
        using var provider = Build(Config(("YandexMetrica:CounterId", "777")));

        provider.GetRequiredService<IOptions<YandexMetricaOptions>>().Value.CounterId.ShouldBe("777");
    }

    [Fact]
    public void YandexSection_NewKeyWinsOverLegacyKey()
    {
        using var provider = Build(Config(
            ("Osirion:Analytics:YandexMetrica:CounterId", "1"),
            ("YandexMetrica:CounterId", "2")));

        provider.GetRequiredService<IOptions<YandexMetricaOptions>>().Value.CounterId.ShouldBe("1");
    }

    [Fact]
    public void InvalidGa4MeasurementId_FailsOptionsValidation()
    {
        using var provider = Build(a => a.AddGA4(o => o.MeasurementId = "not-an-id"));

        var exception = Should.Throw<OptionsValidationException>(() => provider.GetRequiredService<IOptions<GA4Options>>().Value);

        exception.Message.ShouldContain("Osirion:Analytics:GA4:MeasurementId");
    }

    [Fact]
    public void InvalidYandexCounterId_FailsOptionsValidation()
    {
        using var provider = Build(a => a.AddYandexMetrica(o => o.CounterId = "12;alert(1)"));

        var exception = Should.Throw<OptionsValidationException>(() => provider.GetRequiredService<IOptions<YandexMetricaOptions>>().Value);

        exception.Message.ShouldContain("CounterId");
    }

    [Fact]
    public void InvalidYandexAlternativeCdn_FailsOptionsValidation()
    {
        using var provider = Build(a => a.AddYandexMetrica(o => { o.CounterId = "1"; o.AlternativeCdn = "javascript:alert(1)"; }));

        var exception = Should.Throw<OptionsValidationException>(() => provider.GetRequiredService<IOptions<YandexMetricaOptions>>().Value);

        exception.Message.ShouldContain("AlternativeCdn");
    }

    [Fact]
    public void InvalidMatomoTrackerUrl_FailsOptionsValidation()
    {
        using var provider = Build(a => a.AddMatomo(o => { o.SiteId = "1"; o.TrackerUrl = "javascript:alert(1)"; }));

        var exception = Should.Throw<OptionsValidationException>(() => provider.GetRequiredService<IOptions<MatomoOptions>>().Value);

        exception.Message.ShouldContain("Matomo:TrackerUrl");
    }

    [Fact]
    public void InvalidClarityTrackerUrl_FailsOptionsValidation()
    {
        using var provider = Build(a => a.AddClarity(o => { o.SiteId = "x"; o.TrackerUrl = "ftp://x/"; }));

        var exception = Should.Throw<OptionsValidationException>(() => provider.GetRequiredService<IOptions<ClarityOptions>>().Value);

        exception.Message.ShouldContain("Clarity:TrackerUrl");
    }

    [Fact]
    public void DisabledProvider_WithBadValue_DoesNotFailValidation()
    {
        using var provider = Build(a => a.AddGA4(o => { o.Enabled = false; o.MeasurementId = "bad"; }));

        Should.NotThrow(() => provider.GetRequiredService<IOptions<GA4Options>>().Value);
    }

    [Fact]
    public void EnhancedNavigationModule_ListensForEnhancedLoad()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "src",
            "Osirion.Blazor.Analytics", "wwwroot", "js", "osirion-analytics.js");

        var source = File.ReadAllText(path);

        source.ShouldContain("addEventListener('enhancedload'");
        source.ShouldContain("'trackPageView'");
        source.ShouldContain("'hit'");
    }
}
