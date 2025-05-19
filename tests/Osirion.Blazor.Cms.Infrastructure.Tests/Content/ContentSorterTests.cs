using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Infrastructure.Content;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Content;

public class ContentSorterTests
{
    private readonly ContentSorter _sorter;
    private readonly List<ContentItem> _testItems;

    public ContentSorterTests()
    {
        _sorter = new ContentSorter();

        // Create test data
        _testItems = new List<ContentItem>
        {
            ContentItem.Create("id1", "B Title", "Content 1", "path1.md", "test-provider"),
            ContentItem.Create("id2", "A Title", "Content 2", "path2.md", "test-provider"),
            ContentItem.Create("id3", "C Title", "Content 3", "path3.md", "test-provider")
        };

        // Set additional properties for testing different sort fields
        _testItems[0].SetAuthor("Author B");
        _testItems[0].SetCreatedDate(new DateTime(2023, 1, 2));
        _testItems[0].SetSlug("b-title");
        _testItems[0].SetOrderIndex(2);

        _testItems[1].SetAuthor("Author A");
        _testItems[1].SetCreatedDate(new DateTime(2023, 1, 3));
        _testItems[1].SetLastModifiedDate(new DateTime(2023, 2, 1));
        _testItems[1].SetSlug("a-title");
        _testItems[1].SetOrderIndex(1);

        _testItems[2].SetAuthor("Author C");
        _testItems[2].SetCreatedDate(new DateTime(2023, 1, 1));
        _testItems[2].SetLastModifiedDate(new DateTime(2023, 2, 2));
        _testItems[2].SetSlug("c-title");
        _testItems[2].SetOrderIndex(3);
    }

    [Fact]
    public void ApplySorting_SortByTitle_Ascending()
    {
        // Act
        var result = _sorter.ApplySorting(
            _testItems.AsQueryable(),
            SortField.Title,
            Domain.Enums.SortDirection.Ascending).ToList();

        // Assert
        result.Count.ShouldBe(3);
        result[0].Title.ShouldBe("A Title");
        result[1].Title.ShouldBe("B Title");
        result[2].Title.ShouldBe("C Title");
    }

    [Fact]
    public void ApplySorting_SortByTitle_Descending()
    {
        // Act
        var result = _sorter.ApplySorting(
            _testItems.AsQueryable(),
            SortField.Title,
            Domain.Enums.SortDirection.Descending).ToList();

        // Assert
        result.Count.ShouldBe(3);
        result[0].Title.ShouldBe("C Title");
        result[1].Title.ShouldBe("B Title");
        result[2].Title.ShouldBe("A Title");
    }

    [Fact]
    public void ApplySorting_SortByAuthor()
    {
        // Act
        var result = _sorter.ApplySorting(
            _testItems.AsQueryable(),
            SortField.Author,
            Domain.Enums.SortDirection.Ascending).ToList();

        // Assert
        result.Count.ShouldBe(3);
        result[0].Author.ShouldBe("Author A");
        result[1].Author.ShouldBe("Author B");
        result[2].Author.ShouldBe("Author C");
    }

    [Fact]
    public void ApplySorting_SortByCreated()
    {
        // Act
        var result = _sorter.ApplySorting(
            _testItems.AsQueryable(),
            SortField.Created,
            Domain.Enums.SortDirection.Ascending).ToList();

        // Assert
        result.Count.ShouldBe(3);
        result[0].DateCreated.ShouldBe(new DateTime(2023, 1, 1));
        result[1].DateCreated.ShouldBe(new DateTime(2023, 1, 2));
        result[2].DateCreated.ShouldBe(new DateTime(2023, 1, 3));
    }

    [Fact]
    public void ApplySorting_SortByLastModified()
    {
        // Act
        var result = _sorter.ApplySorting(
            _testItems.AsQueryable(),
            SortField.LastModified,
            Domain.Enums.SortDirection.Descending).ToList();

        // Assert
        result.Count.ShouldBe(3);
        // Item 0 has no LastModified, so falls back to DateCreated
        result[0].LastModified.ShouldBe(new DateTime(2023, 2, 2));  // Item 2
        result[1].LastModified.ShouldBe(new DateTime(2023, 2, 1));  // Item 1
        result[2].LastModified.ShouldBe(null);                      // Item 0
    }

    [Fact]
    public void ApplySorting_SortByOrder()
    {
        // Act
        var result = _sorter.ApplySorting(
            _testItems.AsQueryable(),
            SortField.Order,
            Domain.Enums.SortDirection.Ascending).ToList();

        // Assert
        result.Count.ShouldBe(3);
        result[0].OrderIndex.ShouldBe(1);
        result[1].OrderIndex.ShouldBe(2);
        result[2].OrderIndex.ShouldBe(3);
    }

    [Fact]
    public void ApplySorting_SortBySlug()
    {
        // Act
        var result = _sorter.ApplySorting(
            _testItems.AsQueryable(),
            SortField.Slug,
            Domain.Enums.SortDirection.Ascending).ToList();

        // Assert
        result.Count.ShouldBe(3);
        result[0].Slug.ShouldBe("a-title");
        result[1].Slug.ShouldBe("b-title");
        result[2].Slug.ShouldBe("c-title");
    }

    [Fact]
    public void ApplySorting_UnknownSortField_DefaultsToCreated()
    {
        // Act - Use an enum value outside our defined cases
        var result = _sorter.ApplySorting(
            _testItems.AsQueryable(),
            (SortField)999, // Invalid value
            Domain.Enums.SortDirection.Ascending).ToList();

        // Assert - Should fall back to sort by created date
        result.Count.ShouldBe(3);
        result[0].DateCreated.ShouldBe(new DateTime(2023, 1, 1));
        result[1].DateCreated.ShouldBe(new DateTime(2023, 1, 2));
        result[2].DateCreated.ShouldBe(new DateTime(2023, 1, 3));
    }
}