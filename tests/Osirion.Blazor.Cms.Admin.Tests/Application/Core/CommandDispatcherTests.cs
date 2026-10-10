#pragma warning disable CS0618 // The tests keep covering the types until they are removed
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Application.Core;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Application.Core;

public class CommandDispatcherTests
{
    private static CommandDispatcher CreateDispatcher(Action<IServiceCollection>? configure = null)
    {
        var services = new ServiceCollection();
        configure?.Invoke(services);
        return new CommandDispatcher(services.BuildServiceProvider());
    }

    [Fact]
    public async Task DispatchAsync_ShouldResolveAndCallCommandHandler()
    {
        // Arrange
        var command = new TestCommand();
        var handler = Substitute.For<ICommandHandler<TestCommand>>();
        var dispatcher = CreateDispatcher(services => services.AddSingleton(handler));

        // Act
        await dispatcher.DispatchAsync(command);

        // Assert
        await handler.Received(1).HandleAsync(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DispatchAsync_WithResult_ShouldResolveAndCallCommandHandler()
    {
        // Arrange
        var command = new TestCommandWithResult();
        var expectedResult = "Test Result";
        var handler = Substitute.For<ICommandHandler<TestCommandWithResult, string>>();
        handler.HandleAsync(command, Arg.Any<CancellationToken>()).Returns(expectedResult);
        var dispatcher = CreateDispatcher(services => services.AddSingleton(handler));

        // Act
        var result = await dispatcher.DispatchAsync<TestCommandWithResult, string>(command);

        // Assert
        result.ShouldBe(expectedResult);
        await handler.Received(1).HandleAsync(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DispatchAsync_WhenHandlerNotFound_ShouldThrowException()
    {
        // Arrange
        var command = new TestCommand();
        var dispatcher = CreateDispatcher();

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await dispatcher.DispatchAsync(command));
    }

    [Fact]
    public async Task DispatchAsync_WithResult_WhenHandlerNotFound_ShouldThrowException()
    {
        // Arrange
        var command = new TestCommandWithResult();
        var dispatcher = CreateDispatcher();

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await dispatcher.DispatchAsync<TestCommandWithResult, string>(command));
    }

    [Fact]
    public async Task DispatchAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var command = new TestCommand();
        var handler = Substitute.For<ICommandHandler<TestCommand>>();
        var cancellationToken = new CancellationToken();
        var dispatcher = CreateDispatcher(services => services.AddSingleton(handler));

        // Act
        await dispatcher.DispatchAsync(command, cancellationToken);

        // Assert
        await handler.Received(1).HandleAsync(command, cancellationToken);
    }

    public class TestCommand : ICommand
    {
    }

    public class TestCommandWithResult : ICommand<string>
    {
    }
}