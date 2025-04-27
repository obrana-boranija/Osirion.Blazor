namespace Osirion.Blazor.Cms.Application.Queries;

/// <summary>
/// Interface for query handlers in CQRS pattern
/// </summary>
/// <typeparam name="TQuery">Type of query to handle</typeparam>
/// <typeparam name="TResult">Type of result returned by the query</typeparam>
public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Handles a query
    /// </summary>
    /// <param name="query">The query to handle</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The query result</returns>
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}