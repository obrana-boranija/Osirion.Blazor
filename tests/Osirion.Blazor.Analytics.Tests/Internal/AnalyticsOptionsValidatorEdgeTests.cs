using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Analytics.Extensions;
using Osirion.Blazor.Analytics.Options;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Internal;

public class AnalyticsOptionsValidatorEdgeTests
{
    private static ServiceProvider Build(Action<IAnalyticsBuilder> configure)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOsirionAnalytics(configure);
        return services.BuildServiceProvider();
    }

    [Fact]
    public void MatomoValidator_DisabledWithBadTrackerUrl_Passes()
    {
        using var provider = Build(a => a.AddMatomo(o => { o.Enabled = false; o.TrackerUrl = "javascript:alert(1)"; }));

        var options = provider.GetRequiredService<IOptions<MatomoOptions>>().Value;

        options.TrackerUrl.ShouldBe("javascript:alert(1)");
    }

    [Fact]
    public void MatomoValidator_EnabledWithoutTrackerUrl_Passes()
    {
        using var provider = Build(a => a.AddMatomo(o => { o.SiteId = "1"; o.TrackerUrl = " "; }));

        var options = provider.GetRequiredService<IOptions<MatomoOptions>>().Value;

        options.SiteId.ShouldBe("1");
    }

    [Fact]
    public void MatomoValidator_ProtocolRelativeTrackerUrl_Passes()
    {
        using var provider = Build(a => a.AddMatomo(o => { o.SiteId = "1"; o.TrackerUrl = "//analytics.example.com/"; }));

        var options = provider.GetRequiredService<IOptions<MatomoOptions>>().Value;

        options.TrackerUrl.ShouldBe("//analytics.example.com/");
    }

    [Fact]
    public void YandexValidator_DisabledWithBadValues_Passes()
    {
        using var provider = Build(a => a.AddYandexMetrica(o =>
        {
            o.Enabled = false;
            o.CounterId = "not-a-number";
            o.AlternativeCdn = "ftp://cdn.example.com/tag.js";
        }));

        var options = provider.GetRequiredService<IOptions<YandexMetricaOptions>>().Value;

        options.CounterId.ShouldBe("not-a-number");
    }

    [Fact]
    public void YandexValidator_BadCounterAndCdn_ReportsBothFailures()
    {
        using var provider = Build(a => a.AddYandexMetrica(o =>
        {
            o.CounterId = "abc";
            o.AlternativeCdn = "ftp://cdn.example.com/tag.js";
        }));

        var exception = Should.Throw<OptionsValidationException>(
            () => provider.GetRequiredService<IOptions<YandexMetricaOptions>>().Value);

        exception.Failures.Count().ShouldBe(2);
        exception.Message.ShouldContain("CounterId");
        exception.Message.ShouldContain("AlternativeCdn");
    }

    [Fact]
    public void YandexValidator_ValidCounterAndHttpsCdn_Passes()
    {
        using var provider = Build(a => a.AddYandexMetrica(o =>
        {
            o.CounterId = "12345";
            o.AlternativeCdn = "https://cdn.example.com/tag.js";
        }));

        var options = provider.GetRequiredService<IOptions<YandexMetricaOptions>>().Value;

        options.AlternativeCdn.ShouldBe("https://cdn.example.com/tag.js");
    }

    [Fact]
    public void ClarityValidator_EnabledWithoutTrackerUrl_Passes()
    {
        using var provider = Build(a => a.AddClarity(o => { o.SiteId = "abc"; o.TrackerUrl = ""; }));

        var options = provider.GetRequiredService<IOptions<ClarityOptions>>().Value;

        options.SiteId.ShouldBe("abc");
    }

    [Fact]
    public void GA4Validator_EnabledWithoutMeasurementId_Passes()
    {
        using var provider = Build(a => a.AddGA4(o => o.MeasurementId = ""));

        var options = provider.GetRequiredService<IOptions<GA4Options>>().Value;

        options.MeasurementId.ShouldBeEmpty();
    }
}
