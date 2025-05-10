using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Theming.Internal;

/// <summary>
/// Implementation of the theming builder
/// </summary>
internal class ThemingBuilder : IThemingBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ThemingBuilder"/> class.
    /// </summary>
    public ThemingBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <inheritdoc/>
    public IThemingBuilder Configure(Action<ThemingOptions> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        Services.Configure(configure);
        return this;
    }

    /// <inheritdoc/>
    public IThemingBuilder UseFramework(CssFramework framework)
    {
        Services.Configure<ThemingOptions>(options => options.Framework = framework);
        return this;
    }

    /// <inheritdoc/>
    public IThemingBuilder EnableDarkMode(bool enabled = true)
    {
        Services.Configure<ThemingOptions>(options => options.EnableDarkMode = enabled);
        return this;
    }

    /// <inheritdoc/>
    public IThemingBuilder UseSystemPreference(bool useSystem = true)
    {
        Services.Configure<ThemingOptions>(options => options.FollowSystemPreference = useSystem);
        return this;
    }

    /// <inheritdoc/>
    public IThemingBuilder WithCustomVariables(string variables)
    {
        Services.Configure<ThemingOptions>(options => options.CustomVariables = variables);
        return this;
    }
}