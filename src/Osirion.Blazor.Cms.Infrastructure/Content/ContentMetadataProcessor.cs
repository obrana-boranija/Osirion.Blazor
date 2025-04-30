using System.Text;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;

namespace Osirion.Blazor.Cms.Infrastructure.Content;

/// <summary>
/// Implementation of IContentMetadataProcessor for processing content metadata
/// </summary>
public class ContentMetadataProcessor : IContentMetadataProcessor
{
    /// <inheritdoc/>
    public ContentItem ApplyMetadataToContentItem(Dictionary<string, string> frontMatter, ContentItem contentItem)
    {
        foreach (var kvp in frontMatter)
        {
            var key = kvp.Key.ToLowerInvariant();
            var value = kvp.Value;

            switch (key)
            {
                case "title":
                    contentItem.SetTitle(value);
                    break;
                case "author":
                    contentItem.SetAuthor(value);
                    break;
                case "description":
                    contentItem.SetDescription(value);
                    break;
                case "date":
                    if (DateTime.TryParse(value, out var date))
                        contentItem.SetCreatedDate(date);
                    break;
                case "last_modified":
                case "date_modified":
                    if (DateTime.TryParse(value, out var lastModified))
                        contentItem.SetLastModifiedDate(lastModified);
                    break;
                case "slug":
                    contentItem.SetSlug(value);
                    break;
                case "featured":
                case "is_featured":
                    contentItem.SetFeatured(bool.TryParse(value, out var featured) && featured);
                    break;
                case "featured_image":
                case "feature_image":
                case "image":
                    contentItem.SetFeaturedImage(value);
                    break;
                case "content_id":
                case "localization_id":
                    contentItem.SetContentId(value);
                    break;
                case "locale":
                case "language":
                    contentItem.SetLocale(value);
                    break;
                case "status":
                    if (Enum.TryParse<Domain.Enums.ContentStatus>(value, true, out var status))
                        contentItem.SetStatus(status);
                    break;
                case "categories":
                case "category":
                    // Process comma-separated list
                    foreach (var category in ParseListValue(value))
                    {
                        contentItem.AddCategory(category);
                    }
                    break;
                case "tags":
                case "tag":
                    // Process comma-separated list
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

        // Ensure slug is set
        if (string.IsNullOrEmpty(contentItem.Slug) && !string.IsNullOrEmpty(contentItem.Title))
        {
            contentItem.SetSlug(GenerateSlug(contentItem.Title));
        }

        return contentItem;
    }

    /// <inheritdoc/>
    public string GenerateFrontMatter(ContentItem entity)
    {
        var frontMatter = new StringBuilder();
        frontMatter.AppendLine("---");

        // Basic metadata
        if (!string.IsNullOrEmpty(entity.Title))
            frontMatter.AppendLine($"title: \"{EscapeYamlString(entity.Title)}\"");

        if (!string.IsNullOrEmpty(entity.Author))
            frontMatter.AppendLine($"author: \"{EscapeYamlString(entity.Author)}\"");

        if (!string.IsNullOrEmpty(entity.Description))
            frontMatter.AppendLine($"description: \"{EscapeYamlString(entity.Description)}\"");

        // Date created (in ISO format)
        frontMatter.AppendLine($"date: {entity.DateCreated:yyyy-MM-dd}");

        // Last modified date
        if (entity.LastModified.HasValue)
            frontMatter.AppendLine($"last_modified: {entity.LastModified.Value:yyyy-MM-dd}");

        // Content ID for localization
        if (!string.IsNullOrEmpty(entity.ContentId))
            frontMatter.AppendLine($"content_id: \"{entity.ContentId}\"");

        if (!string.IsNullOrEmpty(entity.Locale))
            frontMatter.AppendLine($"locale: \"{entity.Locale}\"");

        // Slug
        if (!string.IsNullOrEmpty(entity.Slug))
            frontMatter.AppendLine($"slug: \"{entity.Slug}\"");

        // Featured status and image
        if (entity.IsFeatured)
            frontMatter.AppendLine("featured: true");

        if (!string.IsNullOrEmpty(entity.FeaturedImageUrl))
            frontMatter.AppendLine($"featured_image: \"{entity.FeaturedImageUrl}\"");

        // Categories
        if (entity.Categories.Count > 0)
        {
            frontMatter.AppendLine("categories:");
            foreach (var category in entity.Categories)
            {
                frontMatter.AppendLine($"  - \"{EscapeYamlString(category)}\"");
            }
        }

        // Tags
        if (entity.Tags.Count > 0)
        {
            frontMatter.AppendLine("tags:");
            foreach (var tag in entity.Tags)
            {
                frontMatter.AppendLine($"  - \"{EscapeYamlString(tag)}\"");
            }
        }

        // Add custom metadata
        foreach (var kvp in entity.Metadata)
        {
            // Skip properties we've already handled
            if (IsStandardFrontMatterKey(kvp.Key))
                continue;

            if (kvp.Value is string strValue)
                frontMatter.AppendLine($"{kvp.Key}: \"{EscapeYamlString(strValue)}\"");
            else if (kvp.Value is bool boolValue)
                frontMatter.AppendLine($"{kvp.Key}: {boolValue.ToString().ToLowerInvariant()}");
            else if (kvp.Value is int intValue)
                frontMatter.AppendLine($"{kvp.Key}: {intValue}");
            else if (kvp.Value is double doubleValue)
                frontMatter.AppendLine($"{kvp.Key}: {doubleValue}");
            else if (kvp.Value is DateTime dateValue)
                frontMatter.AppendLine($"{kvp.Key}: {dateValue:yyyy-MM-dd}");
            else
                frontMatter.AppendLine($"{kvp.Key}: \"{kvp.Value}\"");
        }

        frontMatter.AppendLine("---");

        return frontMatter.ToString();
    }

    /// <inheritdoc/>
    public IEnumerable<string> ParseListValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            yield break;

        // Handle YAML array format [item1, item2]
        if (value.StartsWith("[") && value.EndsWith("]"))
        {
            value = value.Substring(1, value.Length - 2);
        }

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

    /// <summary>
    /// Escapes special characters in a YAML string
    /// </summary>
    private string EscapeYamlString(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }

    /// <summary>
    /// Determines whether a key is a standard front matter key
    /// </summary>
    private bool IsStandardFrontMatterKey(string key)
    {
        var standardKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "title", "author", "description", "date", "last_modified", "date_modified",
            "slug", "featured", "is_featured", "featured_image", "feature_image", "image",
            "content_id", "localization_id", "locale", "language", "status",
            "categories", "category", "tags", "tag"
        };

        return standardKeys.Contains(key);
    }

    /// <summary>
    /// Generates a URL-friendly slug from text
    /// </summary>
    private string GenerateSlug(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "untitled";

        // Convert to lowercase and replace spaces with hyphens
        var slug = text.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-");

        // Remove special characters
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");

        // Remove consecutive hyphens
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-{2,}", "-");

        // Trim hyphens from ends
        slug = slug.Trim('-');

        // Ensure we have something
        if (string.IsNullOrEmpty(slug))
            return "untitled";

        return slug;
    }
}