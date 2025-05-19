using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.UnitOfWork;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.UnitOfWork;

public class BaseUnitOfWorkTests
{
    private readonly IContentRepository _contentRepository;
    private readonly IDirectoryRepository _directoryRepository;
    private readonly ILogger<TestUnitOfWork> _logger;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly TestUnitOfWork _unitOfWork;
    private readonly string _providerId = "test-provider";

    public BaseUnitOfWorkTests()
    {
        _contentRepository = Substitute.For<IContentRepository>();
        _directoryRepository = Substitute.For<IDirectoryRepository>();
        _logger = Substitute.For<ILogger<TestUnitOfWork>>();
        _eventDispatcher = Substitute.For<IDomainEventDispatcher>();

        _unitOfWork = new TestUnitOfWork(
            _contentRepository,
            _directoryRepository,
            _providerId,
            _logger,
            _eventDispatcher);
    }

    [Fact]
    public void Constructor_InitializesProperties()
    {
        // Assert
        _unitOfWork.ContentRepository.ShouldBe(_contentRepository);
        _unitOfWork.DirectoryRepository.ShouldBe(_directoryRepository);
        _unitOfWork.ProviderId.ShouldBe(_providerId);
    }

    [Fact]
    public async Task BeginTransactionAsync_SetsTransactionStartedFlag()
    {
        // Act
        await _unitOfWork.BeginTransactionAsync();

        // Assert
        _unitOfWork.IsTransactionStarted.ShouldBeTrue();
        _unitOfWork.ReceivedCalls.ShouldContain("OnBeginTransactionAsync");
    }

    [Fact]
    public async Task BeginTransactionAsync_WhenAlreadyStarted_ThrowsInvalidOperationException()
    {
        // Arrange - Start a transaction
        await _unitOfWork.BeginTransactionAsync();

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await _unitOfWork.BeginTransactionAsync());
    }

    [Fact]
    public async Task CommitAsync_WhenNoTransaction_ThrowsInvalidOperationException()
    {
        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await _unitOfWork.CommitAsync());
    }

    [Fact]
    public async Task CommitAsync_WithActiveTransaction_CommitsAndDispatchesEvents()
    {
        // Arrange
        var testEvent = new TestDomainEvent();
        _unitOfWork.SetTestDomainEvents(new List<IDomainEvent> { testEvent });

        await _unitOfWork.BeginTransactionAsync();

        // Act
        await _unitOfWork.CommitAsync();

        // Assert
        _unitOfWork.IsTransactionStarted.ShouldBeFalse();
        _unitOfWork.ReceivedCalls.ShouldContain("OnCommitTransactionAsync");

        // Verify events were dispatched
        await _eventDispatcher.Received(1).DispatchAsync(testEvent);
    }

    [Fact]
    public async Task RollbackAsync_WhenNoTransaction_ThrowsInvalidOperationException()
    {
        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await _unitOfWork.RollbackAsync());
    }

    [Fact]
    public async Task RollbackAsync_WithActiveTransaction_RollsBack()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();

        // Act
        await _unitOfWork.RollbackAsync();

        // Assert
        _unitOfWork.IsTransactionStarted.ShouldBeFalse();
        _unitOfWork.ReceivedCalls.ShouldContain("OnRollbackTransactionAsync");
    }

    [Fact]
    public async Task SavePointAsync_WithValidName_CreatesSavePoint()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();
        var savePointName = "test-savepoint";

        // Act
        await _unitOfWork.SavePointAsync(savePointName);

        // Assert
        _unitOfWork.ReceivedCalls.ShouldContain("OnCreateSavePointAsync");
        _unitOfWork.SavePoints.ShouldContainKey(savePointName);
    }

    [Fact]
    public async Task SavePointAsync_WithExistingName_ThrowsArgumentException()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();
        var savePointName = "test-savepoint";
        await _unitOfWork.SavePointAsync(savePointName);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _unitOfWork.SavePointAsync(savePointName));
    }

    [Fact]
    public async Task RollbackToSavePointAsync_WithValidSavePoint_RollsBackToSavePoint()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();
        var savePointName = "test-savepoint";
        await _unitOfWork.SavePointAsync(savePointName);

        // Create another savepoint to test removal
        await _unitOfWork.SavePointAsync("later-savepoint");

        // Act
        await _unitOfWork.RollbackToSavePointAsync(savePointName);

        // Assert
        _unitOfWork.ReceivedCalls.ShouldContain("OnRollbackToSavePointAsync");
        _unitOfWork.SavePoints.ShouldContainKey(savePointName);
        _unitOfWork.SavePoints.ShouldNotContainKey("later-savepoint");
    }

    [Fact]
    public async Task RollbackToSavePointAsync_WithNonExistentSavePoint_ThrowsArgumentException()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _unitOfWork.RollbackToSavePointAsync("non-existent"));
    }

    [Fact]
    public void Dispose_WithActiveTransaction_RollsBackTransaction()
    {
        // Arrange
        _unitOfWork.BeginTransactionAsync().GetAwaiter().GetResult();

        // Act
        _unitOfWork.Dispose();

        // Assert
        _unitOfWork.IsTransactionStarted.ShouldBeFalse();
        _unitOfWork.ReceivedCalls.ShouldContain("OnRollbackTransactionAsync");
    }

    [Fact]
    public async Task DisposeAsync_WithActiveTransaction_RollsBackTransaction()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();

        // Act
        await _unitOfWork.DisposeAsync();

        // Assert
        _unitOfWork.IsTransactionStarted.ShouldBeFalse();
        _unitOfWork.ReceivedCalls.ShouldContain("OnRollbackTransactionAsync");
    }

    // Test concrete implementation
    private class TestUnitOfWork : BaseUnitOfWork
    {
        private List<IDomainEvent> _testDomainEvents = new();

        public bool IsTransactionStarted => base._transactionStarted;
        public Dictionary<string, string> SavePoints => base._savePoints;
        public List<string> ReceivedCalls { get; } = new();

        public TestUnitOfWork(
            IContentRepository contentRepository,
            IDirectoryRepository directoryRepository,
            string providerId,
            ILogger logger,
            IDomainEventDispatcher? eventDispatcher = null)
            : base(contentRepository, directoryRepository, providerId, logger, eventDispatcher)
        {
        }

        public void SetTestDomainEvents(List<IDomainEvent> events)
        {
            _testDomainEvents = events;
        }

        protected override List<IDomainEvent> GetDomainEventsFromTrackedEntities()
        {
            return _testDomainEvents;
        }

        protected override Task OnBeginTransactionAsync(CancellationToken cancellationToken)
        {
            ReceivedCalls.Add("OnBeginTransactionAsync");
            return Task.CompletedTask;
        }

        protected override Task OnCommitTransactionAsync(CancellationToken cancellationToken)
        {
            ReceivedCalls.Add("OnCommitTransactionAsync");
            return Task.CompletedTask;
        }

        protected override Task OnRollbackTransactionAsync(CancellationToken cancellationToken)
        {
            ReceivedCalls.Add("OnRollbackTransactionAsync");
            return Task.CompletedTask;
        }

        protected override Task OnCreateSavePointAsync(string savePointName, CancellationToken cancellationToken)
        {
            ReceivedCalls.Add("OnCreateSavePointAsync");
            return Task.CompletedTask;
        }

        protected override Task OnRollbackToSavePointAsync(string savePointName, CancellationToken cancellationToken)
        {
            ReceivedCalls.Add("OnRollbackToSavePointAsync");
            return Task.CompletedTask;
        }
    }

    // Test event class
    private class TestDomainEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}