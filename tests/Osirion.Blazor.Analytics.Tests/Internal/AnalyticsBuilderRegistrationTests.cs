using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Analytics.Internal;
using Osirion.Blazor.Analytics.Providers;
using Osirion.Blazor.Analytics.Services;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Internal;

public class AnalyticsBuilderRegistrationTests
{
    [Fact]
    public void AddClarity_ShouldRegisterProviderAsInterface()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new AnalyticsBuilder(services);
        services.AddLogging(); // Add logging to resolve ILogger dependencies

        // Act
        builder.AddClarity(options => options.SiteId = "test-clarity-id");
        var provider = services.BuildServiceProvider();

        // Assert
        var allProviders = provider.GetServices<IAnalyticsProvider>().ToList();
        allProviders.Count.ShouldBe(1);
        allProviders[0].ShouldBeOfType<ClarityProvider>();
    }

    [Fact]
    public void AddMultipleProviders_ShouldRegisterAllProvidersAsInterface()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new AnalyticsBuilder(services);
        services.AddLogging(); // Add logging to resolve ILogger dependencies

        // Act
        builder.AddClarity(options => options.SiteId = "clarity-id");
        builder.AddMatomo(options =>
        {
            options.SiteId = "matomo-id";
            options.TrackerUrl = "https://matomo.test/";
        });

        var provider = services.BuildServiceProvider();

        // Assert
        var allProviders = provider.GetServices<IAnalyticsProvider>().ToList();
        allProviders.Count.ShouldBe(2);
        allProviders.Any(p => p is ClarityProvider).ShouldBeTrue();
        allProviders.Any(p => p is MatomoProvider).ShouldBeTrue();
    }

    // Create a simple mock provider for testing
    public class CustomMockProvider : IAnalyticsProvider
    {
        public string ProviderId => "mock";
        public bool IsEnabled => true;
        public bool ShouldRender => true;

        public string GetScript() => "<script>mock</script>";

        public Task TrackEventAsync(string category, string action, string? label = null,
            object? value = null, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task TrackPageViewAsync(string? path = null,
            CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    [Fact]
    public void AddProvider_ShouldRegisterGenericProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new AnalyticsBuilder(services);

        // Act
        builder.AddProvider<CustomMockProvider>();
        var provider = services.BuildServiceProvider();

        // Assert
        var allProviders = provider.GetServices<IAnalyticsProvider>().ToList();
        allProviders.Count.ShouldBe(1);
        allProviders[0].ShouldBeOfType<CustomMockProvider>();
    }

    [Fact]
    public void AnalyticsService_ShouldReceiveAllRegisteredProviders()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new AnalyticsBuilder(services);
        services.AddLogging(); // Add logging to resolve ILogger dependencies

        // Add 3 different providers
        builder.AddClarity(options => options.SiteId = "test-clarity-id");
        builder.AddMatomo(options =>
        {
            options.SiteId = "test-matomo-id";
            options.TrackerUrl = "https://matomo.test/";
        });
        builder.AddProvider<CustomMockProvider>();

        // Add the analytics service
        services.AddSingleton<IAnalyticsService, AnalyticsService>();

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var analyticsService = serviceProvider.GetRequiredService<IAnalyticsService>();

        // Assert
        var providers = analyticsService.Providers.ToList();
        providers.Count.ShouldBe(3);
        providers.Any(p => p is ClarityProvider).ShouldBeTrue();
        providers.Any(p => p is MatomoProvider).ShouldBeTrue();
        providers.Any(p => p is CustomMockProvider).ShouldBeTrue();
    }
}