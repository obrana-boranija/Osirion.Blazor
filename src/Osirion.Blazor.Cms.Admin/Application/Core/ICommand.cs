namespace Osirion.Blazor.Cms.Admin.Application.Core
{
    /// <summary>Defines the ICommand API contract.</summary>
    public interface ICommand { }

    /// <summary>Defines the ICommand API contract.</summary>
    public interface ICommand<TResult> : ICommand { }

    /// <summary>Defines the public member API contract.</summary>
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
    /// <summary>Performs the Handle operation asynchronously.</summary>
        Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }

    /// <summary>Defines the public member API contract.</summary>
    public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
    {
    /// <summary>Performs the Handle operation asynchronously.</summary>
        Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}
