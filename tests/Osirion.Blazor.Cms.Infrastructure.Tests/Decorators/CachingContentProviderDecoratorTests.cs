using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.Decorators;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Decorators
{
    public class CachingContentProviderDecoratorTests
    {
        private readonly IReadContentProvider _inner;
        private readonly IContentCacheService _cache;
        private readonly CachingContentProviderDecorator _decorator;

        public CachingContentProviderDecoratorTests()
        {
            _inner = Substitute.For<IReadContentProvider>();
            _cache = Substitute.For<IContentCacheService>();

            _decorator = new CachingContentProviderDecorator(_inner, _cache);
        }

        [Fact]
        public async Task GetByIdAsync_UsesCaching()
        {
            // Arrange
            var id = Guid.NewGuid();
            var contentItem = new ContentItem();

            _cache.GetOrCreateAsync(
                Arg.Is<string>(s => s.Contains(id.ToString())),
                Arg.Any<Func<CancellationToken, Task<ContentItem>>>())
                .Returns(callInfo =>
                {
                    var factory = callInfo.Arg<Func<CancellationToken, Task<ContentItem>>>(); 
                    return factory(CancellationToken.None);
                });

            _inner.GetByIdAsync(id).Returns(contentItem);

            // Act
            var result = await _decorator.GetByIdAsync(id);

            // Assert
            result.ShouldBeSameAs(contentItem);

            // Verify the cache was used
            await _cache.Received(1).GetOrCreateAsync(
                Arg.Is<string>(s => s.Contains(id.ToString())),
                Arg.Any<Func<CancellationToken, Task<ContentItem>>>());

            // Verify the inner provider was called
            await _inner.Received(1).GetByIdAsync(id);
        }

        [Fact]
        public async Task GetAllAsync_UsesCaching()
        {
            // Arrange
            var contentItems = new List<ContentItem> { new ContentItem(), new ContentItem() };

            _cache.GetOrCreateAsync(
                Arg.Is<string>(s => s.Contains("Content:All")),
                Arg.Any<Func<CancellationToken, Task<IEnumerable<ContentItem>>>>())
                .Returns(callInfo =>
                {
                    var factory = callInfo.Arg<Func<CancellationToken, Task<IEnumerable<ContentItem>>>>();
                    return factory(CancellationToken.None); // Pass CancellationToken.None as required
                });

            _inner.GetAllAsync().Returns(contentItems);

            // Act
            var result = await _decorator.GetAllAsync();

            // Assert
            result.ShouldBeSameAs(contentItems);

            // Verify the cache was used
            await _cache.Received(1).GetOrCreateAsync(
                Arg.Is<string>(s => s.Contains("Content:All")),
                Arg.Any<Func<CancellationToken, Task<IEnumerable<ContentItem>>>>()); // Updated to match the expected delegate type

            // Verify the inner provider was called
            await _inner.Received(1).GetAllAsync();
        }
    }
}