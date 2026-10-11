using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Analytics.Options;

namespace Osirion.Blazor.Analytics.Extensions;

/// <summary>
/// Extension methods for <see cref="IAnalyticsBuilder"/>.
/// </summary>
public static class AnalyticsBuilderExtensions
{
    /// <summary>
    /// Makes every tracker wait for the visitor's analytics consent, as recorded by
    /// <c>OsirionCookieConsent</c>. Without consent no tracker script is rendered at all.
    /// Recommended for sites with visitors in the EU.
    /// </summary>
    /// <param name="builder">The analytics builder.</param>
    /// <param name="configure">Optional action to change the consent category.</param>
    /// <returns>The builder for chaining.</returns>
    public static IAnalyticsBuilder RequireConsent(
        this IAnalyticsBuilder builder,
        Action<AnalyticsConsentOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.Configure<AnalyticsConsentOptions>(options =>
        {
            options.RequireConsent = true;
            configure?.Invoke(options);
        });

        return builder;
    }
}
