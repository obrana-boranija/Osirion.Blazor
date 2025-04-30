using Osirion.Blazor.Cms.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Infrastructure.Markdown;

/// <summary>
/// Default implementation of IHtmlToMarkdownConverter
/// </summary>
public class DefaultHtmlToMarkdownConverter : IHtmlToMarkdownConverter
{
    public Task<string> ConvertHtmlToMarkdownAsync(string html,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(html))
            return Task.FromResult(string.Empty);

        // Simple implementation that could be replaced with a more robust one
        var markdown = html;

        // Replace common HTML elements with Markdown
        markdown = Regex.Replace(markdown, @"<h1[^>]*>(.*?)</h1>", "# $1", RegexOptions.IgnoreCase);
        markdown = Regex.Replace(markdown, @"<h2[^>]*>(.*?)</h2>", "## $1", RegexOptions.IgnoreCase);
        markdown = Regex.Replace(markdown, @"<h3[^>]*>(.*?)</h3>", "### $1", RegexOptions.IgnoreCase);
        // Additional replacements would be implemented here

        // Remove remaining HTML tags
        markdown = Regex.Replace(markdown, @"<[^>]+>", string.Empty);

        return Task.FromResult(markdown);
    }
}