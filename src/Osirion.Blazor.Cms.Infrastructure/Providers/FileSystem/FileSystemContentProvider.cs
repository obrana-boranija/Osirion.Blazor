using Microsoft.Extensions.Configuration;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Providers.Base;

namespace Osirion.Blazor.Cms.Infrastructure.Providers.FileSystem
{
    /// <summary>
    /// Reads content from the local filesystem (no caching/logging here).
    /// Adheres to IReadContentProvider, IDirectoryContentProvider, IQueryContentProvider
    /// (ISP) :contentReference[oaicite:3]{index=3}.
    /// </summary>
    public class FileSystemContentProvider
        : BaseContentProvider,
          IReadContentProvider,
          IDirectoryContentProvider,
          IQueryContentProvider
    {
        public string ProviderId => $"filesystem-{RootFolder.GetHashCode()}";

        public FileSystemContentProvider(IConfiguration config)
            : base(config) { }

        public Task<ContentItem> GetByIdAsync(Guid id) =>
            LoadAsync(Path.Combine(RootFolder, $"{id}.json"));

        public Task<IEnumerable<ContentItem>> GetAllAsync() =>
            Task.WhenAll(
                Directory
                    .EnumerateFiles(RootFolder, "*.json")
                    .Select(LoadAsync))
                .ContinueWith(task => task.Result.AsEnumerable());

        public Task<IEnumerable<string>> GetDirectoriesAsync(string path) =>
            Task.FromResult(
              Directory.EnumerateDirectories(Path.Combine(RootFolder, path)));

        public async Task<IEnumerable<ContentItem>> QueryAsync(ContentQuery filter)
        {
            var all = await GetAllAsync();
            return all.Where(item => item.Tags.Contains(filter.Tag));
        }
    }
}
