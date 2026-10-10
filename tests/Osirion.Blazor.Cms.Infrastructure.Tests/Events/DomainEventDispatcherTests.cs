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
    private readonly List<TestEventHandler> _handlers;

    // Test event classes
    public class TestEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public string Message { get; set; } = "Test Message";
    }

    public class TestEventHandler : IDomainEventHandler<TestEvent>
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
        _handlers = [handler1, handler2];

        // Setup service provider to return our handlers
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IDomainEventHandler<TestEvent>>(handler1);
        serviceCollection.AddSingleton<IDomainEventHandler<TestEvent>>(handler2);

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
        _serviceProvider.GetServices<IDomainEventHandler<TestEvent>>().Count().ShouldBe(2);
        _handlers.ShouldAllBe(h => h.HandlerCalled);
    }

    [Fact]
    public async Task DispatchAsync_WithNullEvent_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await _dispatcher.DispatchAsync<TestEvent>(null!));
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
            Arg.Is<object>(o => o == null ? false : o.ToString()!.Contains("No handlers found")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    public class ConcreteEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public class ConcreteEventHandler : IDomainEventHandler<ConcreteEvent>
    {
        public int Calls { get; private set; }

        public Task HandleAsync(ConcreteEvent domainEvent)
        {
            Calls++;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task DispatchAsync_WithEventTypedAsInterface_RunsHandlersOfTheRuntimeType()
    {
        // Arrange
        var handler = new ConcreteEventHandler();
        var services = new ServiceCollection();
        services.AddSingleton<IDomainEventHandler<ConcreteEvent>>(handler);
        var dispatcher = new DomainEventDispatcher(services.BuildServiceProvider(), _logger);
        IDomainEvent domainEvent = new ConcreteEvent();

        // Act
        await dispatcher.DispatchAsync(domainEvent);

        // Assert
        handler.Calls.ShouldBe(1);
    }
}
