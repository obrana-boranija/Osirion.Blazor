using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Infrastructure.Events;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Events;

public class DomainEventDispatcherFailureTests
{
    private readonly ILogger<DomainEventDispatcher> _logger = Substitute.For<ILogger<DomainEventDispatcher>>();

    public sealed class FailureTestEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; } = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }

    private sealed class ThrowingHandler : IDomainEventHandler<FailureTestEvent>
    {
        public Task HandleAsync(FailureTestEvent domainEvent) => throw new InvalidOperationException("handler failed");
    }

    private sealed class RecordingHandler : IDomainEventHandler<FailureTestEvent>
    {
        public List<FailureTestEvent> Handled { get; } = [];

        public Task HandleAsync(FailureTestEvent domainEvent)
        {
            Handled.Add(domainEvent);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public void Constructor_WithNullDependencies_Throws()
    {
        Should.Throw<ArgumentNullException>(() => new DomainEventDispatcher(null!, _logger));
        Should.Throw<ArgumentNullException>(() => new DomainEventDispatcher(new ServiceCollection().BuildServiceProvider(), null!));
    }

    [Fact]
    public async Task DispatchAsync_WhenAHandlerThrows_LogsItAndRunsTheOtherHandlers()
    {
        var recorder = new RecordingHandler();
        var services = new ServiceCollection();
        services.AddSingleton<IDomainEventHandler<FailureTestEvent>>(new ThrowingHandler());
        services.AddSingleton<IDomainEventHandler<FailureTestEvent>>(recorder);
        var dispatcher = new DomainEventDispatcher(services.BuildServiceProvider(), _logger);
        var domainEvent = new FailureTestEvent();

        await dispatcher.DispatchAsync<IDomainEvent>(domainEvent);

        recorder.Handled.ShouldBe([domainEvent]);
        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<Arg.AnyType>(),
            Arg.Is<Exception>(ex => ex.Message == "handler failed"),
            Arg.Any<Func<Arg.AnyType, Exception?, string>>());
    }

    [Fact]
    public async Task DispatchAsync_WhenHandlersCannotBeResolved_LogsAndRethrows()
    {
        var failure = new InvalidOperationException("container failed");
        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider.GetService(Arg.Any<Type>()).Returns(_ => throw failure);
        var dispatcher = new DomainEventDispatcher(serviceProvider, _logger);

        var thrown = await Should.ThrowAsync<InvalidOperationException>(() => dispatcher.DispatchAsync(new FailureTestEvent()));

        thrown.ShouldBeSameAs(failure);
        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<Arg.AnyType>(),
            failure,
            Arg.Any<Func<Arg.AnyType, Exception?, string>>());
    }
}
