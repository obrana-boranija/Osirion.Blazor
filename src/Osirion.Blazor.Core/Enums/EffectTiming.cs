using System.ComponentModel;

namespace Osirion.Blazor.Components;

/// <summary>
/// Defines when visual effects should be applied
/// </summary>
public enum EffectTiming
{
    /// <summary>
    /// Effect is never applied
    /// </summary>
    [Description("Effect is never applied")]
    Never,

    /// <summary>
    /// Effect is always visible
    /// </summary>
    [Description("Effect is always visible")]
    Always,

    /// <summary>
    /// Effect is only visible on hover
    /// </summary>
    [Description("Effect is only visible on hover")]
    OnHover
}