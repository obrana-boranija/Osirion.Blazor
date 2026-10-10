using Microsoft.JSInterop;

namespace Osirion.Blazor.Cms.Admin.Tests.TestDoubles;

/// <summary>
/// Answers the <c>localStorage</c> and <c>sessionStorage</c> calls protected browser storage makes, from an
/// in-memory dictionary keyed by store and key, so the stored (encrypted) values can be inspected.
/// </summary>
internal sealed class FakeBrowserStorageJSRuntime : IJSRuntime
{
    public Dictionary<string, string> Items { get; } = [];

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
    {
        return InvokeAsync<TValue>(identifier, CancellationToken.None, args);
    }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        var separator = identifier.LastIndexOf('.');
        var storeName = identifier[..separator];
        var operation = identifier[(separator + 1)..];
        var itemKey = $"{storeName}:{args![0]}";

        switch (operation)
        {
            case "setItem":
                Items[itemKey] = (string)args[1]!;
                return default;
            case "getItem":
                Items.TryGetValue(itemKey, out var value);
                return new ValueTask<TValue>((TValue)(object?)value!);
            case "removeItem":
                Items.Remove(itemKey);
                return default;
            default:
                throw new InvalidOperationException($"Unexpected JS call '{identifier}'.");
        }
    }
}
