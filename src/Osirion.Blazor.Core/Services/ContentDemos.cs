using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Core.Services;

/// <summary>
/// A live example that Markdown content can embed: a Razor component, and optionally the source code shown beside it.
/// </summary>
/// <param name="Key">The name content uses for the demo, for example <c>tabs-basic</c>.</param>
/// <param name="ComponentType">The component rendered as the live preview.</param>
public sealed record ContentDemo(string Key, Type ComponentType)
{
    /// <summary>Gets the parameters passed to the component.</summary>
    public IReadOnlyDictionary<string, object?>? Parameters { get; init; }

    /// <summary>Gets a short title, used to name the example for screen readers. Defaults to the key.</summary>
    public string? Title { get; init; }

    /// <summary>
    /// Gets the source code shown in a Code tab next to the preview, usually the demo component's Razor markup. Without
    /// it, only the preview renders.
    /// </summary>
    public string? Source { get; init; }
}

/// <summary>
/// The demos Markdown content can embed with a fenced <c>demo</c> block whose text is the demo's key.
/// </summary>
public interface IContentDemoRegistry
{
    /// <summary>Finds the demo registered under <paramref name="key"/>.</summary>
    /// <param name="key">The demo key, compared ignoring case.</param>
    /// <param name="demo">The demo, when found.</param>
    /// <returns><see langword="true"/> when a demo is registered under the key.</returns>
    bool TryGet(string key, out ContentDemo demo);
}

/// <summary>
/// Registers the demos Markdown content can embed. Configure it with <c>AddOsirionContentDemos</c>.
/// </summary>
public sealed partial class ContentDemoRegistry : IContentDemoRegistry
{
    private readonly Dictionary<string, ContentDemo> _demos = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Gets the registered demos.</summary>
    public IReadOnlyCollection<ContentDemo> Demos => _demos.Values;

    /// <summary>
    /// Registers <typeparamref name="TComponent"/> as the demo <paramref name="key"/>.
    /// </summary>
    /// <typeparam name="TComponent">The component rendered as the live preview.</typeparam>
    /// <param name="key">1 to 60 lowercase letters, digits or hyphens, for example <c>tabs-basic</c>.</param>
    /// <param name="source">The source code shown in a Code tab, or <see langword="null"/> for a preview only.</param>
    /// <param name="title">A short title for screen readers; defaults to the key.</param>
    /// <param name="parameters">Parameters passed to the component.</param>
    /// <returns>This registry, for chaining.</returns>
    /// <exception cref="ArgumentException">The key is not valid or is already registered.</exception>
    public ContentDemoRegistry Add<TComponent>(
        string key,
        string? source = null,
        string? title = null,
        IReadOnlyDictionary<string, object?>? parameters = null)
        where TComponent : IComponent
    {
        if (!IsValidKey(key))
        {
            throw new ArgumentException($"'{key}' is not a valid demo key: use 1 to 60 lowercase letters, digits or hyphens.", nameof(key));
        }

        if (!_demos.TryAdd(key, new ContentDemo(key, typeof(TComponent)) { Source = source, Title = title, Parameters = parameters }))
        {
            throw new ArgumentException($"A demo with the key '{key}' is already registered.", nameof(key));
        }

        return this;
    }

    /// <inheritdoc/>
    public bool TryGet(string key, out ContentDemo demo) => _demos.TryGetValue(key, out demo!);

    /// <summary>
    /// Reads a demo's source code embedded in <paramref name="assembly"/>, for example the demo's <c>.razor</c> file
    /// added as an <c>EmbeddedResource</c>.
    /// </summary>
    /// <param name="assembly">The assembly that embeds the file.</param>
    /// <param name="resourceName">The manifest resource name, such as <c>MySite.Demos.TabsBasic.razor</c>.</param>
    /// <returns>The file's text.</returns>
    /// <exception cref="ArgumentException">No resource has that name.</exception>
    public static string ReadEmbeddedSource(Assembly assembly, string resourceName)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new ArgumentException($"The assembly {assembly.GetName().Name} has no embedded resource '{resourceName}'.", nameof(resourceName));
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>Gets whether <paramref name="key"/> can name a demo.</summary>
    public static bool IsValidKey(string? key) => key is not null && KeyPattern().IsMatch(key);

    [GeneratedRegex("^[a-z0-9-]{1,60}$", RegexOptions.CultureInvariant)]
    private static partial Regex KeyPattern();
}
