using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Theming.Services;

namespace Osirion.Blazor.Theming;

public static class ThemingServiceCollectionExtensions
{
    /// <summary>
    /// Adds theming with automatic CSS framework detection.
    /// </summary>
    public static IServiceCollection AddOsirionAutoTheming(this IServiceCollection services)
    {
        services.AddSingleton<ICssFrameworkDetector, CssFrameworkDetector>();
        services.AddScoped<IThemeService, AutoThemeService>();
        return services;
    }
}
