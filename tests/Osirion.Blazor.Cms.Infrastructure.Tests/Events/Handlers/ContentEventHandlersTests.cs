using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Infrastructure.Events.Handlers;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Events.Handlers;

public class ContentEventHandlersTests
{
    [Fact]
    public async Task ContentCreatedEventHandler_LogsEventInformation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ContentCreatedEventHandler>>();
        var handler = new ContentCreatedEventHandler(logger);

        var eventData = new ContentCreatedEvent(
            "content-id",
            "Test Content",
            "content/test.md",
            "test-provider");

        // Act
        await handler.HandleAsync(eventData);

        // Assert
        logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Content item created: ID content-id")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public async Task ContentDeletedEventHandler_LogsEventInformation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ContentDeletedEventHandler>>();
        var handler = new ContentDeletedEventHandler(logger);

        var eventData = new ContentDeletedEvent(
            "content-id",
            "content/test.md",
            "test-provider");

        // Act
        await handler.HandleAsync(eventData);

        // Assert
        logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Content item deleted: ID content-id")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public async Task ContentStatusChangedEventHandler_LogsEventInformation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ContentStatusChangedEventHandler>>();
        var handler = new ContentStatusChangedEventHandler(logger);

        var eventData = new ContentStatusChangedEvent(
            "content-id",
            "Test Content",
            Domain.Enums.ContentStatus.Draft,
            Domain.Enums.ContentStatus.Published,
            "test-provider");

        // Act
        await handler.HandleAsync(eventData);

        // Assert
        logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Content item status changed: ID content-id")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public async Task ContentUpdatedEventHandler_LogsEventInformation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ContentUpdatedEventHandler>>();
        var handler = new ContentUpdatedEventHandler(logger);

        var eventData = new ContentUpdatedEvent(
            "content-id",
            "Test Content",
            "content/test.md",
            "test-provider");

        // Act
        await handler.HandleAsync(eventData);

        // Assert
        logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Content item updated: ID content-id")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public void ContentCreatedEventHandler_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new ContentCreatedEventHandler(null))
            .ParamName.ShouldBe("logger");
    }

    [Fact]
    public void ContentDeletedEventHandler_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new ContentDeletedEventHandler(null))
            .ParamName.ShouldBe("logger");
    }

    [Fact]
    public void ContentStatusChangedEventHandler_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new ContentStatusChangedEventHandler(null))
            .ParamName.ShouldBe("logger");
    }

    [Fact]
    public void ContentUpdatedEventHandler_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new ContentUpdatedEventHandler(null))
            .ParamName.ShouldBe("logger");
    }
}