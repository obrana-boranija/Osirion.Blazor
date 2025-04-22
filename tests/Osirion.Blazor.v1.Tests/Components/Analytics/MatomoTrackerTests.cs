using Osirion.Blazor.Components.Analytics;
using Osirion.Blazor.Components.Analytics.Options;

namespace Osirion.Blazor.Tests.Components.Analytics;

public class MatomoTrackerTests
{
    [Fact]
    public void ShouldRender_ReturnsFalse_WhenOptionsIsNull()
    {
        // Arrange
        var component = new TestMatomoTracker
        {
            TrackerOptions = null
        };

        // Act
        var shouldRender = component.TestShouldRender;

        // Assert
        Assert.False(shouldRender);
    }

    [Fact]
    public void ShouldRender_ReturnsFalse_WhenTrackerUrlIsNull()
    {
        // Arrange
        var component = new TestMatomoTracker
        {
            TrackerOptions = new MatomoOptions
            {
                TrackerUrl = null,
                SiteId = "test-id"
            }
        };

        // Act
        var shouldRender = component.TestShouldRender;

        // Assert
        Assert.False(shouldRender);
    }

    [Fact]
    public void ShouldRender_ReturnsFalse_WhenSiteIdIsNull()
    {
        // Arrange
        var component = new TestMatomoTracker
        {
            TrackerOptions = new MatomoOptions
            {
                TrackerUrl = "https://test.com",
                SiteId = null
            }
        };

        // Act
        var shouldRender = component.TestShouldRender;

        // Assert
        Assert.False(shouldRender);
    }

    [Fact]
    public void ShouldRender_ReturnsFalse_WhenTrackIsFalse()
    {
        // Arrange
        var component = new TestMatomoTracker
        {
            TrackerOptions = new MatomoOptions
            {
                TrackerUrl = "https://test.com",
                SiteId = "test-id",
                Track = false
            }
        };

        // Act
        var shouldRender = component.TestShouldRender;

        // Assert
        Assert.False(shouldRender);
    }

    [Fact]
    public void ShouldRender_ReturnsTrue_WhenAllPropertiesAreValid()
    {
        // Arrange
        var component = new TestMatomoTracker
        {
            TrackerOptions = new MatomoOptions
            {
                TrackerUrl = "https://test.com",
                SiteId = "test-id",
                Track = true
            }
        };

        // Act
        var shouldRender = component.TestShouldRender;

        // Assert
        Assert.True(shouldRender);
    }

    [Fact]
    public void GetScript_ReturnsCorrectScript()
    {
        // Arrange
        var options = new MatomoOptions
        {
            TrackerUrl = "//analytics.test.com/",
            SiteId = "123"
        };
        var component = new TestMatomoTracker
        {
            TrackerOptions = options
        };

        // Act
        var script = component.TestGetScript();

        // Assert
        Assert.Contains("//analytics.test.com/", script);
        Assert.Contains("123", script);
        Assert.Contains("matomo.php", script);
        Assert.Contains("matomo.js", script);
        Assert.Contains("_paq", script);
    }

    private class TestMatomoTracker : MatomoTracker
    {
        public bool TestShouldRender => base.CouldRender;
        public string TestGetScript() => base.GetScript();
    }
}