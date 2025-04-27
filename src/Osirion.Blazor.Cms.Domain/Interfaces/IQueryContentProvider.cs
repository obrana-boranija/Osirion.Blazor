using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Domain.Interfaces;

public interface IQueryContentProvider
{
    Task<IEnumerable<ContentItem>> QueryAsync(ContentQuery filter);
}
