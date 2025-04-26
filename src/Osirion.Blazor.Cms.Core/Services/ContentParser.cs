using Markdig;
using Osirion.Blazor.Cms.Interfaces;
using Osirion.Blazor.Cms.Models;
using Osirion.Blazor.Core.Extensions;
using System.Text;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Services;

/// <summary>
/// Default implementation of IContentParser
/// </summary>
public class ContentParser : IContentParser
{
    private readonly MarkdownPipeline _markdownPipeline;
    private static readonly Regex _frontMatterRegex = new(@"^\s*---\s*\n(.*?)\n\s*---\s*\n", RegexOptions.Singleline);

    /// <summary>
    /// Initializes a new instance of the ContentParser class
    /// </summary>
    public ContentParser()
    {
        _markdownPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseYamlFrontMatter()
            .Build();
    }

    /// <inheritdoc/>
    public async Task ParseMarkdownContentAsync(string markdownContent, ContentItem contentItem)
    {
        if (string.IsNullOrWhiteSpace(markdownContent))
            return;

        // Store original markdown for later use
        contentItem.OriginalMarkdown = markdownContent;

        // Extract front matter and content
        var match = _frontMatterRegex.Match(markdownContent);

        if (match.Success && match.Groups.Count > 1)
        {
            var frontMatterContent = match.Groups[1].Value;
            ParseFrontMatter(frontMatterContent, contentItem);

            // Extract the content (everything after front matter)
            var contentStartIndex = match.Index + match.Length;
            var content = markdownContent.Substring(contentStartIndex).Trim();

            // Convert Markdown to HTML
            contentItem.Content = await Task.Run(() => Markdown.ToHtml(content, _markdownPipeline));
        }
        else
        {
            // No front matter, treat entire content as markdown
            contentItem.Content = await Task.Run(() => Markdown.ToHtml(markdownContent, _markdownPipeline));
        }
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
            var match = _frontMatterRegex.Match(contentItem.OriginalMarkdown);
            if (match.Success)
            {
                // Extract just the content part, not the front matter
                var contentStartIndex = match.Index + match.Length;
                markdown.Append(contentItem.OriginalMarkdown.Substring(contentStartIndex));
            }
            else
            {
                markdown.Append(contentItem.OriginalMarkdown);
            }
        }
        else if (!string.IsNullOrEmpty(contentItem.Content))
        {
            // Try to convert HTML back to markdown (simplified version - a real implementation would be more complex)
            markdown.Append(StripHtmlTags(contentItem.Content));
        }

        return markdown.ToString();
    }

    /// <inheritdoc/>
    public Dictionary<string, string> ExtractFrontMatter(string content)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var match = _frontMatterRegex.Match(content);
        if (match.Success && match.Groups.Count > 1)
        {
            var frontMatterContent = match.Groups[1].Value;
            var lines = frontMatterContent.Split('\n');

            foreach (var line in lines)
            {
                // Skip empty lines
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine))
                    continue;

                // Parse key-value pair
                var separatorIndex = trimmedLine.IndexOf(':');
                if (separatorIndex > 0)
                {
                    var key = trimmedLine.Substring(0, separatorIndex).Trim();
                    var value = trimmedLine.Substring(separatorIndex + 1).Trim();

                    // Remove quotes if present
                    if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                        (value.StartsWith("'") && value.EndsWith("'")))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    result[key] = value;
                }
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<string> ConvertHtmlToMarkdownAsync(string htmlContent)
    {
        // A proper implementation would use an HTML-to-Markdown converter
        // This is a simplified placeholder that just strips HTML tags
        return await Task.FromResult(StripHtmlTags(htmlContent));
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
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        // Remove front matter if present
        var match = _frontMatterRegex.Match(markdown);
        if (match.Success)
        {
            var contentStartIndex = match.Index + match.Length;
            markdown = markdown.Substring(contentStartIndex).Trim();
        }

        return Markdown.ToHtml(markdown, _markdownPipeline);
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

    private void ParseFrontMatter(string frontMatter, ContentItem contentItem)
    {
        var lines = frontMatter.Split('\n');

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine))
                continue;

            // Check if it's a list item (categories, tags, etc.)
            if (trimmedLine.StartsWith("  - ") || trimmedLine.StartsWith("- "))
            {
                // Currently, we don't handle nested lists in this simplified parser
                continue;
            }

            // Parse key-value pair
            var separatorIndex = trimmedLine.IndexOf(':');
            if (separatorIndex > 0)
            {
                var key = trimmedLine.Substring(0, separatorIndex).Trim().ToLowerInvariant();
                var value = trimmedLine.Substring(separatorIndex + 1).Trim();

                // Remove quotes if present
                if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                    (value.StartsWith("'") && value.EndsWith("'")))
                {
                    value = value.Substring(1, value.Length - 2);
                }

                ApplyFrontMatterValue(key, value, contentItem);
            }
        }

        // Process lists (categories, tags) in a second pass
        var currentList = string.Empty;
        var listItems = new List<string>();

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrEmpty(trimmedLine))
                continue;

            if (trimmedLine.EndsWith(":"))
            {
                // Start of a new list
                if (currentList != string.Empty && listItems.Count > 0)
                {
                    // Apply previous list
                    ApplyFrontMatterList(currentList, listItems, contentItem);
                    listItems.Clear();
                }

                currentList = trimmedLine.Substring(0, trimmedLine.Length - 1).Trim().ToLowerInvariant();
            }
            else if ((trimmedLine.StartsWith("  - ") || trimmedLine.StartsWith("- ")) && !string.IsNullOrEmpty(currentList))
            {
                var item = trimmedLine.TrimStart(' ', '-').Trim();

                // Remove quotes if present
                if ((item.StartsWith("\"") && item.EndsWith("\"")) ||
                    (item.StartsWith("'") && item.EndsWith("'")))
                {
                    item = item.Substring(1, item.Length - 2);
                }

                listItems.Add(item);
            }
        }

        // Apply any final list
        if (currentList != string.Empty && listItems.Count > 0)
        {
            ApplyFrontMatterList(currentList, listItems, contentItem);
        }
    }

    private void ApplyFrontMatterValue(string key, string value, ContentItem contentItem)
    {
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

    private void ApplyFrontMatterList(string listName, List<string> items, ContentItem contentItem)
    {
        switch (listName.ToLowerInvariant())
        {
            case "categories":
            case "category":
                foreach (var item in items)
                {
                    contentItem.AddCategory(item);
                }
                break;
            case "tags":
            case "tag":
                foreach (var item in items)
                {
                    contentItem.AddTag(item);
                }
                break;
            default:
                // Add as custom metadata
                contentItem.SetMetadata(listName, items);
                break;
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

    private string StripHtmlTags(string html)
    {
        // Simple regex-based HTML tag stripper
        // Note: For production, use a proper HTML-to-Markdown converter
        var text = Regex.Replace(html, "<[^>]*>", string.Empty);

        // Replace common HTML entities
        text = text.Replace("&nbsp;", " ")
                   .Replace("&lt;", "<")
                   .Replace("&gt;", ">")
                   .Replace("&amp;", "&")
                   .Replace("&quot;", "\"")
                   .Replace("&#39;", "'");

        // Replace multiple blank lines with a single one
        text = Regex.Replace(text, @"\n\s*\n", "\n\n");

        return text.Trim();
    }

    private bool IsValidSlug(string slug)
    {
        // Slugs should contain only lowercase letters, numbers, and hyphens
        return Regex.IsMatch(slug, "^[a-z0-9-]+$");
    }
}