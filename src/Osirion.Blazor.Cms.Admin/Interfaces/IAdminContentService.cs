using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Admin.Interfaces
{
    public interface IAdminContentService
    {
        Task<ContentItem?> CreateContentAsync(string title, string content, string path, string? author = null, string? description = null, string? slug = null, List<string>? tags = null, List<string>? categories = null, bool isFeatured = false, string? providerId = null, string? commitMessage = null, CancellationToken cancellationToken = default);
        Task DeleteContentAsync(string id, string? providerId = null, string? commitMessage = null, CancellationToken cancellationToken = default);
        Task<ContentItem?> GetContentByIdAsync(string id, string? providerId = null, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ContentItem>> SearchContentAsync(ContentQuery searchQuery, string? providerId = null, CancellationToken cancellationToken = default);
        Task<ContentItem?> UpdateContentAsync(string id, string title, string content, string path, string? author = null, string? description = null, string? slug = null, List<string>? tags = null, List<string>? categories = null, bool isFeatured = false, string? providerId = null, string? commitMessage = null, CancellationToken cancellationToken = default);
    }
}