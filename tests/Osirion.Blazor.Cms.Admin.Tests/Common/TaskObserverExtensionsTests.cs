using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Common.Extensions;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Common;

public class TaskObserverExtensionsTests
{
    [Fact]
    public async Task Observe_FaultedTask_LogsTheExceptionAtError()
    {
        var logger = new RecordingLogger<TaskObserverExtensionsTests>();

        Task.FromException(new InvalidOperationException("boom")).Observe(logger, "demo");

        await TestWait.UntilAsync(() => logger.HasEntry(LogLevel.Error, "demo"));
        logger.Entries.Single().Exception.ShouldBeOfType<InvalidOperationException>();
    }

    [Fact]
    public async Task Observe_CanceledTask_LogsNothing()
    {
        var logger = new RecordingLogger<TaskObserverExtensionsTests>();

        Task.FromCanceled(new CancellationToken(true)).Observe(logger, "demo");

        await Task.Delay(50);
        logger.Entries.ShouldBeEmpty();
    }

    [Fact]
    public void Observe_SuccessfulTask_LogsNothing()
    {
        var logger = new RecordingLogger<TaskObserverExtensionsTests>();

        Task.CompletedTask.Observe(logger, "demo");

        logger.Entries.ShouldBeEmpty();
    }
}
