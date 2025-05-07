using Osirion.Blazor.Cms.Admin.Services.Adapters;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;

public class ContentBrowserService
{
    private readonly IContentRepositoryAdapter _repositoryAdapter;

    public ContentBrowserService(IContentRepositoryAdapter repositoryAdapter)
    {
        _repositoryAdapter = repositoryAdapter;
    }

    public async Task<List<GitHubItem>> GetContentsAsync(string path)
    {
        return await _repositoryAdapter.GetContentsAsync(path);
    }

    public async Task<BlogPost> GetBlogPostAsync(string path)
    {
        return await _repositoryAdapter.GetBlogPostAsync(path);
    }

    public async Task<GitHubFileCommitResponse> DeleteFileAsync(string path, string sha)
    {
        var message = $"Delete {System.IO.Path.GetFileName(path)}";
        return await _repositoryAdapter.DeleteFileAsync(path, message, sha);
    }
}