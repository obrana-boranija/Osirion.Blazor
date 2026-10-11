using System.Text.RegularExpressions;

namespace Osirion.Blazor.ApiDocs;

/// <summary>
/// Replaces the generated sections in Markdown files. A section starts with
/// <c>&lt;!-- osirion:parameters ComponentName --&gt;</c> and ends with <c>&lt;!-- /osirion:parameters --&gt;</c>;
/// everything between the two is rewritten. The comments are removed when the site renders the page.
/// </summary>
internal static partial class ContentUpdater
{
    /// <summary>Returns <paramref name="markdown"/> with every section regenerated, and the components it names.</summary>
    /// <param name="markdown">The file's text.</param>
    /// <param name="table">Builds the table for a component name, or returns <see langword="null"/> when the name is
    /// unknown.</param>
    /// <param name="unknown">Component names no table could be built for.</param>
    public static string Update(string markdown, Func<string, string?> table, out IReadOnlyList<string> unknown)
    {
        var missing = new List<string>();
        var newline = markdown.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
        var updated = Section().Replace(markdown, match =>
        {
            var name = match.Groups["name"].Value;
            var generated = table(name);
            if (generated is null)
            {
                missing.Add(name);
                return match.Value;
            }

            return match.Groups["open"].Value + newline + newline
                + generated.ReplaceLineEndings(newline).TrimEnd() + newline + newline
                + match.Groups["close"].Value;
        });

        unknown = missing;
        return updated;
    }

    [GeneratedRegex(@"(?<open><!--\s*osirion:parameters\s+(?<name>[A-Za-z0-9_.]+)\s*-->)[\s\S]*?(?<close><!--\s*/osirion:parameters\s*-->)")]
    private static partial Regex Section();
}
