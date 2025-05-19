using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Services;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services;

public class ContentProviderInitializerTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ContentProviderInitializer> _logger;
    private readonly ContentProviderInitializer _initializer;
    private readonly IContentProviderRegistry _registry;
    private readonly IContentProvider _standardProvider;
    private readonly IContentProvider _cachingProvider;
    private readonly IDefaultProviderSetter _defaultSetter;

    public ContentProviderInitializerTests()
    {
        _serviceProvider = Substitute.For<IServiceProvider>();
        _logger = Substitute.For<ILogger<ContentProviderInitializer>>();
        _registry = Substitute.For<IContentProviderRegistry>();
        _standardProvider = Substitute.For<IContentProvider>();
        _cachingProvider = (IContentProvider)Substitute.For<IContentCaching, IContentProvider>();
        _defaultSetter = Substitute.For<IDefaultProviderSetter>();

        // Configure mocks
        _standardProvider.ProviderId.Returns("standard-provider");
        _cachingProvider.ProviderId.Returns("caching-provider");

        _registry.GetAllProviders().Returns(new List<IContentProvider>
        {
            _standardProvider,
            _cachingProvider
        });

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(_registry);
        serviceCollection.AddSingleton(_defaultSetter);

        _serviceProvider.GetService(typeof(IContentProviderRegistry)).Returns(_registry);
        _serviceProvider.GetService(typeof(IEnumerable<IDefaultProviderSetter>))
            .Returns(new List<IDefaultProviderSetter> { _defaultSetter });
        _serviceProvider.GetRequiredService(typeof(IContentProviderRegistry)).Returns(_registry);

        _initializer = new ContentProviderInitializer(_serviceProvider, _logger);
    }

    [Fact]
    public async Task InitializeProvidersAsync_SetsDefaultProviders()
    {
        // Act
        await _initializer.InitializeProvidersAsync();

        // Assert
        _defaultSetter.Received(1).SetDefault(_serviceProvider);
    }

    [Fact]
    public async Task InitializeProvidersAsync_InitializesCachingProviders()
    {
        // Act
        await _initializer.InitializeProvidersAsync();

        // Assert
        await (_cachingProvider as IContentCaching).Received(1).InitializeAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InitializeProvidersAsync_LogsProviderInformation()
    {
        // Act
        await _initializer.InitializeProvidersAsync();

        // Assert
        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Found provider: standard-provider")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());

        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Found provider: caching-provider")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());

        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Initialized provider: caching-provider")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public async Task InitializeProvidersAsync_HandlesExceptionInDefaultSetter()
    {
        // Arrange
        _defaultSetter.When(x => x.SetDefault(Arg.Any<IServiceProvider>()))
            .Throw(new Exception("Test exception"));

        // Act
        await _initializer.InitializeProvidersAsync();

        // Assert
        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Is<Exception>(e => e.Message == "Test exception"),
            Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public async Task InitializeProvidersAsync_HandlesExceptionInProviderInitialization()
    {
        // Arrange
        (_cachingProvider as IContentCaching).When(x =>
            x.InitializeAsync(Arg.Any<CancellationToken>()))
            .Throw(new Exception("Initialization exception"));

        // Act
        await _initializer.InitializeProvidersAsync();

        // Assert
        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Is<Exception>(e => e.Message == "Initialization exception"),
            Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public async Task InitializeProvidersAsync_HandlesExceptionInRegistryAccess()
    {
        // Arrange
        _registry.When(x => x.GetAllProviders())
            .Throw(new Exception("Registry exception"));

        // Act
        await _initializer.InitializeProvidersAsync();

        // Assert
        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Is<Exception>(e => e.Message == "Registry exception"),
            Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public async Task InitializeProvidersAsync_WithNoProviders_LogsInformation()
    {
        // Arrange
        _registry.GetAllProviders().Returns(new List<IContentProvider>());

        // Act
        await _initializer.InitializeProvidersAsync();

        // Assert
        _logger.DidNotReceive().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Found provider:")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public async Task InitializeProvidersAsync_WithNoDefaultSetters_DoesNotThrow()
    {
        // Arrange
        _serviceProvider.GetService(typeof(IEnumerable<IDefaultProviderSetter>))
            .Returns(new List<IDefaultProviderSetter>());

        // Act - Should not throw
        await _initializer.InitializeProvidersAsync();

        // Assert - No exception
    }
}