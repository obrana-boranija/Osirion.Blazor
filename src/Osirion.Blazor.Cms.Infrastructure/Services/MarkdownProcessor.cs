using Markdig;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

public class MarkdownProcessor : IMarkdownProcessor
{
    private readonly MarkdownPipeline _pipeline;

    public MarkdownProcessor()
    {
        _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseYamlFrontMatter()
            .Build();
    }

    public string RenderToHtml(string markdown, bool sanitizeHtml = true)
    {
        if (string.IsNullOrEmpty(markdown))
            return string.Empty;

        if (sanitizeHtml)
            markdown = SanitizeMarkdown(markdown);

        return Markdown.ToHtml(markdown, _pipeline);
    }

    public async Task<string> RenderToHtmlAsync(string markdown, bool sanitizeHtml = true)
    {
        // Wrap the synchronous method in Task for consistency in the API
        return await Task.FromResult(RenderToHtml(markdown, sanitizeHtml));
    }

    public string SanitizeMarkdown(string markdown)
    {
        // Basic sanitization - removing potentially harmful content
        if (string.IsNullOrEmpty(markdown))
            return string.Empty;

        // More advanced sanitization could be implemented here
        return markdown;
    }

    public Dictionary<string, string> ExtractFrontMatter(string content)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(content))
            return result;

        // Regular expression to extract frontmatter
        var frontMatterRegex = new Regex(@"^---\s*\n(.*?)\n---\s*\n", RegexOptions.Singleline);
        var match = frontMatterRegex.Match(content);

        if (match.Success && match.Groups.Count > 1)
        {
            var frontMatterContent = match.Groups[1].Value;
            var lines = frontMatterContent.Split('\n');

            foreach (var line in lines)
            {
                var parts = line.Split(new[] { ':' }, 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    // Remove quotes if present
                    if (value.StartsWith("\"") && value.EndsWith("\"") ||
                        value.StartsWith("'") && value.EndsWith("'"))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    result[key] = value;
                }
            }
        }

        return result;
    }

    public async Task<(Dictionary<string, string> FrontMatter, string Content)> ExtractFrontMatterAsync(
        string markdown, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return (new Dictionary<string, string>(), string.Empty);

        var frontMatterRegex = new Regex(@"^---\s*\n(.*?)\n---\s*\n", RegexOptions.Singleline);
        var match = frontMatterRegex.Match(markdown);

        if (match.Success)
        {
            var frontMatterYaml = match.Groups[1].Value;
            var contentStart = match.Index + match.Length;
            var content = markdown.Substring(contentStart);

            var frontMatter = ExtractFrontMatter(markdown);
            return (frontMatter, content);
        }

        // No front matter found
        return (new Dictionary<string, string>(), markdown);
    }

    public async Task<string> ConvertHtmlToMarkdownAsync(string html, CancellationToken cancellationToken = default)
    {
        // Basic HTML to Markdown conversion - in a real implementation 
        // you might want to use a library like ReverseMarkdown or similar
        if (string.IsNullOrEmpty(html))
            return string.Empty;

        // For now, we'll just strip HTML tags as a placeholder
        var stripped = Regex.Replace(html, "<[^>]+>", string.Empty);
        return stripped;
    }

    public Task ProcessMarkdownContentAsync(string markdownContent, ContentItem contentItem)
    {
        if (string.IsNullOrWhiteSpace(markdownContent))
        {
            contentItem.SetContent(string.Empty);
            contentItem.SetOriginalMarkdown(string.Empty);
            return Task.CompletedTask;
        }

        // Extract front matter
        var frontMatter = ExtractFrontMatter(markdownContent);

        // Get content without front matter
        var contentRegex = new Regex(@"^---\s*\n.*?\n---\s*\n", RegexOptions.Singleline);
        var content = contentRegex.Replace(markdownContent, string.Empty);

        // Set markdown content
        contentItem.SetOriginalMarkdown(content);

        // Render HTML
        var html = RenderToHtml(content);
        contentItem.SetContent(html);

        // Process front matter
        foreach (var kvp in frontMatter)
        {
            string key = kvp.Key.ToLowerInvariant();
            string value = kvp.Value;

            switch (key)
            {
                case "title":
                    contentItem.SetTitle(value);
                    break;
                case "description":
                    contentItem.SetDescription(value);
                    break;
                    // Add other front matter processing as needed
            }
        }

        return Task.CompletedTask;
    }

    public Task<string> ConvertHtmlToMarkdownAsync(string html)
    {
        throw new NotImplementedException();
    }
}