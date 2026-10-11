namespace Osirion.Blazor.Core.Models;

/// <summary>
/// A step of <see cref="Osirion.Blazor.Components.OsirionSteps"/>.
/// </summary>
/// <param name="Title">What the step is, for example <c>Install the package</c>.</param>
/// <param name="Description">A sentence or two on the step, as plain text.</param>
public sealed record StepItem(string Title, string? Description = null);
