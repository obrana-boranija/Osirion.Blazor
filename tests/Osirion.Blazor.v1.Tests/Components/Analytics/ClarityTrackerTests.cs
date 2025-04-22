using Osirion.Blazor.Components.Analytics;
using Osirion.Blazor.Components.Analytics.Options;

namespace Osirion.Blazor.Tests.Components.Analytics;

public class ClarityTrackerTests
{
    [Fact]
    public void ShouldRender_ReturnsFalse_WhenOptionsIsNull()
    {
        // Arrange
        var component = new TestClarityTracker
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
        var component = new TestClarityTracker
        {
            TrackerOptions = new ClarityOptions
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
        var component = new TestClarityTracker
        {
            TrackerOptions = new ClarityOptions
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
        var component = new TestClarityTracker
        {
            TrackerOptions = new ClarityOptions
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
        var component = new TestClarityTracker
        {
            TrackerOptions = new ClarityOptions
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
        var options = new ClarityOptions
        {
            TrackerUrl = "https://www.clarity.ms/tag/",
            SiteId = "test-site-id"
        };
        var component = new TestClarityTracker
        {
            TrackerOptions = options
        };

        // Act
        var script = component.TestGetScript();

        // Assert
        Assert.Contains("https://www.clarity.ms/tag/", script);
        Assert.Contains("test-site-id", script);
        Assert.Contains("clarity", script);
    }

    private class TestClarityTracker : ClarityTracker
    {
        public bool TestShouldRender => base.CouldRender;
        public string TestGetScript() => base.GetScript();
    }
}