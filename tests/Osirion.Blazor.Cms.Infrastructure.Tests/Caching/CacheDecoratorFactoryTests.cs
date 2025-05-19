using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Caching;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Caching;

public class CacheDecoratorFactoryTests
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IOptions<CacheOptions> _cacheOptions;
    private readonly CacheDecoratorFactory _factory;
    private readonly IContentRepository _contentRepository;
    private readonly IDirectoryRepository _directoryRepository;

    public CacheDecoratorFactoryTests()
    {
        _memoryCache = Substitute.For<IMemoryCache>();
        _loggerFactory = Substitute.For<ILoggerFactory>();

        var logger = Substitute.For<ILogger<StaleWhileRevalidateCacheDecorator>>();
        var dirLogger = Substitute.For<ILogger<StaleWhileRevalidateDirectoryCache>>();

        _loggerFactory.CreateLogger<StaleWhileRevalidateCacheDecorator>().Returns(logger);
        _loggerFactory.CreateLogger<StaleWhileRevalidateDirectoryCache>().Returns(dirLogger);

        var cacheOptions = new CacheOptions
        {
            Enabled = true,
            MaxAgeMinutes = 30,
            StaleTimeMinutes = 5,
            CompactionPercentage = 0.25
        };

        _cacheOptions = Options.Create(cacheOptions);

        _factory = new CacheDecoratorFactory(_memoryCache, _loggerFactory, _cacheOptions);

        _contentRepository = Substitute.For<IContentRepository>();
        _directoryRepository = Substitute.For<IDirectoryRepository>();
    }

    [Fact]
    public void Constructor_InitializesProperties()
    {
        // No specific assertions needed as constructor initialization is tested implicitly
        // through other tests, but we can verify the factory was created
        _factory.ShouldNotBeNull();
    }

    [Fact]
    public void CreateContentCacheDecorator_WithCachingEnabled_ReturnsDecorator()
    {
        // Arrange
        string providerId = "test-provider";

        // Act
        var result = _factory.CreateContentCacheDecorator(_contentRepository, providerId);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<StaleWhileRevalidateCacheDecorator>();
        result.ShouldNotBe(_contentRepository); // Should be wrapped
    }

    [Fact]
    public void CreateContentCacheDecorator_WithCachingDisabled_ReturnsOriginalRepository()
    {
        // Arrange
        string providerId = "test-provider";

        var disabledOptions = new CacheOptions { Enabled = false };
        var options = Options.Create(disabledOptions);

        var factory = new CacheDecoratorFactory(_memoryCache, _loggerFactory, options);

        // Act
        var result = factory.CreateContentCacheDecorator(_contentRepository, providerId);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(_contentRepository); // Should be the same instance (not wrapped)
    }

    [Fact]
    public void CreateDirectoryCacheDecorator_WithCachingEnabled_ReturnsDecorator()
    {
        // Arrange
        string providerId = "test-provider";

        // Act
        var result = _factory.CreateDirectoryCacheDecorator(_directoryRepository, providerId);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<StaleWhileRevalidateDirectoryCache>();
        result.ShouldNotBe(_directoryRepository); // Should be wrapped
    }

    [Fact]
    public void CreateDirectoryCacheDecorator_WithCachingDisabled_ReturnsOriginalRepository()
    {
        // Arrange
        string providerId = "test-provider";

        var disabledOptions = new CacheOptions { Enabled = false };
        var options = Options.Create(disabledOptions);

        var factory = new CacheDecoratorFactory(_memoryCache, _loggerFactory, options);

        // Act
        var result = factory.CreateDirectoryCacheDecorator(_directoryRepository, providerId);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(_directoryRepository); // Should be the same instance (not wrapped)
    }

    [Fact]
    public void CreateContentCacheDecorator_ConfiguresDecoratorWithCorrectOptions()
    {
        // Arrange
        string providerId = "test-provider";

        // Act
        var result = _factory.CreateContentCacheDecorator(_contentRepository, providerId) as StaleWhileRevalidateCacheDecorator;

        // Assert
        result.ShouldNotBeNull();

        // Check if logger was created with correct type
        _loggerFactory.Received(1).CreateLogger<StaleWhileRevalidateCacheDecorator>();

        // Using reflection to verify private fields (not ideal but necessary for this case)
        var staleTimeField = typeof(StaleWhileRevalidateCacheDecorator)
            .GetField("_staleTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var maxAgeField = typeof(StaleWhileRevalidateCacheDecorator)
            .GetField("_maxAge", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var providerIdField = typeof(StaleWhileRevalidateCacheDecorator)
            .GetField("_providerIdentifier", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        staleTimeField.ShouldNotBeNull();
        maxAgeField.ShouldNotBeNull();
        providerIdField.ShouldNotBeNull();

        var staleTime = staleTimeField.GetValue(result) as TimeSpan?;
        var maxAge = maxAgeField.GetValue(result) as TimeSpan?;
        var resultProviderId = providerIdField.GetValue(result) as string;

        staleTime.ShouldNotBeNull();
        maxAge.ShouldNotBeNull();
        resultProviderId.ShouldNotBeNull();

        staleTime.Value.TotalMinutes.ShouldBe(5);
        maxAge.Value.TotalMinutes.ShouldBe(30);
        resultProviderId.ShouldBe(providerId);
    }
}