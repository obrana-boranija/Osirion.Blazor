using Microsoft.Extensions.DependencyInjection;

namespace Osirion.Blazor.Cms.Application.Commands;

/// <summary>
/// Implementation of ICommandDispatcher that resolves handlers from DI
/// </summary>
public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>Performs the CommandDispatcher operation.</summary>
    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>Dispatches a command to its registered handler.</summary>
    public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        await handler.HandleAsync(command, cancellationToken);
    }
}
