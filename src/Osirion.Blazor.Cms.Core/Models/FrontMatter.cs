using System.Text;
using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Core.Models;

/// <summary>
/// Represents the frontmatter metadata for a blog post or page
/// </summary>
public class FrontMatter
{
    /// <summary>
    /// Gets or sets the title of the post
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the post
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the author of the post
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of the post in ISO format (yyyy-MM-dd)
    /// </summary>
    public string Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

    /// <summary>
    /// Gets or sets the URL for the featured image
    /// </summary>
    public string? FeaturedImage { get; set; }

    /// <summary>
    /// Gets or sets the categories of the post
    /// </summary>
    public List<string> Categories { get; set; } = new();

    /// <summary>
    /// Gets or sets the tags of the post
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Gets or sets whether the post is featured
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Gets or sets whether the post is published
    /// </summary>
    public bool Published { get; set; } = true;

    /// <summary>
    /// Gets or sets the layout template to use for the post
    /// </summary>
    public string? Layout { get; set; }

    /// <summary>
    /// Gets or sets the URL slug for the post
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// Gets or sets custom fields for the post
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object> CustomFields { get; set; } = new();

    /// <summary>
    /// Creates a YAML representation of the frontmatter
    /// </summary>
    public string ToYaml()
    {
        var yaml = new StringBuilder();
        yaml.AppendLine("---");

        // Add title if not empty
        if (!string.IsNullOrWhiteSpace(Title))
            yaml.AppendLine($"title: \"{EscapeYamlString(Title)}\"");

        // Add description if not empty
        if (!string.IsNullOrWhiteSpace(Description))
            yaml.AppendLine($"description: \"{EscapeYamlString(Description)}\"");

        // Add author if not empty
        if (!string.IsNullOrWhiteSpace(Author))
            yaml.AppendLine($"author: \"{EscapeYamlString(Author)}\"");

        // Add date if not empty
        if (!string.IsNullOrWhiteSpace(Date))
            yaml.AppendLine($"date: {Date}");

        // Add featured image if not empty
        if (!string.IsNullOrWhiteSpace(FeaturedImage))
            yaml.AppendLine($"featuredImage: \"{FeaturedImage}\"");

        // Add categories if any
        if (Categories != null && Categories.Count > 0)
        {
            yaml.AppendLine("categories:");
            foreach (var category in Categories)
            {
                yaml.AppendLine($"  - \"{EscapeYamlString(category)}\"");
            }
        }

        // Add tags if any
        if (Tags != null && Tags.Count > 0)
        {
            yaml.AppendLine("tags:");
            foreach (var tag in Tags)
            {
                yaml.AppendLine($"  - \"{EscapeYamlString(tag)}\"");
            }
        }

        // Add featured flag if true
        if (IsFeatured)
            yaml.AppendLine("featured: true");

        // Add published flag if not true (defaults to true)
        if (!Published)
            yaml.AppendLine("published: false");

        // Add layout if specified
        if (!string.IsNullOrWhiteSpace(Layout))
            yaml.AppendLine($"layout: \"{Layout}\"");

        // Add slug if specified
        if (!string.IsNullOrWhiteSpace(Slug))
            yaml.AppendLine($"slug: \"{Slug}\"");

        // Add custom fields if any
        if (CustomFields != null && CustomFields.Count > 0)
        {
            foreach (var field in CustomFields)
            {
                if (field.Value is string strValue)
                {
                    yaml.AppendLine($"{field.Key}: \"{EscapeYamlString(strValue)}\"");
                }
                else if (field.Value is bool boolValue)
                {
                    yaml.AppendLine($"{field.Key}: {boolValue.ToString().ToLowerInvariant()}");
                }
                else if (field.Value is int || field.Value is double || field.Value is float)
                {
                    yaml.AppendLine($"{field.Key}: {field.Value}");
                }
                else if (field.Value is DateTime dateValue)
                {
                    yaml.AppendLine($"{field.Key}: {dateValue:yyyy-MM-dd}");
                }
                else
                {
                    // For complex objects, serialize as string representation
                    yaml.AppendLine($"{field.Key}: \"{field.Value}\"");
                }
            }
        }

        yaml.AppendLine("---");
        return yaml.ToString();
    }

    /// <summary>
    /// Parses frontmatter from YAML
    /// </summary>
    public static FrontMatter FromYaml(string yaml)
    {
        var frontMatter = new FrontMatter();

        if (string.IsNullOrWhiteSpace(yaml))
            return frontMatter;

        // Simple line-by-line parsing for common properties
        var lines = yaml.Split('\n');

        string? currentList = null;
        List<string> currentItems = new();

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            // Skip the opening and closing --- lines
            if (trimmedLine == "---")
                continue;

            // Check if this is a new list
            if (trimmedLine.EndsWith(':'))
            {
                // Save any previous list
                if (currentList != null && currentItems.Count > 0)
                {
                    AssignList(frontMatter, currentList, currentItems);
                }

                // Start a new list
                currentList = trimmedLine.TrimEnd(':');
                currentItems = new List<string>();
                continue;
            }

            // Check if this is a list item
            if (trimmedLine.StartsWith("  - "))
            {
                var item = trimmedLine.Substring(4).Trim();
                // Remove quotes if present
                if (item.StartsWith("\"") && item.EndsWith("\""))
                {
                    item = item.Substring(1, item.Length - 2);
                }

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
                    AssignList(frontMatter, currentList, currentItems);
                    currentList = null;
                    currentItems.Clear();
                }

                var key = trimmedLine.Substring(0, separatorIndex).Trim();
                var value = trimmedLine.Substring(separatorIndex + 1).Trim();

                // Remove quotes if present
                if (value.StartsWith("\"") && value.EndsWith("\""))
                {
                    value = value.Substring(1, value.Length - 2);
                }

                AssignProperty(frontMatter, key, value);
            }
        }

        // Save any final list
        if (currentList != null && currentItems.Count > 0)
        {
            AssignList(frontMatter, currentList, currentItems);
        }

        return frontMatter;
    }

    private static void AssignList(FrontMatter frontMatter, string listName, List<string> items)
    {
        switch (listName.ToLowerInvariant())
        {
            case "categories":
                frontMatter.Categories = new List<string>(items);
                break;
            case "tags":
                frontMatter.Tags = new List<string>(items);
                break;
            default:
                // For custom list properties, add as a custom field
                frontMatter.CustomFields[listName] = items;
                break;
        }
    }

    private static void AssignProperty(FrontMatter frontMatter, string key, string value)
    {
        switch (key.ToLowerInvariant())
        {
            case "title":
                frontMatter.Title = value;
                break;
            case "description":
                frontMatter.Description = value;
                break;
            case "author":
                frontMatter.Author = value;
                break;
            case "date":
                frontMatter.Date = value;
                break;
            case "featuredimage":
                frontMatter.FeaturedImage = value;
                break;
            case "featured":
                frontMatter.IsFeatured = bool.TryParse(value, out var featured) && featured;
                break;
            case "published":
                frontMatter.Published = !bool.TryParse(value, out var published) || published;
                break;
            case "layout":
                frontMatter.Layout = value;
                break;
            case "slug":
                frontMatter.Slug = value;
                break;
            default:
                // For any other properties, add as custom fields
                if (bool.TryParse(value, out var boolValue))
                {
                    frontMatter.CustomFields[key] = boolValue;
                }
                else if (int.TryParse(value, out var intValue))
                {
                    frontMatter.CustomFields[key] = intValue;
                }
                else if (double.TryParse(value, out var doubleValue))
                {
                    frontMatter.CustomFields[key] = doubleValue;
                }
                else
                {
                    frontMatter.CustomFields[key] = value;
                }
                break;
        }
    }

    private static string EscapeYamlString(string value)
    {
        // Replace any double quotes with escaped double quotes
        return value.Replace("\"", "\\\"");
    }
}