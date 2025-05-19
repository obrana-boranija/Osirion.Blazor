using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Content;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Content
{
    public class ContentQueryFilterTests
    {
        private readonly IPathUtilities _pathUtils;
        private readonly ContentQueryFilter _filter;
        private readonly List<ContentItem> _testItems;

        public ContentQueryFilterTests()
        {
            _pathUtils = Substitute.For<IPathUtilities>();
            _pathUtils.NormalizePath(Arg.Any<string>()).Returns(x => x.Arg<string>());

            _filter = new ContentQueryFilter(_pathUtils);

            // Create test content items
            _testItems = new List<ContentItem>
            {
                ContentItem.Create("id1", "Title 1", "Content 1", "blog/post1.md", "test-provider"),
                ContentItem.Create("id2", "Title 2", "Content 2", "blog/post2.md", "test-provider"),
                ContentItem.Create("id3", "Featured Post", "Content 3", "featured/post3.md", "test-provider"),
                ContentItem.Create("id4", "French Post", "Content 4", "fr/post4.md", "test-provider")
            };

            // Set additional properties
            _testItems[0].SetSlug("title-1");
            _testItems[0].AddCategory("Category1");
            _testItems[0].AddTag("tag1");
            _testItems[0].AddTag("tag2");
            _testItems[0].SetAuthor("Author 1");

            _testItems[1].SetSlug("title-2");
            _testItems[1].AddCategory("Category1");
            _testItems[1].AddCategory("Category2");
            _testItems[1].AddTag("tag1");
            _testItems[1].SetAuthor("Author 2");

            _testItems[2].SetSlug("featured-post");
            _testItems[2].AddCategory("Category3");
            _testItems[2].AddTag("tag3");
            _testItems[2].SetFeatured(true);
            _testItems[2].SetAuthor("Author 1");

            _testItems[3].SetSlug("french-post");
            _testItems[3].SetLocale("fr");
            _testItems[3].SetAuthor("Author 2");

            // Set directories
            var dir1 = DirectoryItem.Create("dir1", "blog", "Blog", "test-provider");
            var dir2 = DirectoryItem.Create("dir2", "featured", "Featured", "test-provider");
            var dir3 = DirectoryItem.Create("dir3", "fr", "French", "test-provider");

            _testItems[0].SetDirectory(dir1);
            _testItems[1].SetDirectory(dir1);
            _testItems[2].SetDirectory(dir2);
            _testItems[3].SetDirectory(dir3);
        }

        [Fact]
        public void ApplyFilters_WithDirectoryFilter_ReturnsCorrectItems()
        {
            // Arrange
            var query = new ContentQuery
            {
                DirectoryId = "dir1"
            };

            // Act
            var result = _filter.ApplyFilters(_testItems.AsQueryable(), query).ToList();

            // Assert
            result.Count.ShouldBe(2);
            result.ShouldContain(i => i.Id == "id1");
            result.ShouldContain(i => i.Id == "id2");
        }

        [Fact]
        public void ApplyFilters_WithCategoryFilter_ReturnsCorrectItems()
        {
            // Arrange
            var query = new ContentQuery
            {
                Category = "Category1"
            };

            // Act
            var result = _filter.ApplyFilters(_testItems.AsQueryable(), query).ToList();

            // Assert
            result.Count.ShouldBe(2);
            result.ShouldContain(i => i.Id == "id1");
            result.ShouldContain(i => i.Id == "id2");
        }

        [Fact]
        public void ApplyFilters_WithMultipleCategories_ReturnsOnlyItemsWithAllCategories()
        {
            // Arrange
            var query = new ContentQuery
            {
                Categories = new[] { "Category1", "Category2" }
            };

            // Act
            var result = _filter.ApplyFilters(_testItems.AsQueryable(), query).ToList();

            // Assert
            result.Count.ShouldBe(1);
            result[0].Id.ShouldBe("id2");
        }

        [Fact]
        public void ApplyFilters_WithTagFilter_ReturnsCorrectItems()
        {
            // Arrange
            var query = new ContentQuery
            {
                Tag = "tag1"
            };

            // Act
            var result = _filter.ApplyFilters(_testItems.AsQueryable(), query).ToList();

            // Assert
            result.Count.ShouldBe(2);
            result.ShouldContain(i => i.Id == "id1");
            result.ShouldContain(i => i.Id == "id2");
        }

        [Fact]
        public void ApplyFilters_WithFeaturedFilter_ReturnsOnlyFeaturedItems()
        {
            // Arrange
            var query = new ContentQuery
            {
                IsFeatured = true
            };

            // Act
            var result = _filter.ApplyFilters(_testItems.AsQueryable(), query).ToList();

            // Assert
            result.Count.ShouldBe(1);
            result[0].Id.ShouldBe("id3");
        }

        [Fact]
        public void ApplyFilters_WithAuthorFilter_ReturnsItemsFromSpecificAuthor()
        {
            // Arrange
            var query = new ContentQuery
            {
                Author = "Author 1"
            };

            // Act
            var result = _filter.ApplyFilters(_testItems.AsQueryable(), query).ToList();

            // Assert
            result.Count.ShouldBe(2);
            result.ShouldContain(i => i.Id == "id1");
            result.ShouldContain(i => i.Id == "id3");
        }

        [Fact]
        public void ApplyFilters_WithLocaleFilter_ReturnsItemsInSpecificLocale()
        {
            // Arrange
            var query = new ContentQuery
            {
                Locale = "fr"
            };

            // Act
            var result = _filter.ApplyFilters(_testItems.AsQueryable(), query).ToList();

            // Assert
            result.Count.ShouldBe(1);
            result[0].Id.ShouldBe("id4");
        }

        [Fact]
        public void ApplyFilters_WithSearchQuery_ReturnsMatchingItems()
        {
            // Arrange
            var query = new ContentQuery
            {
                SearchQuery = "featured"
            };

            // Act
            var result = _filter.ApplyFilters(_testItems.AsQueryable(), query).ToList();

            // Assert
            result.Count.ShouldBe(1);
            result[0].Id.ShouldBe("id3");
        }

        [Fact]
        public void ApplyFilters_WithMultipleFilters_ReturnsItemsThatMatchAll()
        {
            // Arrange
            var query = new ContentQuery
            {
                Author = "Author 1",
                Tag = "tag1"
            };

            // Act
            var result = _filter.ApplyFilters(_testItems.AsQueryable(), query).ToList();

            // Assert
            result.Count.ShouldBe(1);
            result[0].Id.ShouldBe("id1");
        }
    }
}