using Osirion.Blazor.Cms.Core.Models;
using Osirion.Blazor.Cms.Enums;
using Osirion.Blazor.Cms.Interfaces;
using Osirion.Blazor.Cms.Models;
using Osirion.Blazor.Cms.Core.Interfaces;
using Osirion.Blazor.Core.Extensions;
using System.Text;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Services;

/// <summary>
/// Default implementation of IContentParser
/// </summary>
public class ContentParser : IContentParser
{
    private readonly IMarkdownProcessor _markdownProcessor;

    /// <summary>
    /// Initializes a new instance of the ContentParser class
    /// </summary>
    public ContentParser(IMarkdownProcessor markdownProcessor)
    {
        _markdownProcessor = markdownProcessor ?? throw new ArgumentNullException(nameof(markdownProcessor));
    }

    /// <inheritdoc/>
    public async Task ParseMarkdownContentAsync(string markdownContent, ContentItem contentItem)
    {
        if (string.IsNullOrWhiteSpace(markdownContent))
            return;

        // Store original markdown for later use
        contentItem.OriginalMarkdown = markdownContent;

        // Extract front matter
        var frontMatter = _markdownProcessor.ExtractFrontMatter(markdownContent);
        ApplyFrontMatterToContentItem(frontMatter, contentItem);

        // Get content without front matter by rendering the markdown
        var htmlContent = await _markdownProcessor.RenderToHtmlAsync(markdownContent);
        contentItem.Content = htmlContent;
    }

    /// <inheritdoc/>
    public string GenerateMarkdownWithFrontMatter(ContentItem contentItem)
    {
        var markdown = new StringBuilder();

        // Generate front matter
        markdown.AppendLine("---");

        // Basic metadata
        if (!string.IsNullOrEmpty(contentItem.Title))
            markdown.AppendLine($"title: \"{EscapeYamlString(contentItem.Title)}\"");

        if (!string.IsNullOrEmpty(contentItem.Author))
            markdown.AppendLine($"author: \"{EscapeYamlString(contentItem.Author)}\"");

        if (!string.IsNullOrEmpty(contentItem.Description))
            markdown.AppendLine($"description: \"{EscapeYamlString(contentItem.Description)}\"");

        // Date created (in ISO format)
        markdown.AppendLine($"date: {contentItem.DateCreated:yyyy-MM-dd}");

        // Last modified date
        if (contentItem.LastModified.HasValue)
            markdown.AppendLine($"last_modified: {contentItem.LastModified.Value:yyyy-MM-dd}");

        // Content ID for localization
        if (!string.IsNullOrEmpty(contentItem.ContentId))
            markdown.AppendLine($"content_id: \"{contentItem.ContentId}\"");

        if (!string.IsNullOrEmpty(contentItem.Locale))
            markdown.AppendLine($"locale: \"{contentItem.Locale}\"");

        // Slug
        if (!string.IsNullOrEmpty(contentItem.Slug))
            markdown.AppendLine($"slug: \"{contentItem.Slug}\"");

        // Featured status and image
        if (contentItem.IsFeatured)
            markdown.AppendLine("featured: true");

        if (!string.IsNullOrEmpty(contentItem.FeaturedImageUrl))
            markdown.AppendLine($"featured_image: \"{contentItem.FeaturedImageUrl}\"");

        // Categories
        if (contentItem.Categories.Count > 0)
        {
            markdown.AppendLine("categories:");
            foreach (var category in contentItem.Categories)
            {
                markdown.AppendLine($"  - \"{EscapeYamlString(category)}\"");
            }
        }

        // Tags
        if (contentItem.Tags.Count > 0)
        {
            markdown.AppendLine("tags:");
            foreach (var tag in contentItem.Tags)
            {
                markdown.AppendLine($"  - \"{EscapeYamlString(tag)}\"");
            }
        }

        // SEO metadata
        if (!string.IsNullOrEmpty(contentItem.Seo.MetaTitle))
            markdown.AppendLine($"meta_title: \"{EscapeYamlString(contentItem.Seo.MetaTitle)}\"");

        if (!string.IsNullOrEmpty(contentItem.Seo.MetaDescription))
            markdown.AppendLine($"meta_description: \"{EscapeYamlString(contentItem.Seo.MetaDescription)}\"");

        if (!string.IsNullOrEmpty(contentItem.Seo.CanonicalUrl))
            markdown.AppendLine($"canonical_url: \"{contentItem.Seo.CanonicalUrl}\"");

        if (!string.IsNullOrEmpty(contentItem.Seo.Robots) && contentItem.Seo.Robots != "index, follow")
            markdown.AppendLine($"robots: \"{contentItem.Seo.Robots}\"");

        // Add custom metadata
        foreach (var kvp in contentItem.Metadata)
        {
            // Skip properties we've already handled
            if (IsStandardFrontMatterKey(kvp.Key))
                continue;

            if (kvp.Value is string strValue)
                markdown.AppendLine($"{kvp.Key}: \"{EscapeYamlString(strValue)}\"");
            else if (kvp.Value is bool boolValue)
                markdown.AppendLine($"{kvp.Key}: {boolValue.ToString().ToLowerInvariant()}");
            else if (kvp.Value is int intValue)
                markdown.AppendLine($"{kvp.Key}: {intValue}");
            else if (kvp.Value is double doubleValue)
                markdown.AppendLine($"{kvp.Key}: {doubleValue}");
            else if (kvp.Value is DateTime dateValue)
                markdown.AppendLine($"{kvp.Key}: {dateValue:yyyy-MM-dd}");
            else
                markdown.AppendLine($"{kvp.Key}: \"{kvp.Value}\"");
        }

        markdown.AppendLine("---");
        markdown.AppendLine();

        // Add original markdown content if available, otherwise use the HTML content
        if (!string.IsNullOrEmpty(contentItem.OriginalMarkdown))
        {
            // Extract just the content part, not the front matter
            var frontMatterMatch = Regex.Match(contentItem.OriginalMarkdown, @"^\s*---\s*\n(.*?)\n\s*---\s*\n", RegexOptions.Singleline);
            if (frontMatterMatch.Success)
            {
                var contentStartIndex = frontMatterMatch.Index + frontMatterMatch.Length;
                markdown.Append(contentItem.OriginalMarkdown.Substring(contentStartIndex));
            }
            else
            {
                markdown.Append(contentItem.OriginalMarkdown);
            }
        }
        else if (!string.IsNullOrEmpty(contentItem.Content))
        {
            // Try to convert HTML back to markdown
            var plainText = _markdownProcessor.ConvertHtmlToMarkdownAsync(contentItem.Content).GetAwaiter().GetResult();
            markdown.Append(plainText);
        }

        return markdown.ToString();
    }

    /// <inheritdoc/>
    public Dictionary<string, string> ExtractFrontMatter(string content)
    {
        return _markdownProcessor.ExtractFrontMatter(content);
    }

    /// <inheritdoc/>
    public async Task<string> ConvertHtmlToMarkdownAsync(string htmlContent)
    {
        return await _markdownProcessor.ConvertHtmlToMarkdownAsync(htmlContent);
    }

    /// <inheritdoc/>
    public ValidationResult ValidateContent(ContentItem content)
    {
        var result = new ValidationResult();

        // Validate title
        if (string.IsNullOrWhiteSpace(content.Title))
        {
            result.AddError(nameof(content.Title), "Title is required");
        }
        else if (content.Title.Length > 200)
        {
            result.AddError(nameof(content.Title), "Title cannot exceed 200 characters");
        }

        // Validate slug
        if (string.IsNullOrWhiteSpace(content.Slug))
        {
            // Auto-generate slug from title if missing
            if (!string.IsNullOrWhiteSpace(content.Title))
            {
                content.Slug = content.Title.ToUrlSlug();
            }
            else
            {
                result.AddError(nameof(content.Slug), "Slug is required");
            }
        }
        else if (!IsValidSlug(content.Slug))
        {
            result.AddError(nameof(content.Slug), "Slug must contain only lowercase letters, numbers, and hyphens");
        }

        // Validate content
        if (string.IsNullOrWhiteSpace(content.Content) && string.IsNullOrWhiteSpace(content.OriginalMarkdown))
        {
            result.AddError("Content", "Content is required");
        }

        return result;
    }

    /// <inheritdoc/>
    public string RenderMarkdownToHtml(string markdown)
    {
        return _markdownProcessor.RenderToHtml(markdown);
    }

    /// <inheritdoc/>
    public BlogPost ConvertToBlogPost(ContentItem contentItem)
    {
        var blogPost = new BlogPost
        {
            Metadata = new FrontMatter
            {
                Title = contentItem.Title,
                Description = contentItem.Description,
                Author = contentItem.Author,
                Date = contentItem.DateCreated.ToString("yyyy-MM-dd"),
                FeaturedImage = contentItem.FeaturedImageUrl,
                Categories = contentItem.Categories.ToList(),
                Tags = contentItem.Tags.ToList(),
                IsFeatured = contentItem.IsFeatured,
                Published = contentItem.Status == ContentStatus.Published,
                Slug = contentItem.Slug
            },
            Content = contentItem.OriginalMarkdown ?? string.Empty,
            FilePath = contentItem.Path,
            Sha = contentItem.ProviderSpecificId ?? string.Empty
        };

        // Add any custom metadata
        foreach (var kvp in contentItem.Metadata)
        {
            if (!IsStandardFrontMatterKey(kvp.Key))
            {
                blogPost.Metadata.CustomFields[kvp.Key] = kvp.Value;
            }
        }

        return blogPost;
    }

    /// <inheritdoc/>
    public ContentItem ConvertToContentItem(BlogPost blogPost)
    {
        var contentItem = new ContentItem
        {
            Title = blogPost.Metadata.Title,
            Description = blogPost.Metadata.Description,
            Author = blogPost.Metadata.Author,
            DateCreated = string.IsNullOrEmpty(blogPost.Metadata.Date)
                ? DateTime.Now
                : DateTime.TryParse(blogPost.Metadata.Date, out var date) ? date : DateTime.Now,
            FeaturedImageUrl = blogPost.Metadata.FeaturedImage,
            IsFeatured = blogPost.Metadata.IsFeatured,
            Status = blogPost.Metadata.Published ? ContentStatus.Published : ContentStatus.Draft,
            Slug = blogPost.Metadata.Slug ?? blogPost.Metadata.Title?.ToUrlSlug() ?? "untitled",
            Path = blogPost.FilePath,
            ProviderSpecificId = blogPost.Sha,
            OriginalMarkdown = blogPost.Content
        };

        // Add categories and tags
        foreach (var category in blogPost.Metadata.Categories)
        {
            contentItem.AddCategory(category);
        }

        foreach (var tag in blogPost.Metadata.Tags)
        {
            contentItem.AddTag(tag);
        }

        // Add custom fields as metadata
        foreach (var kvp in blogPost.Metadata.CustomFields)
        {
            contentItem.SetMetadata(kvp.Key, kvp.Value);
        }

        return contentItem;
    }

    // Helper method to apply front matter to content item
    private void ApplyFrontMatterToContentItem(Dictionary<string, string> frontMatter, ContentItem contentItem)
    {
        foreach (var kvp in frontMatter)
        {
            var key = kvp.Key.ToLowerInvariant();
            var value = kvp.Value;

            switch (key)
            {
                case "title":
                    contentItem.Title = value;
                    break;
                case "author":
                    contentItem.Author = value;
                    break;
                case "description":
                    contentItem.Description = value;
                    break;
                case "date":
                    if (DateTime.TryParse(value, out var date))
                        contentItem.DateCreated = date;
                    break;
                case "last_modified":
                case "date_modified":
                    if (DateTime.TryParse(value, out var lastModified))
                        contentItem.LastModified = lastModified;
                    break;
                case "slug":
                    contentItem.Slug = value;
                    break;
                case "featured":
                case "is_featured":
                    contentItem.IsFeatured = bool.TryParse(value, out var featured) && featured;
                    break;
                case "featured_image":
                case "feature_image":
                case "image":
                    contentItem.FeaturedImageUrl = value;
                    break;
                case "content_id":
                case "localization_id":
                    contentItem.ContentId = value;
                    break;
                case "locale":
                case "language":
                    contentItem.Locale = value;
                    break;
                case "status":
                    if (Enum.TryParse<ContentStatus>(value, true, out var status))
                        contentItem.Status = status;
                    break;
                case "meta_title":
                case "seo_title":
                    contentItem.Seo.MetaTitle = value;
                    break;
                case "meta_description":
                case "seo_description":
                    contentItem.Seo.MetaDescription = value;
                    break;
                case "canonical_url":
                    contentItem.Seo.CanonicalUrl = value;
                    break;
                case "robots":
                    contentItem.Seo.Robots = value;
                    break;
                case "categories":
                case "category":
                    // Handles comma-separated list of categories
                    foreach (var category in ParseListValue(value))
                    {
                        contentItem.AddCategory(category);
                    }
                    break;
                case "tags":
                case "tag":
                    // Handles comma-separated list of tags
                    foreach (var tag in ParseListValue(value))
                    {
                        contentItem.AddTag(tag);
                    }
                    break;
                default:
                    // Add as custom metadata
                    if (bool.TryParse(value, out var boolVal))
                        contentItem.SetMetadata(key, boolVal);
                    else if (int.TryParse(value, out var intVal))
                        contentItem.SetMetadata(key, intVal);
                    else if (double.TryParse(value, out var doubleVal))
                        contentItem.SetMetadata(key, doubleVal);
                    else
                        contentItem.SetMetadata(key, value);
                    break;
            }
        }
    }

    // Parse a comma-separated list value
    private IEnumerable<string> ParseListValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            yield break;

        // Handle YAML array format [item1, item2]
        if (value.StartsWith("[") && value.EndsWith("]"))
        {
            value = value.Substring(1, value.Length - 2);
        }

        // Split by comma (or semicolon) and process each item
        foreach (var item in value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmedItem = item.Trim();

            // Remove quotes if present
            if ((trimmedItem.StartsWith("\"") && trimmedItem.EndsWith("\"")) ||
                (trimmedItem.StartsWith("'") && trimmedItem.EndsWith("'")))
            {
                trimmedItem = trimmedItem.Substring(1, trimmedItem.Length - 2);
            }

            if (!string.IsNullOrEmpty(trimmedItem))
            {
                yield return trimmedItem;
            }
        }
    }

    private bool IsStandardFrontMatterKey(string key)
    {
        var standardKeys = new[]
        {
            "title", "author", "description", "date", "last_modified", "date_modified",
            "slug", "featured", "is_featured", "featured_image", "feature_image", "image",
            "content_id", "localization_id", "locale", "language", "status",
            "meta_title", "seo_title", "meta_description", "seo_description",
            "canonical_url", "robots", "categories", "category", "tags", "tag"
        };

        return standardKeys.Contains(key.ToLowerInvariant());
    }

    private string EscapeYamlString(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }

    private bool IsValidSlug(string slug)
    {
        // Slugs should contain only lowercase letters, numbers, and hyphens
        return Regex.IsMatch(slug, "^[a-z0-9-]+$");
    }
}