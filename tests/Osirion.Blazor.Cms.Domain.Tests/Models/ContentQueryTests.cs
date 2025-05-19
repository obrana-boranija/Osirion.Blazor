using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Domain.Tests.Models;

public class ContentQueryTests
{
    [Fact]
    public void Constructor_CreatesInstanceWithDefaultValues()
    {
        // Act
        var query = new ContentQuery();

        // Assert
        Assert.Null(query.Directory);
        Assert.Null(query.Slug);
        Assert.Null(query.DirectoryId);
        Assert.Null(query.Category);
        Assert.Null(query.Tag);
        Assert.Null(query.SearchQuery);
        Assert.Null(query.IsFeatured);
        Assert.Null(query.Status);
        Assert.Null(query.Author);
        Assert.Null(query.DateFrom);
        Assert.Null(query.DateTo);
        Assert.Null(query.Skip);
        Assert.Null(query.Take);
        Assert.Equal(SortField.Date, query.SortBy);
        Assert.Equal(SortDirection.Descending, query.SortDirection);
        Assert.Null(query.Locale);
        Assert.Null(query.LocalizationId);
        Assert.Null(query.ProviderId);
        Assert.False(query.IncludeUnpublished);
        Assert.False(query.IncludeSubdirectories);
        Assert.Null(query.IncludeIds);
        Assert.Null(query.ExcludeIds);
        Assert.Null(query.Tags);
        Assert.Null(query.Categories);
    }

    [Fact]
    public void Clone_CreatesDeepCopy()
    {
        // Arrange
        var original = new ContentQuery
        {
            Directory = "blog",
            Slug = "test-post",
            DirectoryId = "dir-1",
            Category = "category1",
            Tag = "tag1",
            SearchQuery = "search term",
            IsFeatured = true,
            Status = ContentStatus.Published,
            Author = "John Doe",
            DateFrom = new DateTime(2025, 1, 1),
            DateTo = new DateTime(2025, 12, 31),
            Skip = 10,
            Take = 20,
            SortBy = SortField.Title,
            SortDirection = SortDirection.Ascending,
            Locale = "en-US",
            LocalizationId = "loc-123",
            ProviderId = "provider-1",
            IncludeUnpublished = true,
            IncludeSubdirectories = true,
            IncludeIds = new List<string> { "id1", "id2" },
            ExcludeIds = new List<string> { "id3", "id4" },
            Tags = new List<string> { "tag1", "tag2" },
            Categories = new List<string> { "category1", "category2" }
        };

        // Act
        var clone = original.Clone();

        // Assert
        Assert.NotSame(original, clone);

        Assert.Equal(original.Directory, clone.Directory);
        Assert.Equal(original.Slug, clone.Slug);
        Assert.Equal(original.DirectoryId, clone.DirectoryId);
        Assert.Equal(original.Category, clone.Category);
        Assert.Equal(original.Tag, clone.Tag);
        Assert.Equal(original.SearchQuery, clone.SearchQuery);
        Assert.Equal(original.IsFeatured, clone.IsFeatured);
        Assert.Equal(original.Status, clone.Status);
        Assert.Equal(original.Author, clone.Author);
        Assert.Equal(original.DateFrom, clone.DateFrom);
        Assert.Equal(original.DateTo, clone.DateTo);
        Assert.Equal(original.Skip, clone.Skip);
        Assert.Equal(original.Take, clone.Take);
        Assert.Equal(original.SortBy, clone.SortBy);
        Assert.Equal(original.SortDirection, clone.SortDirection);
        Assert.Equal(original.Locale, clone.Locale);
        Assert.Equal(original.LocalizationId, clone.LocalizationId);
        Assert.Equal(original.ProviderId, clone.ProviderId);
        Assert.Equal(original.IncludeUnpublished, clone.IncludeUnpublished);
        Assert.Equal(original.IncludeSubdirectories, clone.IncludeSubdirectories);

        // Verify the collections are cloned, not referenced
        Assert.NotSame(original.IncludeIds, clone.IncludeIds);
        Assert.NotSame(original.ExcludeIds, clone.ExcludeIds);
        Assert.NotSame(original.Tags, clone.Tags);
        Assert.NotSame(original.Categories, clone.Categories);

        // Verify collection contents
        Assert.Equal(original.IncludeIds.Count, clone.IncludeIds.Count);
        Assert.Equal(original.ExcludeIds.Count, clone.ExcludeIds.Count);
        Assert.Equal(original.Tags.Count, clone.Tags.Count);
        Assert.Equal(original.Categories.Count, clone.Categories.Count);

        Assert.All(original.IncludeIds, id => Assert.Contains(id, clone.IncludeIds));
        Assert.All(original.ExcludeIds, id => Assert.Contains(id, clone.ExcludeIds));
        Assert.All(original.Tags, tag => Assert.Contains(tag, clone.Tags));
        Assert.All(original.Categories, category => Assert.Contains(category, clone.Categories));
    }

    [Fact]
    public void Clone_WithNullCollections_HandlesCorrectly()
    {
        // Arrange
        var original = new ContentQuery
        {
            Directory = "blog",
            IncludeIds = null,
            ExcludeIds = null,
            Tags = null,
            Categories = null
        };

        // Act
        var clone = original.Clone();

        // Assert
        Assert.NotSame(original, clone);
        Assert.Equal(original.Directory, clone.Directory);
        Assert.Null(clone.IncludeIds);
        Assert.Null(clone.ExcludeIds);
        Assert.Null(clone.Tags);
        Assert.Null(clone.Categories);
    }

    [Fact]
    public void Clone_WithEmptyCollections_CreatesEmptyCollections()
    {
        // Arrange
        var original = new ContentQuery
        {
            Directory = "blog",
            IncludeIds = new List<string>(),
            ExcludeIds = new List<string>(),
            Tags = new List<string>(),
            Categories = new List<string>()
        };

        // Act
        var clone = original.Clone();

        // Assert
        Assert.NotSame(original, clone);
        Assert.Equal(original.Directory, clone.Directory);

        Assert.NotNull(clone.IncludeIds);
        Assert.NotNull(clone.ExcludeIds);
        Assert.NotNull(clone.Tags);
        Assert.NotNull(clone.Categories);

        Assert.Empty(clone.IncludeIds);
        Assert.Empty(clone.ExcludeIds);
        Assert.Empty(clone.Tags);
        Assert.Empty(clone.Categories);
    }

    [Fact]
    public void Clone_WithPopulatedCollections_ClonesElementsCorrectly()
    {
        // Arrange
        var original = new ContentQuery
        {
            IncludeIds = new List<string> { "id1", "id2", "id3" },
            Tags = new List<string> { "tag1", "tag2", "tag3" }
        };

        // Act
        var clone = original.Clone();

        // Modify original collections
        original.IncludeIds.Add("id4");
        original.Tags.Add("tag4");

        // Assert
        Assert.Equal(3, clone.IncludeIds.Count);
        Assert.Equal(3, clone.Tags.Count);
        Assert.DoesNotContain("id4", clone.IncludeIds);
        Assert.DoesNotContain("tag4", clone.Tags);
    }
}