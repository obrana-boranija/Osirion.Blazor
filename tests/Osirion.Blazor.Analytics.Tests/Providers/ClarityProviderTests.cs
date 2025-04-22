using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Providers;

public class ClarityProviderTests
{
    [Fact]
    public void IsEnabled_ReturnsFalse_WhenOptionsDisabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new ClarityOptions { Enabled = false, SiteId = "abc123" });
        var provider = new ClarityProvider(options);

        // Act & Assert
        provider.IsEnabled.ShouldBeFalse();
    }

    [Fact]
    public void IsEnabled_ReturnsFalse_WhenSiteIdIsEmpty()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new ClarityOptions { Enabled = true, SiteId = "" });
        var provider = new ClarityProvider(options);

        // Act & Assert
        provider.IsEnabled.ShouldBeFalse();
    }

    [Fact]
    public void IsEnabled_ReturnsTrue_WhenEnabledAndHasSiteId()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new ClarityOptions { Enabled = true, SiteId = "abc123" });
        var provider = new ClarityProvider(options);

        // Act & Assert
        provider.IsEnabled.ShouldBeTrue();
    }

    [Fact]
    public void GetScript_ReturnsCorrectScript()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new ClarityOptions
        {
            SiteId = "abc123",
            TrackerUrl = "https://www.clarity.ms/tag/"
        });
        var provider = new ClarityProvider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        script.ShouldContain("clarity");
        script.ShouldContain("abc123");
        script.ShouldContain("https://www.clarity.ms/tag/");
    }
}