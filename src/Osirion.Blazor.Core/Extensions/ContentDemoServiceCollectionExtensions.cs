using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Core.Services;

namespace Osirion.Blazor.Core.Extensions;

/// <summary>
/// Registers the live demos that Markdown content can embed.
/// </summary>
public static class ContentDemoServiceCollectionExtensions
{
    /// <summary>
    /// Registers demos for Markdown content. A fenced block with the language <c>demo</c> whose text is a registered
    /// key renders the demo component in its place (with a Code tab when the demo has source code), wherever
    /// <c>OsirionHtmlRenderer</c> renders the content. Calling it again adds to the same registry.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Adds the demos.</param>
    /// <returns>The service collection, for chaining.</returns>
    /// <example>
    /// <code>
    /// builder.Services.AddOsirionContentDemos(demos =&gt; demos
    ///     .Add&lt;TabsBasicDemo&gt;("tabs-basic", source: ContentDemoRegistry.ReadEmbeddedSource(
    ///         typeof(TabsBasicDemo).Assembly, "MySite.Demos.TabsBasicDemo.razor")));
    /// </code>
    /// </example>
    public static IServiceCollection AddOsirionContentDemos(this IServiceCollection services, Action<ContentDemoRegistry> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var registry = services
            .Where(descriptor => descriptor.ServiceType == typeof(ContentDemoRegistry))
            .Select(descriptor => descriptor.ImplementationInstance)
            .OfType<ContentDemoRegistry>()
            .FirstOrDefault();

        if (registry is null)
        {
            registry = new ContentDemoRegistry();
            services.AddSingleton(registry);
            services.AddSingleton<IContentDemoRegistry>(registry);
        }

        configure(registry);
        return services;
    }
}
