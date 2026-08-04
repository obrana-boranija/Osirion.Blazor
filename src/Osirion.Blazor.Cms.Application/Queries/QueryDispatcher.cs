using Microsoft.Extensions.DependencyInjection;

namespace Osirion.Blazor.Cms.Application.Queries;

/// <summary>
/// Implementation of IQueryDispatcher that resolves handlers from DI
/// </summary>
public class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>Performs the QueryDispatcher operation.</summary>
    public QueryDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>Dispatches a query to its registered handler and returns the result.</summary>
    public async Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>
    {
        var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
        return await handler.HandleAsync(query, cancellationToken);
    }
}
