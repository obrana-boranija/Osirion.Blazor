using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;

namespace Osirion.Blazor.Cms.Domain.Interfaces.Content;

/// <summary>
/// Responsible for sorting content items
/// </summary>
public interface IContentSorter
{
    /// <summary>
    /// Applies sorting to content items
    /// </summary>
    /// <param name="items">Items to sort</param>
    /// <param name="sortField">Field to sort by</param>
    /// <param name="direction">Sort direction</param>
    /// <returns>Sorted items</returns>
    IQueryable<ContentItem> ApplySorting(
        IQueryable<ContentItem> items,
        SortField sortField,
        SortDirection direction);
}