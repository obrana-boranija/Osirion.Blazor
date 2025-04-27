using Markdig;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Infrastructure.Markdown;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Core.Services;

/// <summary>
/// Implementation of IMarkdownProcessor that provides markdown processing capabilities
/// </summary>
public class MarkdownProcessor : IMarkdownProcessor
{
    private readonly ILogger<MarkdownProcessor> _logger;
    private readonly MarkdownPipeline _defaultPipeline;
    private readonly static Regex _frontMatterRegex = new(@"^\s*---\s*\n(.*?)\n\s*---\s*\n", RegexOptions.Singleline);

    /// <summary>
    /// Initializes a new instance of the MarkdownProcessor class
    /// </summary>
    public MarkdownProcessor(ILogger<MarkdownProcessor> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Create secure default pipeline
        _defaultPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseYamlFrontMatter()
            .DisableHtml() // Prevent raw HTML injection
            .UseAutoLinks()
            .UseTaskLists()
            .UsePipeTables()
            .UseEmojiAndSmiley()
            .Build();
    }

    /// <inheritdoc/>
    public string RenderToHtml(string markdown, bool sanitizeHtml = true)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        try
        {
            if (sanitizeHtml)
            {
                markdown = SanitizeMarkdown(markdown);
            }

            var match = _frontMatterRegex.Match(markdown);
            if (match.Success)
            {
                var contentStartIndex = match.Index + match.Length;
                markdown = markdown.Substring(contentStartIndex).Trim();
            }

            return Markdig.Markdown.ToHtml(markdown, _defaultPipeline);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rendering markdown: {Message}", ex.Message);
            return FormatErrorMessage(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<string> RenderToHtmlAsync(string markdown, bool sanitizeHtml = true)
    {
        // Use Task.Run to offload CPU-intensive rendering to a background thread
        return await Task.Run(() => RenderToHtml(markdown, sanitizeHtml));
    }

    /// <inheritdoc/>
    public Dictionary<string, string> ExtractFrontMatter(string content)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var match = _frontMatterRegex.Match(content);
        if (match.Success && match.Groups.Count > 1)
        {
            var frontMatterContent = match.Groups[1].Value;
            var lines = frontMatterContent.Split('\n');

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine))
                    continue;

                var separatorIndex = trimmedLine.IndexOf(':');
                if (separatorIndex > 0)
                {
                    var key = trimmedLine.Substring(0, separatorIndex).Trim();
                    var value = trimmedLine.Substring(separatorIndex + 1).Trim();

                    if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                        (value.StartsWith("'") && value.EndsWith("'")))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    result[key] = value;
                }
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<string> ConvertHtmlToMarkdownAsync(string html)
    {
        return await Task.Run(() => {
            // Simple regex-based HTML tag stripper
            // Note: In future, use a proper HTML-to-Markdown converter
            var text = Regex.Replace(html, "<[^>]*>", string.Empty);

            // Replace common HTML entities
            text = text.Replace("&nbsp;", " ")
                       .Replace("&lt;", "<")
                       .Replace("&gt;", ">")
                       .Replace("&amp;", "&")
                       .Replace("&quot;", "\"")
                       .Replace("&#39;", "'");

            text = Regex.Replace(text, @"\n\s*\n", "\n\n");

            return text.Trim();
        });
    }

    /// <inheritdoc/>
    public string SanitizeMarkdown(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        // Remove potentially dangerous scripts and HTML
        markdown = Regex.Replace(
            markdown,
            @"<\s*script.*?>.*?<\s*/\s*script\s*>",
            "",
            RegexOptions.IgnoreCase | RegexOptions.Singleline
        );

        // Remove inline event handlers
        markdown = Regex.Replace(
            markdown,
            @"on\w+\s*=\s*""[^""]*""",
            "",
            RegexOptions.IgnoreCase
        );

        // Remove iframe tags
        markdown = Regex.Replace(
            markdown,
            @"<\s*iframe.*?>.*?<\s*/\s*iframe\s*>",
            "",
            RegexOptions.IgnoreCase | RegexOptions.Singleline
        );

        // Remove object and embed tags
        markdown = Regex.Replace(
            markdown,
            @"<\s*(object|embed).*?>.*?<\s*/\s*(object|embed)\s*>",
            "",
            RegexOptions.IgnoreCase | RegexOptions.Singleline
        );

        // Sanitize links - remove javascript: protocol
        markdown = Regex.Replace(
            markdown,
            @"\[([^\]]*)\]\(javascript:[^\)]*\)",
            "[$1](#)",
            RegexOptions.IgnoreCase
        );

        return markdown;
    }

    /// <summary>
    /// Formats an error message to be displayed safely in HTML
    /// </summary>
    private string FormatErrorMessage(string message)
    {
        // Sanitize the error message to prevent XSS
        var sanitizedMessage = System.Web.HttpUtility.HtmlEncode(message);

        return $@"
                <div class=""markdown-error"">
                    <p>There was an error rendering this markdown content.</p>
                    <pre>{sanitizedMessage}</pre>
                </div>";
    }

    public Task<(Dictionary<string, string> FrontMatter, string Content)> ExtractFrontMatterAsync(string markdown, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<string> ConvertHtmlToMarkdownAsync(string html, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}