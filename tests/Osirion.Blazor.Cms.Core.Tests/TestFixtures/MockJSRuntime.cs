using Microsoft.JSInterop;

namespace Osirion.Blazor.Cms.Core.Tests.TestFixtures;

/// <summary>
/// A mock implementation of IJSRuntime that returns predictable values
/// and avoids the issues with mocking ValueTask/ValueTask<T> returns.
/// </summary>
public class MockJSRuntime : IJSRuntime
{
    /// <summary>
    /// Mocked implementation of InvokeAsync that returns predictable values
    /// based on the requested type.
    /// </summary>
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object[] args)
    {
        // Handle different return types
        if (typeof(TValue) == typeof(double))
        {
            return new ValueTask<TValue>((TValue)(object)0.5d);
        }
        else if (typeof(TValue) == typeof(string))
        {
            return new ValueTask<TValue>((TValue)(object)"");
        }
        else if (typeof(TValue) == typeof(IJSObjectReference))
        {
            return new ValueTask<TValue>((TValue)(object)new MockJSObjectReference());
        }
        else
        {
            // Default for other types (including object)
            return new ValueTask<TValue>(result: default(TValue));
        }
    }

    /// <summary>
    /// Mocked implementation of InvokeAsync overload
    /// </summary>
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object[] args)
    {
        return InvokeAsync<TValue>(identifier, args);
    }

    /// <summary>
    /// Mocked implementation of InvokeVoidAsync
    /// </summary>
    public ValueTask InvokeVoidAsync(string identifier, params object[] args)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Mocked implementation of InvokeVoidAsync overload
    /// </summary>
    public ValueTask InvokeVoidAsync(string identifier, CancellationToken cancellationToken, params object[] args)
    {
        return ValueTask.CompletedTask;
    }
}

/// <summary>
/// Mock implementation of IJSObjectReference
/// </summary>
public class MockJSObjectReference : IJSObjectReference
{
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object[] args)
    {
        return new ValueTask<TValue>(result: default(TValue));
    }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object[] args)
    {
        return new ValueTask<TValue>(result: default(TValue));
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
