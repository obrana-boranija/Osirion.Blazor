using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Infrastructure.Events;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Events;

public class DomainEventDispatcherTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DomainEventDispatcher> _logger;
    private readonly DomainEventDispatcher _dispatcher;

    // Test event classes
    public class TestEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public string Message { get; set; } = "Test Message";
    }

    public interface ITestEventHandler : IDomainEventHandler<TestEvent> { }

    public class TestEventHandler : ITestEventHandler
    {
        public bool HandlerCalled { get; private set; }

        public Task HandleAsync(TestEvent domainEvent)
        {
            HandlerCalled = true;
            return Task.CompletedTask;
        }
    }

    public DomainEventDispatcherTests()
    {
        _logger = Substitute.For<ILogger<DomainEventDispatcher>>();

        // Create test handlers
        var handler1 = new TestEventHandler();
        var handler2 = new TestEventHandler();

        // Setup service provider to return our handlers
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ITestEventHandler>(handler1);
        serviceCollection.AddSingleton<ITestEventHandler>(handler2);

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _dispatcher = new DomainEventDispatcher(_serviceProvider, _logger);
    }

    [Fact]
    public async Task DispatchAsync_WithValidEvent_CallsAllHandlers()
    {
        // Arrange
        var testEvent = new TestEvent { Message = "Test Event" };

        // Act
        await _dispatcher.DispatchAsync(testEvent);

        // Assert
        var handlers = _serviceProvider.GetServices<ITestEventHandler>().ToList();
        handlers.Count.ShouldBe(2);
        handlers.ShouldAllBe(h => ((TestEventHandler)h).HandlerCalled);
    }

    [Fact]
    public async Task DispatchAsync_WithNullEvent_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await _dispatcher.DispatchAsync<TestEvent>(null));
    }

    [Fact]
    public async Task DispatchAsync_WithNoHandlers_LogsWarning()
    {
        // Arrange
        var emptyServiceProvider = new ServiceCollection().BuildServiceProvider();
        var dispatcher = new DomainEventDispatcher(emptyServiceProvider, _logger);
        var testEvent = new TestEvent();

        // Act
        await dispatcher.DispatchAsync(testEvent);

        // Assert
        _logger.Received(1).Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("No handlers found")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }
}