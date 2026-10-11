using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Content;
using Shouldly;

namespace Osirion.Blazor.Cms.Tests.Content;

public class ContentQueryFilterPaginationTests
{
    private readonly ContentQueryFilter _filter;

    public ContentQueryFilterPaginationTests()
    {
        var pathUtilities = Substitute.For<IPathUtilities>();
        pathUtilities.NormalizePath(Arg.Any<string>()).Returns(call => call.Arg<string>());
        _filter = new ContentQueryFilter(pathUtilities);
    }

    [Fact]
    public void ApplyFilters_WithSkipAndTake_ReturnsRequestedPageOfSortedItems()
    {
        // Arrange
        var items = Enumerable.Range(1, 10)
            .Select(i => ContentItem.Create(
                i.ToString(),
                $"Item {i:00}",
                $"Content {i}",
                $"path{i}",
                "test-provider"))
            .Reverse()
            .AsQueryable();

        var query = new ContentQuery
        {
            Skip = 2,
            Take = 3,
            IncludeUnpublished = true,
            SortBy = SortField.Title,
            SortDirection = Cms.Domain.Enums.SortDirection.Ascending
        };

        // Act
        var result = _filter.ApplyFilters(items, query).ToList();

        // Assert
        result.Select(i => i.Title).ShouldBe(["Item 03", "Item 04", "Item 05"]);
    }
}
