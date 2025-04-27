using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Infrastructure.Decorators
{
    public class LoggingContentProviderDecorator : IReadContentProvider
    {
        private readonly IReadContentProvider _inner;
        private readonly ILogger<LoggingContentProviderDecorator> _logger;

        public LoggingContentProviderDecorator(
            IReadContentProvider inner,
            ILogger<LoggingContentProviderDecorator> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task<ContentItem> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching content {Id}", id);
            var result = await _inner.GetByIdAsync(id);
            _logger.LogInformation("Fetched content {Id}", id);
            return result;
        }

        public async Task<IEnumerable<ContentItem>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all content");
            var items = await _inner.GetAllAsync();
            _logger.LogInformation("Fetched {Count} items", items.Count());
            return items;
        }
    }
}
