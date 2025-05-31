using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Options;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Infrastructure.Directory
{
    /// <summary>
    /// Implementation of IPathUtilities for path handling
    /// </summary>
    public class PathUtilities : IPathUtilities
    {
        private readonly string _contentPath;
        private readonly bool _enableLocalization;
        private readonly string _defaultLocale;
        private readonly IReadOnlyList<string> _supportedLocales;

        public PathUtilities(IOptions<FileSystemOptions> options)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            _contentPath = options.Value.ContentRoot ?? string.Empty;
            _enableLocalization = options.Value.EnableLocalization;
            _defaultLocale = options.Value.DefaultLocale;
            _supportedLocales = options.Value.SupportedLocales;
        }

        public string NormalizePath(string path)
        {
            return path.Replace('\\', '/').Trim('/');
        }

        public string ExtractLocaleFromPath(string path)
        {
            // If localization is disabled, always return default locale
            if (!_enableLocalization)
            {
                return _defaultLocale;
            }

            // Remove content path prefix if present
            path = RemoveContentPathPrefix(path);

            // Try to extract locale from path format like "en/blog" or "es/articles"
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length > 0 && IsValidLocale(segments[0]))
            {
                return segments[0];
            }

            // No valid locale found, return default
            return _defaultLocale;
        }

        public bool MatchesGlobPattern(string path, string pattern)
        {
            // Convert glob pattern to regex
            var regexPattern = "^" + Regex.Escape(pattern)
                .Replace("\\*\\*", ".*")
                .Replace("\\*", "[^/]*")
                .Replace("\\?", ".") + "$";

            return Regex.IsMatch(path, regexPattern, RegexOptions.IgnoreCase);
        }

        public string RemoveLocaleFromPath(string path)
        {
            if (!_enableLocalization)
                return path;

            // Remove content path prefix if present
            path = RemoveContentPathPrefix(path);

            // Check if first segment is a locale
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length > 0 && IsValidLocale(segments[0]))
            {
                // Remove locale segment
                return string.Join("/", segments.Skip(1));
            }

            return path;
        }

        public string GenerateDirectoryUrl(string path)
        {
            // Normalize path
            path = NormalizePath(path);

            // Remove content path prefix if present
            path = RemoveContentPathPrefix(path);

            // If using localization, check if the first segment is a locale and remove it
            if (_enableLocalization && !string.IsNullOrWhiteSpace(path))
            {
                path = RemoveLocaleFromPath(path);
            }

            return path;
        }

        public string GenerateContentUrl(string path, string slug)
        {
            // Normalize path
            path = NormalizePath(path);

            // Remove content path prefix
            path = RemoveContentPathPrefix(path);

            // Remove locale if present
            if (_enableLocalization)
            {
                path = RemoveLocaleFromPath(path);
            }

            // Remove filename from path
            int lastSlashIndex = path.LastIndexOf('/');
            if (lastSlashIndex >= 0)
            {
                path = path.Substring(0, lastSlashIndex);
            }
            else
            {
                path = string.Empty;
            }

            // Append slug
            if (!string.IsNullOrWhiteSpace(path))
            {
                return path + "/" + slug;
            }
            else
            {
                return slug;
            }
        }

        private string RemoveContentPathPrefix(string path)
        {
            var contentPath = NormalizePath(_contentPath);
            if (!string.IsNullOrWhiteSpace(contentPath) && path.StartsWith(contentPath))
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
            return path;
        }

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
}