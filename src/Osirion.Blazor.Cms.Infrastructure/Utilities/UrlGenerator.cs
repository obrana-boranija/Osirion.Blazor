using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Infrastructure.Utilities;

/// <summary>
/// Utility class for generating URLs and slugs
/// </summary>
public static class UrlGenerator
{
    /// <summary>
    /// Generates a URL for content
    /// </summary>
    public static string GenerateUrl(string path, string slug, bool enableLocalization, List<string>? supportedLocales, string? skipSegment = null)
    {
        // Check if pathToTrim is null, whitespace, or just "/"
        bool skipPrefixRemoval = string.IsNullOrWhiteSpace(skipSegment) || skipSegment == "/";

        // Normalize path
        path = NormalizePath(path);

        // Step 1: Remove skipSegment from the beginning of the path if present
        if (!skipPrefixRemoval && path.StartsWith(skipSegment!))
        {
            // Only remove 'skipSegment' if it's followed by a slash or is the entire string
            if (path.Length == skipSegment.Length || path[skipSegment.Length] == '/')
            {
                // If skipSegment is followed by a slash, remove both skipSegment and the slash
                // If skipSegment is the entire string, this will result in an empty string
                path = path.Length > skipSegment.Length ? path.Substring(skipSegment.Length + 1) : "";
            }
        }

        // Step 2: Remove the filename from the path
        int lastSlashIndex = path.LastIndexOf('/');
        if (lastSlashIndex >= 0)
        {
            path = path.Substring(0, lastSlashIndex);
        }
        else
        {
            // If there's no slash, path is a single segment, so clear it
            path = "";
        }

        // If using localization, check if the first segment is a locale and remove it
        if (enableLocalization && path.Length > 0)
        {
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length > 0 && IsValidLocale(segments[0], supportedLocales))
            {
                // Remove the locale segment
                path = string.Join("/", segments.Skip(1));
            }
        }

        // Step 3: Append slug, with a slash if path is not empty
        if (!string.IsNullOrWhiteSpace(path))
        {
            return path + "/" + slug;
        }
        else
        {
            return slug;
        }
    }

    /// <summary>
    /// Generates a URL-friendly slug from text
    /// </summary>
    public static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "untitled";

        // Convert to lowercase
        var slug = text.ToLowerInvariant();

        // Remove special characters
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

        // Replace spaces with hyphens
        slug = Regex.Replace(slug, @"\s+", "-");

        // Remove consecutive hyphens
        slug = Regex.Replace(slug, @"-{2,}", "-");

        // Trim hyphens from ends
        slug = slug.Trim('-');

        return slug;
    }

    /// <summary>
    /// Gets the directory path from a file path
    /// </summary>
    public static string GetDirectoryPath(string filePath)
    {
        var lastSlashIndex = filePath.LastIndexOf('/');
        if (lastSlashIndex >= 0)
        {
            return filePath.Substring(0, lastSlashIndex);
        }
        return string.Empty;
    }

    /// <summary>
    /// Removes locale prefix from a path
    /// </summary>
    public static string RemoveLocaleFromPath(string path, string contentPath, bool enableLocalization, List<string>? supportedLocales)
    {
        if (!enableLocalization)
            return path;

        // Check if content path is set and remove it
        var _contentPath = NormalizePath(contentPath);
        if (!string.IsNullOrWhiteSpace(_contentPath) && path.StartsWith(_contentPath))
        {
            // Only remove if it's followed by a slash or is the entire path
            if (path.Length == _contentPath.Length || path[_contentPath.Length] == '/')
            {
                // Remove content path prefix
                path = path.Length > _contentPath.Length
                    ? path.Substring(_contentPath.Length + 1)
                    : "";
            }
        }

        // Check if first segment is a locale
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length > 0 && IsValidLocale(segments[0], supportedLocales))
        {
            // Remove locale segment
            return string.Join("/", segments.Skip(1));
        }

        return path;
    }

    /// <summary>
    /// Extracts locale from a path
    /// </summary>
    public static string ExtractLocaleFromPath(string path, string contentPath, bool enableLocalization, List<string>? supportedLocales, string defaultLocale)
    {
        // If localization is disabled, always return default locale
        if (!enableLocalization)
        {
            return defaultLocale;
        }

        // Check if content path is set and remove it from the beginning
        var _contentPath = UrlGenerator.NormalizePath(contentPath);
        if (!string.IsNullOrWhiteSpace(_contentPath) && path.StartsWith(_contentPath))
        {
            // Only remove if it's followed by a slash or is the entire path
            if (path.Length == _contentPath.Length || path[_contentPath.Length] == '/')
            {
                // Remove content path prefix
                path = path.Length > _contentPath.Length
                    ? path.Substring(_contentPath.Length + 1)
                    : "";
            }
        }

        // Try to extract locale from path format like "en/blog" or "es/articles"
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length > 0 && UrlGenerator.IsValidLocale(segments[0], supportedLocales))
        {
            return segments[0];
        }

        // No valid locale found, return default
        return defaultLocale;
    }

    /// <summary>
    /// Checks if a string is a valid locale
    /// </summary>
    public static bool IsValidLocale(string locale, List<string>? supportedLocales)
    {
        // Check against supported locales list if defined
        if (supportedLocales is not null && supportedLocales.Any())
        {
            return supportedLocales.Contains(locale, StringComparer.OrdinalIgnoreCase);
        }

        // Fallback to simple validation: 2-letter language code or language-region format
        return (locale.Length == 2 && locale.All(char.IsLetter)) ||
               (locale.Length == 5 && locale[2] == '-' &&
                locale.Substring(0, 2).All(char.IsLetter) &&
                locale.Substring(3, 2).All(char.IsLetter));
    }

    /// <summary>
    /// Normalizes a path for consistent comparison
    /// </summary>
    public static string NormalizePath(string path)
    {
        return path.Replace('\\', '/').Trim('/');
    }
}
