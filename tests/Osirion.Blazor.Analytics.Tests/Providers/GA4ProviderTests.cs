using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Providers;

#pragma warning disable CS0618 // LinkAttribution is obsolete; the test pins that it writes nothing
public class GA4ProviderTests
{
    [Fact]
    public void IsEnabled_ReturnsFalse_WhenOptionsDisabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new GA4Options { Enabled = false, MeasurementId = "G-XXXXXXXX" });
        var provider = new GA4Provider(options);

        // Act & Assert
        provider.IsEnabled.ShouldBeFalse();
    }

    [Fact]
    public void IsEnabled_ReturnsFalse_WhenMeasurementIdIsEmpty()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new GA4Options { Enabled = true, MeasurementId = "" });
        var provider = new GA4Provider(options);

        // Act & Assert
        provider.IsEnabled.ShouldBeFalse();
    }

    [Fact]
    public void IsEnabled_ReturnsTrue_WhenEnabledAndHasMeasurementId()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new GA4Options
        {
            Enabled = true,
            MeasurementId = "G-XXXXXXXX"
        });
        var provider = new GA4Provider(options);

        // Act & Assert
        provider.IsEnabled.ShouldBeTrue();
    }

    [Fact]
    public void GetScript_ReturnsCorrectScript_WithDefaultOptions()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new GA4Options
        {
            MeasurementId = "G-XXXXXXXX"
        });
        var provider = new GA4Provider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        script.ShouldContain("https://www.googletagmanager.com/gtag/js?id=G-XXXXXXXX");
        script.ShouldContain("gtag('js', new Date());");
        script.ShouldContain("gtag('config', \"G-XXXXXXXX\"");
    }

    [Fact]
    public void GetScript_IncludesDebugMode_WhenEnabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new GA4Options
        {
            MeasurementId = "G-XXXXXXXX",
            DebugMode = true
        });
        var provider = new GA4Provider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        // Fails before the fix: the script wrote window.ga_debug, an analytics.js mechanism, instead of debug_mode
        // in the config call that the DebugView page documents.
        script.IndexOf("\"debug_mode\":true", StringComparison.Ordinal)
            .ShouldBeGreaterThan(script.IndexOf("gtag('config'", StringComparison.Ordinal));
        script.ShouldNotContain("ga_debug");
    }

    [Fact]
    public void GetScript_IncludesConfigOptions_WhenProvided()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new GA4Options
        {
            MeasurementId = "G-XXXXXXXX",
            AnonymizeIp = true,
            SendPageView = true,
            TransportType = "beacon"
        });
        var provider = new GA4Provider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        script.ShouldContain("\"anonymize_ip\":true");
        script.ShouldContain("\"send_page_view\":true");
        script.ShouldContain("\"transport_type\":\"beacon\"");
    }

    [Fact]
    public void GetScript_LinkAttributionEnabled_WritesNoRequireCommand()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new GA4Options
        {
            MeasurementId = "G-XXXXXXXX",
            LinkAttribution = true
        });
        var provider = new GA4Provider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        // Fails before the fix: gtag('require', 'linkid') is an analytics.js command, and the gtag.js command
        // reference lists only config, get, set, event and consent.
        script.ShouldNotContain("require");
        script.ShouldNotContain("linkid");
    }

    [Fact]
    public void GetScript_IncludesOutboundLinkTracking_WhenEnabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new GA4Options
        {
            MeasurementId = "G-XXXXXXXX",
            TrackOutboundLinks = true
        });
        var provider = new GA4Provider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        script.ShouldContain("transport_type': 'beacon'");
        script.ShouldNotContain("preventDefault");
        script.ShouldNotContain("event_callback");
        script.ShouldContain("document.addEventListener('click'");
    }

    [Fact]
    public void ShouldRender_MatchesIsEnabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new GA4Options
        {
            Enabled = true,
            MeasurementId = "G-XXXXXXXX"
        });
        var provider = new GA4Provider(options);

        // Act & Assert
        provider.ShouldRender.ShouldBe(provider.IsEnabled);
    }

    [Fact]
    public void GetScript_IncludesCustomConfigParameters_WhenProvided()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new GA4Options
        {
            MeasurementId = "G-XXXXXXXX",
            ConfigParameters = new Dictionary<string, object> {
                { "custom_map", new Dictionary<string, string> { { "dimension1", "user_id" } } }
            }
        });
        var provider = new GA4Provider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        script.ShouldContain("\"custom_map\":");
    }

    [Fact]
    public async Task TrackPageViewAsync_ReturnsCompletedTask()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new GA4Options
        {
            Enabled = true,
            MeasurementId = "G-XXXXXXXX"
        });
        var provider = new GA4Provider(options);

        // Act & Assert
        // GA4 automatically tracks page views or uses JS interop
        await provider.TrackPageViewAsync();
    }

    [Fact]
    public async Task TrackEventAsync_ReturnsCompletedTask()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new GA4Options
        {
            Enabled = true,
            MeasurementId = "G-XXXXXXXX"
        });
        var provider = new GA4Provider(options);

        // Act & Assert
        // GA4 requires JS interop for events
        await provider.TrackEventAsync("category", "action", "label", 123);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenOptionsIsNull()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new GA4Provider(null!));
    }
}