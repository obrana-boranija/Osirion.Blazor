using Markdig;

namespace Osirion.Blazor.Cms.Options;

/// <summary>
/// Configuration options for Markdown editor
/// </summary>
public class MarkdownConfigurationOptions
{
    /// <summary>
    /// Maximum allowed markdown length
    /// </summary>
    public int MaxMarkdownLength { get; set; } = 100_000; // 100KB default

    /// <summary>
    /// Custom Markdig pipeline configuration
    /// </summary>
    public Action<MarkdownPipelineBuilder>? ConfigurePipeline { get; set; }

    /// <summary>
    /// Custom error handling configuration
    /// </summary>
    public Func<Exception, string>? CustomErrorHandler { get; set; }

    /// <summary>
    /// Allowed file extensions for markdown
    /// </summary>
    public string[] AllowedExtensions { get; set; } = new[] { ".md", ".markdown", ".txt" };
}