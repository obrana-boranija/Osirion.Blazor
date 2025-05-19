using Osirion.Blazor.Cms.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Infrastructure.Markdown;

/// <summary>
/// Basic implementation of IHtmlToMarkdownConverter
/// For a more robust implementation, consider using a dedicated library
/// </summary>
public class HtmlToMarkdownConverter : IHtmlToMarkdownConverter
{
    /// <inheritdoc/>
    public Task<string> ConvertHtmlToMarkdownAsync(string? html, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(html))
            return Task.FromResult(string.Empty);

        // This is a simple implementation
        // In a production environment, consider using a dedicated library
        var markdown = html;

        // Replace common HTML elements with Markdown
        markdown = Regex.Replace(markdown, @"<h1[^>]*>(.*?)</h1>", "# $1", RegexOptions.IgnoreCase);
        markdown = Regex.Replace(markdown, @"<h2[^>]*>(.*?)</h2>", "## $1", RegexOptions.IgnoreCase);
        markdown = Regex.Replace(markdown, @"<h3[^>]*>(.*?)</h3>", "### $1", RegexOptions.IgnoreCase);
        markdown = Regex.Replace(markdown, @"<h4[^>]*>(.*?)</h4>", "#### $1", RegexOptions.IgnoreCase);
        markdown = Regex.Replace(markdown, @"<h5[^>]*>(.*?)</h5>", "##### $1", RegexOptions.IgnoreCase);
        markdown = Regex.Replace(markdown, @"<h6[^>]*>(.*?)</h6>", "###### $1", RegexOptions.IgnoreCase);

        markdown = Regex.Replace(markdown, @"<strong[^>]*>(.*?)</strong>", "**$1**", RegexOptions.IgnoreCase);
        markdown = Regex.Replace(markdown, @"<b[^>]*>(.*?)</b>", "**$1**", RegexOptions.IgnoreCase);
        markdown = Regex.Replace(markdown, @"<em[^>]*>(.*?)</em>", "*$1*", RegexOptions.IgnoreCase);
        markdown = Regex.Replace(markdown, @"<i[^>]*>(.*?)</i>", "*$1*", RegexOptions.IgnoreCase);

        markdown = Regex.Replace(markdown, @"<a href=""(.*?)""[^>]*>(.*?)</a>", "[$2]($1)", RegexOptions.IgnoreCase);
        markdown = Regex.Replace(markdown, @"<img src=""(.*?)""[^>]*>", "![]($1)", RegexOptions.IgnoreCase);

        // Remove remaining HTML tags
        markdown = Regex.Replace(markdown, @"<[^>]+>", string.Empty);

        return Task.FromResult(markdown);
    }
}