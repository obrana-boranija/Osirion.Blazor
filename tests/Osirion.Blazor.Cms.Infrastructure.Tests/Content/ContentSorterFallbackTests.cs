using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Infrastructure.Content;
using Shouldly;
using SortDirection = Osirion.Blazor.Cms.Domain.Enums.SortDirection;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Content;

public class ContentSorterFallbackTests
{
    private readonly ContentSorter _sorter = new();

    private static IQueryable<ContentItem> Items()
    {
        var first = ContentItem.Create("first", "First", "Body", "first.md", "provider");
        first.SetSlug("b-slug");
        first.SetCreatedDate(new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc));
        var second = ContentItem.Create("second", "Second", "Body", "second.md", "provider");
        second.SetSlug("a-slug");
        second.SetCreatedDate(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var third = ContentItem.Create("third", "Third", "Body", "third.md", "provider");
        third.SetSlug("c-slug");
        third.SetCreatedDate(new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc));
        return new[] { first, second, third }.AsQueryable();
    }

    [Theory]
    [InlineData(SortDirection.Ascending, "second,first,third")]
    [InlineData(SortDirection.Descending, "third,first,second")]
    public void ApplySorting_BySlug_OrdersBySlug(SortDirection direction, string expected)
    {
        var result = _sorter.ApplySorting(Items(), SortField.Slug, direction);

        string.Join(",", result.Select(item => item.Id)).ShouldBe(expected);
    }

    [Theory]
    [InlineData(SortDirection.Ascending, "second,third,first")]
    [InlineData(SortDirection.Descending, "first,third,second")]
    public void ApplySorting_WithUnknownField_OrdersByCreationDate(SortDirection direction, string expected)
    {
        var result = _sorter.ApplySorting(Items(), (SortField)999, direction);

        string.Join(",", result.Select(item => item.Id)).ShouldBe(expected);
    }

    [Theory]
    [InlineData(SortDirection.Ascending, "second,third,first")]
    [InlineData(SortDirection.Descending, "first,third,second")]
    public void ApplySorting_ByPublishDate_UsesTheCreationDateWhenNoPublishDateIsSet(SortDirection direction, string expected)
    {
        var result = _sorter.ApplySorting(Items(), SortField.PublishDate, direction);

        string.Join(",", result.Select(item => item.Id)).ShouldBe(expected);
    }

    [Theory]
    [InlineData(SortDirection.Ascending, "short,medium,long")]
    [InlineData(SortDirection.Descending, "long,medium,short")]
    public void ApplySorting_ByReadTime_OrdersByWordCount(SortDirection direction, string expected)
    {
        static ContentItem WithWords(string id, int words) =>
            ContentItem.Create(id, id, string.Join(' ', Enumerable.Repeat("word", words)), id + ".md", "provider");
        var items = new[] { WithWords("medium", 1000), WithWords("long", 2000), WithWords("short", 10) }.AsQueryable();

        var result = _sorter.ApplySorting(items, SortField.ReadTime, direction);

        string.Join(",", result.Select(item => item.Id)).ShouldBe(expected);
    }
}
