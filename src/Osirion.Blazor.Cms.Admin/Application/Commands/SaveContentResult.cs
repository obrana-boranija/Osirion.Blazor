namespace Osirion.Blazor.Cms.Application.Commands;

public class SaveContentResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ContentId { get; set; } = string.Empty;
    public string Sha { get; set; } = string.Empty;
}