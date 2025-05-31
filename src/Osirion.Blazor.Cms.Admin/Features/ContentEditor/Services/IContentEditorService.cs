using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;

/// <summary>
/// Interface for editing content in the admin interface
/// </summary>
public interface IContentEditorService
{
    /// <summary>
    /// Gets a blog post by path
    /// </summary>
    /// <param name="path">The path to the blog post</param>
    /// <returns>The blog post</returns>
    Task<ContentItem> GetBlogPostAsync(string path);

    /// <summary>
    /// Saves a blog post
    /// </summary>
    /// <param name="post">The blog post to save</param>
    /// <param name="commitMessage">Optional commit message</param>
    /// <returns>The commit response</returns>
    Task<GitHubFileCommitResponse> SaveBlogPostAsync(ContentItem post, string commitMessage);

    /// <summary>
    /// Deletes a blog post
    /// </summary>
    /// <param name="path">The path to the blog post</param>
    /// <param name="sha">The file SHA</param>
    /// <returns>The commit response</returns>
    Task<GitHubFileCommitResponse> DeleteBlogPostAsync(string path, string sha);

    /// <summary>
    /// Creates a new blog post with default content
    /// </summary>
    /// <param name="path">Optional directory path</param>
    /// <param name="title">Optional title</param>
    /// <returns>A new blog post</returns>
    ContentItem CreateNewBlogPost(string path = "", string title = "New Post");

    /// <summary>
    /// Generates a file name from a title
    /// </summary>
    /// <param name="title">The title</param>
    /// <returns>A file name</returns>
    string GenerateFileNameFromTitle(string title);

    /// <summary>
    /// Converts a ContentItem to a BlogPost
    /// </summary>
    /// <param name="item">The content item to convert</param>
    /// <returns>A blog post</returns>
    ContentItem ConvertToBlogPost(ContentItem item);
}