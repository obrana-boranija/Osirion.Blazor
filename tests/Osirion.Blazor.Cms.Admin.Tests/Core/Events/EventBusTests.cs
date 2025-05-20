using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Core.Events;

public class EventBusTests
{
    private readonly ILogger<EventBus> _logger;
    private readonly EventBus _eventBus;

    public EventBusTests()
    {
        _logger = Substitute.For<ILogger<EventBus>>();
        _eventBus = new EventBus(_logger);
    }

    [Fact]
    public void Subscribe_ShouldRegisterHandlerForEvent()
    {
        // Arrange
        var eventReceived = false;
        TestEvent? receivedEvent = null;

        // Act
        _eventBus.Subscribe<TestEvent>(e =>
        {
            eventReceived = true;
            receivedEvent = e;
        });

        var testEvent = new TestEvent("Test Message");
        _eventBus.Publish(testEvent);

        // Assert
        eventReceived.ShouldBeTrue();
        receivedEvent.ShouldNotBeNull();
        receivedEvent.Message.ShouldBe("Test Message");
    }

    [Fact]
    public void Unsubscribe_ShouldRemoveHandlerForEvent()
    {
        // Arrange
        var callCount = 0;
        Action<TestEvent> handler = _ => callCount++;

        _eventBus.Subscribe(handler);

        // Verify subscription works
        _eventBus.Publish(new TestEvent("Test"));
        callCount.ShouldBe(1);

        // Act
        _eventBus.Unsubscribe(handler);

        // Publish again
        _eventBus.Publish(new TestEvent("Test Again"));

        // Assert
        callCount.ShouldBe(1); // Should still be 1 since we unsubscribed
    }

    [Fact]
    public void Publish_ShouldInvokeAllHandlers()
    {
        // Arrange
        var handlerOneCallCount = 0;
        var handlerTwoCallCount = 0;

        _eventBus.Subscribe<TestEvent>(_ => handlerOneCallCount++);
        _eventBus.Subscribe<TestEvent>(_ => handlerTwoCallCount++);

        // Act
        _eventBus.Publish(new TestEvent("Test"));

        // Assert
        handlerOneCallCount.ShouldBe(1);
        handlerTwoCallCount.ShouldBe(1);
    }

    [Fact]
    public void Publish_WithNoHandlers_ShouldNotThrowException()
    {
        // Act & Assert
        Should.NotThrow(() => _eventBus.Publish(new AnotherTestEvent()));
    }

    [Fact]
    public void Publish_WithErrorInHandler_ShouldNotAffectOtherHandlers()
    {
        // Arrange
        var successfulHandlerCalled = false;

        _eventBus.Subscribe<TestEvent>(_ => throw new InvalidOperationException("Test exception"));
        _eventBus.Subscribe<TestEvent>(_ => successfulHandlerCalled = true);

        // Act
        _eventBus.Publish(new TestEvent("Test"));

        // Assert
        successfulHandlerCalled.ShouldBeTrue();
        _logger.Received().Log(
            Arg.Is<LogLevel>(l => l == LogLevel.Error),
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void Publish_WithDifferentEventTypes_ShouldOnlyInvokeMatchingHandlers()
    {
        // Arrange
        var testEventCalled = false;
        var anotherTestEventCalled = false;

        _eventBus.Subscribe<TestEvent>(_ => testEventCalled = true);
        _eventBus.Subscribe<AnotherTestEvent>(_ => anotherTestEventCalled = true);

        // Act
        _eventBus.Publish(new TestEvent("Test"));

        // Assert
        testEventCalled.ShouldBeTrue();
        anotherTestEventCalled.ShouldBeFalse();
    }

    // Test event classes
    private class TestEvent
    {
        public string Message { get; }

        public TestEvent(string message)
        {
            Message = message;
        }
    }

    private class AnotherTestEvent
    {
    }
}