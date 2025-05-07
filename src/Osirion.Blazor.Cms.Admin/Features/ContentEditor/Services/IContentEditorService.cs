using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;

public interface IContentEditorService
{
    Task<BlogPost> GetBlogPostAsync(string path);
    Task<GitHubFileCommitResponse> SaveBlogPostAsync(BlogPost post, string commitMessage);
    Task<GitHubFileCommitResponse> DeleteBlogPostAsync(string path, string sha);
    BlogPost CreateNewBlogPost(string path = "", string title = "New Post");
    string GenerateFileNameFromTitle(string title);
}