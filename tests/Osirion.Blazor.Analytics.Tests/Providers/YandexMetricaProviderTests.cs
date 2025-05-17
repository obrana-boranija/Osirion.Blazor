using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Providers;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Providers;

public class YandexMetricaProviderTests
{
    [Fact]
    public void IsEnabled_ReturnsFalse_WhenOptionsDisabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new YandexMetricaOptions { Enabled = false, CounterId = "12345678" });
        var provider = new YandexMetricaProvider(options);

        // Act & Assert
        provider.IsEnabled.ShouldBeFalse();
    }

    [Fact]
    public void IsEnabled_ReturnsFalse_WhenCounterIdIsEmpty()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new YandexMetricaOptions { Enabled = true, CounterId = "" });
        var provider = new YandexMetricaProvider(options);

        // Act & Assert
        provider.IsEnabled.ShouldBeFalse();
    }

    [Fact]
    public void IsEnabled_ReturnsTrue_WhenEnabledAndHasCounterId()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new YandexMetricaOptions
        {
            Enabled = true,
            CounterId = "12345678"
        });
        var provider = new YandexMetricaProvider(options);

        // Act & Assert
        provider.IsEnabled.ShouldBeTrue();
    }

    [Fact]
    public void GetScript_ReturnsCorrectScript_WithDefaultOptions()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new YandexMetricaOptions
        {
            CounterId = "12345678"
        });
        var provider = new YandexMetricaProvider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        script.ShouldContain("ym(12345678, \"init\"");
        script.ShouldContain("https://mc.yandex.ru/metrika/tag.js");
        script.ShouldContain("https://mc.yandex.ru/watch/12345678");
    }

    [Fact]
    public void GetScript_IncludesClickMapOption_WhenEnabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new YandexMetricaOptions
        {
            CounterId = "12345678",
            ClickMap = true
        });
        var provider = new YandexMetricaProvider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        script.ShouldContain("\"clickmap\":true");
    }

    [Fact]
    public void GetScript_IncludesTrackLinksOption_WhenEnabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new YandexMetricaOptions
        {
            CounterId = "12345678",
            TrackLinks = true
        });
        var provider = new YandexMetricaProvider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        script.ShouldContain("\"trackLinks\":true");
    }

    [Fact]
    public void GetScript_IncludesAccurateTrackBounceOption_WhenEnabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new YandexMetricaOptions
        {
            CounterId = "12345678",
            AccurateTrackBounce = true
        });
        var provider = new YandexMetricaProvider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        script.ShouldContain("\"accurateTrackBounce\":true");
    }

    [Fact]
    public void GetScript_IncludesWebVisorOption_WhenEnabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new YandexMetricaOptions
        {
            CounterId = "12345678",
            WebVisor = true
        });
        var provider = new YandexMetricaProvider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        script.ShouldContain("\"webvisor\":true");
    }

    [Fact]
    public void GetScript_IncludesTrackHashOption_WhenProvided()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new YandexMetricaOptions
        {
            CounterId = "12345678",
            TrackHash = true
        });
        var provider = new YandexMetricaProvider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        script.ShouldContain("\"trackHash\":true");
    }

    [Fact]
    public void GetScript_IncludesDeferOption_WhenEnabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new YandexMetricaOptions
        {
            CounterId = "12345678",
            DeferLoad = true
        });
        var provider = new YandexMetricaProvider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        script.ShouldContain("\"defer\":true");
    }

    [Fact]
    public void GetScript_UsesAlternativeCdn_WhenProvided()
    {
        // Arrange
        var alternativeCdn = "https://cdn.example.com/yandex-metrica.js";
        var options = Microsoft.Extensions.Options.Options.Create(new YandexMetricaOptions
        {
            CounterId = "12345678",
            AlternativeCdn = alternativeCdn
        });
        var provider = new YandexMetricaProvider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        script.ShouldContain(alternativeCdn);
        script.ShouldNotContain("https://mc.yandex.ru/metrika/tag.js");
    }

    [Fact]
    public void GetScript_IncludesEcommerceContainerName_WhenEcommerceEnabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new YandexMetricaOptions
        {
            CounterId = "12345678",
            EcommerceEnabled = true,
            EcommerceContainerName = "testDataLayer"
        });
        var provider = new YandexMetricaProvider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        script.ShouldContain("\"ecommerce\":\"testDataLayer\"");
    }

    [Fact]
    public void GetScript_IncludesCustomParams_WhenProvided()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new YandexMetricaOptions
        {
            CounterId = "12345678",
            Params = new Dictionary<string, object> {
                { "site_section", "blog" }
            }
        });
        var provider = new YandexMetricaProvider(options);

        // Act
        var script = provider.GetScript();

        // Assert
        script.ShouldContain("\"params\":{");
        script.ShouldContain("\"site_section\":\"blog\"");
    }

    [Fact]
    public void ShouldRender_MatchesIsEnabled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new YandexMetricaOptions
        {
            Enabled = true,
            CounterId = "12345678"
        });
        var provider = new YandexMetricaProvider(options);

        // Act & Assert
        provider.ShouldRender.ShouldBe(provider.IsEnabled);
    }

    [Fact]
    public async Task TrackPageViewAsync_ReturnsCompletedTask()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new YandexMetricaOptions
        {
            Enabled = true,
            CounterId = "12345678"
        });
        var provider = new YandexMetricaProvider(options);

        // Act & Assert
        // Yandex Metrica automatically tracks page views
        await provider.TrackPageViewAsync();
    }

    [Fact]
    public async Task TrackEventAsync_ReturnsCompletedTask()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new YandexMetricaOptions
        {
            Enabled = true,
            CounterId = "12345678"
        });
        var provider = new YandexMetricaProvider(options);

        // Act & Assert
        // Yandex Metrica requires JS interop for events
        await provider.TrackEventAsync("category", "action", "label", 123);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenOptionsIsNull()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new YandexMetricaProvider(null!));
    }
}