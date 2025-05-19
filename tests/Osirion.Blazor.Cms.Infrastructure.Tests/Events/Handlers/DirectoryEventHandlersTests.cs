using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Infrastructure.Events.Handlers;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Events.Handlers;

public class DirectoryEventHandlersTests
{
    [Fact]
    public async Task DirectoryCreatedEventHandler_LogsEventInformation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<DirectoryCreatedEventHandler>>();
        var handler = new DirectoryCreatedEventHandler(logger);

        var eventData = new DirectoryCreatedEvent(
            "dir-id",
            "Test Directory",
            "directories/test",
            "test-provider");

        // Act
        await handler.HandleAsync(eventData);

        // Assert
        logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Directory created: ID dir-id")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public async Task DirectoryDeletedEventHandler_LogsEventInformation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<DirectoryDeletedEventHandler>>();
        var handler = new DirectoryDeletedEventHandler(logger);

        var eventData = new DirectoryDeletedEvent(
            "dir-id",
            "directories/test",
            "test-provider",
            true);

        // Act
        await handler.HandleAsync(eventData);

        // Assert
        logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Directory deleted: ID dir-id")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public async Task DirectoryUpdatedEventHandler_LogsEventInformation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<DirectoryUpdatedEventHandler>>();
        var handler = new DirectoryUpdatedEventHandler(logger);

        var eventData = new DirectoryUpdatedEvent(
            "dir-id",
            "Test Directory",
            "directories/test",
            "test-provider");

        // Act
        await handler.HandleAsync(eventData);

        // Assert
        logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Directory updated: ID dir-id")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public void DirectoryCreatedEventHandler_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new DirectoryCreatedEventHandler(null))
            .ParamName.ShouldBe("logger");
    }

    [Fact]
    public void DirectoryDeletedEventHandler_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new DirectoryDeletedEventHandler(null))
            .ParamName.ShouldBe("logger");
    }

    [Fact]
    public void DirectoryUpdatedEventHandler_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new DirectoryUpdatedEventHandler(null))
            .ParamName.ShouldBe("logger");
    }

    [Fact]
    public void DirectoryCreatedEventHandler_ConstructorInitializesProperties()
    {
        // Arrange
        var logger = Substitute.For<ILogger<DirectoryCreatedEventHandler>>();

        // Act
        var handler = new DirectoryCreatedEventHandler(logger);

        // Assert
        var loggerField = typeof(DirectoryCreatedEventHandler)
            .GetField("_logger", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        loggerField.ShouldNotBeNull();
        var handlerLogger = loggerField.GetValue(handler);
        handlerLogger.ShouldBe(logger);
    }

    [Fact]
    public void DirectoryDeletedEventHandler_ConstructorInitializesProperties()
    {
        // Arrange
        var logger = Substitute.For<ILogger<DirectoryDeletedEventHandler>>();

        // Act
        var handler = new DirectoryDeletedEventHandler(logger);

        // Assert
        var loggerField = typeof(DirectoryDeletedEventHandler)
            .GetField("_logger", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        loggerField.ShouldNotBeNull();
        var handlerLogger = loggerField.GetValue(handler);
        handlerLogger.ShouldBe(logger);
    }

    [Fact]
    public void DirectoryUpdatedEventHandler_ConstructorInitializesProperties()
    {
        // Arrange
        var logger = Substitute.For<ILogger<DirectoryUpdatedEventHandler>>();

        // Act
        var handler = new DirectoryUpdatedEventHandler(logger);

        // Assert
        var loggerField = typeof(DirectoryUpdatedEventHandler)
            .GetField("_logger", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        loggerField.ShouldNotBeNull();
        var handlerLogger = loggerField.GetValue(handler);
        handlerLogger.ShouldBe(logger);
    }

    [Fact]
    public async Task DirectoryCreatedEventHandler_HandlesNullEvent_ThrowsArgumentNullException()
    {
        // Arrange
        var logger = Substitute.For<ILogger<DirectoryCreatedEventHandler>>();
        var handler = new DirectoryCreatedEventHandler(logger);

        // Act & Assert
        await Should.ThrowAsync<NullReferenceException>(async () =>
            await handler.HandleAsync(null!));
    }

    [Fact]
    public async Task DirectoryDeletedEventHandler_HandlesNullEvent_ThrowsArgumentNullException()
    {
        // Arrange
        var logger = Substitute.For<ILogger<DirectoryDeletedEventHandler>>();
        var handler = new DirectoryDeletedEventHandler(logger);

        // Act & Assert
        await Should.ThrowAsync<NullReferenceException>(async () =>
            await handler.HandleAsync(null!));
    }

    [Fact]
    public async Task DirectoryUpdatedEventHandler_HandlesNullEvent_ThrowsArgumentNullException()
    {
        // Arrange
        var logger = Substitute.For<ILogger<DirectoryUpdatedEventHandler>>();
        var handler = new DirectoryUpdatedEventHandler(logger);

        // Act & Assert
        await Should.ThrowAsync<NullReferenceException>(async () =>
            await handler.HandleAsync(null!));
    }
}