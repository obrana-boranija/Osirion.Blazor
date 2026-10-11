using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.UnitOfWork;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.UnitOfWork;

public class BaseUnitOfWorkEdgeCaseTests
{
    private static PlainUnitOfWork Plain(IDomainEventDispatcher? dispatcher = null) =>
        new(Substitute.For<IContentRepository>(), Substitute.For<IDirectoryRepository>(), dispatcher);

    private static FailingUnitOfWork Failing() =>
        new(Substitute.For<IContentRepository>(), Substitute.For<IDirectoryRepository>());

    [Fact]
    public void Constructor_NullArgument_Throws()
    {
        var content = Substitute.For<IContentRepository>();
        var directories = Substitute.For<IDirectoryRepository>();

        Should.Throw<ArgumentNullException>(() => new PlainUnitOfWork(null!, directories, null));
        Should.Throw<ArgumentNullException>(() => new PlainUnitOfWork(content, null!, null));
        Should.Throw<ArgumentNullException>(() => new NamedUnitOfWork(content, directories, null!));
    }

    [Fact]
    public async Task DefaultHooks_FullTransactionWithoutADispatcher_CompletesAndEndsTheTransaction()
    {
        var unitOfWork = Plain();

        await unitOfWork.BeginTransactionAsync();
        await unitOfWork.SavePointAsync("first");
        await unitOfWork.RollbackToSavePointAsync("first");
        await unitOfWork.CommitAsync();

        unitOfWork.TransactionStarted.ShouldBeFalse();
        unitOfWork.SavePoints.ShouldBeEmpty();
    }

    [Fact]
    public async Task RollbackAsync_DefaultHook_EndsTheTransaction()
    {
        var unitOfWork = Plain();
        await unitOfWork.BeginTransactionAsync();
        await unitOfWork.SavePointAsync("first");

        await unitOfWork.RollbackAsync();

        unitOfWork.TransactionStarted.ShouldBeFalse();
        unitOfWork.SavePoints.ShouldBeEmpty();
    }

    [Fact]
    public async Task CommitAsync_DefaultEventSource_DispatchesNothing()
    {
        var dispatcher = Substitute.For<IDomainEventDispatcher>();
        var unitOfWork = Plain(dispatcher);
        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.CommitAsync();

        await dispatcher.DidNotReceiveWithAnyArgs().DispatchAsync(default(IDomainEvent)!);
    }

    [Fact]
    public async Task DispatchDomainEventsAsync_NoDispatcher_DoesNothing()
    {
        var unitOfWork = Plain();

        await Should.NotThrowAsync(() => unitOfWork.Dispatch([Substitute.For<IDomainEvent>()]));
    }

    [Fact]
    public async Task DispatchDomainEventsAsync_WithDispatcher_DispatchesEachEventInOrder()
    {
        var dispatcher = Substitute.For<IDomainEventDispatcher>();
        var first = Substitute.For<IDomainEvent>();
        var second = Substitute.For<IDomainEvent>();
        var unitOfWork = Plain(dispatcher);

        await unitOfWork.Dispatch([first, second]);

        Received.InOrder(() =>
        {
            dispatcher.DispatchAsync(first);
            dispatcher.DispatchAsync(second);
        });
    }

    [Fact]
    public async Task SavePointAsync_NoTransaction_Throws()
    {
        var unitOfWork = Plain();

        await Should.ThrowAsync<InvalidOperationException>(() => unitOfWork.SavePointAsync("a"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task SavePointAsync_BlankName_Throws(string name)
    {
        var unitOfWork = Plain();
        await unitOfWork.BeginTransactionAsync();

        await Should.ThrowAsync<ArgumentException>(() => unitOfWork.SavePointAsync(name));
    }

    [Fact]
    public async Task RollbackToSavePointAsync_NoTransaction_Throws()
    {
        var unitOfWork = Plain();

        await Should.ThrowAsync<InvalidOperationException>(() => unitOfWork.RollbackToSavePointAsync("a"));
    }

    [Fact]
    public async Task RollbackToSavePointAsync_HookFails_RethrowsAndKeepsLaterSavePoints()
    {
        var unitOfWork = Failing();
        await unitOfWork.BeginTransactionAsync();
        await unitOfWork.SavePointAsync("first");
        await unitOfWork.SavePointAsync("second");

        await Should.ThrowAsync<IOException>(() => unitOfWork.RollbackToSavePointAsync("first"));

        unitOfWork.SavePoints.Keys.ShouldBe(["first", "second"]);
    }

    [Fact]
    public async Task RollbackAsync_HookFails_RethrowsAndKeepsTheTransactionOpen()
    {
        var unitOfWork = Failing();
        await unitOfWork.BeginTransactionAsync();

        await Should.ThrowAsync<IOException>(() => unitOfWork.RollbackAsync());

        unitOfWork.TransactionStarted.ShouldBeTrue();
    }

    [Fact]
    public async Task Dispose_RollbackFails_SwallowsTheError()
    {
        var unitOfWork = Failing();
        await unitOfWork.BeginTransactionAsync();

        Should.NotThrow(unitOfWork.Dispose);

        unitOfWork.RollbackAttempts.ShouldBe(1);
    }

    [Fact]
    public async Task DisposeAsync_RollbackFails_SwallowsTheError()
    {
        var unitOfWork = Failing();
        await unitOfWork.BeginTransactionAsync();

        await Should.NotThrowAsync(async () => await unitOfWork.DisposeAsync());

        unitOfWork.RollbackAttempts.ShouldBe(1);
    }

    [Fact]
    public async Task DisposeAsync_NoTransaction_DoesNotRollBack()
    {
        var unitOfWork = Failing();

        await unitOfWork.DisposeAsync();

        unitOfWork.RollbackAttempts.ShouldBe(0);
    }

    private class PlainUnitOfWork(IContentRepository content, IDirectoryRepository directories, IDomainEventDispatcher? dispatcher)
        : BaseUnitOfWork(content, directories, "plain", NullLogger.Instance, dispatcher)
    {
        public bool TransactionStarted => _transactionStarted;

        public Dictionary<string, string> SavePoints => _savePoints;

        public Task Dispatch(IEnumerable<IDomainEvent> events) => DispatchDomainEventsAsync(events);
    }

    private sealed class NamedUnitOfWork(IContentRepository content, IDirectoryRepository directories, string providerId)
        : BaseUnitOfWork(content, directories, providerId, NullLogger.Instance);

    private sealed class FailingUnitOfWork(IContentRepository content, IDirectoryRepository directories)
        : PlainUnitOfWork(content, directories, null)
    {
        public int RollbackAttempts { get; private set; }

        protected override Task OnRollbackTransactionAsync(CancellationToken cancellationToken)
        {
            RollbackAttempts++;
            throw new IOException("rollback failed");
        }

        protected override Task OnRollbackToSavePointAsync(string savePointName, CancellationToken cancellationToken) =>
            throw new IOException("savepoint rollback failed");
    }
}
