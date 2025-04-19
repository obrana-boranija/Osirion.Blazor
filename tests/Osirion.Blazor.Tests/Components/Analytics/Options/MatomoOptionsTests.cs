using Osirion.Blazor.Components.Analytics.Options;

namespace Osirion.Blazor.Tests.Components.Analytics.Options;

public class MatomoOptionsTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultTrackerUrl()
    {
        // Arrange & Act
        var options = new MatomoOptions();

        // Assert
        Assert.Equal("//analytics.tridesetri.com/", options.TrackerUrl);
    }

    [Fact]
    public void Section_ShouldBeCorrect()
    {
        // Assert
        Assert.Equal("Matomo", MatomoOptions.Section);
    }

    [Fact]
    public void TrackerUrl_ShouldBeSettable()
    {
        // Arrange
        var options = new MatomoOptions();
        const string newUrl = "https://custom.matomo.com/";

        // Act
        options.TrackerUrl = newUrl;

        // Assert
        Assert.Equal(newUrl, options.TrackerUrl);
    }

    [Fact]
    public void SiteId_ShouldBeSettable()
    {
        // Arrange
        var options = new MatomoOptions();
        const string siteId = "123";

        // Act
        options.SiteId = siteId;

        // Assert
        Assert.Equal(siteId, options.SiteId);
    }

    [Fact]
    public void Track_ShouldBeDefaultedToTrue()
    {
        // Arrange & Act
        var options = new MatomoOptions();

        // Assert
        Assert.True(options.Track);
    }
}