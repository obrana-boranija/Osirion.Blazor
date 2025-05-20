using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Application.Core;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Application.Core;

public class CommandDispatcherTests
{
    private readonly CommandDispatcher _commandDispatcher;
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcherTests()
    {
        _serviceProvider = Substitute.For<IServiceProvider>();
        _commandDispatcher = new CommandDispatcher(_serviceProvider);
    }

    [Fact]
    public async Task DispatchAsync_ShouldResolveAndCallCommandHandler()
    {
        // Arrange
        var command = new TestCommand();
        var handler = Substitute.For<ICommandHandler<TestCommand>>();

        _serviceProvider.GetRequiredService<ICommandHandler<TestCommand>>().Returns(handler);

        // Act
        await _commandDispatcher.DispatchAsync(command);

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

        _serviceProvider.GetRequiredService<ICommandHandler<TestCommandWithResult, string>>().Returns(handler);

        // Act
        var result = await _commandDispatcher.DispatchAsync<TestCommandWithResult, string>(command);

        // Assert
        result.ShouldBe(expectedResult);
        await handler.Received(1).HandleAsync(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DispatchAsync_WhenHandlerNotFound_ShouldThrowException()
    {
        // Arrange
        var command = new TestCommand();

        _serviceProvider
            .GetRequiredService<ICommandHandler<TestCommand>>()
            .Returns(x => throw new InvalidOperationException("Handler not found"));

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await _commandDispatcher.DispatchAsync(command));
    }

    [Fact]
    public async Task DispatchAsync_WithResult_WhenHandlerNotFound_ShouldThrowException()
    {
        // Arrange
        var command = new TestCommandWithResult();

        _serviceProvider
            .GetRequiredService<ICommandHandler<TestCommandWithResult, string>>()
            .Returns(x => throw new InvalidOperationException("Handler not found"));

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await _commandDispatcher.DispatchAsync<TestCommandWithResult, string>(command));
    }

    [Fact]
    public async Task DispatchAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var command = new TestCommand();
        var handler = Substitute.For<ICommandHandler<TestCommand>>();
        var cancellationToken = new CancellationToken();

        _serviceProvider.GetRequiredService<ICommandHandler<TestCommand>>().Returns(handler);

        // Act
        await _commandDispatcher.DispatchAsync(command, cancellationToken);

        // Assert
        await handler.Received(1).HandleAsync(command, cancellationToken);
    }

    private class TestCommand : ICommand
    {
    }

    private class TestCommandWithResult : ICommand<string>
    {
    }
}