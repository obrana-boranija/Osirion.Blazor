using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Caching;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Caching
{
    public class StaleWhileRevalidateCacheDecoratorTests
    {
        private readonly IContentRepository _decorated;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<StaleWhileRevalidateCacheDecorator> _logger;
        private readonly StaleWhileRevalidateCacheDecorator _decorator;

        public StaleWhileRevalidateCacheDecoratorTests()
        {
            _decorated = Substitute.For<IContentRepository>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _logger = Substitute.For<ILogger<StaleWhileRevalidateCacheDecorator>>();

            _decorator = new StaleWhileRevalidateCacheDecorator(
                _decorated,
                _memoryCache,
                _logger,
                TimeSpan.FromMinutes(5),  // Stale time
                TimeSpan.FromMinutes(30), // Max age
                "test-provider");
        }

        [Fact]
        public async Task GetByIdAsync_CachesResult()
        {
            // Arrange
            var itemId = "test-id";
            var contentItem = ContentItem.Create(
                itemId,
                "Test Title",
                "Test content",
                "test.md",
                "test-provider");

            _decorated.GetByIdAsync(itemId, Arg.Any<CancellationToken>())
                .Returns(contentItem);

            // Act - First call
            var firstResult = await _decorator.GetByIdAsync(itemId);

            // Set up to return a different result on second call to repository
            // This shouldn't be called because of caching
            _decorated.GetByIdAsync(itemId, Arg.Any<CancellationToken>())
                .Returns(ContentItem.Create(
                    itemId,
                    "Different Title",
                    "Different content",
                    "different.md",
                    "test-provider"));

            // Act - Second call
            var secondResult = await _decorator.GetByIdAsync(itemId);

            // Assert
            firstResult.ShouldBe(contentItem);
            secondResult.ShouldBe(contentItem); // Should be the same as first result due to caching

            // Repository should only be called once
            await _decorated.Received(1).GetByIdAsync(itemId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetAllAsync_CachesResult()
        {
            // Arrange
            var items = new List<ContentItem>
            {
                ContentItem.Create("id1", "Title 1", "Content 1", "path1.md", "test-provider"),
                ContentItem.Create("id2", "Title 2", "Content 2", "path2.md", "test-provider")
            };

            _decorated.GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(items);

            // Act - First call
            var firstResult = await _decorator.GetAllAsync();

            // Change the returned list
            var differentItems = new List<ContentItem>
            {
                ContentItem.Create("id3", "Title 3", "Content 3", "path3.md", "test-provider")
            };

            _decorated.GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(differentItems);

            // Act - Second call
            var secondResult = await _decorator.GetAllAsync();

            // Assert
            firstResult.Count.ShouldBe(2);
            secondResult.Count.ShouldBe(2); // Should be the same as first result due to caching

            // Repository should only be called once
            await _decorated.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task SaveAsync_InvalidatesCache()
        {
            // Arrange
            var itemId = "test-id";
            var contentItem = ContentItem.Create(
                itemId,
                "Test Title",
                "Test content",
                "test.md",
                "test-provider");

            _decorated.GetByIdAsync(itemId, Arg.Any<CancellationToken>())
                .Returns(contentItem);

            _decorated.SaveAsync(contentItem, Arg.Any<CancellationToken>())
                .Returns(contentItem);

            // First call to populate cache
            await _decorator.GetByIdAsync(itemId);

            // Act - Save, which should invalidate cache
            await _decorator.SaveAsync(contentItem);

            // Setup repository to return different content on next call
            var updatedItem = ContentItem.Create(
                itemId,
                "Updated Title",
                "Updated content",
                "test.md",
                "test-provider");

            _decorated.GetByIdAsync(itemId, Arg.Any<CancellationToken>())
                .Returns(updatedItem);

            // Get item again
            var resultAfterSave = await _decorator.GetByIdAsync(itemId);

            // Assert
            resultAfterSave.ShouldBe(updatedItem); // Should get the updated item since cache was invalidated

            // Repository should be called twice (once before save, once after)
            await _decorated.Received(2).GetByIdAsync(itemId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task DeleteAsync_InvalidatesCache()
        {
            // Arrange
            var itemId = "test-id";
            var contentItem = ContentItem.Create(
                itemId,
                "Test Title",
                "Test content",
                "test.md",
                "test-provider");

            _decorated.GetByIdAsync(itemId, Arg.Any<CancellationToken>())
                .Returns(contentItem);

            // First call to populate cache
            await _decorator.GetByIdAsync(itemId);

            // Act - Delete, which should invalidate cache
            await _decorator.DeleteAsync(itemId);

            // Setup repository to return null for deleted item
            _decorated.GetByIdAsync(itemId, Arg.Any<CancellationToken>())
                .Returns((ContentItem)null);

            // Get item again
            var resultAfterDelete = await _decorator.GetByIdAsync(itemId);

            // Assert
            resultAfterDelete.ShouldBeNull(); // Item should be gone

            // Repository should be called twice (once before delete, once after)
            await _decorated.Received(2).GetByIdAsync(itemId, Arg.Any<CancellationToken>());
        }
    }
}