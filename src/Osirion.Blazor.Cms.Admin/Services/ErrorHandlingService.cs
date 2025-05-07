using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Services.State;

namespace Osirion.Blazor.Cms.Admin.Services;

public class ErrorHandlingService
{
    private readonly CmsApplicationState _state;
    private readonly CmsEventMediator _eventMediator;
    private readonly ILogger<ErrorHandlingService> _logger;

    public ErrorHandlingService(
        CmsApplicationState state,
        CmsEventMediator eventMediator,
        ILogger<ErrorHandlingService> logger)
    {
        _state = state;
        _eventMediator = eventMediator;
        _logger = logger;

        // Subscribe to error events
        _eventMediator.Subscribe<ErrorOccurredEvent>(HandleError);
    }

    private void HandleError(ErrorOccurredEvent errorEvent)
    {
        // Log the error
        if (errorEvent.Exception != null)
        {
            _logger.LogError(
                errorEvent.Exception,
                "Application error: {Message}",
                errorEvent.Message);
        }
        else
        {
            _logger.LogError("Application error: {Message}", errorEvent.Message);
        }

        // Update application state
        _state.SetErrorMessage(errorEvent.Message);
    }

    public void HandleException(Exception exception, string context, bool updateState = true)
    {
        var message = $"Error in {context}: {exception.Message}";

        _logger.LogError(exception, message);

        if (updateState)
        {
            _state.SetErrorMessage(message);
        }

        // Publish error event for other components to react
        _eventMediator.Publish(new ErrorOccurredEvent(message, exception));
    }
}