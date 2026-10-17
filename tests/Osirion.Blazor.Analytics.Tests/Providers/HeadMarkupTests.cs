using AngleSharp.Html.Parser;
using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Providers;

/// <summary>
/// The trackers are placed in the document head, so their markup must be valid there and must not depend on the
/// address of the page that carries it.
/// </summary>
public class HeadMarkupTests
{
    // Fails before the fix: the script ended with <noscript><div><img ...></div></noscript>, and a div inside a
    // noscript element is not valid in the head (the parser moves the rest of the page into the body).
    [Fact]
    public void YandexGetScript_WritesNoNoscriptElement()
    {
        var provider = new YandexMetricaProvider(
            Microsoft.Extensions.Options.Options.Create(new YandexMetricaOptions { CounterId = "12345678" }));

        var script = provider.GetScript();

        script.ShouldNotContain("noscript", Case.Insensitive);
        script.ShouldNotContain("<div", Case.Insensitive);
        script.ShouldNotContain("<img", Case.Insensitive);
    }

    [Fact]
    public void YandexGetScript_ParsedInTheHead_LeavesTheHeadWithOnlyScripts()
    {
        var provider = new YandexMetricaProvider(
            Microsoft.Extensions.Options.Options.Create(new YandexMetricaOptions { CounterId = "12345678" }));

        var document = new HtmlParser().ParseDocument($"<!doctype html><html><head>{provider.GetScript()}</head><body></body></html>");

        document.Head!.Children.ShouldAllBe(element => element.LocalName == "script");
        document.Body!.Children.ShouldBeEmpty();
    }

    // Fails before the fix: the src was "_content/...", which a browser resolves against the current address, so a
    // page at /blog/post asked for /blog/_content/... unless the document had <base href="/">.
    [Fact]
    public void MatomoGetScript_WritesARootRelativeModuleSrc()
    {
        var provider = new MatomoProvider(Microsoft.Extensions.Options.Options.Create(
            new MatomoOptions { SiteId = "1", TrackerUrl = "https://analytics.example.com/" }));

        provider.GetScript().ShouldContain("src=\"/_content/Osirion.Blazor.Analytics/js/osirion-analytics.js\"");
    }

    [Fact]
    public void YandexGetScript_WritesARootRelativeModuleSrc()
    {
        var provider = new YandexMetricaProvider(
            Microsoft.Extensions.Options.Options.Create(new YandexMetricaOptions { CounterId = "12345678" }));

        provider.GetScript().ShouldContain("src=\"/_content/Osirion.Blazor.Analytics/js/osirion-analytics.js\"");
    }
}
