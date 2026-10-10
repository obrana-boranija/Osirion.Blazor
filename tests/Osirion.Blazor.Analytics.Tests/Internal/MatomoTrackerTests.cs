using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Analytics.Components;
using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Components;

public class MatomoTrackerTests : BunitContext
{
    [Fact]
    public void MatomoTracker_ShouldRenderScript_WhenProviderIsEnabled()
    {
        // Arrange - use a real provider with real options
        var options = Microsoft.Extensions.Options.Options.Create(
            new MatomoOptions
            {
                Enabled = true,
                SiteId = "123",
                TrackerUrl = "https://analytics.example.com/"
            });

        var provider = new MatomoProvider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = Render<MatomoTracker>();

        // Assert
        cut.Markup.ShouldContain("<script");
        cut.Markup.ShouldContain("_paq");
        cut.Markup.ShouldContain("123");
    }

    [Fact]
    public void MatomoTracker_ShouldNotRenderScript_WhenProviderIsDisabled()
    {
        // Arrange - use a real provider with Enabled = false
        var options = Microsoft.Extensions.Options.Options.Create(
            new MatomoOptions
            {
                Enabled = false,
                SiteId = "123",
                TrackerUrl = "https://analytics.example.com/"
            });

        var provider = new MatomoProvider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = Render<MatomoTracker>();

        // Assert
        cut.MarkupMatches(string.Empty);
    }

    [Fact]
    public void MatomoTracker_ShouldNotRenderScript_WhenSiteIdIsEmpty()
    {
        // Arrange - provider with Enabled = true but empty SiteId
        var options = Microsoft.Extensions.Options.Options.Create(
            new MatomoOptions
            {
                Enabled = true,
                SiteId = "", // Empty ID will disable the provider
                TrackerUrl = "https://analytics.example.com/"
            });

        var provider = new MatomoProvider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = Render<MatomoTracker>();

        // Assert
        cut.MarkupMatches(string.Empty);
    }

    [Fact]
    public void MatomoTracker_ShouldNotRenderScript_WhenTrackerUrlIsEmpty()
    {
        // Arrange - provider with Enabled = true but empty TrackerUrl
        var options = Microsoft.Extensions.Options.Options.Create(
            new MatomoOptions
            {
                Enabled = true,
                SiteId = "123",
                TrackerUrl = "" // Empty URL will disable the provider
            });

        var provider = new MatomoProvider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = Render<MatomoTracker>();

        // Assert
        cut.MarkupMatches(string.Empty);
    }

    [Fact]
    public void MatomoTracker_ShouldUseParameter_ToOverrideProviderSiteId()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(
            new MatomoOptions
            {
                Enabled = true,
                SiteId = "original-id",
                TrackerUrl = "https://analytics.example.com/"
            });

        var provider = new MatomoProvider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = Render<MatomoTracker>(parameters => parameters
            .Add(p => p.SiteId, "custom-site-id"));

        // Assert
        cut.Markup.ShouldContain("\"custom-site-id\"");
        cut.Markup.ShouldNotContain("original-id");
    }

    [Fact]
    public void MatomoTracker_ShouldUseParameter_ToOverrideProviderEnabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(
            new MatomoOptions
            {
                Enabled = true,
                SiteId = "123",
                TrackerUrl = "https://analytics.example.com/"
            });

        var provider = new MatomoProvider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = Render<MatomoTracker>(parameters => parameters
            .Add(p => p.Enabled, false));

        // Assert
        cut.Markup.ShouldNotContain("<script");
    }
}