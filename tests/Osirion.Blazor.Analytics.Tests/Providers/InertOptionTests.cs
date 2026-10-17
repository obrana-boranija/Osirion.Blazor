using System.Reflection;
using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Providers;

/// <summary>
/// Pins what the options that once did nothing do now: <c>HashTracking</c> reaches Yandex Metrica as
/// <c>trackHash</c>, and the two that map to no documented provider feature say so with <c>[Obsolete]</c>.
/// </summary>
public class InertOptionTests
{
    // Fails before the fix: HashTracking was copied by the builder and never read, so the init options carried no
    // trackHash.
    [Fact]
    public void YandexGetScript_HashTrackingOn_WritesTrackHash()
    {
        var script = CreateYandexProvider(new YandexMetricaOptions { CounterId = "12345678", HashTracking = true }).GetScript();

        script.ShouldContain("\"trackHash\":true");
    }

    [Fact]
    public void YandexGetScript_TrackHashWinsOverHashTracking()
    {
        var script = CreateYandexProvider(
            new YandexMetricaOptions { CounterId = "12345678", HashTracking = true, TrackHash = false }).GetScript();

        script.ShouldContain("\"trackHash\":false");
    }

    [Fact]
    public void YandexGetScript_NeitherSet_WritesNoTrackHash()
    {
        var script = CreateYandexProvider(new YandexMetricaOptions { CounterId = "12345678" }).GetScript();

        script.ShouldNotContain("trackHash");
    }

    // Fails before the fix: nothing read these two options and nothing told the caller so.
    [Theory]
    [InlineData(typeof(ClarityOptions), "TrackUserAttributes")]
    [InlineData(typeof(MatomoOptions), "TrackDownloads")]
    public void OptionWithNoProviderFeature_IsMarkedObsolete(Type optionsType, string propertyName)
    {
        optionsType.GetProperty(propertyName)!.GetCustomAttribute<ObsoleteAttribute>().ShouldNotBeNull();
    }

    private static YandexMetricaProvider CreateYandexProvider(YandexMetricaOptions options) =>
        new(Microsoft.Extensions.Options.Options.Create(options));
}
