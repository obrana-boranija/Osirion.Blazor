namespace Osirion.Blazor.Cms.Application.Commands;

/// <summary>
/// Interface for command handlers in CQRS pattern
/// </summary>
/// <typeparam name="TCommand">Type of command to handle</typeparam>
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    /// <summary>
    /// Handles a command
    /// </summary>
    /// <param name="command">The command to handle</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}