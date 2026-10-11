#pragma warning disable CS0618 // The tests keep covering the types until they are removed
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Services;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Services;

public class ErrorHandlingServiceTests
{
    private readonly CmsState _state;
    private readonly CmsEventMediator _eventMediator;
    private readonly RecordingLogger<ErrorHandlingService> _logger;
    private readonly List<ErrorOccurredEvent> _errors;
    private readonly ErrorHandlingService _service;

    public ErrorHandlingServiceTests()
    {
        _state = new CmsState();
        _eventMediator = new CmsEventMediator(NullLogger<CmsEventMediator>.Instance);
        _logger = new RecordingLogger<ErrorHandlingService>();
        _service = new ErrorHandlingService(_state, _eventMediator, _logger);
        _errors = EventRecorder.Record<ErrorOccurredEvent>(_eventMediator);
    }

    [Fact]
    public void HandleException_ShouldLogErrorAndUpdateState()
    {
        // Arrange
        var exception = new InvalidOperationException("Test error");
        var context = "TestOperation";

        // Act
        _service.HandleException(exception, context);

        // Assert
        var logged = _logger.Entries.ShouldHaveSingleItem();
        logged.Level.ShouldBe(LogLevel.Error);
        logged.Exception.ShouldBe(exception);
        logged.Message.ShouldContain(context);
        logged.Message.ShouldContain(exception.Message);

        _state.ErrorMessage.ShouldNotBeNull();
        _state.ErrorMessage.ShouldContain(context);
        _state.ErrorMessage.ShouldContain(exception.Message);

        var published = _errors.ShouldHaveSingleItem();
        published.Message.ShouldContain(context);
        published.Message.ShouldContain(exception.Message);
        published.Exception.ShouldBe(exception);
    }

    [Fact]
    public void HandleException_WithUpdateStateFalse_ShouldNotUpdateState()
    {
        // Arrange
        var exception = new InvalidOperationException("Test error");
        var context = "TestOperation";

        // Act
        _service.HandleException(exception, context, updateState: false);

        // Assert
        var logged = _logger.Entries.ShouldHaveSingleItem();
        logged.Level.ShouldBe(LogLevel.Error);
        logged.Exception.ShouldBe(exception);
        logged.Message.ShouldContain(context);
        logged.Message.ShouldContain(exception.Message);

        _state.ErrorMessage.ShouldBeNull();

        var published = _errors.ShouldHaveSingleItem();
        published.Message.ShouldContain(context);
        published.Message.ShouldContain(exception.Message);
        published.Exception.ShouldBe(exception);
    }

    [Fact]
    public void HandleError_ShouldLogAndUpdateState()
    {
        // Arrange
        var errorEvent = new ErrorOccurredEvent("Test error message");

        // Act
        _eventMediator.Publish(errorEvent);

        // Assert
        var logged = _logger.Entries.ShouldHaveSingleItem();
        logged.Level.ShouldBe(LogLevel.Error);
        logged.Exception.ShouldBeNull();
        logged.Message.ShouldContain(errorEvent.Message);

        _state.ErrorMessage.ShouldBe(errorEvent.Message);
    }

    [Fact]
    public void HandleError_WithException_ShouldLogWithException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var errorEvent = new ErrorOccurredEvent("Test error message", exception);

        // Act
        _eventMediator.Publish(errorEvent);

        // Assert
        var logged = _logger.Entries.ShouldHaveSingleItem();
        logged.Level.ShouldBe(LogLevel.Error);
        logged.Exception.ShouldBe(exception);
        logged.Message.ShouldContain(errorEvent.Message);

        _state.ErrorMessage.ShouldBe(errorEvent.Message);
    }
}
