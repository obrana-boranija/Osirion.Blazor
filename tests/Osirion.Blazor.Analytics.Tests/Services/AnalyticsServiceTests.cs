using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Analytics.Services;
using Shouldly;

namespace Osirion.Blazor.Analytics.Tests.Services;

public class AnalyticsServiceTests
{
    [Fact]
    public async Task TrackPageViewAsync_ShouldCallAllEnabledProviders()
    {
        // Arrange
        var mockProvider1 = Substitute.For<IAnalyticsProvider>();
        mockProvider1.IsEnabled.Returns(true);
        mockProvider1.ProviderId.Returns("provider1");

        var mockProvider2 = Substitute.For<IAnalyticsProvider>();
        mockProvider2.IsEnabled.Returns(true);
        mockProvider2.ProviderId.Returns("provider2");

        var mockProvider3 = Substitute.For<IAnalyticsProvider>();
        mockProvider3.IsEnabled.Returns(false);
        mockProvider3.ProviderId.Returns("provider3");

        var providers = new[] { mockProvider1, mockProvider2, mockProvider3 };
        var logger = Substitute.For<ILogger<AnalyticsService>>();

        var service = new AnalyticsService(providers, logger);

        // Act
        await service.TrackPageViewAsync("/test-path");

        // Assert
        await mockProvider1.Received(1).TrackPageViewAsync("/test-path", Arg.Any<CancellationToken>());
        await mockProvider2.Received(1).TrackPageViewAsync("/test-path", Arg.Any<CancellationToken>());
        await mockProvider3.DidNotReceive().TrackPageViewAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task TrackEventAsync_ShouldCallAllEnabledProviders()
    {
        // Arrange
        var mockProvider1 = Substitute.For<IAnalyticsProvider>();
        mockProvider1.IsEnabled.Returns(true);
        mockProvider1.ProviderId.Returns("provider1");

        var mockProvider2 = Substitute.For<IAnalyticsProvider>();
        mockProvider2.IsEnabled.Returns(true);
        mockProvider2.ProviderId.Returns("provider2");

        var mockProvider3 = Substitute.For<IAnalyticsProvider>();
        mockProvider3.IsEnabled.Returns(false);
        mockProvider3.ProviderId.Returns("provider3");

        var providers = new[] { mockProvider1, mockProvider2, mockProvider3 };
        var logger = Substitute.For<ILogger<AnalyticsService>>();

        var service = new AnalyticsService(providers, logger);

        // Act
        await service.TrackEventAsync("category", "action", "label", 123);

        // Assert
        await mockProvider1.Received(1).TrackEventAsync("category", "action", "label", 123, Arg.Any<CancellationToken>());
        await mockProvider2.Received(1).TrackEventAsync("category", "action", "label", 123, Arg.Any<CancellationToken>());
        await mockProvider3.DidNotReceive().TrackEventAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task TrackPageViewAsync_ShouldHandleExceptions_AndContinueToNextProvider()
    {
        // Arrange
        var mockProvider1 = Substitute.For<IAnalyticsProvider>();
        mockProvider1.IsEnabled.Returns(true);
        mockProvider1.ProviderId.Returns("provider1");
        mockProvider1.When(p => p.TrackPageViewAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()))
            .Do(_ => throw new Exception("Test exception"));

        var mockProvider2 = Substitute.For<IAnalyticsProvider>();
        mockProvider2.IsEnabled.Returns(true);
        mockProvider2.ProviderId.Returns("provider2");

        var providers = new[] { mockProvider1, mockProvider2 };
        var logger = Substitute.For<ILogger<AnalyticsService>>();

        var service = new AnalyticsService(providers, logger);

        // Act
        await service.TrackPageViewAsync("/test-path");

        // Assert
        // Verify the second provider still gets called, even after the first one throws
        await mockProvider2.Received(1).TrackPageViewAsync("/test-path", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task TrackEventAsync_ShouldHandleExceptions_AndContinueToNextProvider()
    {
        // Arrange
        var mockProvider1 = Substitute.For<IAnalyticsProvider>();
        mockProvider1.IsEnabled.Returns(true);
        mockProvider1.ProviderId.Returns("provider1");
        mockProvider1.When(p => p.TrackEventAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<object>(), Arg.Any<CancellationToken>()))
            .Do(_ => throw new Exception("Test exception"));

        var mockProvider2 = Substitute.For<IAnalyticsProvider>();
        mockProvider2.IsEnabled.Returns(true);
        mockProvider2.ProviderId.Returns("provider2");

        var providers = new[] { mockProvider1, mockProvider2 };
        var logger = Substitute.For<ILogger<AnalyticsService>>();

        var service = new AnalyticsService(providers, logger);

        // Act
        await service.TrackEventAsync("category", "action");

        // Assert
        // Verify the second provider still gets called, even after the first one throws
        await mockProvider2.Received(1).TrackEventAsync("category", "action", null, null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public void GetProvider_ShouldReturnCorrectProvider_WhenFound()
    {
        // Arrange
        var mockProvider1 = Substitute.For<IAnalyticsProvider>();
        mockProvider1.ProviderId.Returns("provider1");

        var mockProvider2 = Substitute.For<IAnalyticsProvider>();
        mockProvider2.ProviderId.Returns("provider2");

        var providers = new[] { mockProvider1, mockProvider2 };
        var logger = Substitute.For<ILogger<AnalyticsService>>();

        var service = new AnalyticsService(providers, logger);

        // Act
        var result = service.GetProvider("provider2");

        // Assert
        result.ShouldBe(mockProvider2);
    }

    [Fact]
    public void GetProvider_ShouldReturnNull_WhenProviderNotFound()
    {
        // Arrange
        var mockProvider1 = Substitute.For<IAnalyticsProvider>();
        mockProvider1.ProviderId.Returns("provider1");

        var mockProvider2 = Substitute.For<IAnalyticsProvider>();
        mockProvider2.ProviderId.Returns("provider2");

        var providers = new[] { mockProvider1, mockProvider2 };
        var logger = Substitute.For<ILogger<AnalyticsService>>();

        var service = new AnalyticsService(providers, logger);

        // Act
        var result = service.GetProvider("unknown");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void GetProvider_ShouldBeCaseInsensitive()
    {
        // Arrange
        var mockProvider = Substitute.For<IAnalyticsProvider>();
        mockProvider.ProviderId.Returns("Provider1");

        var providers = new[] { mockProvider };
        var logger = Substitute.For<ILogger<AnalyticsService>>();

        var service = new AnalyticsService(providers, logger);

        // Act
        var result = service.GetProvider("provider1");

        // Assert
        result.ShouldBe(mockProvider);
    }

    [Fact]
    public void Providers_ShouldReturnReadOnlyListOfAllProviders()
    {
        // Arrange
        var mockProvider1 = Substitute.For<IAnalyticsProvider>();
        var mockProvider2 = Substitute.For<IAnalyticsProvider>();

        var providers = new[] { mockProvider1, mockProvider2 };
        var logger = Substitute.For<ILogger<AnalyticsService>>();

        var service = new AnalyticsService(providers, logger);

        // Act
        var result = service.Providers;

        // Assert
        result.Count.ShouldBe(2);
        result.ShouldContain(mockProvider1);
        result.ShouldContain(mockProvider2);
    }
}