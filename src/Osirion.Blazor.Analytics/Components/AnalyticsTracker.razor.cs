using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Handlers;

namespace Osirion.Blazor.Analytics.Components;

/// <summary>
/// Base component for analytics trackers
/// </summary>
public abstract class AnalyticsTracker : OsirionComponentBase
{
    /// <summary>
    /// Gets the analytics provider
    /// </summary>
    protected abstract IAnalyticsProvider? Provider { get; }

    /// <summary>
    /// Gets or sets the site ID. When set it replaces the configured site ID for this render.
    /// Used by the Clarity and Matomo trackers; the GA4 and Yandex Metrica trackers use
    /// their own identifier parameters instead.
    /// </summary>
    [Parameter]
    public string? SiteId { get; set; }

    /// <summary>
    /// Gets or sets whether the tracker is enabled. When set it replaces the configured
    /// <c>Enabled</c> option for this render.
    /// </summary>
    [Parameter]
    public bool? Enabled { get; set; }

    [CascadingParameter]
    private HttpContext? CascadedHttpContext { get; set; }

    [Inject]
    private IServiceProvider TrackerServices { get; set; } = default!;

    /// <summary>Gets the registered <typeparamref name="TService"/>, or <see langword="null"/> when it is not registered.</summary>
    protected TService? GetOptionalService<TService>() where TService : class => TrackerServices?.GetService<TService>();

    /// <summary>
    /// Gets whether the provider can render with this tracker's parameter overrides applied.
    /// </summary>
    protected virtual bool CanRenderProvider => Enabled != false && (Provider?.ShouldRender ?? false);

    /// <summary>
    /// Builds the tracker markup with this tracker's parameter overrides applied.
    /// </summary>
    protected virtual string BuildScript(AnalyticsScriptContext context) =>
        Provider?.GetScript(context) ?? string.Empty;

    /// <summary>
    /// Gets whether the tracker should emit its markup: the provider can render and, when the
    /// consent gate is on, the visitor consented to analytics.
    /// </summary>
    protected bool ShouldRenderTracker => CanRenderProvider && IsConsentSatisfied();

    /// <summary>
    /// Gets the markup to emit. Every value in it is encoded by the provider.
    /// </summary>
    protected MarkupString TrackerMarkup => new(BuildScript(CreateScriptContext()));

    /// <summary>
    /// Determines whether the component should render
    /// </summary>
    protected override bool ShouldRender() => ShouldRenderTracker;

    private AnalyticsScriptContext CreateScriptContext() => GetConsentOptions().RequireConsent
        ? new AnalyticsScriptContext { ConsentGateActive = true }
        : AnalyticsScriptContext.Default;

    private bool IsConsentSatisfied()
    {
        var consent = GetConsentOptions();
        if (!consent.RequireConsent)
        {
            return true;
        }

        // Fail closed: without a request there is no cookie to read.
        var httpContext = CascadedHttpContext ?? TrackerServices?.GetService<IHttpContextAccessor>()?.HttpContext;
        return httpContext is not null
            && CookieConsentHandler.IsCategoryConsented(httpContext, consent.Category);
    }

    private AnalyticsConsentOptions GetConsentOptions() =>
        TrackerServices?.GetService<IOptions<AnalyticsConsentOptions>>()?.Value ?? new AnalyticsConsentOptions();
}
