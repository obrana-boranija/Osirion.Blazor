using Osirion.Blazor.Components.Analytics.Options;

namespace Osirion.Blazor.Tests.Components.Analytics.Options;

public class ClarityOptionsTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultTrackerUrl()
    {
        // Arrange & Act
        var options = new ClarityOptions();

        // Assert
        Assert.Equal("https://www.clarity.ms/tag/", options.TrackerUrl);
    }

    [Fact]
    public void Section_ShouldBeCorrect()
    {
        // Assert
        Assert.Equal("Clarity", ClarityOptions.Section);
    }

    [Fact]
    public void TrackerUrl_ShouldBeSettable()
    {
        // Arrange
        var options = new ClarityOptions();
        const string newUrl = "https://custom.clarity.ms/tag/";

        // Act
        options.TrackerUrl = newUrl;

        // Assert
        Assert.Equal(newUrl, options.TrackerUrl);
    }

    [Fact]
    public void SiteId_ShouldBeSettable()
    {
        // Arrange
        var options = new ClarityOptions();
        const string siteId = "test-site-id";

        // Act
        options.SiteId = siteId;

        // Assert
        Assert.Equal(siteId, options.SiteId);
    }

    [Fact]
    public void Track_ShouldBeDefaultedToTrue()
    {
        // Arrange & Act
        var options = new ClarityOptions();

        // Assert
        Assert.True(options.Track);
    }
}