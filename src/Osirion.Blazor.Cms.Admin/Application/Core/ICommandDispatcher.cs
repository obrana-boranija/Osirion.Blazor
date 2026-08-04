namespace Osirion.Blazor.Cms.Admin.Application.Core;

    /// <summary>Defines the public member API contract.</summary>
public interface ICommandDispatcher
{
    /// <summary>Gets or sets the public member value.</summary>
    Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    /// <summary>Gets or sets the public member value.</summary>
    Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>;
}
