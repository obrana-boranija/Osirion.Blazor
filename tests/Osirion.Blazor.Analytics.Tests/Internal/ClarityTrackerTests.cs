using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Analytics.Components;
using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Components;

public class ClarityTrackerTests : TestContext
{
    [Fact]
    public void ClarityTracker_ShouldRenderScript_WhenProviderIsEnabled()
    {
        // Arrange - use a real provider with real options
        var options = Microsoft.Extensions.Options.Options.Create(
            new ClarityOptions
            {
                Enabled = true,
                SiteId = "test-site-id",
                TrackerUrl = "https://www.clarity.ms/tag/"
            });

        var provider = new ClarityProvider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = RenderComponent<ClarityTracker>();

        // Assert
        cut.Markup.ShouldContain("<script");
        cut.Markup.ShouldContain("clarity");
        cut.Markup.ShouldContain("test-site-id");
    }

    [Fact]
    public void ClarityTracker_ShouldNotRenderScript_WhenProviderIsDisabled()
    {
        // Arrange - use a real provider with Enabled = false
        var options = Microsoft.Extensions.Options.Options.Create(
            new ClarityOptions
            {
                Enabled = false,
                SiteId = "test-site-id"
            });

        var provider = new ClarityProvider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = RenderComponent<ClarityTracker>();

        // Assert
        cut.MarkupMatches(string.Empty);
    }

    [Fact]
    public void ClarityTracker_ShouldNotRenderScript_WhenSiteIdIsEmpty()
    {
        // Arrange - provider with Enabled = true but empty SiteId
        var options = Microsoft.Extensions.Options.Options.Create(
            new ClarityOptions
            {
                Enabled = true,
                SiteId = "" // Empty ID will disable the provider
            });

        var provider = new ClarityProvider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = RenderComponent<ClarityTracker>();

        // Assert
        cut.MarkupMatches(string.Empty);
    }

    [Fact]
    public void ClarityTracker_ShouldUseParameter_ToOverrideProviderSiteId()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(
            new ClarityOptions
            {
                Enabled = true,
                SiteId = "original-site-id",
                TrackerUrl = "https://www.clarity.ms/tag/"
            });

        var provider = new ClarityProvider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = RenderComponent<ClarityTracker>(parameters => parameters
            .Add(p => p.SiteId, "custom-site-id"));

        // Assert
        cut.Instance.SiteId.ShouldBe("custom-site-id");
    }

    [Fact]
    public void ClarityTracker_ShouldUseParameter_ToOverrideProviderEnabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(
            new ClarityOptions
            {
                Enabled = true,
                SiteId = "test-site-id",
                TrackerUrl = "https://www.clarity.ms/tag/"
            });

        var provider = new ClarityProvider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = RenderComponent<ClarityTracker>(parameters => parameters
            .Add(p => p.Enabled, false));

        // Assert
        cut.Instance.Enabled.ShouldBe(false);
    }
}