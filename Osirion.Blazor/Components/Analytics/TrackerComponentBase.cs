using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components.Analytics.Options;

namespace Osirion.Blazor.Components.Analytics;

/// <summary>
/// Base class for analytics tracker components
/// </summary>
/// <typeparam name="TOptions">The type of tracker options</typeparam>
public abstract class TrackerComponentBase<TOptions> : ComponentBase where TOptions : TrackerBaseOptions
{
    /// <summary>
    /// Gets or sets the options for the tracker
    /// </summary>
    [Parameter]
    public TOptions? TrackerOptions { get; set; }

    /// <summary>
    /// Determines whether the tracker should render
    /// </summary>
    protected bool CouldRender => TrackerOptions?.TrackerUrl is not null
        && TrackerOptions?.SiteId is not null
        && TrackerOptions?.Track == true;

    /// <summary>
    /// Gets the script to be rendered
    /// </summary>
    protected abstract string GetScript();

    /// <summary>
    /// Gets the script as a MarkupString
    /// </summary>
    protected MarkupString Script => (MarkupString)GetScript();
}