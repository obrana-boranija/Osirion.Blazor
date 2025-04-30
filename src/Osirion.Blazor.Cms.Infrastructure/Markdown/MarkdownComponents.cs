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
        if (string.IsNullOrEmpty(markdown))
            return string.Empty;

        var pipeline = _defaultPipeline;
        if (configureOptions != null)
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
}