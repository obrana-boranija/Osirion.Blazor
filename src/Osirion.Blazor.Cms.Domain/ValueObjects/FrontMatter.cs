using Osirion.Blazor.Cms.Domain.Common;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Osirion.Blazor.Cms.Domain.ValueObjects;

/// <summary>
/// Represents the frontmatter metadata for a blog post or page
/// </summary>
public class FrontMatter : ValueObject
{
    /// <summary>
    /// Gets the Id
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets the order
    /// </summary>
    public int? Order { get; set; }

    /// <summary>
    /// The layout template to use from the _layouts directory.
    /// Set to null or "none" to bypass layouts (useful for RSS, JSON feeds).
    /// </summary>
    public string? Layout { get; set; }

    /// <summary>
    /// Gets the title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Custom URL for this content. Can use placeholders like :year, :title.
    /// Example: "/blog/:year/:title/" or "/custom-url/"
    /// </summary>
    public string? Permalink { get; set; }

    /// <summary>
    /// Gets the description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets the author
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Gets the date in ISO format (yyyy-MM-dd)
    /// </summary>
    public string Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

    /// <summary>
    /// Gets the URL for the featured image
    /// </summary>
    public string? FeaturedImage { get; set; }

    /// <summary>
    /// Gets the categories
    /// </summary>
    public List<string> Categories { get; set; } = new List<string>();

    /// <summary>
    /// Gets the tags
    /// </summary>
    public List<string> Tags { get; set; } = new List<string>();

    /// <summary>
    /// Gets whether the post is featured
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Gets whether the post is published
    /// </summary>
    public bool Published { get; set; } = true;

    /// <summary>
    /// Gets the URL slug
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// Gets the language
    /// </summary>
    public string Lang { get; set; } = "en";

    /// <summary>
    /// Gets custom fields
    /// </summary>
    public Dictionary<string, object> CustomFields { get; set; } = new Dictionary<string, object>();

    // Value objects
    public SeoMetadata SeoProperties { get; set; } = new SeoMetadata();

    // Private constructor to enforce creation through factory method
    public FrontMatter() { }

    /// <summary>
    /// Factory method to create a new FrontMatter instance
    /// </summary>
    public static FrontMatter Create(string title)
    {
        var frontMatter = new FrontMatter
        {
            Title = title
        };

        return frontMatter;
    }

    /// <summary>
    /// Factory method to create a new FrontMatter instance
    /// </summary>
    public static FrontMatter Create(string title, string description,  DateTime dateCreated)
    {
        var frontMatter = new FrontMatter
        {
            Title = title,
            Description = description,
            Date = dateCreated.ToString()
        };

        return frontMatter;
    }

    /// <summary>
    /// Factory method to create a new FrontMatter instance
    /// </summary>
    public static FrontMatter Create(
        string id,
        string title,
        string? description = null,
        string? author = null,
        DateTime? date = null,
        string? featuredImage = null,
        IEnumerable<string>? categories = null,
        IEnumerable<string>? tags = null,
        bool isFeatured = false,
        bool published = true,
        string? layout = null,
        string? slug = null,
        IDictionary<string, object>? customFields = null)
    {
        var frontMatter = new FrontMatter
        {
            Title = title,
            Description = description ?? string.Empty,
            Author = author ?? string.Empty,
            Date = (date ?? DateTime.Now).ToString("yyyy-MM-dd"),
            FeaturedImage = featuredImage,
            IsFeatured = isFeatured,
            Published = published,
            Layout = layout,
            Slug = slug
        };

        // Set categories if provided
        if (categories is not null)
        {
            frontMatter = frontMatter.WithCategories(categories);
        }

        // Set tags if provided
        if (tags is not null)
        {
            frontMatter = frontMatter.WithTags(tags);
        }

        // Set custom fields if provided
        if (customFields is not null)
        {
            frontMatter = frontMatter.WithCustomFields(customFields);
        }

        return frontMatter;
    }

    // Fluent builder methods
    public FrontMatter WithTitle(string title)
    {
        var clone = Clone();
        clone.Title = title;
        return clone;
    }

    public FrontMatter WithDescription(string description)
    {
        var clone = Clone();
        clone.Description = description;
        return clone;
    }

    public FrontMatter WithAuthor(string author)
    {
        var clone = Clone();
        clone.Author = author;
        return clone;
    }

    public FrontMatter WithDate(DateTime date)
    {
        var clone = Clone();
        clone.Date = date.ToString("yyyy-MM-dd");
        return clone;
    }

    public FrontMatter WithFeaturedImage(string? featuredImage)
    {
        var clone = Clone();
        clone.FeaturedImage = featuredImage;
        return clone;
    }

    public FrontMatter WithCategories(IEnumerable<string> categories)
    {
        var clone = Clone();
        clone.Categories = categories is not null
            ? new List<string>(categories)
            : new List<string>();
        return clone;
    }

    public FrontMatter WithTags(IEnumerable<string> tags)
    {
        var clone = Clone();
        clone.Tags = tags is not null
            ? new List<string>(tags)
            : new List<string>();
        return clone;
    }

    public FrontMatter WithFeatured(bool isFeatured)
    {
        var clone = Clone();
        clone.IsFeatured = isFeatured;
        return clone;
    }

    public FrontMatter WithPublished(bool published)
    {
        var clone = Clone();
        clone.Published = published;
        return clone;
    }

    public FrontMatter WithLayout(string? layout)
    {
        var clone = Clone();
        clone.Layout = layout;
        return clone;
    }

    public FrontMatter WithSlug(string? slug)
    {
        var clone = Clone();
        clone.Slug = slug;
        return clone;
    }

    public FrontMatter WithCustomFields(IDictionary<string, object> customFields)
    {
        if (customFields is null)
            throw new ArgumentNullException(nameof(customFields));

        var clone = Clone();

        // Create a new dictionary with the custom fields
        var newCustomFields = new Dictionary<string, object>();
        foreach (var kvp in customFields)
        {
            newCustomFields[kvp.Key] = kvp.Value;
        }

        clone.CustomFields = newCustomFields;
        return clone;
    }

    public FrontMatter WithCustomField(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        var clone = Clone();

        // Create a new dictionary with the existing custom fields plus the new one
        var newCustomFields = new Dictionary<string, object>(CustomFields);
        newCustomFields[key] = value;
        clone.CustomFields = newCustomFields;

        return clone;
    }

    /// <summary>
    /// Creates a YAML representation of the frontmatter
    /// </summary>
    public string ToYaml()
    {
        // Configure serializer for clean Jekyll-compatible output
        var serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build();

        // Serialize just the frontmatter
        string yamlContent = serializer.Serialize(this);

        // Build the complete Jekyll file with proper structure
        var jekyllFile = new StringBuilder();
        jekyllFile.AppendLine("---");
        jekyllFile.Append(yamlContent); // YAML serializer adds its own final newline
        jekyllFile.AppendLine("---");
        jekyllFile.AppendLine(); // Empty line between frontmatter and content

        return jekyllFile.ToString();
    }

    /// <summary>
    /// Creates a deep clone of this FrontMatter
    /// </summary>
    public FrontMatter Clone()
    {
        return new FrontMatter
        {
            Title = Title,
            Description = Description,
            Author = Author,
            Date = Date,
            FeaturedImage = FeaturedImage,
            IsFeatured = IsFeatured,
            Published = Published,
            Layout = Layout,
            Slug = Slug,
            Categories = new List<string>(Categories),
            Tags = new List<string>(Tags),
            CustomFields = new Dictionary<string, object>(CustomFields),
            SeoProperties = SeoProperties,
        };
    }

    /// <summary>
    /// Static method to create FrontMatter from a dictionary of values
    /// </summary>
    public static FrontMatter FromDictionary(IDictionary<string, string> values)
    {
        if (values is null)
            throw new ArgumentNullException(nameof(values));

        // Extract required title or use empty string
        string title = values.TryGetValue("title", out var titleValue) ? titleValue : string.Empty;

        // Create the base front matter
        var frontMatter = Create(title);

        // Update properties from the dictionary
        if (values.TryGetValue("description", out var description))
            frontMatter = frontMatter.WithDescription(description);

        if (values.TryGetValue("author", out var author))
            frontMatter = frontMatter.WithAuthor(author);

        if (values.TryGetValue("date", out var dateStr) && DateTime.TryParse(dateStr, out var date))
            frontMatter = frontMatter.WithDate(date);

        if (values.TryGetValue("featuredImage", out var featuredImage) ||
            values.TryGetValue("featured_image", out featuredImage))
            frontMatter = frontMatter.WithFeaturedImage(featuredImage);

        if (values.TryGetValue("featured", out var featuredStr) ||
            values.TryGetValue("isFeatured", out featuredStr))
            frontMatter = frontMatter.WithFeatured(bool.TryParse(featuredStr, out var featured) && featured);

        if (values.TryGetValue("published", out var publishedStr))
            frontMatter = frontMatter.WithPublished(!bool.TryParse(publishedStr, out var published) || published);

        if (values.TryGetValue("layout", out var layout))
            frontMatter = frontMatter.WithLayout(layout);

        if (values.TryGetValue("slug", out var slug))
            frontMatter = frontMatter.WithSlug(slug);

        // Handle categories and tags
        if (values.TryGetValue("categories", out var categoriesStr) ||
            values.TryGetValue("category", out categoriesStr))
        {
            var categories = ParseListValue(categoriesStr);
            frontMatter = frontMatter.WithCategories(categories);
        }

        if (values.TryGetValue("tags", out var tagsStr) ||
            values.TryGetValue("tag", out tagsStr))
        {
            var tags = ParseListValue(tagsStr);
            frontMatter = frontMatter.WithTags(tags);
        }

        // Add remaining values as custom fields
        var customFields = new Dictionary<string, object>();
        foreach (var kvp in values)
        {
            if (!IsStandardKey(kvp.Key))
            {
                // Try to parse as different types
                if (bool.TryParse(kvp.Value, out var boolVal))
                    customFields[kvp.Key] = boolVal;
                else if (int.TryParse(kvp.Value, out var intVal))
                    customFields[kvp.Key] = intVal;
                else if (double.TryParse(kvp.Value, out var doubleVal))
                    customFields[kvp.Key] = doubleVal;
                else
                    customFields[kvp.Key] = kvp.Value;
            }
        }

        if (customFields.Count > 0)
            frontMatter = frontMatter.WithCustomFields(customFields);

        return frontMatter;
    }

    /// <summary>
    /// Provides equality comparison components for the value object
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Title;
        yield return Description;
        yield return Author;
        yield return Date;
        yield return FeaturedImage ?? string.Empty;
        yield return IsFeatured;
        yield return Published;
        yield return Layout ?? string.Empty;
        yield return Slug ?? string.Empty;

        // Include collections in equality check
        foreach (var category in Categories)
        {
            yield return category;
        }

        foreach (var tag in Tags)
        {
            yield return tag;
        }

        foreach (var field in CustomFields)
        {
            yield return $"{field.Key}:{field.Value}";
        }
    }

    /// <summary>
    /// Helper method to parse list values from string
    /// </summary>
    private static List<string> ParseListValue(string value)
    {
        var result = new List<string>();

        if (string.IsNullOrWhiteSpace(value))
            return result;

        // Handle YAML array format [item1, item2]
        if (value.StartsWith("[") && value.EndsWith("]"))
        {
            value = value.Substring(1, value.Length - 2);
        }

        // Split by comma or semicolon and process each item
        foreach (var item in value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmedItem = item.Trim();

            // Remove quotes if present
            if ((trimmedItem.StartsWith("\"") && trimmedItem.EndsWith("\"")) ||
                (trimmedItem.StartsWith("'") && trimmedItem.EndsWith("'")))
            {
                trimmedItem = trimmedItem.Substring(1, trimmedItem.Length - 2);
            }

            if (!string.IsNullOrWhiteSpace(trimmedItem))
            {
                result.Add(trimmedItem);
            }
        }

        return result;
    }

    /// <summary>
    /// Checks if a key is a standard frontmatter key
    /// </summary>
    private static bool IsStandardKey(string key)
    {
        var standardKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "title", "description", "author", "date", "featuredimage", "featured_image",
            "featured", "isFeatured", "published", "layout", "slug", "categories",
            "category", "tags", "tag"
        };

        return standardKeys.Contains(key);
    }

    /// <summary>
    /// Helper method to find Jekyll frontmatter boundaries.
    /// Frontmatter starts and ends with "---" on its own line.
    /// </summary>
    private static (int Start, int End, int Length)? FindFrontmatterBoundaries(string content)
    {
        const string delimiter = "---";

        // Must start with delimiter
        if (!content.StartsWith(delimiter))
            return null;

        // Find the ending delimiter (after the opening one)
        int firstDelimiterEnd = content.IndexOf('\n') + 1;
        int secondDelimiterPos = content.IndexOf($"\n{delimiter}\n", firstDelimiterEnd);

        if (secondDelimiterPos == -1)
            return null;

        int yamlStart = firstDelimiterEnd;
        int yamlEnd = secondDelimiterPos;
        int fullFrontmatterEnd = secondDelimiterPos + delimiter.Length + 2; // Include delimiter and newline

        return (yamlStart, fullFrontmatterEnd, yamlEnd - yamlStart);
    }



    public static FrontMatter FromYaml(string yaml)
    {

        var frontmatterBoundaries = FindFrontmatterBoundaries(yaml);

        // Extract the YAML frontmatter
        string yamlContent = yaml.Substring(
            frontmatterBoundaries.Value.Start,
            frontmatterBoundaries.Value.Length
        );

        // Extract the markdown content after frontmatter
        string markdownContent = yaml.Substring(frontmatterBoundaries.Value.End);

        // Configure deserializer with custom settings for flexibility
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties() // Important: Allows YAML to have extra fields
            .Build();

        // Deserialize frontmatter to our comprehensive class
        var frontmatter = deserializer.Deserialize<FrontMatter>(yamlContent);

        return frontmatter;

        //var frontMatter = new FrontMatter();

        //if (string.IsNullOrWhiteSpace(yaml))
        //    return frontMatter;

        //// Simple line-by-line parsing for common properties
        //var lines = yaml.Split('\n');

        //string? currentList = null;
        //List<string> currentItems = new();

        //foreach (var line in lines)
        //{
        //    var trimmedLine = line.Trim();

        //    // Skip the opening and closing --- lines
        //    if (trimmedLine == "---")
        //        continue;

        //    // Check if this is a new list
        //    if (trimmedLine.EndsWith(':'))
        //    {
        //        // Save any previous list
        //        if (currentList is not null && currentItems.Count > 0)
        //        {
        //            AssignList(frontMatter, currentList, currentItems);
        //        }

        //        // Start a new list
        //        currentList = trimmedLine.TrimEnd(':');
        //        currentItems = new List<string>();
        //        continue;
        //    }

        //    // Check if this is a list item
        //    if (trimmedLine.StartsWith("  - "))
        //    {
        //        var item = trimmedLine.Substring(4).Trim();
        //        // Remove quotes if present
        //        if (item.StartsWith("\"") && item.EndsWith("\""))
        //        {
        //            item = item.Substring(1, item.Length - 2);
        //        }

        //        currentItems.Add(item);
        //        continue;
        //    }

        //    // Process key-value pair
        //    var separatorIndex = trimmedLine.IndexOf(':');
        //    if (separatorIndex > 0)
        //    {
        //        // Save any previous list
        //        if (currentList is not null && currentItems.Count > 0)
        //        {
        //            AssignList(frontMatter, currentList, currentItems);
        //            currentList = null;
        //            currentItems.Clear();
        //        }

        //        var key = trimmedLine.Substring(0, separatorIndex).Trim();
        //        var value = trimmedLine.Substring(separatorIndex + 1).Trim();

        //        // Remove quotes if present
        //        if (value.StartsWith("\"") && value.EndsWith("\""))
        //        {
        //            value = value.Substring(1, value.Length - 2);
        //        }

        //        AssignProperty(frontMatter, key, value);
        //    }
        //}

        //// Save any final list
        //if (currentList is not null && currentItems.Count > 0)
        //{
        //    AssignList(frontMatter, currentList, currentItems);
        //}

        //return frontMatter;
    }

    private static void AssignList(FrontMatter frontMatter, string listName, List<string> items)
    {
        switch (listName.ToLowerInvariant())
        {
            case "categories":
                frontMatter = frontMatter.WithCategories(items);
                break;
            case "tags":
                frontMatter = frontMatter.WithTags(items);
                break;
            default:
                // For custom list properties, add as a custom field
                frontMatter = frontMatter.WithCustomField(listName, items);
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
                if (bool.TryParse(value, out var boolValue))
                {
                    frontMatter = frontMatter.WithCustomField(key, boolValue);
                }
                else if (int.TryParse(value, out var intValue))
                {
                    frontMatter = frontMatter.WithCustomField(key, intValue);
                }
                else if (double.TryParse(value, out var doubleValue))
                {
                    frontMatter = frontMatter.WithCustomField(key, doubleValue);
                }
                else
                {
                    frontMatter = frontMatter.WithCustomField(key, value);
                }
                break;
        }
    }
}