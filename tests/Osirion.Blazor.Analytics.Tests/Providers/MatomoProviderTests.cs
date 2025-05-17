using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Providers;

public class MatomoProviderTests
{
    [Fact]
    public void IsEnabled_ReturnsFalse_WhenOptionsDisabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new MatomoOptions { Enabled = false, SiteId = "123" });
        var provider = new MatomoProvider(options);

        // Act & Assert
        provider.IsEnabled.ShouldBeFalse();
    }

    [Fact]
    public void IsEnabled_ReturnsFalse_WhenSiteIdIsEmpty()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new MatomoOptions { Enabled = true, SiteId = "" });
        var provider = new MatomoProvider(options);

        // Act & Assert
        provider.IsEnabled.ShouldBeFalse();
    }

    [Fact]
    public void IsEnabled_ReturnsFalse_WhenTrackerUrlIsEmpty()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new MatomoOptions { Enabled = true, SiteId = "123", TrackerUrl = "" });
        var provider = new MatomoProvider(options);

        // Act & Assert
        provider.IsEnabled.ShouldBeFalse();
    }

    [Fact]
    public void IsEnabled_ReturnsTrue_WhenEnabledAndHasSiteIdAndTrackerUrl()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new MatomoOptions
        {
            Enabled = true,
            SiteId = "123",
            TrackerUrl = "https://example.com/matomo/"
        });
        var provider = new MatomoProvider(options);

        // Act & Assert
        provider.IsEnabled.ShouldBeTrue();
    }

    [Fact]
    public void GetScript_ReturnsCorrectScript_WithDefaultOptions()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new MatomoOptions
        {
            SiteId = "123",
            TrackerUrl = "https://example.com/matomo/"
        });
        var provider = new MatomoProvider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        script.ShouldContain("_paq.push(['trackPageView']);");
        script.ShouldContain("_paq.push(['setSiteId', '123']);");
        script.ShouldContain("https://example.com/matomo/");
        script.ShouldNotContain("_paq.push(['requireConsent']);");
    }

    [Fact]
    public void GetScript_IncludesRequireConsent_WhenEnabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new MatomoOptions
        {
            SiteId = "123",
            TrackerUrl = "https://example.com/matomo/",
            RequireConsent = true
        });
        var provider = new MatomoProvider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        script.ShouldContain("_paq.push(['requireConsent']);");
    }

    [Fact]
    public void GetScript_IncludesLinkTracking_WhenEnabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new MatomoOptions
        {
            SiteId = "123",
            TrackerUrl = "https://example.com/matomo/",
            TrackLinks = true
        });
        var provider = new MatomoProvider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        script.ShouldContain("_paq.push(['enableLinkTracking']);");
    }

    [Fact]
    public void GetScript_DoesNotIncludeLinkTracking_WhenDisabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new MatomoOptions
        {
            SiteId = "123",
            TrackerUrl = "https://example.com/matomo/",
            TrackLinks = false
        });
        var provider = new MatomoProvider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        script.ShouldNotContain("_paq.push(['enableLinkTracking']);");
    }

    [Fact]
    public void ShouldRender_MatchesIsEnabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new MatomoOptions
        {
            Enabled = true,
            SiteId = "123",
            TrackerUrl = "https://example.com/matomo/"
        });
        var provider = new MatomoProvider(options);

        // Act & Assert
        provider.ShouldRender.ShouldBe(provider.IsEnabled);
    }

    [Fact]
    public async Task TrackPageViewAsync_ReturnsCompletedTask()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new MatomoOptions
        {
            Enabled = true,
            SiteId = "123",
            TrackerUrl = "https://example.com/matomo/"
        });
        var provider = new MatomoProvider(options);

        // Act & Assert
        // Since Matomo automatically tracks page views, this method should return a completed task
        await provider.TrackPageViewAsync();
    }

    [Fact]
    public async Task TrackEventAsync_ReturnsCompletedTask()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new MatomoOptions
        {
            Enabled = true,
            SiteId = "123",
            TrackerUrl = "https://example.com/matomo/"
        });
        var provider = new MatomoProvider(options);

        // Act & Assert
        // Since Matomo requires JS interop for events, this method should return a completed task
        await provider.TrackEventAsync("category", "action", "label", 123);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenOptionsIsNull()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new MatomoProvider(null!));
    }
}