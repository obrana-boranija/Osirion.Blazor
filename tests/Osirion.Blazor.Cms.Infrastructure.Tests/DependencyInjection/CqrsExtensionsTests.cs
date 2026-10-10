using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Application.Commands;
using Osirion.Blazor.Cms.Application.Commands.Content;
using Osirion.Blazor.Cms.Application.Validation;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Infrastructure.DependencyInjection;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.DependencyInjection;

public class CqrsExtensionsTests
{
    [Fact]
    public async Task AddOsirionCqrs_CommandWithoutValidator_IsDispatchedToItsHandler()
    {
        // Before the fix an open-generic adapter matched every command, and resolving it for a command with no
        // FluentValidation validator threw, so DeleteContentCommand could never be dispatched.
        var handler = Substitute.For<ICommandHandler<DeleteContentCommand>>();
        using var provider = BuildProvider(services => services.AddScoped(_ => handler));
        using var scope = provider.CreateScope();
        var command = new DeleteContentCommand { Id = "post-1" };

        await scope.ServiceProvider.GetRequiredService<ICommandDispatcher>().DispatchAsync(command);

        await handler.Received(1).HandleAsync(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public void AddOsirionCqrs_CommandWithoutValidator_HasNoValidatorAdapter()
    {
        using var provider = BuildProvider();

        provider.GetService<IValidator<DeleteContentCommand>>().ShouldBeNull();
    }

    [Fact]
    public async Task AddOsirionCqrs_CommandWithValidator_IsStillValidated()
    {
        var handler = Substitute.For<ICommandHandler<CreateContentCommand>>();
        using var provider = BuildProvider(services => services.AddScoped(_ => handler));
        using var scope = provider.CreateScope();
        var invalidCommand = new CreateContentCommand();

        await Should.ThrowAsync<ContentValidationException>(
            () => scope.ServiceProvider.GetRequiredService<ICommandDispatcher>().DispatchAsync(invalidCommand));

        await handler.DidNotReceiveWithAnyArgs().HandleAsync(default!, default);
    }

    private static ServiceProvider BuildProvider(Action<IServiceCollection>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOsirionCqrs();
        configure?.Invoke(services);
        return services.BuildServiceProvider();
    }
}
