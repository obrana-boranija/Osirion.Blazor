using Microsoft.Extensions.Logging;

namespace Osirion.Blazor.Cms.Admin.Common.Extensions;

/// <summary>
/// Makes the outcome of intentionally un-awaited work observable. A discarded task or an <c>async void</c>
/// handler would otherwise lose its exception (or, for <c>async void</c>, crash the circuit).
/// </summary>
internal static class TaskObserverExtensions
{
    /// <summary>
    /// Logs the exception of a task nobody awaits. Cancellation is not an error and is not logged.
    /// </summary>
    /// <param name="task">The task to observe.</param>
    /// <param name="logger">The logger that receives the failure.</param>
    /// <param name="operation">A short description of the work, used in the log message.</param>
    public static void Observe(this Task task, ILogger logger, string operation)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(logger);

        if (task.IsCompletedSuccessfully)
        {
            return;
        }

        _ = ObserveAsync(task, logger, operation);
    }

    private static async Task ObserveAsync(Task task, ILogger logger, string operation)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Cancellation is expected when the owner goes away.
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Background operation {Operation} failed", operation);
        }
    }
}
