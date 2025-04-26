using Markdig;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Core.Interfaces;
using Osirion.Blazor.Cms.Services;

namespace Osirion.Blazor.Cms.Extensions;

/// <summary>
/// Extension methods for registering and configuring markdown editor components
/// </summary>
public static class MarkdownEditorExtensions
{
    /// <summary>
    /// Adds markdown editor services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionMarkdownEditor(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        // Register Markdig pipeline as a singleton
        services.AddSingleton(_ => new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseYamlFrontMatter()
            .DisableHtml()  // Prevents raw HTML injection
            .UseEmojiAndSmiley()
            .UseAutoLinks()
            .UseTaskLists()
            .UsePipeTables()
            .Build());

        AddMarkdownEditorDependencies(services);

        return services;
    }

    /// <summary>
    /// Adds markdown editor services to the service collection with custom configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureMarkdig">Action to configure the Markdig pipeline</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionMarkdownEditor(
        this IServiceCollection services,
        Action<MarkdownPipelineBuilder> configureMarkdig)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configureMarkdig == null) throw new ArgumentNullException(nameof(configureMarkdig));

        // Register Markdig pipeline with custom configuration
        services.AddSingleton(_ =>
        {
            var builder = new MarkdownPipelineBuilder();
            configureMarkdig(builder);
            return builder.Build();
        });

        AddMarkdownEditorDependencies(services);

        return services;
    }

    private static void AddMarkdownEditorDependencies(IServiceCollection services)
    {
        services.AddScoped<IMarkdownRendererService, MarkdownRendererService>();
    }
}