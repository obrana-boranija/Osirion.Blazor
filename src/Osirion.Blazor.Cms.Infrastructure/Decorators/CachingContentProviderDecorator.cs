using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Infrastructure.Decorators
{
    public class CachingContentProviderDecorator : IReadContentProvider
    {
        private readonly IReadContentProvider _inner;
        private readonly IContentCacheService _cache;

        public CachingContentProviderDecorator(
            IReadContentProvider inner,
            IContentCacheService cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public async Task<ContentItem?> GetByIdAsync(Guid id) =>
            await _cache.GetOrCreateAsync(
                $"Content:{id}",
                entry => _inner.GetByIdAsync(id));

        public async Task<IEnumerable<ContentItem>?> GetAllAsync() =>
            await _cache.GetOrCreateAsync(
                "Content:All",
                entry => _inner.GetAllAsync());
    }
}
