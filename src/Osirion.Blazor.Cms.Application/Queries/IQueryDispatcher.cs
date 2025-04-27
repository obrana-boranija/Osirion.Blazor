namespace Osirion.Blazor.Cms.Application.Queries;

/// <summary>
/// Interface for dispatching queries to their handlers
/// </summary>
public interface IQueryDispatcher
{
    /// <summary>
    /// Dispatches a query to its handler
    /// </summary>
    /// <typeparam name="TQuery">Type of query</typeparam>
    /// <typeparam name="TResult">Type of result</typeparam>
    /// <param name="query">The query to dispatch</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The query result</returns>
    Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>;
}