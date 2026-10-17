using System.Runtime.CompilerServices;
using Bunit;

namespace Osirion.Blazor.Navigation.Tests;

/// <summary>Gives render waits room on slow CI runners.</summary>
internal static class SlowRunnerWaits
{
    // bUnit waits one second by default for a render a test waits on. On a busy macOS runner the renderer's first
    // check did not run within that second ("Check count: 0"), so loading-state tests failed there and passed
    // everywhere else. A wait that succeeds returns as soon as the render lands, so only a real failure takes longer.
    [ModuleInitializer]
    internal static void Initialize() => BunitContext.DefaultWaitTimeout = TimeSpan.FromSeconds(10);
}
