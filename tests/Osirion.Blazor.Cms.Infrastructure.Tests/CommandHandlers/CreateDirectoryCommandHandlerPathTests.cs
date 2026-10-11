using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Application.Commands.Directory;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Repositories;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.CommandHandlers;

public class CreateDirectoryCommandHandlerPathTests
{
    [Fact]
    public async Task HandleAsync_NoParentAndNoPath_UsesTheNameAsThePath()
    {
        var directoryRepository = Substitute.For<IDirectoryRepository>();
        directoryRepository.SaveAsync(Arg.Any<DirectoryItem>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<DirectoryItem>());
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.DirectoryRepository.Returns(directoryRepository);
        unitOfWork.ProviderId.Returns("test-provider");
        var factory = Substitute.For<IUnitOfWorkFactory>();
        factory.CreateForDefaultProvider().Returns(unitOfWork);
        var handler = new CreateDirectoryCommandHandler(factory, Substitute.For<IDomainEventDispatcher>(), NullLogger<CreateDirectoryCommandHandler>.Instance);

        await handler.HandleAsync(new CreateDirectoryCommand { Name = "Guides", Path = string.Empty });

        await directoryRepository.Received(1).SaveAsync(
            Arg.Is<DirectoryItem>(directory => directory.Path == "Guides" && directory.Name == "Guides"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public void Constructor_NullDependency_Throws()
    {
        var factory = Substitute.For<IUnitOfWorkFactory>();
        var dispatcher = Substitute.For<IDomainEventDispatcher>();
        var logger = NullLogger<CreateDirectoryCommandHandler>.Instance;

        Should.Throw<ArgumentNullException>(() => new CreateDirectoryCommandHandler(null!, dispatcher, logger));
        Should.Throw<ArgumentNullException>(() => new CreateDirectoryCommandHandler(factory, null!, logger));
        Should.Throw<ArgumentNullException>(() => new CreateDirectoryCommandHandler(factory, dispatcher, null!));
    }
}
