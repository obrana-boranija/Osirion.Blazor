namespace Osirion.Blazor.Cms.Admin.Tests.TestDoubles;

/// <summary>Waits for work that finishes on another thread, with a bounded wait instead of a fixed sleep.</summary>
public static class TestWait
{
    public static async Task UntilAsync(Func<bool> condition, int timeoutMilliseconds = 5000)
    {
        using var timeout = new CancellationTokenSource(timeoutMilliseconds);
        while (!condition())
        {
            await Task.Delay(10, timeout.Token);
        }
    }
}
