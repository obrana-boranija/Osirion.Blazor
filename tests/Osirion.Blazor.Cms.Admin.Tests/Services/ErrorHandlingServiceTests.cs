using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Services;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Services;

public class ErrorHandlingServiceTests
{
    private readonly CmsState _state;
    private readonly CmsEventMediator _eventMediator;
    private readonly ILogger<ErrorHandlingService> _logger;
    private readonly ErrorHandlingService _service;

    public ErrorHandlingServiceTests()
    {
        _state = new CmsState();
        _eventMediator = Substitute.For<CmsEventMediator>(Substitute.For<ILogger<CmsEventMediator>>());
        _logger = Substitute.For<ILogger<ErrorHandlingService>>();
        _service = new ErrorHandlingService(_state, _eventMediator, _logger);
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
        _logger.Received().LogError(
            Arg.Is(exception),
            Arg.Is<string>(s => s.Contains(context) && s.Contains(exception.Message))
        );

        _state.ErrorMessage.ShouldNotBeNull();
        _state.ErrorMessage.ShouldContain(context);
        _state.ErrorMessage.ShouldContain(exception.Message);

        _eventMediator.Received().Publish(
            Arg.Is<ErrorOccurredEvent>(e =>
                e.Message.Contains(context) &&
                e.Message.Contains(exception.Message) &&
                e.Exception == exception)
        );
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
        _logger.Received().LogError(
            Arg.Is(exception),
            Arg.Is<string>(s => s.Contains(context) && s.Contains(exception.Message))
        );

        _state.ErrorMessage.ShouldBeNull(); // State should not be updated

        _eventMediator.Received().Publish(
            Arg.Is<ErrorOccurredEvent>(e =>
                e.Message.Contains(context) &&
                e.Message.Contains(exception.Message) &&
                e.Exception == exception)
        );
    }

    [Fact]
    public void HandleError_ShouldLogAndUpdateState()
    {
        // Arrange
        var errorEvent = new ErrorOccurredEvent("Test error message");

        // Access the private HandleError method through reflection to test it
        var method = typeof(ErrorHandlingService).GetMethod("HandleError",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act
        method?.Invoke(_service, new object[] { errorEvent });

        // Assert
        _logger.Received().LogError(
            Arg.Is<string>(s => s.Contains(errorEvent.Message))
        );

        _state.ErrorMessage.ShouldBe(errorEvent.Message);
    }

    [Fact]
    public void HandleError_WithException_ShouldLogWithException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var errorEvent = new ErrorOccurredEvent("Test error message", exception);

        // Access the private HandleError method through reflection to test it
        var method = typeof(ErrorHandlingService).GetMethod("HandleError",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act
        method?.Invoke(_service, new object[] { errorEvent });

        // Assert
        _logger.Received().LogError(
            Arg.Is(exception),
            Arg.Is<string>(s => s.Contains(errorEvent.Message))
        );

        _state.ErrorMessage.ShouldBe(errorEvent.Message);
    }
}