using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;

namespace Osirion.Blazor.Cms.Infrastructure.Content;

/// <summary>
/// Implementation of IContentSorter for sorting content items
/// </summary>
public class ContentSorter : IContentSorter
{
    /// <inheritdoc/>
    public IQueryable<ContentItem> ApplySorting(
        IQueryable<ContentItem> items,
        SortField sortField,
        SortDirection direction)
    {
        return sortField switch
        {
            SortField.Title => direction == SortDirection.Ascending ?
                items.OrderBy(item => item.Title) :
                items.OrderByDescending(item => item.Title),

            SortField.Author => direction == SortDirection.Ascending ?
                items.OrderBy(item => item.Author) :
                items.OrderByDescending(item => item.Author),

            SortField.LastModified => direction == SortDirection.Ascending ?
                items.OrderBy(item => item.LastModified ?? item.PublishDate) :
                items.OrderByDescending(item => item.LastModified ?? item.PublishDate),

            SortField.Created => direction == SortDirection.Ascending ?
                items.OrderBy(item => item.DateCreated) :
                items.OrderByDescending(item => item.DateCreated),

            SortField.Order => direction == SortDirection.Ascending ?
                items.OrderBy(item => item.OrderIndex) :
                items.OrderByDescending(item => item.OrderIndex),

            SortField.PublishDate => direction == SortDirection.Ascending ?
                items.OrderBy(item => item.PublishDate) :
                items.OrderByDescending(item => item.PublishDate),

            SortField.Slug => direction == SortDirection.Ascending ?
                items.OrderBy(item => item.Slug) :
                items.OrderByDescending(item => item.Slug),

            SortField.ReadTime => direction == SortDirection.Ascending ?
                items.OrderBy(item => item.ReadTimeMinutes) :
                items.OrderByDescending(item => item.ReadTimeMinutes),

            _ => direction == SortDirection.Ascending ?
                items.OrderBy(item => item.DateCreated) :
                items.OrderByDescending(item => item.DateCreated)
        };
    }
}