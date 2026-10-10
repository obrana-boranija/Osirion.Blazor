namespace Osirion.Blazor.Cms.Application.Validation.Content;

/// <summary>
/// Path rules shared by the content command validators.
/// </summary>
internal static class ContentPathRules
{
    /// <summary>
    /// The validation message for a path that fails <see cref="IsValidRelativePath"/>.
    /// </summary>
    internal const string InvalidPathMessage =
        "Path must be relative and must not contain '.', '..' or empty segments, control characters, or any of \\ ? * : < >";

    private static readonly char[] ForbiddenCharacters = ['\\', '?', '*', ':', '<', '>'];

    /// <summary>
    /// Determines whether <paramref name="path"/> is a relative content path.
    /// </summary>
    /// <remarks>
    /// Segments are separated by <c>/</c>. A name that merely contains dots, such as <c>v1..2.md</c>,
    /// is accepted; a <c>.</c> or <c>..</c> segment is not. Backslashes and colons are rejected, which
    /// also rules out Windows rooted forms such as <c>\\server\share</c> and <c>C:x</c>.
    /// </remarks>
    /// <param name="path">The path to check.</param>
    /// <returns><see langword="true"/> when the path is safe to combine with a content root.</returns>
    internal static bool IsValidRelativePath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) ||
            path.StartsWith('/') ||
            path.IndexOfAny(ForbiddenCharacters) >= 0 ||
            path.Any(char.IsControl))
        {
            return false;
        }

        return path.Split('/').All(segment => segment is not ("" or "." or ".."));
    }
}
