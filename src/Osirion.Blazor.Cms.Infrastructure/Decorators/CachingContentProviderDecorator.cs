using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Infrastructure.Decorators
{
    /// <summary>Defines the CachingContentProviderDecorator API contract.</summary>
    public class CachingContentProviderDecorator : IReadContentProvider
    {
        private readonly IReadContentProvider _inner;
        private readonly IContentCacheService _cache;

        /// <summary>Performs the CachingContentProviderDecorator operation.</summary>
        public CachingContentProviderDecorator(
            IReadContentProvider inner,
            IContentCacheService cache)
        {
            _inner = inner;
            _cache = cache;
        }

        /// <summary>Performs the GetById operation asynchronously.</summary>
        public async Task<ContentItem?> GetByIdAsync(Guid id) =>
            await _cache.GetOrCreateAsync(
                $"Content:{id}",
                entry => _inner.GetByIdAsync(id));

        /// <summary>Performs the GetAll operation asynchronously.</summary>
        public async Task<IEnumerable<ContentItem>> GetAllAsync() =>
            await _cache.GetOrCreateAsync(
                "Content:All",
            entry => _inner.GetAllAsync()) ?? Enumerable.Empty<ContentItem>();
    }
}
