using Osirion.Blazor.Cms.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Infrastructure.Markdown;

/// <summary>
/// Implementation of IFrontMatterExtractor to extract YAML front matter from Markdown
/// </summary>
public class FrontMatterExtractor : IFrontMatterExtractor
{
    private static readonly Regex FrontMatterRegex = new(@"^---\s*\n(.*?)\n---\s*\n", RegexOptions.Singleline | RegexOptions.Compiled);

    /// <inheritdoc/>
    public Dictionary<string, string> ExtractFrontMatter(string content)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(content))
            return result;

        var match = FrontMatterRegex.Match(content);
        if (match.Success && match.Groups.Count > 1)
        {
            var frontMatterContent = match.Groups[1].Value;
            ParseFrontMatterContent(frontMatterContent, result);
        }

        return result;
    }

    /// <inheritdoc/>
    public Task<(Dictionary<string, string> FrontMatter, string Content)> ExtractFrontMatterAsync(
        string markdown, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return Task.FromResult((new Dictionary<string, string>(), string.Empty));

        var match = FrontMatterRegex.Match(markdown);
        if (match.Success)
        {
            var frontMatter = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var frontMatterContent = match.Groups[1].Value;
            ParseFrontMatterContent(frontMatterContent, frontMatter);

            var contentStart = match.Index + match.Length;
            var content = markdown.Substring(contentStart);

            return Task.FromResult((frontMatter, content));
        }

        // No front matter found
        return Task.FromResult((new Dictionary<string, string>(), markdown));
    }

    private void ParseFrontMatterContent(string frontMatterContent, Dictionary<string, string> result)
    {
        var lines = frontMatterContent.Split('\n');
        foreach (var line in lines)
        {
            var parts = line.Split(new[] { ':' }, 2);
            if (parts.Length == 2)
            {
                var key = parts[0].Trim();
                var value = parts[1].Trim();

                // Remove quotes if present
                if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                    (value.StartsWith("'") && value.EndsWith("'")))
                {
                    value = value.Substring(1, value.Length - 2);
                }

                result[key] = value;
            }
        }
    }
}