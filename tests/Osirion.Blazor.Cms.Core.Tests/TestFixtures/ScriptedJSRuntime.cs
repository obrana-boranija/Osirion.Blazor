using Microsoft.JSInterop;

namespace Osirion.Blazor.Cms.Core.Tests.TestFixtures;

/// <summary>
/// A JavaScript runtime that records the scripts passed to <c>eval</c> and can be switched to fail every call,
/// which is how the editor components see JavaScript when it is unavailable.
/// </summary>
public sealed class ScriptedJSRuntime : IJSRuntime
{
    private readonly List<string> _scripts = [];

    /// <summary>Gets or sets whether every call throws a <see cref="JSException"/>.</summary>
    public bool Fail { get; set; }

    /// <summary>Gets or sets the value returned for calls that expect a number.</summary>
    public double NumberResult { get; set; } = 0.25;

    /// <summary>Gets the scripts passed to <c>eval</c>, in call order.</summary>
    public IReadOnlyList<string> Scripts => _scripts;

    /// <inheritdoc />
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
    {
        if (args is [string script, ..])
        {
            _scripts.Add(script);
        }

        if (Fail)
        {
            throw new JSException("JavaScript is unavailable");
        }

        if (typeof(TValue) == typeof(double))
        {
            return new ValueTask<TValue>((TValue)(object)NumberResult);
        }

        return new ValueTask<TValue>(default(TValue)!);
    }

    /// <inheritdoc />
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) =>
        InvokeAsync<TValue>(identifier, args);
}
