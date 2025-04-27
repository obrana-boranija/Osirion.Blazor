using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Infrastructure.GitHub.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Core.Models;

/// <summary>
/// Represents a blog post with content and metadata
/// </summary>
public class BlogPost
{
    /// <summary>
    /// Gets or sets the frontmatter metadata for the post
    /// </summary>
    public FrontMatter Metadata { get; set; }

    /// <summary>
    /// Gets or sets the markdown content of the post
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file path relative to the repository root
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the SHA hash of the file in the repository
    /// </summary>
    public string Sha { get; set; } = string.Empty;

    /// <summary>
    /// Gets the filename portion of the file path
    /// </summary>
    public string FileName => Path.GetFileName(FilePath);

    /// <summary>
    /// Gets the directory portion of the file path
    /// </summary>
    public string Directory => Path.GetDirectoryName(FilePath) ?? string.Empty;

    /// <summary>
    /// Gets the extension of the file
    /// </summary>
    public string Extension => Path.GetExtension(FilePath);

    /// <summary>
    /// Creates a new blog post
    /// </summary>
    public BlogPost()
    {
    }

    /// <summary>
    /// Creates a new blog post with specified metadata and content
    /// </summary>
    /// <param name="metadata">The frontmatter metadata</param>
    /// <param name="content">The markdown content</param>
    public BlogPost(FrontMatter metadata, string content)
    {
        Metadata = metadata;
        Content = content;
    }

    /// <summary>
    /// Converts the blog post to markdown with frontmatter
    /// </summary>
    /// <returns>The full markdown content with frontmatter</returns>
    public string ToMarkdown()
    {
        var markdown = new StringBuilder();

        // Add frontmatter
        markdown.Append(Metadata.ToYaml());

        // Add content
        markdown.AppendLine(Content);

        return markdown.ToString();
    }

    /// <summary>
    /// Creates a blog post from markdown with frontmatter
    /// </summary>
    /// <param name="markdown">The full markdown content with frontmatter</param>
    /// <returns>A blog post object</returns>
    public static BlogPost FromMarkdown(string markdown)
    {
        var blogPost = new BlogPost();

        if (string.IsNullOrWhiteSpace(markdown))
        {
            return blogPost;
        }

        // Regular expression to extract frontmatter
        var frontMatterRegex = new Regex(@"^---\s*\n(.*?)\n---\s*\n", RegexOptions.Singleline);
        var match = frontMatterRegex.Match(markdown);

        if (match.Success)
        {
            // Extract and parse frontmatter
            var frontMatterYaml = match.Groups[1].Value;
            blogPost.Metadata = FrontMatter.FromYaml(frontMatterYaml);

            // Extract content (everything after frontmatter)
            var contentStartIndex = match.Index + match.Length;
            blogPost.Content = markdown.Substring(contentStartIndex).Trim();
        }
        else
        {
            // No frontmatter found, treat entire content as markdown
            blogPost.Content = markdown.Trim();
        }

        return blogPost;
    }

    /// <summary>
    /// Creates a blog post from a GitHub file content object
    /// </summary>
    /// <param name="fileContent">The GitHub file content</param>
    /// <returns>A blog post object</returns>
    public static BlogPost FromGitHubFile(GitHubFileContent fileContent)
    {
        var blogPost = new BlogPost();

        if (fileContent == null)
        {
            return blogPost;
        }

        // Set file information
        blogPost.FilePath = fileContent.Path;
        blogPost.Sha = fileContent.Sha;

        if (fileContent.IsMarkdownFile())
        {
            // If it's a markdown file, decode and parse the content
            var content = fileContent.GetDecodedContent();
            var parsedPost = FromMarkdown(content);

            blogPost.Metadata = parsedPost.Metadata;
            blogPost.Content = parsedPost.Content;
        }

        return blogPost;
    }
}