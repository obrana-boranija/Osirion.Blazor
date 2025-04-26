using Markdig;
using Osirion.Blazor.Cms.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Providers;

/// <summary>
/// Interface for content parsing and generation
/// </summary>
public interface IContentParser
{
    /// <summary>
    /// Parses markdown content and populates the content item
    /// </summary>
    Task ParseMarkdownContentAsync(string markdownContent, ContentItem contentItem);

    /// <summary>
    /// Generates markdown content with front matter from a content item
    /// </summary>
    string GenerateMarkdownWithFrontMatter(ContentItem contentItem);

    /// <summary>
    /// Extracts front matter from markdown content
    /// </summary>
    Dictionary<string, string> ExtractFrontMatter(string content);

    /// <summary>
    /// Converts HTML content to markdown
    /// </summary>
    Task<string> ConvertHtmlToMarkdownAsync(string htmlContent);
}

/// <summary>
/// Default implementation of the content parser
/// </summary>
public class ContentParser : IContentParser
{
    private readonly MarkdownPipeline _markdownPipeline;

    /// <summary>
    /// Initializes a new instance of the ContentParser
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
        // Store the original markdown if needed for editing later
        contentItem.OriginalMarkdown = markdownContent;

        // Extract front matter and content
        var frontMatterMatch = Regex.Match(markdownContent, @"^\s*---\s*\n(.*?)\n\s*---\s*\n", RegexOptions.Singleline);

        if (frontMatterMatch.Success && frontMatterMatch.Groups.Count > 1)
        {
            // Extract front matter
            var frontMatterContent = frontMatterMatch.Groups[1].Value;
            ParseFrontMatter(frontMatterContent, contentItem);

            // Extract the content part (everything after front matter)
            var contentStartIndex = frontMatterMatch.Index + frontMatterMatch.Length;
            var contentPart = markdownContent.Substring(contentStartIndex).Trim();

            // Convert Markdown to HTML
            contentItem.Content = await Task.Run(() => Markdig.Markdown.ToHtml(contentPart, _markdownPipeline));
        }
        else
        {
            // No front matter, treat entire content as markdown
            contentItem.Content = await Task.Run(() => Markdig.Markdown.ToHtml(markdownContent, _markdownPipeline));
        }
    }

    /// <inheritdoc/>
    public string GenerateMarkdownWithFrontMatter(ContentItem contentItem)
    {
        // If we have the original markdown and no content has changed, return it
        if (!string.IsNullOrEmpty(contentItem.OriginalMarkdown) && !HasContentChanged(contentItem))
        {
            return contentItem.OriginalMarkdown;
        }

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

        // Categories and tags (as YAML arrays)
        if (contentItem.Categories.Count > 0)
        {
            markdown.AppendLine("categories:");
            foreach (var category in contentItem.Categories)
            {
                markdown.AppendLine($"  - \"{EscapeYamlString(category)}\"");
            }
        }

        if (contentItem.Tags.Count > 0)
        {
            markdown.AppendLine("tags:");
            foreach (var tag in contentItem.Tags)
            {
                markdown.AppendLine($"  - \"{EscapeYamlString(tag)}\"");
            }
        }

        // SEO metadata
        if (!string.IsNullOrEmpty(contentItem.Seo.MetaTitle) && contentItem.Seo.MetaTitle != contentItem.Title)
            markdown.AppendLine($"meta_title: \"{EscapeYamlString(contentItem.Seo.MetaTitle)}\"");

        if (!string.IsNullOrEmpty(contentItem.Seo.MetaDescription) && contentItem.Seo.MetaDescription != contentItem.Description)
            markdown.AppendLine($"meta_description: \"{EscapeYamlString(contentItem.Seo.MetaDescription)}\"");

        if (!string.IsNullOrEmpty(contentItem.Seo.OgTitle) && contentItem.Seo.OgTitle != contentItem.Seo.MetaTitle)
            markdown.AppendLine($"og_title: \"{EscapeYamlString(contentItem.Seo.OgTitle)}\"");

        if (!string.IsNullOrEmpty(contentItem.Seo.OgDescription) && contentItem.Seo.OgDescription != contentItem.Seo.MetaDescription)
            markdown.AppendLine($"og_description: \"{EscapeYamlString(contentItem.Seo.OgDescription)}\"");

        if (!string.IsNullOrEmpty(contentItem.Seo.OgImageUrl))
            markdown.AppendLine($"og_image: \"{contentItem.Seo.OgImageUrl}\"");

        if (!string.IsNullOrEmpty(contentItem.Seo.OgType) && contentItem.Seo.OgType != "article")
            markdown.AppendLine($"og_type: \"{contentItem.Seo.OgType}\"");

        if (!string.IsNullOrEmpty(contentItem.Seo.CanonicalUrl))
            markdown.AppendLine($"canonical_url: \"{contentItem.Seo.CanonicalUrl}\"");

        if (!string.IsNullOrEmpty(contentItem.Seo.Robots) && contentItem.Seo.Robots != "index, follow")
            markdown.AppendLine($"robots: \"{contentItem.Seo.Robots}\"");

        // Custom metadata
        foreach (var kvp in contentItem.Metadata)
        {
            // Skip properties we've already handled
            if (IsStandardMetadataKey(kvp.Key))
                continue;

            if (kvp.Value is string strValue)
                markdown.AppendLine($"{kvp.Key}: \"{EscapeYamlString(strValue)}\"");
            else if (kvp.Value is bool boolValue)
                markdown.AppendLine($"{kvp.Key}: {boolValue.ToString().ToLowerInvariant()}");
            else if (kvp.Value is int || kvp.Value is double || kvp.Value is float)
                markdown.AppendLine($"{kvp.Key}: {kvp.Value}");
            else if (kvp.Value is DateTime dateValue)
                markdown.AppendLine($"{kvp.Key}: {dateValue:yyyy-MM-dd}");
            else if (kvp.Value is IEnumerable<string> listValue)
            {
                markdown.AppendLine($"{kvp.Key}:");
                foreach (var item in listValue)
                {
                    markdown.AppendLine($"  - \"{EscapeYamlString(item)}\"");
                }
            }
            else
                markdown.AppendLine($"{kvp.Key}: \"{kvp.Value}\"");
        }

        markdown.AppendLine("---");
        markdown.AppendLine();

        // Add content - convert from HTML if we have HTML content
        if (!string.IsNullOrEmpty(contentItem.Content))
        {
            // TODO: Implement proper HTML to Markdown conversion
            // For now, we'll just use a placeholder
            markdown.AppendLine(StripHtmlTags(contentItem.Content));
        }

        return markdown.ToString();
    }

    /// <inheritdoc/>
    public Dictionary<string, string> ExtractFrontMatter(string content)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Look for front matter between --- delimiters
        var match = Regex.Match(content, @"^\s*---\s*\n(.*?)\n\s*---\s*\n", RegexOptions.Singleline);
        if (match.Success && match.Groups.Count > 1)
        {
            var frontMatterContent = match.Groups[1].Value;
            ParseFrontMatterLines(frontMatterContent.Split('\n'), result);
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<string> ConvertHtmlToMarkdownAsync(string htmlContent)
    {
        // This would be a placeholder implementation - for a real implementation,
        // consider using a library like HtmlAgilityPack combined with custom logic
        // or a specialized HTML-to-Markdown converter

        // For now, just strip HTML tags as a minimal implementation
        return await Task.Run(() => StripHtmlTags(htmlContent));
    }

    private void ParseFrontMatter(string frontMatter, ContentItem contentItem)
    {
        var lines = frontMatter.Split('\n');
        var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        ParseFrontMatterLines(lines, metadata);

        // Now apply the extracted metadata to the content item
        ApplyMetadataToContentItem(metadata, contentItem);
    }

    private void ParseFrontMatterLines(string[] lines, Dictionary<string, string> metadata)
    {
        string? currentList = null;
        List<string> currentItems = new();

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine))
                continue;

            // Check if this is a new list
            if (trimmedLine.EndsWith(':'))
            {
                // Save any previous list
                if (currentList != null && currentItems.Count > 0)
                {
                    metadata[currentList] = $"[{string.Join(", ", currentItems.Select(QuoteValue))}]";
                }

                // Start a new list
                currentList = trimmedLine.TrimEnd(':');
                currentItems = new List<string>();
                continue;
            }

            // Check if this is a list item
            if (trimmedLine.StartsWith("  - ") || trimmedLine.StartsWith("- "))
            {
                var item = trimmedLine.TrimStart(' ', '-').Trim();
                // Remove quotes if present
                item = TrimQuotes(item);

                currentItems.Add(item);
                continue;
            }

            // Process key-value pair
            var separatorIndex = trimmedLine.IndexOf(':');
            if (separatorIndex > 0)
            {
                // Save any previous list
                if (currentList != null && currentItems.Count > 0)
                {
                    metadata[currentList] = $"[{string.Join(", ", currentItems.Select(QuoteValue))}]";
                    currentList = null;
                    currentItems.Clear();
                }

                var key = trimmedLine.Substring(0, separatorIndex).Trim();
                var value = trimmedLine.Substring(separatorIndex + 1).Trim();

                // Remove quotes if present
                value = TrimQuotes(value);

                metadata[key] = value;
            }
        }

        // Save any final list
        if (currentList != null && currentItems.Count > 0)
        {
            metadata[currentList] = $"[{string.Join(", ", currentItems.Select(QuoteValue))}]";
        }
    }

    private void ApplyMetadataToContentItem(Dictionary<string, string> metadata, ContentItem contentItem)
    {
        foreach (var kvp in metadata)
        {
            var key = kvp.Key.ToLowerInvariant();
            var value = kvp.Value;

            switch (key)
            {
                // Basic metadata
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
                case "date_created":
                    if (DateTime.TryParse(value, out var dateCreated))
                        contentItem.DateCreated = dateCreated;
                    break;
                case "date_modified":
                case "last_modified":
                    if (DateTime.TryParse(value, out var dateModified))
                        contentItem.LastModified = dateModified;
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

                // Localization
                case "content_id":
                case "localization_id":
                    contentItem.ContentId = value;
                    break;
                case "locale":
                case "language":
                    contentItem.Locale = value;
                    break;

                // Categories and tags
                case "categories":
                case "category":
                    ProcessList(value, item => contentItem.AddCategory(item));
                    break;
                case "tags":
                case "tag":
                    ProcessList(value, item => contentItem.AddTag(item));
                    break;

                // SEO metadata
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
                case "og_title":
                    contentItem.Seo.OgTitle = value;
                    break;
                case "og_description":
                    contentItem.Seo.OgDescription = value;
                    break;
                case "og_image":
                    contentItem.Seo.OgImageUrl = value;
                    break;
                case "og_type":
                    contentItem.Seo.OgType = value;
                    break;
                case "twitter_card":
                    contentItem.Seo.TwitterCard = value;
                    break;
                case "twitter_title":
                    contentItem.Seo.TwitterTitle = value;
                    break;
                case "twitter_description":
                    contentItem.Seo.TwitterDescription = value;
                    break;
                case "twitter_image":
                    contentItem.Seo.TwitterImageUrl = value;
                    break;
                case "schema_type":
                    contentItem.Seo.SchemaType = value;
                    break;
                case "json_ld":
                    contentItem.Seo.JsonLd = value;
                    break;

                // Other metadata
                default:
                    contentItem.SetMetadata(key, value);
                    break;
            }
        }

        // Set defaults for SEO fields if not provided
        if (string.IsNullOrEmpty(contentItem.Seo.MetaTitle))
        {
            contentItem.Seo.MetaTitle = contentItem.Title;
        }

        if (string.IsNullOrEmpty(contentItem.Seo.MetaDescription))
        {
            contentItem.Seo.MetaDescription = contentItem.Description;
        }

        if (string.IsNullOrEmpty(contentItem.Seo.OgTitle))
        {
            contentItem.Seo.OgTitle = contentItem.Seo.MetaTitle;
        }

        if (string.IsNullOrEmpty(contentItem.Seo.OgDescription))
        {
            contentItem.Seo.OgDescription = contentItem.Seo.MetaDescription;
        }

        if (string.IsNullOrEmpty(contentItem.Seo.TwitterTitle))
        {
            contentItem.Seo.TwitterTitle = contentItem.Seo.OgTitle;
        }

        if (string.IsNullOrEmpty(contentItem.Seo.TwitterDescription))
        {
            contentItem.Seo.TwitterDescription = contentItem.Seo.OgDescription;
        }
    }

    private void ProcessList(string value, Action<string> addAction)
    {
        if (string.IsNullOrEmpty(value))
            return;

        // Check if the value is a YAML-like array
        if (value.StartsWith("[") && value.EndsWith("]"))
        {
            // Split by comma, but handle quoted values correctly
            var items = SplitYamlArray(value.Substring(1, value.Length - 2));
            foreach (var item in items)
            {
                var trimmedItem = TrimQuotes(item.Trim());
                if (!string.IsNullOrEmpty(trimmedItem))
                {
                    addAction(trimmedItem);
                }
            }
        }
        else
        {
            // Single value
            var trimmedValue = TrimQuotes(value.Trim());
            if (!string.IsNullOrEmpty(trimmedValue))
            {
                addAction(trimmedValue);
            }
        }
    }

    private string[] SplitYamlArray(string arrayContent)
    {
        var result = new List<string>();
        var currentValue = new StringBuilder();
        bool inQuotes = false;
        char quoteChar = '"';

        for (int i = 0; i < arrayContent.Length; i++)
        {
            char c = arrayContent[i];

            if ((c == '"' || c == '\'') && (i == 0 || arrayContent[i - 1] != '\\'))
            {
                if (!inQuotes)
                {
                    inQuotes = true;
                    quoteChar = c;
                }
                else if (c == quoteChar)
                {
                    inQuotes = false;
                }
                else
                {
                    currentValue.Append(c);
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(currentValue.ToString());
                currentValue.Clear();
            }
            else
            {
                currentValue.Append(c);
            }
        }

        if (currentValue.Length > 0)
        {
            result.Add(currentValue.ToString());
        }

        return result.Select(s => s.Trim()).ToArray();
    }

    private string TrimQuotes(string value)
    {
        if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
            (value.StartsWith("'") && value.EndsWith("'")))
        {
            return value.Substring(1, value.Length - 2);
        }

        return value;
    }

    private string QuoteValue(string value)
    {
        return $"\"{EscapeYamlString(value)}\"";
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

    private bool IsStandardMetadataKey(string key)
    {
        var standardKeys = new[]
        {
            "title", "author", "description", "date", "date_created", "date_modified",
            "last_modified", "slug", "featured", "is_featured", "featured_image",
            "feature_image", "image", "content_id", "localization_id", "locale",
            "language", "categories", "category", "tags", "tag", "meta_title",
            "seo_title", "meta_description", "seo_description", "canonical_url",
            "robots", "og_title", "og_description", "og_image", "og_type",
            "twitter_card", "twitter_title", "twitter_description", "twitter_image",
            "schema_type", "json_ld"
        };

        return standardKeys.Contains(key.ToLowerInvariant());
    }

    private bool HasContentChanged(ContentItem contentItem)
    {
        // If there's no original markdown, consider it changed
        if (string.IsNullOrEmpty(contentItem.OriginalMarkdown))
            return true;

        // Parse the original markdown
        var originalFrontMatter = ExtractFrontMatter(contentItem.OriginalMarkdown);
        var tempItem = new ContentItem();
        ApplyMetadataToContentItem(originalFrontMatter, tempItem);

        // Compare key properties
        if (tempItem.Title != contentItem.Title ||
            tempItem.Description != contentItem.Description ||
            tempItem.Author != contentItem.Author ||
            tempItem.IsFeatured != contentItem.IsFeatured ||
            tempItem.Slug != contentItem.Slug ||
            tempItem.FeaturedImageUrl != contentItem.FeaturedImageUrl)
            return true;

        // Compare collections (tags, categories)
        if (!AreCollectionsEqual(tempItem.Tags, contentItem.Tags) ||
            !AreCollectionsEqual(tempItem.Categories, contentItem.Categories))
            return true;

        // For now, consider content changed if we have HTML content
        // A more sophisticated implementation would compare the actual content
        return !string.IsNullOrEmpty(contentItem.Content);
    }

    private bool AreCollectionsEqual(IReadOnlyList<string> first, IReadOnlyList<string> second)
    {
        if (first.Count != second.Count)
            return false;

        for (int i = 0; i < first.Count; i++)
        {
            if (!first[i].Equals(second[i], StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

    private string StripHtmlTags(string html)
    {
        // Simple regex-based HTML tag stripper
        // Note: This is a naive implementation and won't handle all HTML correctly
        // For production use, consider a proper HTML-to-Markdown converter
        var text = Regex.Replace(html, "<.*?>", string.Empty);

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
}