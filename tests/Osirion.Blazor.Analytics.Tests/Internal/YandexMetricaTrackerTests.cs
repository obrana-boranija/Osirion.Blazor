using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Analytics.Components;
using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Components;

public class YandexMetricaTrackerTests : TestContext
{
    [Fact]
    public void YandexMetricaTracker_ShouldRenderScript_WhenProviderIsEnabled()
    {
        // Arrange - use a real provider instance with custom options
        var options = Microsoft.Extensions.Options.Options.Create(
            new YandexMetricaOptions
            {
                Enabled = true,
                CounterId = "12345678"
            });

        var provider = new YandexMetricaProvider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = RenderComponent<YandexMetricaTracker>();

        // Assert
        cut.Markup.ShouldContain("<script");
        cut.Markup.ShouldContain("ym");
        cut.Markup.ShouldContain("12345678");
    }

    [Fact]
    public void YandexMetricaTracker_ShouldNotRenderScript_WhenProviderIsDisabled()
    {
        // Arrange - just use a real provider with Enabled = false
        var options = Microsoft.Extensions.Options.Options.Create(
            new YandexMetricaOptions
            {
                Enabled = false,
                CounterId = "12345678"
            });

        var provider = new YandexMetricaProvider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = RenderComponent<YandexMetricaTracker>();

        // Assert
        cut.MarkupMatches(string.Empty);
    }

    [Fact]
    public void YandexMetricaTracker_ShouldNotRenderScript_WhenCounterIdIsEmpty()
    {
        // Arrange - provider with Enabled = true but empty CounterId
        var options = Microsoft.Extensions.Options.Options.Create(
            new YandexMetricaOptions
            {
                Enabled = true,
                CounterId = "" // Empty ID will disable the provider
            });

        var provider = new YandexMetricaProvider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = RenderComponent<YandexMetricaTracker>();

        // Assert
        cut.MarkupMatches(string.Empty);
    }

    [Fact]
    public void YandexMetricaTracker_ShouldUseParameter_ToOverrideCounterId()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(
            new YandexMetricaOptions
            {
                Enabled = true,
                CounterId = "87654321"
            });

        var provider = new YandexMetricaProvider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = RenderComponent<YandexMetricaTracker>(parameters => parameters
            .Add(p => p.CounterId, "12345678"));

        // Assert
        cut.Instance.CounterId.ShouldBe("12345678");
    }

    [Fact]
    public void YandexMetricaTracker_ShouldUseParameter_ToOverrideProviderEnabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(
            new YandexMetricaOptions
            {
                Enabled = true,
                CounterId = "12345678"
            });

        var provider = new YandexMetricaProvider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = RenderComponent<YandexMetricaTracker>(parameters => parameters
            .Add(p => p.Enabled, false));

        // Assert
        cut.Instance.Enabled.ShouldBe(false);
    }

    [Fact]
    public void YandexMetricaTracker_ShouldUseParameter_ToOverrideWebVisor()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(
            new YandexMetricaOptions
            {
                Enabled = true,
                CounterId = "12345678",
                WebVisor = false
            });

        var provider = new YandexMetricaProvider(options);
        Services.AddSingleton(provider);
        SetRendererInfo(new RendererInfo("Server", false));

        // Act
        var cut = RenderComponent<YandexMetricaTracker>(parameters => parameters
            .Add(p => p.WebVisor, true));

        // Assert
        cut.Instance.WebVisor.ShouldBe(true);
    }
}