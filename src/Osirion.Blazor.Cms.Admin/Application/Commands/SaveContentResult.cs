namespace Osirion.Blazor.Cms.Application.Commands;

    /// <summary>Defines the public member API contract.</summary>
public class SaveContentResult
{
    /// <summary>Gets or sets the IsSuccess value.</summary>
    public bool IsSuccess { get; set; }
    /// <summary>Gets or sets the Message value.</summary>
    public string Message { get; set; } = string.Empty;
    /// <summary>Gets or sets the ContentId value.</summary>
    public string ContentId { get; set; } = string.Empty;
    /// <summary>Gets or sets the Sha value.</summary>
    public string Sha { get; set; } = string.Empty;
}
