using Markdig;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Osirion.Blazor.Cms.Core.Extensions;

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
            .UseEmojiAndSmiley()
            .UseAutoLinks()
            .UseTaskLists()
            .UsePipeTables()
            .Build());

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

        return services;
    }

    /// <summary>
    /// Registers the markdown editor JavaScript module during application startup
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime</param>
    /// <returns>Task representing the operation</returns>
    public static async Task RegisterMarkdownEditorModuleAsync(this IJSRuntime jsRuntime)
    {
        if (jsRuntime == null) throw new ArgumentNullException(nameof(jsRuntime));

        // Only register in browser context
        if (OperatingSystem.IsBrowser())
        {
            try
            {
                // This registers all the JavaScript functions for the editor components
                await jsRuntime.InvokeVoidAsync("import", "./_content/Osirion.Blazor.Cms.Core/js/markdownEditorInterop.js");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to register markdown editor module: {ex.Message}");
            }
        }
    }
}