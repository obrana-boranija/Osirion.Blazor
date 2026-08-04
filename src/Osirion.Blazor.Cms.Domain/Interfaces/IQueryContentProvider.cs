using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>Defines the IQueryContentProvider API contract.</summary>
public interface IQueryContentProvider
{
    /// <summary>Performs the Query operation asynchronously.</summary>
    Task<IEnumerable<ContentItem>> QueryAsync(ContentQuery filter);
}
