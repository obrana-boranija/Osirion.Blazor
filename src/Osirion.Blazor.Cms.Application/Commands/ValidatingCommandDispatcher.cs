using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Application.Validation;
using Osirion.Blazor.Cms.Domain.Exceptions;

namespace Osirion.Blazor.Cms.Application.Commands;

/// <summary>
/// Command dispatcher that validates commands before dispatching them
/// </summary>
public class ValidatingCommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidatingCommandDispatcher"/> class.
    /// </summary>
    /// <param name="serviceProvider"></param>
    public ValidatingCommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Dispatches a command to its handler after validating it.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        await ValidateCommandAsync(command, cancellationToken);

        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        await handler.HandleAsync(command, cancellationToken);
    }

    private async Task ValidateCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand
    {
        // Get validator if exists
        var validator = _serviceProvider.GetService<IValidator<TCommand>>();

        if (validator is not null)
        {
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new ContentValidationException("Validation failed", validationResult.Errors);
            }
        }
    }
}