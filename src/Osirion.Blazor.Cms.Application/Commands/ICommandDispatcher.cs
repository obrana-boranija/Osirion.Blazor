namespace Osirion.Blazor.Cms.Application.Commands;

/// <summary>
/// Interface for dispatching commands to their handlers
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Dispatches a command to its handler
    /// </summary>
    /// <typeparam name="TCommand">Type of command</typeparam>
    /// <param name="command">The command to dispatch</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;
}