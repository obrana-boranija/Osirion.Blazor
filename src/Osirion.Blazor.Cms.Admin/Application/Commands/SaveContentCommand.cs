using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Application.Commands;

namespace Osirion.Blazor.Cms.Admin.Application.Commands;

public class SaveContentCommand : ICommand
{
    public string Path { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string CommitMessage { get; set; } = string.Empty;
    public string? Sha { get; set; }
}