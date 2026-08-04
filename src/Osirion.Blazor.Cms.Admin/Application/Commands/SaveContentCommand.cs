using Osirion.Blazor.Cms.Application.Commands;

namespace Osirion.Blazor.Cms.Admin.Application.Commands;

    /// <summary>Defines the public member API contract.</summary>
public class SaveContentCommand : ICommand
{
    /// <summary>Gets or sets the Path value.</summary>
    public string Path { get; set; } = string.Empty;
    /// <summary>Gets or sets the Content value.</summary>
    public string Content { get; set; } = string.Empty;
    /// <summary>Gets or sets the CommitMessage value.</summary>
    public string CommitMessage { get; set; } = string.Empty;
    /// <summary>Gets or sets the Sha value.</summary>
    public string? Sha { get; set; }
}
