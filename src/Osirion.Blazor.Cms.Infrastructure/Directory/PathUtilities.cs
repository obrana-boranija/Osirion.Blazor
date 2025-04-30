using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Options;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Infrastructure.Directory;

/// <summary>
/// Implementation of IPathUtilities for file system paths
/// </summary>
public class PathUtilities : IPathUtilities
{
    private readonly string _contentPath;
    private readonly bool _enableLocalization;
    private readonly string _defaultLocale;
    private readonly IReadOnlyList<string> _supportedLocales;

    public PathUtilities(IOptions<FileSystemOptions> options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        _contentPath = options.Value.ContentRoot ?? string.Empty;
        _enableLocalization = options.Value.EnableLocalization;
        _defaultLocale = options.Value.DefaultLocale;
        _supportedLocales = options.Value.SupportedLocales;
    }

    /// <inheritdoc/>
    public string NormalizePath(string path)
    {
        return path.Replace('\\', '/').Trim('/');
    }

    /// <inheritdoc/>
    public string ExtractLocaleFromPath(string path)
    {
        // If localization is disabled, always return default locale
        if (!_enableLocalization)
        {
            return _defaultLocale;
        }

        // Check if content path is set and remove it from the beginning
        var contentPath = NormalizePath(_contentPath);
        if (!string.IsNullOrEmpty(contentPath) && path.StartsWith(contentPath))
        {
            // Only remove if it's followed by a slash or is the entire path
            if (path.Length == contentPath.Length || path[contentPath.Length] == '/')
            {
                // Remove content path prefix
                path = path.Length > contentPath.Length
                    ? path.Substring(contentPath.Length + 1)
                    : "";
            }
        }

        // Try to extract locale from path format like "en/blog" or "es/articles"
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length > 0 && IsValidLocale(segments[0]))
        {
            return segments[0];
        }

        // No valid locale found, return default
        return _defaultLocale;
    }

    /// <inheritdoc/>
    public bool MatchesGlobPattern(string path, string pattern)
    {
        // Convert glob pattern to regex
        var regexPattern = pattern
            .Replace(".", "\\.")
            .Replace("*", ".*")
            .Replace("?", ".")
            .Replace("\\*\\*", ".*");

        return Regex.IsMatch(path, $"^{regexPattern}$", RegexOptions.IgnoreCase);
    }

    /// <inheritdoc/>
    public string RemoveLocaleFromPath(string path)
    {
        if (!_enableLocalization)
            return path;

        // Check if content path is set and remove it
        var contentPath = NormalizePath(_contentPath);
        if (!string.IsNullOrEmpty(contentPath) && path.StartsWith(contentPath))
        {
            // Only remove if it's followed by a slash or is the entire path
            if (path.Length == contentPath.Length || path[contentPath.Length] == '/')
            {
                // Remove content path prefix
                path = path.Length > contentPath.Length
                    ? path.Substring(contentPath.Length + 1)
                    : "";
            }
        }

        // Check if first segment is a locale
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length > 0 && IsValidLocale(segments[0]))
        {
            // Remove locale segment
            return string.Join("/", segments.Skip(1));
        }

        return path;
    }

    /// <inheritdoc/>
    public string GenerateDirectoryUrl(string path)
    {
        // Normalize path
        path = NormalizePath(path);

        // Remove content path prefix if present
        var contentPath = NormalizePath(_contentPath);
        if (!string.IsNullOrEmpty(contentPath) && path.StartsWith(contentPath))
        {
            if (path.Length == contentPath.Length || path[contentPath.Length] == '/')
            {
                path = path.Length > contentPath.Length
                    ? path.Substring(contentPath.Length + 1)
                    : "";
            }
        }

        // If using localization, check if the first segment is a locale and remove it
        if (_enableLocalization && !string.IsNullOrEmpty(path))
        {
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length > 0 && IsValidLocale(segments[0]))
            {
                path = string.Join("/", segments.Skip(1));
            }
        }

        return path;
    }

    /// <summary>
    /// Checks if a string is a valid locale
    /// </summary>
    private bool IsValidLocale(string locale)
    {
        // Check against supported locales list if defined
        if (_supportedLocales.Count > 0)
        {
            return _supportedLocales.Contains(locale, StringComparer.OrdinalIgnoreCase);
        }

        // Fallback to simple validation: 2-letter language code or language-region format
        return (locale.Length == 2 && locale.All(char.IsLetter)) ||
               (locale.Length == 5 && locale[2] == '-' &&
                locale.Substring(0, 2).All(char.IsLetter) &&
                locale.Substring(3, 2).All(char.IsLetter));
    }
}