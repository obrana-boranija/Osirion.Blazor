using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Domain.Interfaces.Content;

/// <summary>
/// Responsible for filtering content based on queries
/// </summary>
public interface IContentQueryFilter
{
    /// <summary>
    /// Applies query filters to content items
    /// </summary>
    /// <param name="items">Items to filter</param>
    /// <param name="query">Query criteria</param>
    /// <returns>Filtered items</returns>
    IQueryable<ContentItem> ApplyFilters(IQueryable<ContentItem> items, ContentQuery query);
}