using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Osirion.Blazor.Core.Configuration;
using Osirion.Blazor.Core.Services;
using Osirion.Blazor.Core.Services.Implementations;

namespace Osirion.Blazor.Core.Extensions;

/// <summary>
/// Extension methods for registering and resolving the content HTML sanitizer.
/// </summary>
public static class HtmlSanitizerServiceExtensions
{
    /// <summary>
    /// Registers <see cref="IHtmlContentSanitizer"/> as a singleton and optionally configures its allow-list.
    /// </summary>
    /// <remarks>
    /// Safe to call more than once: the sanitizer is registered once and every <paramref name="configure"/>
    /// delegate is applied, in registration order, when the sanitizer is first resolved.
    /// </remarks>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional delegate that changes the allow-list, for example to allow a custom element.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOsirionHtmlSanitizer(
        this IServiceCollection services,
        Action<HtmlContentSanitizerOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<HtmlContentSanitizerOptions>();

        if (configure is not null)
        {
            services.Configure(configure);
        }

        services.TryAddSingleton<IHtmlContentSanitizer, HtmlContentSanitizer>();

        return services;
    }

    /// <summary>
    /// Gets the registered <see cref="IHtmlContentSanitizer"/>, or <see cref="HtmlContentSanitizer.Default"/>
    /// when none is registered, so that content is never written unsanitized.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve from.</param>
    /// <returns>The sanitizer to use.</returns>
    public static IHtmlContentSanitizer GetHtmlContentSanitizer(this IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        return serviceProvider.GetService<IHtmlContentSanitizer>() ?? HtmlContentSanitizer.Default;
    }
}
