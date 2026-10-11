using Markdig;
using Osirion.Blazor.Cms.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Infrastructure.Markdown;

/// <summary>
/// Default implementation of IMarkdownRenderer
/// </summary>
public class DefaultMarkdownRenderer : IMarkdownRendererService
{
    private readonly MarkdownPipeline _defaultPipeline;

    /// <summary>Performs the DefaultMarkdownRenderer operation.</summary>
    public DefaultMarkdownRenderer()
    {
        _defaultPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseYamlFrontMatter()
            .Build();
    }

    /// <inheritdoc/>
    public string RenderToHtml(string markdown, Action<MarkdownPipelineBuilder>? configureOptions = null)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        var pipeline = _defaultPipeline;
        if (configureOptions is not null)
        {
            var pipelineBuilder = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseYamlFrontMatter();

            configureOptions(pipelineBuilder);
            pipeline = pipelineBuilder.Build();
        }

        return Markdig.Markdown.ToHtml(markdown, pipeline);
    }

    /// <inheritdoc/>
    public Task<string> RenderToHtmlAsync(string markdown, Action<MarkdownPipelineBuilder>? configureOptions = null,
        CancellationToken cancellationToken = default)
    {
        // Markdig is synchronous, so we wrap the method in a Task
        return Task.FromResult(RenderToHtml(markdown, configureOptions));
    }

    /// <inheritdoc/>
    public string SanitizeMarkdown(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        // Remove potentially dangerous scripts and HTML
        markdown = ReplaceWithinTimeout(
            markdown,
            @"<\s*script.*?>.*?<\s*/\s*script\s*>",
            "",
            RegexOptions.IgnoreCase | RegexOptions.Singleline
        );

        // Remove inline event handlers
        markdown = ReplaceWithinTimeout(
            markdown,
            @"on\w+\s*=\s*""[^""]*""",
            "",
            RegexOptions.IgnoreCase
        );

        // Remove iframe tags
        markdown = ReplaceWithinTimeout(
            markdown,
            @"<\s*iframe.*?>.*?<\s*/\s*iframe\s*>",
            "",
            RegexOptions.IgnoreCase | RegexOptions.Singleline
        );

        // Remove object and embed tags
        markdown = ReplaceWithinTimeout(
            markdown,
            @"<\s*(object|embed).*?>.*?<\s*/\s*(object|embed)\s*>",
            "",
            RegexOptions.IgnoreCase | RegexOptions.Singleline
        );

        // Sanitize links: remove the javascript: protocol. This is a best-effort pre-pass; the HTML sanitizer that runs
        // after rendering is the real defence. The balancing group consumes nested parentheses so the whole destination
        // (and any title) is replaced instead of leaving a stray ")".
        markdown = ReplaceWithinTimeout(
            markdown,
            @"\[([^\]]*)\]\(\s*javascript:(?>[^()]+|\((?<depth>)|\)(?<-depth>))*(?(depth)(?!))\)",
            "[$1](#)",
            RegexOptions.IgnoreCase
        );

        // Reference-style link definitions ([id]: javascript:...).
        markdown = ReplaceWithinTimeout(
            markdown,
            @"^( {0,3}\[[^\]]+\]:)[ \t]*javascript:[^\r\n]*",
            "$1 #",
            RegexOptions.IgnoreCase | RegexOptions.Multiline
        );

        return markdown;
    }

    // Each pattern runs with a match timeout so a pathological document cannot hold a request. On timeout the step
    // is skipped: this pre-pass is best effort, and the HTML sanitizer that runs after rendering still applies.
    private static string ReplaceWithinTimeout(string input, string pattern, string replacement, RegexOptions options)
    {
        try
        {
            return Regex.Replace(input, pattern, replacement, options, SanitizeMatchTimeout);
        }
        catch (RegexMatchTimeoutException)
        {
            return input;
        }
    }

    private static readonly TimeSpan SanitizeMatchTimeout = TimeSpan.FromMilliseconds(250);
}
