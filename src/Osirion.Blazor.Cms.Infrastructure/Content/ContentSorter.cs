using System.Linq.Expressions;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;

namespace Osirion.Blazor.Cms.Infrastructure.Content;

/// <summary>
/// Implementation of IContentSorter for sorting content items
/// </summary>
public class ContentSorter : IContentSorter
{
    private static readonly Dictionary<SortField, Func<IQueryable<ContentItem>, bool, IQueryable<ContentItem>>> SortersByField = new()
    {
        [SortField.Title] = By(item => item.Title),
        [SortField.Author] = By(item => item.Author),
        [SortField.LastModified] = By(item => item.LastModified ?? item.PublishDate),
        [SortField.Created] = By(item => item.DateCreated),
        [SortField.Order] = By(item => item.OrderIndex),
        [SortField.PublishDate] = By(item => item.PublishDate),
        [SortField.Slug] = By(item => item.Slug),
        [SortField.ReadTime] = By(item => item.ReadTimeMinutes)
    };

    // Fields without their own sorter, such as Date, sort by creation date.
    private static readonly Func<IQueryable<ContentItem>, bool, IQueryable<ContentItem>> DefaultSorter = By(item => item.DateCreated);

    /// <inheritdoc/>
    public IQueryable<ContentItem> ApplySorting(
        IQueryable<ContentItem> items,
        SortField sortField,
        SortDirection direction)
    {
        var sorter = SortersByField.GetValueOrDefault(sortField, DefaultSorter);
        return sorter(items, direction == SortDirection.Ascending);
    }

    private static Func<IQueryable<ContentItem>, bool, IQueryable<ContentItem>> By<TKey>(Expression<Func<ContentItem, TKey>> key) =>
        (items, ascending) => ascending ? items.OrderBy(key) : items.OrderByDescending(key);
}
