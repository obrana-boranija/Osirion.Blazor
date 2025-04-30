using Osirion.Blazor.Cms.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Infrastructure.Markdown;

/// <summary>
/// Default implementation of IMarkdownSanitizer
/// </summary>
public class DefaultMarkdownSanitizer : IMarkdownSanitizer
{
    // Patterns for potentially harmful content
    private static readonly Regex ScriptBlocksRegex = new(@"```(?:js|javascript).*?```",
        RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex InlineJsRegex = new(@"`javascript:.*?`",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public string SanitizeMarkdown(string markdown)
    {
        if (string.IsNullOrEmpty(markdown))
            return string.Empty;

        // Remove potential script blocks
        markdown = ScriptBlocksRegex.Replace(markdown,
            "```\nCode block removed for security reasons\n```");

        // Remove potential inline JavaScript
        markdown = InlineJsRegex.Replace(markdown, "`[code removed]`");

        return markdown;
    }
}