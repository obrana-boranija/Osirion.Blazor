using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Shared.Components;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Components;

public class BaseComponentFailureTests : AdminComponentTestContext
{
    private readonly RecordingLogger<BaseComponent> _logger = new();

    private sealed class Probe : BaseComponent
    {
        public Task RunAsync(Func<Task> action) => ExecuteAsync(action);

        public string? Error => ErrorMessage;
    }

    public BaseComponentFailureTests()
    {
        Services.AddSingleton<ILogger<BaseComponent>>(_logger);
        RenderInteractively();
    }

    [Fact]
    public async Task ExecuteAsync_ActionThrows_ShowsGenericMessageLogsExceptionAndPublishesGenericEvent()
    {
        var cut = Render<Probe>();

        await cut.InvokeAsync(() => cut.Instance.RunAsync(() => throw new InvalidOperationException("/srv/secret/path token=abc")));

        cut.Instance.Error.ShouldNotBeNullOrWhiteSpace();
        cut.Instance.Error.ShouldNotContain("secret");
        _logger.Entries.ShouldContain(entry => entry.Level == LogLevel.Error && entry.Exception != null);
        Services.GetRequiredService<IEventPublisher>().Received(1)
            .Publish(Arg.Is<ErrorOccurredEvent>(e => !e.Message.Contains("secret") && e.Exception != null));
    }
}
