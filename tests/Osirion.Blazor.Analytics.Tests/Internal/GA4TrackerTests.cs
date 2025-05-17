using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Analytics.Components;
using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Components;

public class GA4TrackerTests : TestContext
{
    [Fact]
    public void GA4Tracker_ShouldRenderScript_WhenProviderIsEnabled()
    {
        // Arrange - use a real provider with real options
        var options = Microsoft.Extensions.Options.Options.Create(
            new GA4Options
            {
                Enabled = true,
                MeasurementId = "G-TESTID"
            });

        var provider = new GA4Provider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = RenderComponent<GA4Tracker>();

        // Assert
        cut.Markup.ShouldContain("<script");
        cut.Markup.ShouldContain("gtag");
        cut.Markup.ShouldContain("G-TESTID");
    }

    [Fact]
    public void GA4Tracker_ShouldNotRenderScript_WhenProviderIsDisabled()
    {
        // Arrange - use a real provider with Enabled = false
        var options = Microsoft.Extensions.Options.Options.Create(
            new GA4Options
            {
                Enabled = false,
                MeasurementId = "G-TESTID"
            });

        var provider = new GA4Provider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = RenderComponent<GA4Tracker>();

        // Assert
        cut.MarkupMatches(string.Empty);
    }

    [Fact]
    public void GA4Tracker_ShouldNotRenderScript_WhenMeasurementIdIsEmpty()
    {
        // Arrange - provider with Enabled = true but empty MeasurementId
        var options = Microsoft.Extensions.Options.Options.Create(
            new GA4Options
            {
                Enabled = true,
                MeasurementId = "" // Empty ID will disable the provider
            });

        var provider = new GA4Provider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = RenderComponent<GA4Tracker>();

        // Assert
        cut.MarkupMatches(string.Empty);
    }

    [Fact]
    public void GA4Tracker_ShouldUseParameter_ToOverrideMeasurementId()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(
            new GA4Options
            {
                Enabled = true,
                MeasurementId = "G-ORIGINAL"
            });

        var provider = new GA4Provider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = RenderComponent<GA4Tracker>(parameters => parameters
            .Add(p => p.MeasurementId, "G-CUSTOM"));

        // Assert
        cut.Instance.MeasurementId.ShouldBe("G-CUSTOM");
    }

    [Fact]
    public void GA4Tracker_ShouldUseParameter_ToOverrideProviderEnabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(
            new GA4Options
            {
                Enabled = true,
                MeasurementId = "G-TESTID"
            });

        var provider = new GA4Provider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = RenderComponent<GA4Tracker>(parameters => parameters
            .Add(p => p.Enabled, false));

        // Assert
        cut.Instance.Enabled.ShouldBe(false);
    }

    [Fact]
    public void GA4Tracker_ShouldUseParameter_ToOverrideDebugMode()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(
            new GA4Options
            {
                Enabled = true,
                MeasurementId = "G-TESTID",
                DebugMode = false
            });

        var provider = new GA4Provider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = RenderComponent<GA4Tracker>(parameters => parameters
            .Add(p => p.DebugMode, true));

        // Assert
        cut.Instance.DebugMode.ShouldBe(true);
    }
}