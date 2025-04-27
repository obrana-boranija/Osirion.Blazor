using Microsoft.Extensions.Configuration;
using Octokit;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Providers.Base;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Infrastructure.Providers.GitHub
{
    /// <summary>
    /// Fetches content JSON files from a GitHub repo via Octokit.
    /// Keeps external API details out of Core (SoC) :contentReference[oaicite:4]{index=4}.
    /// </summary>
    public class GitHubContentProvider :
        BaseContentProvider,
        IReadContentProvider,
        IDirectoryContentProvider,
        IQueryContentProvider
    {
        private readonly GitHubClient _client;
        private readonly string _owner, _repo, _path;

        public GitHubContentProvider(IConfiguration config)
            : base(config)
        {
            _client = new GitHubClient(new ProductHeaderValue("Osirion.Cms"));
            _owner = config["Cms:GitHub:Owner"]!;
            _repo = config["Cms:GitHub:Repo"]!;
            _path = config["Cms:GitHub:ContentPath"]!;
        }

        public async Task<ContentItem> GetByIdAsync(Guid id)
        {
            var file = await _client.Repository.Content
                .GetAllContentsByRef(_owner, _repo,
                  $"{_path}/{id}.json", "main");
            return JsonSerializer.Deserialize<ContentItem>(file.First().Content)!;
        }

        public async Task<IEnumerable<ContentItem>> GetAllAsync()
        {
            var items = await _client.Repository.Content
                .GetAllContentsByRef(_owner, _repo, _path, "main");
            return items
              .Where(f => f.Name.EndsWith(".json"))
              .Select(f => JsonSerializer.Deserialize<ContentItem>(f.Content)!)
              .ToList();
        }

        public Task<IEnumerable<string>> GetDirectoriesAsync(string path) =>
            // Not supported by GitHub API; return empty or throw
            Task.FromResult<IEnumerable<string>>(Array.Empty<string>());

        public Task<IEnumerable<ContentItem>> QueryAsync(ContentQuery filter) =>
            GetAllAsync()
              .ContinueWith(t => t.Result.Where(i => i.Tags.Contains(filter.Tag)));
    }
}
