using Markdig;
using Markdig.Extensions.AutoLinks;
using Microsoft.Extensions.Logging;

namespace Osirion.Blazor.Cms.Core.Services
{
    /// <summary>
    /// Advanced Markdown rendering service with enhanced security and configuration
    /// </summary>
    public interface IMarkdownRendererService
    {
        /// <summary>
        /// Renders markdown to HTML with advanced security and configuration
        /// </summary>
        /// <param name="markdown">Markdown content to render</param>
        /// <param name="configuration">Optional configuration action</param>
        /// <returns>Rendered HTML</returns>
        string RenderToHtml(
            string markdown,
            Action<MarkdownPipelineBuilder>? configuration = null);

        /// <summary>
        /// Asynchronously renders markdown to HTML
        /// </summary>
        Task<string> RenderToHtmlAsync(
            string markdown,
            Action<MarkdownPipelineBuilder>? configuration = null);
    }

    /// <summary>
    /// Implementation of advanced markdown rendering service
    /// </summary>
    public class MarkdownRendererService : IMarkdownRendererService
    {
        private readonly ILogger<MarkdownRendererService> _logger;

        public MarkdownRendererService(ILogger<MarkdownRendererService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Creates a secure default markdown pipeline
        /// </summary>
        private MarkdownPipeline CreateDefaultPipeline()
        {
            return new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseYamlFrontMatter()
                .DisableHtml()  // Prevent raw HTML injection
                .UseAutoLinks()
                .UseTaskLists()
                .UsePipeTables()
                .UseEmphasisExtras()
                .Build();
        }

        /// <summary>
        /// Renders markdown to HTML with robust error handling
        /// </summary>
        public string RenderToHtml(
            string markdown,
            Action<MarkdownPipelineBuilder>? configuration = null)
        {
            if (string.IsNullOrWhiteSpace(markdown))
                return string.Empty;

            try
            {
                // Create pipeline
                var pipelineBuilder = new MarkdownPipelineBuilder();
                configuration?.Invoke(pipelineBuilder);

                // If no custom configuration, use default
                if (configuration == null)
                {
                    pipelineBuilder = new MarkdownPipelineBuilder()
                        .UseAdvancedExtensions()
                        .UseYamlFrontMatter()
                        .DisableHtml()
                        .UseAutoLinks()
                        .UseTaskLists()
                        .UsePipeTables();
                }

                var pipeline = pipelineBuilder.Build();
                var html = Markdown.ToHtml(markdown, pipeline);

                return html;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Markdown rendering failed: {Message}", ex.Message);

                // Return a safe, formatted error message
                return $@"
                    <div class=""markdown-error"">
                        <h3>Markdown Rendering Error</h3>
                        <p>An error occurred while rendering markdown.</p>
                        <pre>{System.Web.HttpUtility.HtmlEncode(ex.Message)}</pre>
                    </div>";
            }
        }

        /// <summary>
        /// Asynchronous rendering with the same error handling
        /// </summary>
        public async Task<string> RenderToHtmlAsync(
            string markdown,
            Action<MarkdownPipelineBuilder>? configuration = null)
        {
            // Use Task.Run to keep the async method pattern
            return await Task.Run(() => RenderToHtml(markdown, configuration));
        }
    }
}