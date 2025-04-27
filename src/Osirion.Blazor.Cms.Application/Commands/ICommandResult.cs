namespace Osirion.Blazor.Cms.Application.Commands;

/// <summary>
/// Interface for command results in CQRS pattern
/// </summary>
public interface ICommandResult
{
    /// <summary>
    /// Gets whether the command was successful
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets the error message if the command failed
    /// </summary>
    string? ErrorMessage { get; }
}