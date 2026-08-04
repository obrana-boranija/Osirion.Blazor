using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>Defines the IReadContentProvider API contract.</summary>
public interface IReadContentProvider
{
    /// <summary>Performs the GetById operation asynchronously.</summary>
    Task<ContentItem?> GetByIdAsync(Guid id);
    /// <summary>Performs the GetAll operation asynchronously.</summary>
    Task<IEnumerable<ContentItem>> GetAllAsync();
}
