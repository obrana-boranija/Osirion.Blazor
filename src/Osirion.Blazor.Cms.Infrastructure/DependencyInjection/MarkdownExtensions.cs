using Markdig.Renderers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.Markdown;

namespace Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for registering markdown services
/// </summary>
public static class MarkdownExtensions
{
    /// <summary>
    /// Adds markdown processing services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddMarkdownServices(this IServiceCollection services)
    {
        //// Register individual services
        //services.TryAddSingleton<IMarkdownRenderer, MarkdigRenderer>();
        //services.TryAddSingleton<IFrontMatterExtractor, FrontMatterExtractor>();
        //services.TryAddSingleton<IMarkdownSanitizer, MarkdownSanitizer>();
        //services.TryAddSingleton<IHtmlToMarkdownConverter, HtmlToMarkdownConverter>();

        //// Register the main processor
        //services.TryAddSingleton<IMarkdownProcessor, MarkdownProcessor>();

        //return services;

        // Register individual markdown components
        services.TryAddSingleton<IMarkdownRendererService, DefaultMarkdownRenderer>();
        services.TryAddSingleton<IFrontMatterExtractor, DefaultFrontMatterExtractor>();
        services.TryAddSingleton<IMarkdownSanitizer, DefaultMarkdownSanitizer>();
        services.TryAddSingleton<IHtmlToMarkdownConverter, DefaultHtmlToMarkdownConverter>();

        // Register the composed processor
        services.TryAddSingleton<IMarkdownProcessor, Markdown.MarkdownProcessor>();

        return services;
    }
}