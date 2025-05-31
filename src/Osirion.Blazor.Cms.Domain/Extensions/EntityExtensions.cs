namespace Osirion.Blazor.Cms.Domain.Extensions;

/// <summary>
/// Extension methods for domain entities
/// </summary>
public static class EntityExtensions
{
    /// <summary>
    /// Generates a URL-friendly slug from text
    /// </summary>
    public static string GenerateSlug(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "untitled";

        // Convert to lowercase
        var slug = text.ToLowerInvariant();

        // Remove special characters
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");

        // Replace spaces with hyphens
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");

        // Remove consecutive hyphens
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-{2,}", "-");

        // Trim hyphens from ends
        slug = slug.Trim('-');

        return string.IsNullOrWhiteSpace(slug) ? "untitled" : slug;
    }

    /// <summary>
    /// Validates if a string is a valid slug
    /// </summary>
    public static bool IsValidSlug(this string slug)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(slug, "^[a-z0-9-]+$");
    }

    /// <summary>
    /// Escapes a string for YAML
    /// </summary>
    public static string EscapeYamlString(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }
}