using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;

/// <summary>
/// A content provider the container can construct, for registration tests that resolve a provider type without
/// reading any content.
/// </summary>
public sealed class NamedContentProvider : IContentProvider
{
    public const string Id = "named-test-provider";

    public string ProviderId => Id;

    public string DisplayName => "Named test provider";

    public bool IsReadOnly => true;

    /// <summary>Gets or sets a value a registration delegate can change, so a test can see that it ran.</summary>
    public string? ConfiguredBy { get; set; }

    public Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public Task<ContentItem?> GetItemByIdAsync(string id, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public Task<ContentItem?> GetItemByUrlAsync(string url, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public Task<Dictionary<string, ContentItem>> GetContentTranslationsAsync(string localizationId) => throw new NotSupportedException();

    public Task<IReadOnlyList<ContentItem>?> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public Task<IReadOnlyList<ContentCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public Task<IReadOnlyList<ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public Task<IReadOnlyList<DirectoryItem>> GetDirectoriesAsync(string? locale = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public Task<DirectoryItem?> GetDirectoryByPathAsync(string path, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public Task<DirectoryItem?> GetDirectoryByIdAsync(string id, string? locale = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public Task<DirectoryItem?> GetDirectoryByUrlAsync(string url, CancellationToken cancellationToken = default) => throw new NotSupportedException();
}
