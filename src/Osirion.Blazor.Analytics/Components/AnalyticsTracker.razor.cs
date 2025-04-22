using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;

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
    /// Gets or sets the site ID (overrides configured value)
    /// </summary>
    [Parameter]
    public string? SiteId { get; set; }

    /// <summary>
    /// Gets or sets whether the tracker is enabled
    /// </summary>
    [Parameter]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Determines whether the component should render
    /// </summary>
    protected override bool ShouldRender()
    {
        return Provider?.ShouldRender ?? false;
    }
}