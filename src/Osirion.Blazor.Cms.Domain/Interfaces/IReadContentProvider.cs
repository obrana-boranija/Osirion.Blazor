using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Domain.Interfaces;

public interface IReadContentProvider
{
    Task<ContentItem> GetByIdAsync(Guid id);
    Task<IEnumerable<ContentItem>> GetAllAsync();
}