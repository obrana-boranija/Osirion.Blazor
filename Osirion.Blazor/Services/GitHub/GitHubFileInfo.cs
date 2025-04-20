namespace Osirion.Blazor.Services.GitHub;

public class GitHubFileInfo
{
    public string Path { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdatedDate { get; set; }
}