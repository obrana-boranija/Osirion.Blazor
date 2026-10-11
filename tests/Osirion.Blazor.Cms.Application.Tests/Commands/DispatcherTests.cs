using NSubstitute;
using Osirion.Blazor.Cms.Application.Commands;
using Osirion.Blazor.Cms.Application.Queries;
using Osirion.Blazor.Cms.Application.Validation;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Shouldly;

namespace Osirion.Blazor.Cms.Application.Tests.Commands;

public class DispatcherTests
{
    private readonly IServiceProvider _services = Substitute.For<IServiceProvider>();
    private readonly ICommandHandler<SampleCommand> _commandHandler = Substitute.For<ICommandHandler<SampleCommand>>();
    private readonly SampleCommand _command = new();

    public DispatcherTests()
    {
        _services.GetService(typeof(ICommandHandler<SampleCommand>)).Returns(_commandHandler);
    }

    [Fact]
    public async Task CommandDispatcher_RegisteredHandler_HandlesTheCommand()
    {
        using var cancellation = new CancellationTokenSource();

        await new CommandDispatcher(_services).DispatchAsync(_command, cancellation.Token);

        await _commandHandler.Received(1).HandleAsync(_command, cancellation.Token);
    }

    [Fact]
    public async Task CommandDispatcher_NoHandler_Throws()
    {
        _services.GetService(typeof(ICommandHandler<SampleCommand>)).Returns(null);

        await Should.ThrowAsync<InvalidOperationException>(() => new CommandDispatcher(_services).DispatchAsync(_command));
    }

    [Fact]
    public async Task ValidatingCommandDispatcher_NoValidator_HandlesTheCommand()
    {
        await new ValidatingCommandDispatcher(_services).DispatchAsync(_command);

        await _commandHandler.Received(1).HandleAsync(_command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ValidatingCommandDispatcher_ValidCommand_HandlesTheCommand()
    {
        GivenValidator(ValidationResult.Success());

        await new ValidatingCommandDispatcher(_services).DispatchAsync(_command);

        await _commandHandler.Received(1).HandleAsync(_command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ValidatingCommandDispatcher_InvalidCommand_ThrowsWithTheErrorsAndSkipsTheHandler()
    {
        GivenValidator(ValidationResult.Failure("Title", "Title is required"));

        var exception = await Should.ThrowAsync<ContentValidationException>(() =>
            new ValidatingCommandDispatcher(_services).DispatchAsync(_command));

        exception.Errors["Title"].ShouldBe(["Title is required"]);
        await _commandHandler.DidNotReceive().HandleAsync(Arg.Any<SampleCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task QueryDispatcher_RegisteredHandler_ReturnsTheHandlersResult()
    {
        var handler = Substitute.For<IQueryHandler<SampleQuery, string>>();
        var query = new SampleQuery();
        handler.HandleAsync(query, Arg.Any<CancellationToken>()).Returns("answer");
        _services.GetService(typeof(IQueryHandler<SampleQuery, string>)).Returns(handler);

        var result = await new QueryDispatcher(_services).DispatchAsync<SampleQuery, string>(query);

        result.ShouldBe("answer");
    }

    [Fact]
    public async Task QueryDispatcher_NoHandler_Throws()
    {
        await Should.ThrowAsync<InvalidOperationException>(() =>
            new QueryDispatcher(_services).DispatchAsync<SampleQuery, string>(new SampleQuery()));
    }

    private void GivenValidator(ValidationResult result)
    {
        var validator = Substitute.For<IValidator<SampleCommand>>();
        validator.ValidateAsync(_command, Arg.Any<CancellationToken>()).Returns(result);
        _services.GetService(typeof(IValidator<SampleCommand>)).Returns(validator);
    }

    public sealed class SampleCommand : ICommand;

    public sealed class SampleQuery : IQuery<string>;
}
