using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Infrastructure.Decorators
{
    /// <summary>Defines the LoggingContentProviderDecorator API contract.</summary>
    public class LoggingContentProviderDecorator : IReadContentProvider
    {
        private readonly IReadContentProvider _inner;
        private readonly ILogger<LoggingContentProviderDecorator> _logger;

        /// <summary>Performs the LoggingContentProviderDecorator operation.</summary>
        public LoggingContentProviderDecorator(
            IReadContentProvider inner,
            ILogger<LoggingContentProviderDecorator> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        /// <summary>Performs the GetById operation asynchronously.</summary>
        public async Task<ContentItem?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching content {Id}", id);
            var result = await _inner.GetByIdAsync(id);
            _logger.LogInformation("Fetched content {Id}", id);
            return result;
        }

        /// <summary>Performs the GetAll operation asynchronously.</summary>
        public async Task<IEnumerable<ContentItem>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all content");
            var items = await _inner.GetAllAsync();
            _logger.LogInformation("Fetched {Count} items", items.Count());
            return items;
        }
    }
}
