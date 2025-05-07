using Osirion.Blazor.Cms.Admin.Services.Adapters;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;

public class ContentEditorService
{
    private readonly IContentRepositoryAdapter _repositoryAdapter;

    public ContentEditorService(IContentRepositoryAdapter repositoryAdapter)
    {
        _repositoryAdapter = repositoryAdapter;
    }

    public async Task<GitHubFileCommitResponse> SaveContentAsync(BlogPost post, string commitMessage)
    {
        var content = post.ToMarkdown();
        var message = string.IsNullOrEmpty(commitMessage)
            ? $"Update {Path.GetFileName(post.FilePath)}"
            : commitMessage;

        return await _repositoryAdapter.SaveContentAsync(
            post.FilePath,
            content,
            message,
            post.Sha);
    }

    public async Task<BlogPost> GetBlogPostAsync(string path)
    {
        return await _repositoryAdapter.GetBlogPostAsync(path);
    }
}