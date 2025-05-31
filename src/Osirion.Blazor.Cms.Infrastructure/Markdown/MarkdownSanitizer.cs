using Osirion.Blazor.Cms.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Infrastructure.Markdown;

/// <summary>
/// Basic implementation of IMarkdownSanitizer to remove potentially harmful content
/// </summary>
public class MarkdownSanitizer : IMarkdownSanitizer
{
    // Patterns for potentially harmful content
    private static readonly Regex ScriptBlocksRegex = new(@"```(?:js|javascript).*?```", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex InlineJsRegex = new(@"`javascript:.*?`", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <inheritdoc/>
    public string SanitizeMarkdown(string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        // Remove potential script blocks
        markdown = ScriptBlocksRegex.Replace(markdown, "```\nCode block removed for security reasons\n```");

        // Remove potential inline JavaScript
        markdown = InlineJsRegex.Replace(markdown, "`[code removed]`");

        // Additional sanitization could be implemented here
        return markdown;
    }
}