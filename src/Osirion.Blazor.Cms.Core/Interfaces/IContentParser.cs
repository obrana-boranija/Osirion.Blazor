using Osirion.Blazor.Cms.Models;
using System.ComponentModel.DataAnnotations;

namespace Osirion.Blazor.Cms.Interfaces;

/// <summary>
/// Interface for parsing and generating content
/// </summary>
public interface IContentParser
{
    /// <summary>
    /// Parses markdown content into a content item
    /// </summary>
    /// <param name="markdownContent">Raw markdown content with front matter</param>
    /// <param name="contentItem">The content item to populate</param>
    /// <returns>Task representing the async operation</returns>
    Task ParseMarkdownContentAsync(string markdownContent, ContentItem contentItem);

    /// <summary>
    /// Generates markdown content with front matter from a content item
    /// </summary>
    /// <param name="contentItem">The content item</param>
    /// <returns>Markdown content with front matter</returns>
    string GenerateMarkdownWithFrontMatter(ContentItem contentItem);

    /// <summary>
    /// Extracts front matter from markdown content
    /// </summary>
    /// <param name="content">The markdown content with front matter</param>
    /// <returns>Dictionary of front matter key-value pairs</returns>
    Dictionary<string, string> ExtractFrontMatter(string content);

    /// <summary>
    /// Converts HTML content to markdown
    /// </summary>
    /// <param name="htmlContent">HTML content</param>
    /// <returns>Markdown content</returns>
    Task<string> ConvertHtmlToMarkdownAsync(string htmlContent);

    /// <summary>
    /// Validates the content according to the specified rules
    /// </summary>
    /// <param name="content">The content to validate</param>
    /// <returns>Validation result containing any errors</returns>
    ValidationResult ValidateContent(ContentItem content);

    /// <summary>
    /// Renders markdown to HTML
    /// </summary>
    /// <param name="markdown">Markdown content</param>
    /// <returns>HTML content</returns>
    string RenderMarkdownToHtml(string markdown);

    /// <summary>
    /// Converts a content item to a blog post model
    /// </summary>
    /// <param name="contentItem">Content item</param>
    /// <returns>Blog post model</returns>
    BlogPost ConvertToBlogPost(ContentItem contentItem);

    /// <summary>
    /// Converts a blog post model to a content item
    /// </summary>
    /// <param name="blogPost">Blog post</param>
    /// <returns>Content item</returns>
    ContentItem ConvertToContentItem(BlogPost blogPost);
}