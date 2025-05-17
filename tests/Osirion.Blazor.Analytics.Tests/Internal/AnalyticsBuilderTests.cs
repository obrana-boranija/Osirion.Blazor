using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Analytics.Internal;
using Osirion.Blazor.Analytics.Providers;
using Osirion.Blazor.Analytics.Services;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Internal;

public class AnalyticsBuilderTests
{
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenServicesIsNull()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new AnalyticsBuilder(null!));
    }

    [Fact]
    public void Services_ShouldReturnProvidedServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new AnalyticsBuilder(services);

        // Act & Assert
        builder.Services.ShouldBe(services);
    }

    [Fact]
    public void AddClarity_ShouldRegisterClarityProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new AnalyticsBuilder(services);

        // Act
        builder.AddClarity(options => {
            options.SiteId = "test-clarity-id";
            options.Enabled = true;
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var provider = serviceProvider.GetService<ClarityProvider>();
        provider.ShouldNotBeNull();

        var analyticsProvider = serviceProvider.GetServices<IAnalyticsProvider>()
            .FirstOrDefault(p => p is ClarityProvider);
        analyticsProvider.ShouldNotBeNull();
    }

    [Fact]
    public void AddMatomo_ShouldRegisterMatomoProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new AnalyticsBuilder(services);

        // Act
        builder.AddMatomo(options => {
            options.SiteId = "test-matomo-id";
            options.TrackerUrl = "https://matomo.test/";
            options.Enabled = true;
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var provider = serviceProvider.GetService<MatomoProvider>();
        provider.ShouldNotBeNull();

        var analyticsProvider = serviceProvider.GetServices<IAnalyticsProvider>()
            .FirstOrDefault(p => p is MatomoProvider);
        analyticsProvider.ShouldNotBeNull();
    }

    [Fact]
    public void AddGA4_ShouldRegisterGA4Provider()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new AnalyticsBuilder(services);

        // Act
        builder.AddGA4(options => {
            options.MeasurementId = "G-TESTID";
            options.Enabled = true;
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var provider = serviceProvider.GetService<GA4Provider>();
        provider.ShouldNotBeNull();

        var analyticsProvider = serviceProvider.GetServices<IAnalyticsProvider>()
            .FirstOrDefault(p => p is GA4Provider);
        analyticsProvider.ShouldNotBeNull();
    }

    [Fact]
    public void AddYandexMetrica_ShouldRegisterYandexMetricaProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new AnalyticsBuilder(services);

        // Act
        builder.AddYandexMetrica(options => {
            options.CounterId = "12345678";
            options.Enabled = true;
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var provider = serviceProvider.GetService<YandexMetricaProvider>();
        provider.ShouldNotBeNull();

        var analyticsProvider = serviceProvider.GetServices<IAnalyticsProvider>()
            .FirstOrDefault(p => p is YandexMetricaProvider);
        analyticsProvider.ShouldNotBeNull();
    }

    [Fact]
    public void AddProvider_ShouldRegisterCustomProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new AnalyticsBuilder(services);
        var customProvider = Substitute.For<IAnalyticsProvider>();

        // Act
        builder.AddProvider<TestAnalyticsProvider>(provider => {
            provider.CustomSetting = "test-value";
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var provider = serviceProvider.GetService<TestAnalyticsProvider>();
        provider.ShouldNotBeNull();
        provider.CustomSetting.ShouldBe("test-value");

        var analyticsProvider = serviceProvider.GetServices<IAnalyticsProvider>()
            .FirstOrDefault(p => p is TestAnalyticsProvider);
        analyticsProvider.ShouldNotBeNull();
    }

    [Fact]
    public void AddProvider_ShouldRegisterProviderWithoutConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new AnalyticsBuilder(services);

        // Act
        builder.AddProvider<TestAnalyticsProvider>();

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var provider = serviceProvider.GetService<TestAnalyticsProvider>();
        provider.ShouldNotBeNull();

        var analyticsProvider = serviceProvider.GetServices<IAnalyticsProvider>()
            .FirstOrDefault(p => p is TestAnalyticsProvider);
        analyticsProvider.ShouldNotBeNull();
    }

    [Fact]
    public void Builder_ShouldRegisterMultipleProviders()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new AnalyticsBuilder(services);

        // Act
        builder
            .AddClarity(options => options.SiteId = "clarity-id")
            .AddMatomo(options => {
                options.SiteId = "matomo-id";
                options.TrackerUrl = "https://matomo.test/";
            })
            .AddGA4(options => options.MeasurementId = "G-TESTID")
            .AddYandexMetrica(options => options.CounterId = "12345678")
            .AddProvider<TestAnalyticsProvider>();

        // Add a mock logger for AnalyticsService
        services.AddSingleton(Substitute.For<ILogger<AnalyticsService>>());

        // Register the service
        services.AddSingleton<IAnalyticsService, AnalyticsService>();

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var analyticsService = serviceProvider.GetService<IAnalyticsService>();
        analyticsService.ShouldNotBeNull();

        // Let's check each provider type is registered
        var providerTypes = analyticsService.Providers
            .Select(p => p.GetType().Name)
            .ToList();

        providerTypes.ShouldContain(nameof(ClarityProvider));
        providerTypes.ShouldContain(nameof(MatomoProvider));
        providerTypes.ShouldContain(nameof(GA4Provider));
        providerTypes.ShouldContain(nameof(YandexMetricaProvider));
        providerTypes.ShouldContain(nameof(TestAnalyticsProvider));
    }

    // Test provider for testing custom provider registration
    private class TestAnalyticsProvider : IAnalyticsProvider
    {
        public string ProviderId => "test-provider";
        public bool IsEnabled => true;
        public bool ShouldRender => true;
        public string CustomSetting { get; set; } = "";

        public string GetScript() => "<script>test provider</script>";

        public Task TrackEventAsync(string category, string action, string? label = null, object? value = null, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task TrackPageViewAsync(string? path = null, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}