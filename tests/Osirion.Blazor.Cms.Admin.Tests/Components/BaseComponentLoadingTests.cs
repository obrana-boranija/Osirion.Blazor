using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Common.Constants;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Shared.Components;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Components;

public class BaseComponentLoadingTests : AdminComponentTestContext
{
    private readonly RecordingLogger<BaseComponent> _logger = new();

    public BaseComponentLoadingTests()
    {
        Services.AddSingleton<ILogger<BaseComponent>>(_logger);
        RenderInteractively();
    }

    public sealed class Probe : BaseComponent
    {
        public Task RunWithLoadingAsync(Func<Task> action) => ExecuteWithLoadingAsync(action);

        public Task RunAsync(Func<Task> action) => ExecuteAsync(action);

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "p");
            builder.AddAttribute(1, "id", "probe");
            builder.AddContent(2, IsLoading ? "loading" : ErrorMessage ?? "idle");
            builder.CloseElement();
        }
    }

    [Fact]
    public async Task ExecuteWithLoadingAsync_ShowsLoadingUntilTheActionFinishes()
    {
        var cut = Render<Probe>();
        var pending = new TaskCompletionSource();

        var running = cut.InvokeAsync(() => cut.Instance.RunWithLoadingAsync(() => pending.Task));
        cut.Find("#probe").TextContent.ShouldBe("loading");
        pending.SetResult();
        await running;

        cut.Find("#probe").TextContent.ShouldBe("idle");
    }

    [Fact]
    public async Task ExecuteWithLoadingAsync_ActionThrows_ReportsTheGenericMessage()
    {
        var cut = Render<Probe>();

        await cut.InvokeAsync(() => cut.Instance.RunWithLoadingAsync(() => throw new InvalidOperationException("secret")));

        cut.Find("#probe").TextContent.ShouldBe(UserMessages.OperationFailed);
        _logger.Entries.ShouldContain(entry => entry.Level == LogLevel.Error && entry.Message.Contains("Probe"));
        Services.GetRequiredService<IEventPublisher>().Received(1).Publish(Arg.Is<ErrorOccurredEvent>(e => e.Message == UserMessages.OperationFailed));
    }

    [Fact]
    public async Task ExecuteAsync_Success_ClearsAnEarlierError()
    {
        var cut = Render<Probe>();
        await cut.InvokeAsync(() => cut.Instance.RunAsync(() => throw new InvalidOperationException("first")));

        await cut.InvokeAsync(() => cut.Instance.RunAsync(() => Task.CompletedTask));

        cut.Find("#probe").TextContent.ShouldBe("idle");
    }
}
