using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Application.Commands.Directory;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Repositories;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.CommandHandlers;

public class CreateDirectoryCommandHandlerTests
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<CreateDirectoryCommandHandler> _logger;
    private readonly CreateDirectoryCommandHandler _handler;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDirectoryRepository _directoryRepository;

    public CreateDirectoryCommandHandlerTests()
    {
        _unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        _eventDispatcher = Substitute.For<IDomainEventDispatcher>();
        _logger = Substitute.For<ILogger<CreateDirectoryCommandHandler>>();

        // Set up unit of work and repositories
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _directoryRepository = Substitute.For<IDirectoryRepository>();

        _unitOfWork.DirectoryRepository.Returns(_directoryRepository);
        _unitOfWork.ProviderId.Returns("test-provider");

        _unitOfWorkFactory.Create(Arg.Any<string>()).Returns(_unitOfWork);
        _unitOfWorkFactory.CreateForDefaultProvider().Returns(_unitOfWork);

        _handler = new CreateDirectoryCommandHandler(
            _unitOfWorkFactory,
            _eventDispatcher,
            _logger);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_CreatesDirectory()
    {
        // Arrange
        var command = new CreateDirectoryCommand
        {
            Name = "TestDir",
            Path = "test-path",
            Description = "Test Description",
            ProviderId = "test-provider"
        };

        var savedDirectory = DirectoryItem.Create(
            "test-hash",
            "test-path",
            "TestDir",
            "test-provider");

        _directoryRepository.SaveAsync(Arg.Any<DirectoryItem>(), Arg.Any<CancellationToken>())
            .Returns(savedDirectory);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        await _unitOfWork.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
        await _directoryRepository.Received(1).SaveAsync(Arg.Any<DirectoryItem>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());

        await _eventDispatcher.Received(1).DispatchAsync(Arg.Is<DirectoryCreatedEvent>(
            e => e.DirectoryId == savedDirectory.Id &&
                 e.Name == savedDirectory.Name &&
                 e.Path == savedDirectory.Path &&
                 e.ProviderId == savedDirectory.ProviderId));
    }

    [Fact]
    public async Task HandleAsync_WithParentId_CreatesDirectoryUnderParent()
    {
        // Arrange
        var parentId = "parent-id";
        var parentDirectory = DirectoryItem.Create(
            parentId,
            "parent-path",
            "Parent",
            "test-provider");

        var command = new CreateDirectoryCommand
        {
            Name = "ChildDir",
            ParentId = parentId,
            Description = "Child Description",
            ProviderId = "test-provider"
        };

        var savedDirectory = DirectoryItem.Create(
            "child-hash",
            "parent-path/ChildDir",
            "ChildDir",
            "test-provider");
        savedDirectory.SetParent(parentDirectory);

        _directoryRepository.GetByIdAsync(parentId, Arg.Any<CancellationToken>())
            .Returns(parentDirectory);

        _directoryRepository.SaveAsync(Arg.Any<DirectoryItem>(), Arg.Any<CancellationToken>())
            .Returns(savedDirectory);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        await _directoryRepository.Received(1).GetByIdAsync(parentId, Arg.Any<CancellationToken>());
        await _directoryRepository.Received(1).SaveAsync(
            Arg.Is<DirectoryItem>(d => d.Parent == parentDirectory),
            Arg.Any<CancellationToken>());

        await _eventDispatcher.Received(1).DispatchAsync(Arg.Is<DirectoryCreatedEvent>(
            e => e.DirectoryId == savedDirectory.Id));
    }

    [Fact]
    public async Task HandleAsync_WithParentNotFound_ThrowsDirectoryNotFoundException()
    {
        // Arrange
        var parentId = "non-existent-parent";
        var command = new CreateDirectoryCommand
        {
            Name = "TestDir",
            ParentId = parentId,
            ProviderId = "test-provider"
        };

        _directoryRepository.GetByIdAsync(parentId, Arg.Any<CancellationToken>())
            .Returns((DirectoryItem?)null);

        // Act & Assert
        await Should.ThrowAsync<Domain.Exceptions.DirectoryNotFoundException>(async () =>
            await _handler.HandleAsync(command));

        await _unitOfWork.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithRepositoryError_RollsBackAndRethrows()
    {
        // Arrange
        var command = new CreateDirectoryCommand
        {
            Name = "TestDir",
            Path = "test-path",
            ProviderId = "test-provider"
        };

        var expectedException = new Exception("Test repository error");
        _directoryRepository.SaveAsync(Arg.Any<DirectoryItem>(), Arg.Any<CancellationToken>())
            .Throws(expectedException);

        // Act & Assert
        var exception = await Should.ThrowAsync<Exception>(async () =>
            await _handler.HandleAsync(command));

        exception.ShouldBe(expectedException);
        await _unitOfWork.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_CreatesDirectoryWithCorrectProperties()
    {
        // Arrange
        var command = new CreateDirectoryCommand
        {
            Name = "TestDir",
            Path = "test-path",
            Description = "Test Description",
            Locale = "en",
            Order = 5,
            ProviderId = "test-provider"
        };

        DirectoryItem? capturedDirectory = null;
        _directoryRepository.SaveAsync(Arg.Do<DirectoryItem>(d => capturedDirectory = d), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<DirectoryItem>());

        // Act
        await _handler.HandleAsync(command);

        // Assert
        capturedDirectory.ShouldNotBeNull();
        capturedDirectory.Name.ShouldBe("TestDir");
        capturedDirectory.Path.ShouldBe("test-path");
        capturedDirectory.Description.ShouldBe("Test Description");
        capturedDirectory.Locale.ShouldBe("en");
        capturedDirectory.Order.ShouldBe(5);
        capturedDirectory.ProviderId.ShouldBe("test-provider");
    }

    [Fact]
    public async Task HandleAsync_WithNoProviderSpecified_UsesDefaultProvider()
    {
        // Arrange
        var command = new CreateDirectoryCommand
        {
            Name = "TestDir",
            Path = "test-path"
        };

        // Act
        await _handler.HandleAsync(command);

        // Assert
        _unitOfWorkFactory.Received(1).CreateForDefaultProvider();
        _unitOfWorkFactory.DidNotReceive().Create(Arg.Any<string>());
    }
}