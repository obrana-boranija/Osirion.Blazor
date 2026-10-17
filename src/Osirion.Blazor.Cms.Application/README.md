# Osirion.Blazor.Cms.Application

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Cms.Application)](https://www.nuget.org/packages/Osirion.Blazor.Cms.Application)
[![License](https://img.shields.io/github/license/demonjic/Osirion.Blazor)](https://github.com/demonjic/Osirion.Blazor/blob/master/LICENSE.txt)

Application layer for the Osirion.Blazor CMS ecosystem, implementing core business logic, use cases, and application services.

## Overview

The Application project represents the application layer in the Clean Architecture pattern, bridging the domain models with the infrastructure and presentation layers. It uses a command/query split: commands and queries are plain classes, each handled by one handler, and two dispatchers resolve the handler from the service provider. This package ships handlers for the content commands, for `GetContentByIdQuery`, `GetContentByPathQuery`, `SearchContentQuery`, `GetDirectoryByPathQuery` and `GetDirectoryTreeQuery`; the other commands and queries need a handler registered by the host. Input is validated with FluentValidation before a command runs.

## Components

### Commands

A command implements `ICommand` and is handled by an `ICommandHandler<TCommand>` (namespace `Osirion.Blazor.Cms.Application.Commands`):

```csharp
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
```

Content commands (`Osirion.Blazor.Cms.Application.Commands.Content`) are `CreateContentCommand`, `UpdateContentCommand` and `DeleteContentCommand`. Directory commands (`Osirion.Blazor.Cms.Application.Commands.Directory`) are `CreateDirectoryCommand`, `UpdateDirectoryCommand`, `MoveDirectoryCommand` and `DeleteDirectoryCommand`.

```csharp
await commandDispatcher.DispatchAsync(new CreateContentCommand
{
    Title = "Hello",
    Content = "# Hello",
    Path = "blog/hello.md",
    CommitMessage = "Add hello post"
});
```

`ICommandDispatcher.DispatchAsync<TCommand>(command, cancellationToken)` is implemented by `ValidatingCommandDispatcher`. It runs the registered validator, if there is one, and throws `ContentValidationException` when validation fails.

### Queries

A query implements `IQuery<TResult>` and is handled by an `IQueryHandler<TQuery, TResult>` (namespace `Osirion.Blazor.Cms.Application.Queries`):

```csharp
public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
```

Content queries (`Osirion.Blazor.Cms.Application.Queries.Content`) include `GetContentByPathQuery`, `GetContentByIdQuery`, `GetContentByUrlQuery`, `GetAllContentQuery`, `SearchContentQuery`, `GetContentCategoriesQuery` and `GetContentTagsQuery`. Directory queries (`Osirion.Blazor.Cms.Application.Queries.Directory`) are `GetDirectoryByIdQuery`, `GetDirectoryByPathQuery` and `GetDirectoryTreeQuery`.

```csharp
var item = await queryDispatcher.DispatchAsync<GetContentByPathQuery, ContentItem?>(
    new GetContentByPathQuery { Path = "blog/hello.md" });
```

### Validators

Input validation uses FluentValidation. `CreateContentCommandValidator` and `UpdateContentCommandValidator` (namespace `Osirion.Blazor.Cms.Application.Validation.Content`) are `AbstractValidator<T>` classes, for example:

```csharp
public class CreateContentCommandValidator : AbstractValidator<CreateContentCommand>
{
    public CreateContentCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        // Further rules for Content, Path and Slug
    }
}
```

`FluentValidationAdapter<T>` exposes a FluentValidation validator through the application's own `IValidator<T>`.

## Registration

This package does not register its services. `AddOsirionCqrs()` in the `Osirion.Blazor.Cms.Infrastructure` package (namespace `Osirion.Blazor.Cms.Infrastructure.DependencyInjection`) registers the dispatchers, the command and query handlers, the validators and their adapters.

## Dependencies

The application layer has the following dependencies:

- **Osirion.Blazor.Cms.Domain**: For domain entities and interfaces
- **FluentValidation**: For input validation
- **Microsoft.Extensions.DependencyInjection** and **Microsoft.Extensions.Logging.Abstractions**: For service resolution and logging

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/demonjic/Osirion.Blazor/blob/master/LICENSE.txt) file for details.