namespace Osirion.Blazor.Cms.Application.Queries;

/// <summary>
/// Marker interface for query objects in CQRS pattern
/// </summary>
/// <typeparam name="TResult">Type of the query result</typeparam>
public interface IQuery<out TResult> { }