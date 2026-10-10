using Osirion.Blazor.Cms.Domain.Exceptions;

namespace Osirion.Blazor.Cms.Infrastructure.Utilities;

/// <summary>
/// Resolves caller-supplied content paths against a file system root and refuses any path that
/// would reach outside it.
/// </summary>
/// <remarks>
/// A valid target lies strictly under the root; the root itself is never a valid target. The check
/// is lexical: it normalizes the path but does not follow symbolic links or junctions that already
/// exist inside the root.
/// </remarks>
internal static class ContentPathGuard
{
    private const string PathPropertyName = "Path";
    private static readonly char[] SegmentSeparators = ['/', '\\'];

    /// <summary>
    /// Combines <paramref name="rootPath"/> and <paramref name="relativePath"/> and returns the
    /// normalized full path.
    /// </summary>
    /// <param name="rootPath">The configured content root, absolute or relative to the current directory.</param>
    /// <param name="relativePath">The caller-supplied path, relative to the root.</param>
    /// <returns>The full path, which is guaranteed to lie under the root.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="rootPath"/> is empty.</exception>
    /// <exception cref="ContentValidationException">
    /// Thrown when <paramref name="relativePath"/> is empty, rooted, contains a null character or a
    /// <c>.</c> or <c>..</c> segment, or resolves outside the root.
    /// </exception>
    internal static string ResolveUnderRoot(string rootPath, string relativePath)
    {
        var root = GetRootWithSeparator(rootPath);

        if (string.IsNullOrWhiteSpace(relativePath))
        {
            throw Violation("Path cannot be empty");
        }

        if (relativePath.Contains('\0'))
        {
            throw Violation("Path contains a null character");
        }

        if (Path.IsPathRooted(relativePath))
        {
            throw Violation("Path must be relative to the content root");
        }

        if (relativePath.Split(SegmentSeparators).Any(segment => segment is "." or ".."))
        {
            throw Violation("Path must not contain '.' or '..' segments");
        }

        var fullPath = Path.GetFullPath(Path.Combine(root, relativePath));
        if (!IsStrictlyUnder(root, fullPath))
        {
            throw Violation("Path resolves outside the content root");
        }

        return fullPath;
    }

    /// <summary>
    /// Determines whether an absolute or root-relative <paramref name="fullPath"/> lies strictly
    /// under <paramref name="rootPath"/> once both are normalized.
    /// </summary>
    /// <param name="rootPath">The configured content root.</param>
    /// <param name="fullPath">The path to check, for example a stored provider-specific id.</param>
    /// <returns><see langword="true"/> when the path is inside the root.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="rootPath"/> is empty.</exception>
    internal static bool IsUnderRoot(string rootPath, string fullPath)
    {
        var root = GetRootWithSeparator(rootPath);

        if (string.IsNullOrWhiteSpace(fullPath) || fullPath.Contains('\0'))
        {
            return false;
        }

        return IsStrictlyUnder(root, Path.GetFullPath(fullPath));
    }

    /// <summary>
    /// Throws when <paramref name="fullPath"/> does not lie strictly under <paramref name="rootPath"/>.
    /// </summary>
    /// <param name="rootPath">The configured content root.</param>
    /// <param name="fullPath">The path to check.</param>
    /// <exception cref="ContentValidationException">Thrown when the path is outside the root.</exception>
    internal static void EnsureUnderRoot(string rootPath, string fullPath)
    {
        if (!IsUnderRoot(rootPath, fullPath))
        {
            throw Violation("Path resolves outside the content root");
        }
    }

    private static string GetRootWithSeparator(string rootPath)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            throw new InvalidOperationException("The file system content root (BasePath) is not configured.");
        }

        var root = Path.GetFullPath(rootPath);
        return Path.EndsInDirectorySeparator(root) ? root : root + Path.DirectorySeparatorChar;
    }

    private static bool IsStrictlyUnder(string rootWithSeparator, string fullPath)
    {
        // Windows file systems are case-insensitive; elsewhere a case-insensitive match could let
        // "/Content/x" pass for the root "/content/".
        var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        return fullPath.Length > rootWithSeparator.Length &&
               fullPath.StartsWith(rootWithSeparator, comparison);
    }

    private static ContentValidationException Violation(string message)
    {
        return new ContentValidationException(PathPropertyName, message);
    }
}
